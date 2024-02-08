// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.Utils;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGroupingBuilder
    {
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher dispatcher;
        private readonly IReadOnlyWorkspace workspace;

        private readonly Dictionary<Uri, ResultWithDiagnostic<ISourceFile>> fileResultByUri;
        // For each .bicep file key, a dictionary (keyed by 'module' or other syntax statement in the .bicep file) of the resolved URI for that syntax statement
        private readonly ConcurrentDictionary<BicepSourceFile, Dictionary<IArtifactReferenceSyntax, Result<Uri, UriResolutionError>>> uriResultByBicepSourceFileByArtifactReferenceSyntax;
        // For each .bicep file key, a dictionary (keyed by implicit provider artifact references in the scope of the .bicep file) of the resolved URI for the provider types data file
        private readonly ConcurrentDictionary<BicepSourceFile, Dictionary<ArtifactReference, Result<Uri, UriResolutionError>>> uriResultByBicepSourceFileByArtifactReference;
        // For each .bicep file key, the bundle of resource type provider artifact descriptors of both explicit and implicit provider
        private readonly ConcurrentDictionary<BicepSourceFile, ProviderDescriptorBundleBuilder> providerDescriptorBundleBuilderBySourceFile;
        private readonly bool forceRestore;

        private SourceFileGroupingBuilder(
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IReadOnlyWorkspace workspace,
            bool forceModulesRestore = false)
        {
            this.fileResolver = fileResolver;
            this.dispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.uriResultByBicepSourceFileByArtifactReferenceSyntax = new();
            this.uriResultByBicepSourceFileByArtifactReference = new();
            this.providerDescriptorBundleBuilderBySourceFile = new();
            this.fileResultByUri = new();
            this.forceRestore = forceModulesRestore;
        }

        private SourceFileGroupingBuilder(
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IReadOnlyWorkspace workspace,
            SourceFileGrouping current,
            bool forceArtifactRestore = false)
        {
            this.fileResolver = fileResolver;
            this.dispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.uriResultByBicepSourceFileByArtifactReferenceSyntax = new(current.FileUriResultByBicepSourceFileByArtifactReferenceSyntax.Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value.ToDictionary(p => p.Key, p => p.Value))));
            this.uriResultByBicepSourceFileByArtifactReference = new(current.FileUriResultByBicepSourceFileByArtifactReference.Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value.ToDictionary(p => p.Key, p => p.Value))));
            this.providerDescriptorBundleBuilderBySourceFile = new(current.ProvidersToRestoreByFileResult.Select(kvp => KeyValuePair.Create(kvp.Key, new ProviderDescriptorBundleBuilder(kvp.Value.ImplicitProviders, kvp.Value.ExplicitProviderLookup))));
            this.fileResultByUri = current.FileResultByUri.Where(x => x.Value.TryUnwrap() is not null).ToDictionary(x => x.Key, x => x.Value);
            this.forceRestore = forceArtifactRestore;
        }

        public static SourceFileGrouping Build(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IConfigurationManager configurationManager, IReadOnlyWorkspace workspace, Uri entryFileUri, IFeatureProviderFactory featuresFactory, bool forceModulesRestore = false)
        {
            var builder = new SourceFileGroupingBuilder(fileResolver, moduleDispatcher, workspace, forceModulesRestore);

            return builder.Build(entryFileUri, featuresFactory, configurationManager);
        }

        public static SourceFileGrouping Rebuild(IFeatureProviderFactory featuresFactory, IModuleDispatcher moduleDispatcher, IConfigurationManager configurationManager, IReadOnlyWorkspace workspace, SourceFileGrouping current)
        {
            var builder = new SourceFileGroupingBuilder(current.FileResolver, moduleDispatcher, workspace, current);
            var isParamsFile = current.EntryPoint is BicepParamFile;
            var artifactsToRestore = current.GetExplicitArtifactsToRestore().ToHashSet();

            foreach (var (module, sourceFile) in artifactsToRestore)
            {
                builder.uriResultByBicepSourceFileByArtifactReferenceSyntax[sourceFile].Remove(module);
            }

            // Rebuild source files that contain external artifact references restored during the initial build.
            var sourceFilesToRebuild = current.SourceFiles.OfType<BicepSourceFile>()
                .Where(sourceFile
                    => GetArtifactReferenceDeclarations(sourceFile)
                        .Any(moduleDeclaration
                            => artifactsToRestore.Contains(new ArtifactResolutionInfo(moduleDeclaration, sourceFile))))
                .ToImmutableHashSet()
                .SelectMany(current.GetFilesDependingOn)
                .ToImmutableHashSet();

            return builder.Build(current.EntryPoint.FileUri, featuresFactory, configurationManager, sourceFilesToRebuild);
        }

        private SourceFileGrouping Build(Uri entryFileUri, IFeatureProviderFactory featuresFactory, IConfigurationManager configurationManager, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild = null)
        {
            var fileResult = this.PopulateRecursive(entryFileUri, null, sourceFilesToRebuild, featuresFactory, configurationManager);

            if (!fileResult.IsSuccess(out _, out var errorBuilder))
            {
                var diagnostic = errorBuilder(ForDocumentStart());

                throw new ErrorDiagnosticException(diagnostic);
            }

            var sourceFileDependencies = this.ReportFailuresForCycles();

            return new SourceFileGrouping(
                fileResolver,
                entryFileUri,
                fileResultByUri.ToImmutableDictionary(),
                providerDescriptorBundleBuilderBySourceFile.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.Build()),
                uriResultByBicepSourceFileByArtifactReferenceSyntax.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableDictionary()),
                uriResultByBicepSourceFileByArtifactReference.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableDictionary()),
                sourceFileDependencies.InvertLookup().ToImmutableDictionary());
        }

        private ResultWithDiagnostic<ISourceFile> GetFileResolutionResult(Uri fileUri, ArtifactReference? moduleReference)
        {
            if (workspace.TryGetSourceFile(fileUri, out var sourceFile))
            {
                return new(sourceFile);
            }

            if (!fileResolver.TryRead(fileUri).IsSuccess(out var fileContents, out var failureBuilder))
            {
                return new(failureBuilder);
            }

            sourceFile = SourceFileFactory.CreateSourceFile(fileUri, fileContents, moduleReference);

            return new(sourceFile);
        }

        private ResultWithDiagnostic<ISourceFile> GetFileResolutionResultWithCaching(Uri fileUri, ArtifactReference? moduleReference)
        {
            if (!fileResultByUri.TryGetValue(fileUri, out var resolutionResult))
            {
                resolutionResult = GetFileResolutionResult(fileUri, moduleReference);
                fileResultByUri[fileUri] = resolutionResult;
            }

            return resolutionResult;
        }

        private ResultWithDiagnostic<ISourceFile> PopulateRecursive(Uri fileUri, ArtifactReference? moduleReference, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild, IFeatureProviderFactory featuresFactory, IConfigurationManager configurationManager)
        {
            var fileResult = GetFileResolutionResultWithCaching(fileUri, moduleReference);
            if (fileResult.TryUnwrap() is BicepSourceFile bicepSource)
            {
                PopulateRecursive(bicepSource, featuresFactory, configurationManager, sourceFilesToRebuild);
            }

            return fileResult;
        }

        private void PopulateRecursive(BicepSourceFile file, IFeatureProviderFactory featureProviderFactory, IConfigurationManager configurationManager, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild)
        {
            var featureProvider = featureProviderFactory.GetFeatureProvider(file.FileUri);
            if (featureProvider.ExtensibilityEnabled && featureProvider.DynamicTypeLoadingEnabled)
            {
                PopulateProviderDeclarations(file, featureProviderFactory, configurationManager);
            }

            foreach (var restorable in file.ProgramSyntax.Children.OfType<IArtifactReferenceSyntax>())
            {
                var (childArtifactReference, uriResult) = GetArtifactRestoreResult(file.FileUri, restorable);
                uriResultByBicepSourceFileByArtifactReferenceSyntax.GetOrAdd(file, f => new())[restorable] = uriResult;

                if (!uriResult.IsSuccess(out var artifactUri))
                {
                    continue;
                }

                if (restorable is not ProviderDeclarationSyntax)
                {
                    if (!fileResultByUri.TryGetValue(artifactUri, out var childResult) ||
                        (childResult.IsSuccess(out var childFile) && sourceFilesToRebuild is not null && sourceFilesToRebuild.Contains(childFile)))
                    {
                        // only recurse if we've not seen this file before - to avoid infinite loops
                        childResult = PopulateRecursive(artifactUri, childArtifactReference, sourceFilesToRebuild, featureProviderFactory, configurationManager);
                    }
                    fileResultByUri[artifactUri] = childResult;
                }
            }
        }

        private (ArtifactReference? reference, Result<Uri, UriResolutionError> result) GetArtifactRestoreResult(Uri parentFileUri, IArtifactReferenceSyntax referenceSyntax)
        {
            if (!dispatcher.TryGetArtifactReference(referenceSyntax, parentFileUri).IsSuccess(out var artifactReference, out var referenceResolutionError))
            {
                // module reference is not valid
                return (null, new(new UriResolutionError(referenceResolutionError, RequiresRestore: false)));
            }
            return (artifactReference, GetArtifactRestoreResult(artifactReference));
        }

        private Result<Uri, UriResolutionError> GetArtifactRestoreResult(ArtifactReference artifactReference)
        {
            if (!dispatcher.TryGetLocalArtifactEntryPointUri(artifactReference).IsSuccess(out var artifactFileUri, out var artifactGetPathFailureBuilder))
            {
                return new(new UriResolutionError(artifactGetPathFailureBuilder, RequiresRestore: false));
            }

            if (forceRestore)
            {
                //override the status to force restore
                return new(new UriResolutionError(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference), RequiresRestore: true));
            }

            var restoreStatus = dispatcher.GetArtifactRestoreStatus(artifactReference, out var restoreErrorBuilder);
            switch (restoreStatus)
            {
                case ArtifactRestoreStatus.Unknown:
                    // we have not yet attempted to restore the module, so let's do it
                    return new(new UriResolutionError(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference), RequiresRestore: true));
                case ArtifactRestoreStatus.Failed:
                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    return new(new UriResolutionError(restoreErrorBuilder ?? (x => x.ArtifactRestoreFailed(artifactReference.FullyQualifiedReference)), RequiresRestore: false));
                default:
                    break;
            }
            return new(artifactFileUri);
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.TryUnwrap())
                .WhereNotNull()
                .SelectMany(sourceFile => GetReferenceSourceNodes(sourceFile)
                    .SelectMany(moduleDeclaration
                        => this.uriResultByBicepSourceFileByArtifactReferenceSyntax.Values.Select(
                            f => f.TryGetValue(moduleDeclaration)?.TryUnwrap()))
                    .WhereNotNull()
                    .Select(fileUri => this.fileResultByUri[fileUri].TryUnwrap())
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (file, uriResultByModuleForFile) in uriResultByBicepSourceFileByArtifactReferenceSyntax)
            {
                foreach (var (statement, urlResult) in uriResultByModuleForFile)
                {
                    if (statement.GetArtifactType() == ArtifactType.Module &&
                        urlResult.IsSuccess(out var fileUri) &&
                        fileResultByUri[fileUri].IsSuccess(out var sourceFile) &&
                        cycles.TryGetValue(sourceFile, out var cycle))
                    {
                        if (cycle.Length == 1)
                        {
                            uriResultByModuleForFile[statement] = cycle[0] switch
                            {
                                BicepParamFile => new(new UriResolutionError(x => x.CyclicParametersSelfReference(), false)),
                                _ => new(new UriResolutionError(x => x.CyclicModuleSelfReference(), false)),
                            };
                        }
                        else
                        {
                            // the error message is generic so it should work for either bicep module or params
                            uriResultByModuleForFile[statement] = new(new UriResolutionError(x => x.CyclicFile(cycle.Select(u => u.FileUri.LocalPath)), false));
                        }
                    }
                }
            }

            return sourceFileGraph;
        }

        /// <remarks>
        /// This method only looks at top-level statements. If nested syntax nodes can be foreign template references at any point in the future,
        /// a SyntaxAggregator will need to be used in place of the <code>sourceFile.ProgramSyntax.Children</code> expressions.
        /// </remarks>
        private static IEnumerable<IArtifactReferenceSyntax> GetReferenceSourceNodes(ISourceFile sourceFile) => sourceFile switch
        {
            BicepFile bicepFile => bicepFile.ProgramSyntax.Children.OfType<IArtifactReferenceSyntax>().Where(x => x is not ProviderDeclarationSyntax),
            BicepParamFile paramsFile => paramsFile.ProgramSyntax.Children.OfType<IArtifactReferenceSyntax>().Where(x => x is not ProviderDeclarationSyntax),
            _ => Enumerable.Empty<IArtifactReferenceSyntax>(),
        };

        private static IEnumerable<IArtifactReferenceSyntax> GetArtifactReferenceDeclarations(BicepSourceFile sourceFile)
            => sourceFile.ProgramSyntax.Declarations.OfType<IArtifactReferenceSyntax>();

        private void PopulateProviderDeclarations(BicepSourceFile file, IFeatureProviderFactory featureProviderFactory, IConfigurationManager configurationManager)
        {
            var featureProvider = featureProviderFactory.GetFeatureProvider(file.FileUri);
            var config = configurationManager.GetConfiguration(file.FileUri);

            if (!this.providerDescriptorBundleBuilderBySourceFile.TryGetValue(file, out var providerBundleBuilder))
            {
                this.providerDescriptorBundleBuilderBySourceFile[file] = providerBundleBuilder = new ProviderDescriptorBundleBuilder();
                providerBundleBuilder.AddImplicitProvider(new(new ResourceTypesProviderDescriptor(SystemNamespaceType.BuiltInName, file.FileUri)));
            }

            ProcessAllImplicitProviderDeclarations(file, providerBundleBuilder, config);

            foreach (var restorable in file.ProgramSyntax.Children.OfType<ProviderDeclarationSyntax>())
            {
                var (childArtifactReference, uriResult) = GetArtifactRestoreResult(file.FileUri, restorable);

                uriResultByBicepSourceFileByArtifactReferenceSyntax.GetOrAdd(file, f => new())[restorable] = uriResult;
                var descriptor = ResolveProviderDescriptor(restorable, childArtifactReference, uriResult, featureProvider, config, file);

                providerBundleBuilder.AddOrUpdateExplicitProvider(restorable, descriptor);
            }
        }

        private void ProcessSingleImplicitProviderDeclaration(string providerName, ProviderDescriptorBundleBuilder providerBundleBuilder, BicepSourceFile file, RootConfiguration config)
        {
            if (!config.ProvidersConfig.TryGetProviderSource(providerName).IsSuccess(out var providerEntry, out var errorBuilder))
            {
                providerBundleBuilder.AddImplicitProvider(new(errorBuilder));
                return;
            }
            if (providerEntry.BuiltIn)
            {
                providerBundleBuilder.AddImplicitProvider(new(new ResourceTypesProviderDescriptor(providerName, file.FileUri)));
                return;
            }

            var artifactAddress = $"br:{providerEntry.Source}:{providerEntry.Version}";
            if (!OciArtifactReference.TryParse(ArtifactType.Provider, null, artifactAddress, config, file.FileUri).IsSuccess(out var artifactReference, out errorBuilder))
            {
                providerBundleBuilder.AddImplicitProvider(new(errorBuilder));
                return;
            }

            uriResultByBicepSourceFileByArtifactReference.GetOrAdd(file, f => new())[artifactReference] = GetArtifactRestoreResult(artifactReference);

            if (!dispatcher.TryGetLocalArtifactEntryPointUri(artifactReference).IsSuccess(out var artifactFileUri, out var artifactGetPathFailureBuilder))
            {
                providerBundleBuilder.AddImplicitProvider(new(artifactGetPathFailureBuilder));
                return;
            }

            providerBundleBuilder.AddImplicitProvider(new(
                new ResourceTypesProviderDescriptor(
                    providerEntry.Source?.Split('/')[^1] ?? throw new UnreachableException("provider source is validated during artifact creation"),
                    file.FileUri,
                    providerEntry.Version ?? throw new UnreachableException("provider version is validated during artifact creation"),
                    providerName,
                    artifactReference,
                    artifactFileUri)));
        }

        private static ResultWithDiagnostic<ResourceTypesProviderDescriptor> ResolveProviderDescriptor(
            ProviderDeclarationSyntax providerDeclarationSyntax,
            ArtifactReference? artifactReference,
            Result<Uri, UriResolutionError> uriResult,
            IFeatureProvider featureProvider,
            RootConfiguration config,
            BicepSourceFile file)
        {
            if (!featureProvider.ExtensibilityEnabled)
            {
                return new(x => x.ProvidersAreDisabled());
            }

            if (providerDeclarationSyntax.Specification is LegacyProviderSpecificationSyntax { } legacySpecification)
            {
                return new(new ResourceTypesProviderDescriptor(legacySpecification.NamespaceIdentifier, file.FileUri));
            }

            if (!featureProvider.DynamicTypeLoadingEnabled)
            {
                return new(x => x.UnrecognizedProvider(providerDeclarationSyntax.Specification.NamespaceIdentifier));
            }

            if (providerDeclarationSyntax.Specification is InlinedProviderSpecificationSyntax { } inlinedSpecification)
            {
                if (!uriResult.IsSuccess(out var typesDataUri, out var uriResolutionError))
                {
                    return new(uriResolutionError.ErrorBuilder);
                }
                return new(new ResourceTypesProviderDescriptor(
                               inlinedSpecification.NamespaceIdentifier,
                               file.FileUri,
                               inlinedSpecification.Version,
                               providerDeclarationSyntax.Alias?.IdentifierName ?? inlinedSpecification.NamespaceIdentifier,
                               artifactReference,
                               typesDataUri));
            }

            // The provider is configuration managed, fetch the provider source & version from the configuration
            var configLookupValue = providerDeclarationSyntax.Specification.NamespaceIdentifier;

            // Special case the "sys" provider for backwards compatibility
            if (configLookupValue == SystemNamespaceType.BuiltInName)
            {
                return new(new ResourceTypesProviderDescriptor(SystemNamespaceType.BuiltInName, file.FileUri));
            }

            if (!config.ProvidersConfig.TryGetProviderSource(configLookupValue).IsSuccess(out var providerEntry, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            //handle built-in providers
            if (providerEntry.BuiltIn)
            {
                return new(new ResourceTypesProviderDescriptor(configLookupValue, file.FileUri));
            }

            var unreachableException = new UnreachableException("artifact path is validated during artifact creation so Source & Version must cannot be null");
            var providerNamespaceIdentifier = providerEntry.Source?.Split('/')[^1] ?? throw unreachableException;

            if (!uriResult.IsSuccess(out var uri, out var uriResoulutionError))
            {
                return new(uriResoulutionError.ErrorBuilder);
            }

            return new(new ResourceTypesProviderDescriptor(
                providerNamespaceIdentifier,
                file.FileUri,
                providerEntry.Version ?? throw unreachableException,
                providerDeclarationSyntax.Alias?.IdentifierName ?? configLookupValue,
                artifactReference,
                uri));
        }

        private void ProcessAllImplicitProviderDeclarations(BicepSourceFile file, ProviderDescriptorBundleBuilder providerBundleBuilder, RootConfiguration config)
        {
            foreach (var providerName in config.ImplicitProvidersConfig.GetImplicitProviderNames())
            {
                try
                {
                    ProcessSingleImplicitProviderDeclaration(providerName, providerBundleBuilder, file, config);
                }
                catch (Exception ex)
                {
                    this.providerDescriptorBundleBuilderBySourceFile[file].AddImplicitProvider(new(x => x.ArtifactRestoreFailedWithMessage(providerName, ex.Message)));
                }
            }
        }
    }
}

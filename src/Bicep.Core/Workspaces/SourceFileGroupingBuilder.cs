// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGroupingBuilder
    {
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher dispatcher;
        private readonly IReadOnlyWorkspace workspace;
        private readonly IConfigurationManager configurationManager;

        private readonly Dictionary<Uri, ResultWithDiagnostic<ISourceFile>> fileResultByUri;
        private readonly ConcurrentDictionary<BicepSourceFile, Dictionary<IArtifactReferenceSyntax, Result<Uri, UriResolutionError>>> fileUriResultByArtifactReference;
        private readonly ConcurrentDictionary<BicepSourceFile, ProviderDescriptorBundleBuilder> providerDescriptorBundleBuilderBySourceFile;
        private readonly bool forceRestore;

        private SourceFileGroupingBuilder(
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            IReadOnlyWorkspace workspace,
            bool forceModulesRestore = false)
        {
            this.fileResolver = fileResolver;
            this.dispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.fileUriResultByArtifactReference = new();
            this.providerDescriptorBundleBuilderBySourceFile = new();
            this.fileResultByUri = new();
            this.forceRestore = forceModulesRestore;
            this.configurationManager = configurationManager;
        }

        private SourceFileGroupingBuilder(
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            IReadOnlyWorkspace workspace,
            SourceFileGrouping current,
            bool forceArtifactRestore = false)
        {
            this.fileResolver = fileResolver;
            this.dispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.fileUriResultByArtifactReference = new(current.FileUriResultByArtifactReference.Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value.ToDictionary(p => p.Key, p => p.Value))));
            this.providerDescriptorBundleBuilderBySourceFile = new(current.ProvidersToRestoreByFileResult.Select(kvp => KeyValuePair.Create(kvp.Key, new ProviderDescriptorBundleBuilder(kvp.Value.ImplicitProviders, kvp.Value.ExplicitProviderLookup))));
            this.fileResultByUri = current.FileResultByUri.Where(x => x.Value.TryUnwrap() is not null).ToDictionary(x => x.Key, x => x.Value);
            this.forceRestore = forceArtifactRestore;
            this.configurationManager = configurationManager;
        }

        public static SourceFileGrouping Build(
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            IReadOnlyWorkspace workspace,
            Uri entryFileUri,
            IFeatureProviderFactory featuresFactory,
            bool forceModulesRestore = false)
        {
            var builder = new SourceFileGroupingBuilder(fileResolver, moduleDispatcher, configurationManager, workspace, forceModulesRestore);

            return builder.Build(entryFileUri, featuresFactory, configurationManager);
        }

        public static SourceFileGrouping Rebuild(
            IFeatureProviderFactory featuresFactory,
            IModuleDispatcher moduleDispatcher,
            IConfigurationManager configurationManager,
            IReadOnlyWorkspace workspace,
            SourceFileGrouping current)
        {
            var builder = new SourceFileGroupingBuilder(current.FileResolver, moduleDispatcher, configurationManager, workspace, current);
            var isParamsFile = current.EntryPoint is BicepParamFile;
            var artifactsToRestore = current.GetArtifactsToRestore().ToHashSet();

            foreach (var (module, sourceFile) in artifactsToRestore)
            {
                builder.fileUriResultByArtifactReference[sourceFile].Remove(module);
            }

            // Rebuild source files that contain external module references restored during the inital build.
            var sourceFilesToRebuild = current.SourceFiles.OfType<BicepSourceFile>()
                .Where(sourceFile
                    => GetArtifactReferenceDeclarations(sourceFile)
                        .Any(moduleDeclaration
                            => artifactsToRestore.Contains(new ArtifactResolutionInfo(moduleDeclaration, sourceFile))))
                .ToImmutableHashSet()
                .SelectMany(sourceFile => current.GetFilesDependingOn(sourceFile))
                .ToImmutableHashSet();

            return builder.Build(current.EntryPoint.FileUri, featuresFactory, configurationManager, sourceFilesToRebuild);
        }

        private SourceFileGrouping Build(
            Uri entryFileUri,
            IFeatureProviderFactory featuresFactory,
            IConfigurationManager configurationManager,
            ImmutableHashSet<ISourceFile>? sourceFilesToRebuild = null)
        {
            var fileResult = this.PopulateRecursive(entryFileUri, null, sourceFilesToRebuild, featuresFactory);

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
                fileUriResultByArtifactReference.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableDictionary()),
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

        private ResultWithDiagnostic<ISourceFile> PopulateRecursive(Uri fileUri, ArtifactReference? moduleReference, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild, IFeatureProviderFactory featuresFactory)
        {
            var fileResult = GetFileResolutionResultWithCaching(fileUri, moduleReference);
            if (fileResult.TryUnwrap() is BicepSourceFile bicepSource)
            {
                PopulateRecursive(bicepSource, featuresFactory, sourceFilesToRebuild);
            }

            return fileResult;
        }

        private void PopulateRecursive(BicepSourceFile file, IFeatureProviderFactory featureProviderFactory, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild)
        {
            foreach (var restorable in file.ProgramSyntax.Children.OfType<IArtifactReferenceSyntax>())
            {
                var (childArtifactReference, uriResult) = GetArtifactRestoreResult(file.FileUri, restorable);

                fileUriResultByArtifactReference.GetOrAdd(file, f => new())[restorable] = uriResult;

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
                        childResult = PopulateRecursive(artifactUri, childArtifactReference, sourceFilesToRebuild, featureProviderFactory);
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
                return (null, new(new UriResolutionError(referenceResolutionError, false)));
            }

            if (!dispatcher.TryGetLocalArtifactEntryPointUri(artifactReference).IsSuccess(out var artifactFileUri, out var artifactGetPathFailureBuilder))
            {
                return (artifactReference, new(new UriResolutionError(artifactGetPathFailureBuilder, false)));
            }

            if (forceRestore)
            {
                //override the status to force restore
                return (artifactReference, new(new UriResolutionError(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference), true)));
            }

            var restoreStatus = dispatcher.GetArtifactRestoreStatus(artifactReference, out var restoreErrorBuilder);
            switch (restoreStatus)
            {
                case ArtifactRestoreStatus.Unknown:
                    // we have not yet attempted to restore the module, so let's do it
                    return (artifactReference, new(new UriResolutionError(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference), true)));
                case ArtifactRestoreStatus.Failed:
                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    return (artifactReference, new(new UriResolutionError(restoreErrorBuilder ?? (x => x.ArtifactRestoreFailed(artifactReference.FullyQualifiedReference)), false)));
                default:
                    break;
            }

            return (artifactReference, new(artifactFileUri));
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.TryUnwrap())
                .WhereNotNull()
                .SelectMany(sourceFile => GetReferenceSourceNodes(sourceFile)
                    .SelectMany(moduleDeclaration => this.fileUriResultByArtifactReference.Values.Select(f => f.TryGetValue(moduleDeclaration)?.TryUnwrap()))
                    .WhereNotNull()
                    .Select(fileUri => this.fileResultByUri[fileUri].TryUnwrap())
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (file, uriResultByModuleForFile) in fileUriResultByArtifactReference)
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
    }
}

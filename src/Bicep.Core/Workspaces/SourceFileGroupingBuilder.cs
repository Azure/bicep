// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
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
        private readonly Dictionary<IArtifactReferenceSyntax, ArtifactResolutionInfo> artifactLookup;
        private readonly HashSet<ArtifactResolutionInfo> implicitArtifacts;
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
            this.artifactLookup = new();
            this.implicitArtifacts = new();
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
            this.artifactLookup = current.ArtifactLookup.Where(x => x.Value.Result.IsSuccess()).ToDictionary();
            this.implicitArtifacts = current.ImplicitArtifacts.ToHashSet();
            this.fileResultByUri = current.SourceFileLookup.ToDictionary();
            this.forceRestore = forceArtifactRestore;
        }

        public static SourceFileGrouping Build(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IConfigurationManager configurationManager, IReadOnlyWorkspace workspace, Uri entryFileUri, IFeatureProviderFactory featuresFactory, bool forceModulesRestore = false)
        {
            var builder = new SourceFileGroupingBuilder(fileResolver, moduleDispatcher, workspace, forceModulesRestore);

            return builder.Build(entryFileUri, featuresFactory, configurationManager);
        }

        public static SourceFileGrouping Rebuild(IFileResolver fileResolver, IFeatureProviderFactory featuresFactory, IModuleDispatcher moduleDispatcher, IConfigurationManager configurationManager, IReadOnlyWorkspace workspace, SourceFileGrouping current)
        {
            var builder = new SourceFileGroupingBuilder(fileResolver, moduleDispatcher, workspace, current);
            var artifactsToRestore = current.GetArtifactsToRestore();

            // Rebuild source files that contain external artifact references restored during the initial build.
            var sourceFilesToRebuild = artifactsToRestore
                .Select(artifact =>
                {
                    if (artifact.Syntax is {})
                    {
                        builder.artifactLookup.Remove(artifact.Syntax);
                    }
                    else
                    {
                        builder.implicitArtifacts.Remove(artifact);
                    }

                    return artifact.Origin;
                })
                .Distinct()
                .SelectMany(current.GetFilesDependingOn)
                .ToImmutableHashSet();

            return builder.Build(current.EntryPoint.FileUri, featuresFactory, configurationManager, sourceFilesToRebuild);
        }

        private SourceFileGrouping Build(Uri entryFileUri, IFeatureProviderFactory featuresFactory, IConfigurationManager configurationManager, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild = null)
        {
            var fileResult = this.PopulateRecursive(entryFileUri, null, sourceFilesToRebuild, featuresFactory, configurationManager);

            if (!fileResult.IsSuccess(out var entryFile, out var errorBuilder))
            {
                var diagnostic = errorBuilder(ForDocumentStart());

                throw new ErrorDiagnosticException(diagnostic);
            }

            if (entryFile is not BicepSourceFile bicepSourceFile)
            {
                throw new InvalidOperationException($"Unexpected entry source file {entryFile.FileUri}");
            }

            var sourceFileGraph = this.ReportFailuresForCycles();

            return new(
                bicepSourceFile,
                fileResultByUri.Values.Select(x => x.TryUnwrap()).WhereNotNull().ToImmutableArray(),
                sourceFileGraph.InvertLookup().ToImmutableDictionary(),
                artifactLookup.ToImmutableDictionary(),
                implicitArtifacts.ToImmutableArray(),
                fileResultByUri.ToImmutableDictionary());
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

        private ResultWithDiagnostic<ISourceFile> GetFileResolutionResultWithCaching(Uri fileUri, ArtifactReference? reference)
        {
            if (!fileResultByUri.TryGetValue(fileUri, out var resolutionResult))
            {
                resolutionResult = GetFileResolutionResult(fileUri, reference);
                fileResultByUri[fileUri] = resolutionResult;
            }

            return resolutionResult;
        }

        private ResultWithDiagnostic<ISourceFile> PopulateRecursive(Uri fileUri, ArtifactReference? reference, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild, IFeatureProviderFactory featuresFactory, IConfigurationManager configurationManager)
        {
            var fileResult = GetFileResolutionResultWithCaching(fileUri, reference);
            if (fileResult.TryUnwrap() is BicepSourceFile bicepSource)
            {
                PopulateRecursive(bicepSource, featuresFactory, configurationManager, sourceFilesToRebuild);
            }

            return fileResult;
        }

        private void PopulateRecursive(BicepSourceFile file, IFeatureProviderFactory featureProviderFactory, IConfigurationManager configurationManager, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild)
        {
            var config = configurationManager.GetConfiguration(file.FileUri);

            // process "implicit" providers (providers defined in bicepconfig.json)
            foreach (var providerName in config.ImplicitProvidersConfig.GetImplicitProviderNames())
            {
                if (TryGetImplicitProvider(providerName, file, config) is { } implicitProvider)
                {
                    implicitArtifacts.Add(implicitProvider);
                }
            }

            // process all artifact references - modules & providers
            foreach (var restorable in GetArtifactReferences(file.ProgramSyntax))
            {
                if (restorable is ProviderDeclarationSyntax providerDeclaration)
                {
                    var isBuiltInProvider = providerDeclaration.Specification switch {
                        LegacyProviderSpecification => true,
                        ConfigurationManagedProviderSpecification configSpec => config.ProvidersConfig.IsSysOrBuiltIn(configSpec.NamespaceIdentifier),
                        _ => false,
                    };

                    if (isBuiltInProvider)
                    {
                        // built-in provider - no restoration required
                        continue;
                    }

                    artifactLookup[restorable] = GetArtifactRestoreResult(file, restorable);

                    // recursion not needed for provider declarations
                    continue;
                }

                var resolutionInfo = GetArtifactRestoreResult(file, restorable);
                artifactLookup[restorable] = resolutionInfo;
                if (!resolutionInfo.Result.IsSuccess(out var artifactUri))
                {
                    // recursion not possible
                    continue;
                }

                // recurse into child modules, to ensure we have an exhaustive list of restorable artifacts for the full compilation
                if (!fileResultByUri.TryGetValue(artifactUri, out var childResult) ||
                    (childResult.IsSuccess(out var childFile) && sourceFilesToRebuild is not null && sourceFilesToRebuild.Contains(childFile)))
                {
                    // only recurse if we've not seen this file before - to avoid infinite loops
                    childResult = PopulateRecursive(artifactUri, resolutionInfo.Reference, sourceFilesToRebuild, featureProviderFactory, configurationManager);
                }
                fileResultByUri[artifactUri] = childResult;
            }
        }

        private ArtifactResolutionInfo? TryGetImplicitProvider(string providerName, BicepSourceFile file, RootConfiguration config)
        {
            if (!config.ProvidersConfig.TryGetProviderSource(providerName).IsSuccess(out var providerEntry, out var errorBuilder))
            {
                return new(file, null, null, new(errorBuilder), RequiresRestore: false);
            }

            if (providerEntry.BuiltIn)
            {
                return null;
            }

            if (!OciArtifactReference.TryParse(ArtifactType.Provider, null, providerEntry.Path, config, file.FileUri).IsSuccess(out var artifactReference, out errorBuilder))
            {
                // reference is not valid
                return new(file, null, null, new(errorBuilder), RequiresRestore: false);
            }

            var (result, requiresRestore) = GetArtifactRestoreResult(artifactReference);
            return new(file, null, artifactReference, result, RequiresRestore: requiresRestore);
        }

        private ArtifactResolutionInfo GetArtifactRestoreResult(BicepSourceFile sourceFile, IArtifactReferenceSyntax referenceSyntax)
        {
            if (!dispatcher.TryGetArtifactReference(referenceSyntax, sourceFile.FileUri).IsSuccess(out var artifactReference, out var errorBuilder))
            {
                // artifact reference is not valid
                return new(sourceFile, referenceSyntax, null, new(errorBuilder), RequiresRestore: false);
            }

            var (result, requiresRestore) = GetArtifactRestoreResult(artifactReference);

            return new(sourceFile, referenceSyntax, artifactReference, result, RequiresRestore: requiresRestore);
        }

        private (ResultWithDiagnostic<Uri> result, bool requiresRestore) GetArtifactRestoreResult(ArtifactReference artifactReference)
        {
            if (!dispatcher.TryGetLocalArtifactEntryPointUri(artifactReference).IsSuccess(out var artifactFileUri, out var artifactGetPathFailureBuilder))
            {
                // invalid artifact reference - exit early to show the user the diagnostic
                return (new(artifactGetPathFailureBuilder), requiresRestore: false);
            }

            if (forceRestore)
            {
                //override the status to force restore
                return (new(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference)), requiresRestore: true);
            }

            var restoreStatus = dispatcher.GetArtifactRestoreStatus(artifactReference, out var restoreErrorBuilder);
            switch (restoreStatus)
            {
                case ArtifactRestoreStatus.Unknown:
                    // we have not yet attempted to restore the module, so let's do it
                    return (new(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference)), requiresRestore: true);
                case ArtifactRestoreStatus.Failed:
                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    restoreErrorBuilder ??= new(x => x.ArtifactRestoreFailed(artifactReference.FullyQualifiedReference));
                    return (new(restoreErrorBuilder), requiresRestore: false);
                default:
                    break;
            }

            return (new(artifactFileUri), requiresRestore: false);
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.TryUnwrap())
                .WhereNotNull()
                .SelectMany(sourceFile => GetArtifactReferenceDeclarations(sourceFile)
                    .Select(x => this.artifactLookup.TryGetValue(x)?.Result.TryUnwrap())
                    .WhereNotNull()
                    .Select(uri => this.fileResultByUri.TryGetValue(uri)?.TryUnwrap())
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (statement, info) in artifactLookup)
            {
                if (statement.GetArtifactType() == ArtifactType.Module &&
                    info.Result.IsSuccess(out var fileUri) &&
                    fileResultByUri[fileUri].IsSuccess(out var sourceFile) &&
                    cycles.TryGetValue(sourceFile, out var cycle))
                {
                    ResultWithDiagnostic<Uri> result = cycle switch {
                        { Length: 1 } when cycle[0] is BicepParamFile paramFile => new(x => x.CyclicParametersSelfReference()),
                        { Length: 1 } => new(x => x.CyclicModuleSelfReference()),
                        // the error message is generic so it should work for either bicep module or params
                        _ => new(x => x.CyclicFile(cycle.Select(u => u.FileUri.LocalPath))),
                    };

                    // overwrite to add the cycle error
                    artifactLookup[statement] = info with { Result = result };
                }
            }

            return sourceFileGraph;
        }

        private static IEnumerable<IArtifactReferenceSyntax> GetArtifactReferenceDeclarations(ISourceFile sourceFile) => sourceFile switch
        {
            BicepSourceFile x => GetArtifactReferences(x.ProgramSyntax),
            _ => [],
        };

        private static IEnumerable<IArtifactReferenceSyntax> GetArtifactReferences(ProgramSyntax program)
            => program.Declarations.OfType<IArtifactReferenceSyntax>();
    }
}

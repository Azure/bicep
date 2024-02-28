// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;
using System.Linq;
using System.Linq.Expressions;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGroupingBuilder
    {
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher dispatcher;
        private readonly IReadOnlyWorkspace workspace;

        private readonly Dictionary<Uri, ResultWithDiagnostic<ISourceFile>> fileResultByUri;
        private readonly ConcurrentDictionary<BicepSourceFile, Dictionary<IArtifactReferenceSyntax, Result<ArtifactUriResolution, UriResolutionError>>> ArtifactResolutionPerFileBySyntax;

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
            this.ArtifactResolutionPerFileBySyntax = new();
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
            this.ArtifactResolutionPerFileBySyntax = new(current.ArtifactResolutionPerFileBySyntax.Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value.ToDictionary(p => p.Key, p => p.Value))));
            this.fileResultByUri = current.FileResultByUri.Where(x => x.Value.TryUnwrap() is not null).ToDictionary(x => x.Key, x => x.Value);
            this.forceRestore = forceArtifactRestore;
        }

        public static SourceFileGrouping Build(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, Uri entryFileUri, IFeatureProviderFactory featuresFactory, bool forceModulesRestore = false)
        {
            var builder = new SourceFileGroupingBuilder(fileResolver, moduleDispatcher, workspace, forceModulesRestore);

            return builder.Build(entryFileUri, featuresFactory);
        }

        public static SourceFileGrouping Rebuild(IFeatureProviderFactory featuresFactory, IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, SourceFileGrouping current)
        {
            var builder = new SourceFileGroupingBuilder(current.FileResolver, moduleDispatcher, workspace, current);
            var isParamsFile = current.EntryPoint is BicepParamFile;
            var modulesToRestore = current.GetArtifactsToRestore().ToHashSet();

            foreach (var (module, sourceFile) in modulesToRestore)
            {
                builder.ArtifactResolutionPerFileBySyntax[sourceFile].Remove(module);
            }

            // Rebuild source files that contain external module references restored during the initial build.
            var sourceFilesToRebuild = current.SourceFiles.OfType<BicepSourceFile>()
                .Where(sourceFile
                    => GetArtifactReferenceDeclarations(sourceFile)
                        .Any(moduleDeclaration
                            => modulesToRestore.Contains(new ArtifactResolutionInfo(moduleDeclaration, sourceFile))))
                .ToImmutableHashSet()
                .SelectMany(sourceFile => current.GetFilesDependingOn(sourceFile))
                .ToImmutableHashSet();

            return builder.Build(current.EntryPoint.FileUri, featuresFactory, sourceFilesToRebuild);
        }

        private SourceFileGrouping Build(Uri entryFileUri, IFeatureProviderFactory featuresFactory, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild = null)
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
                ArtifactResolutionPerFileBySyntax.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableDictionary()),
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
                var result = GetArtifactRestoreResult(file.FileUri, restorable);
                ArtifactResolutionPerFileBySyntax.GetOrAdd(file, f => new())[restorable] = result;

                if (!result.IsSuccess(out var resolution))
                {
                    continue;
                }

                if (restorable is not ProviderDeclarationSyntax)
                {
                    var (childArtifactReference, artifactUri) = resolution;

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

        private Result<ArtifactUriResolution, UriResolutionError> GetArtifactRestoreResult(Uri parentFileUri, IArtifactReferenceSyntax referenceSyntax)
        {
            if (!dispatcher.TryGetArtifactReference(referenceSyntax, parentFileUri).IsSuccess(out var artifactReference, out var referenceResolutionError))
            {
                // module reference is not valid
                return new Result<ArtifactUriResolution, UriResolutionError>(new UriResolutionError(referenceResolutionError, false));
            }

            if (!dispatcher.TryGetLocalArtifactEntryPointUri(artifactReference).IsSuccess(out var artifactFileUri, out var artifactGetPathFailureBuilder))
            {
                return new Result<ArtifactUriResolution, UriResolutionError>(new UriResolutionError(artifactGetPathFailureBuilder, false));
            }

            if (forceRestore)
            {
                //override the status to force restore
                return new Result<ArtifactUriResolution, UriResolutionError>(new UriResolutionError(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference), true));
            }

            var restoreStatus = dispatcher.GetArtifactRestoreStatus(artifactReference, out var restoreErrorBuilder);
            switch (restoreStatus)
            {
                case ArtifactRestoreStatus.Unknown:
                    // we have not yet attempted to restore the module, so let's do it
                    return new Result<ArtifactUriResolution, UriResolutionError>(new UriResolutionError(x => x.ArtifactRequiresRestore(artifactReference.FullyQualifiedReference), true));
                case ArtifactRestoreStatus.Failed:
                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    return new Result<ArtifactUriResolution, UriResolutionError>(new UriResolutionError(restoreErrorBuilder ?? (x => x.ArtifactRestoreFailed(artifactReference.FullyQualifiedReference)), false));
                default:
                    break;
            }

            return new Result<ArtifactUriResolution, UriResolutionError>(new ArtifactUriResolution(artifactReference, artifactFileUri)); ;
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.TryUnwrap())
                .WhereNotNull()
                .SelectMany(sourceFile => GetReferenceSourceNodes(sourceFile)
                    .SelectMany(moduleDeclaration => this.ArtifactResolutionPerFileBySyntax.Values.Select(f => f.TryGetValue(moduleDeclaration)?.TryUnwrap()))
                    .WhereNotNull()
                    .Select(resolution => this.fileResultByUri[resolution.Uri].TryUnwrap())
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (file, artifactResolutionByModuleForFile) in ArtifactResolutionPerFileBySyntax)
            {
                foreach (var (statement, artifactResolutionResult) in artifactResolutionByModuleForFile)
                {
                    if (statement.GetArtifactType() == ArtifactType.Module &&
                        artifactResolutionResult.IsSuccess(out var artifactResolution) &&
                        fileResultByUri[artifactResolution.Uri].IsSuccess(out var sourceFile) &&
                        cycles.TryGetValue(sourceFile, out var cycle))
                    {
                        if (cycle.Length == 1)
                        {
                            var uriResolutionError = cycle[0] switch
                            {
                                BicepParamFile => new UriResolutionError(x => x.CyclicParametersSelfReference(), false),
                                _ => new UriResolutionError(x => x.CyclicModuleSelfReference(), false),
                            };
                            artifactResolutionByModuleForFile[statement] = new Result<ArtifactUriResolution, UriResolutionError>(uriResolutionError);
                        }
                        else
                        {
                            // the error message is generic so it should work for either bicep module or params
                            artifactResolutionByModuleForFile[statement] = new Result<ArtifactUriResolution, UriResolutionError>(
                                new UriResolutionError(x => x.CyclicFile(cycle.Select(u => u.FileUri.LocalPath)), false));
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

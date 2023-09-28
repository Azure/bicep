// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGroupingBuilder
    {
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IReadOnlyWorkspace workspace;

        private readonly Dictionary<Uri, ResultWithDiagnostic<ISourceFile>> fileResultByUri;
        private readonly ConcurrentDictionary<ISourceFile, Dictionary<IArtifactReferenceSyntax, Result<Uri, UriResolutionError>>> uriResultByArtifactReference;

        private readonly bool forceModulesRestore;

        private SourceFileGroupingBuilder(
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IReadOnlyWorkspace workspace,
            bool forceModulesRestore = false)
        {
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.uriResultByArtifactReference = new();
            this.fileResultByUri = new();
            this.forceModulesRestore = forceModulesRestore;
        }

        private SourceFileGroupingBuilder(
            IFileResolver fileResolver,
            IModuleDispatcher moduleDispatcher,
            IReadOnlyWorkspace workspace,
            SourceFileGrouping current,
            bool forceforceModulesRestore = false)
        {
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.uriResultByArtifactReference = new(current.UriResultByArtifactReference.Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value.ToDictionary(p => p.Key, p => p.Value))));
            this.fileResultByUri = current.FileResultByUri.Where(x => x.Value.TryUnwrap() is not null).ToDictionary(x => x.Key, x => x.Value);
            this.forceModulesRestore = forceforceModulesRestore;
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
            var modulesToRestore = current.GetModulesToRestore().ToHashSet();

            foreach (var (module, sourceFile) in modulesToRestore)
            {
                builder.uriResultByArtifactReference[sourceFile].Remove(module);
            }

            // Rebuild source files that contain external module references restored during the inital build.
            var sourceFilesToRebuild = current.SourceFiles
                .Where(sourceFile
                    => GetModuleDeclarations(sourceFile)
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
                uriResultByArtifactReference.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableDictionary()),
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
            var features = featureProviderFactory.GetFeatureProvider(file.FileUri);
            foreach (var restorable in file.ProgramSyntax.Children.OfType<IArtifactReferenceSyntax>())
            {
                // NOTE(asilverman): The below check is ugly but temporary until we have a better way to
                // handle dynamic type loading in a way that is decoupled from modules.
                if (restorable is ProviderDeclarationSyntax providerImport &&
                    (providerImport.Specification.Name != AzNamespaceType.BuiltInName || !features.DynamicTypeLoadingEnabled))
                {
                    continue;
                }
                var (childModuleReference, uriResult) = GetModuleRestoreResult(file.FileUri, restorable);

                uriResultByArtifactReference.GetOrAdd(file, f => new())[restorable] = uriResult;

                if (!uriResult.IsSuccess(out var artifactUri))
                {
                    continue;
                }

                if (!fileResultByUri.TryGetValue(artifactUri, out var childResult) ||
                    (childResult.IsSuccess(out var childFile) && sourceFilesToRebuild is not null && sourceFilesToRebuild.Contains(childFile)))
                {
                    // only recurse if we've not seen this file before - to avoid infinite loops
                    childResult = PopulateRecursive(artifactUri, childModuleReference, sourceFilesToRebuild, featureProviderFactory);
                }

                fileResultByUri[artifactUri] = childResult;
            }
        }

        private (ArtifactReference? reference, Result<Uri, UriResolutionError> result) GetModuleRestoreResult(Uri parentFileUri, IArtifactReferenceSyntax foreignTemplateReference)
        {
            if (!moduleDispatcher.TryGetModuleReference(foreignTemplateReference, parentFileUri).IsSuccess(out var moduleReference, out var referenceResolutionError))
            {
                // module reference is not valid
                return (null, new(new UriResolutionError(referenceResolutionError, false)));
            }

            if (!moduleDispatcher.TryGetLocalModuleEntryPointUri(moduleReference).IsSuccess(out var moduleFileUri, out var moduleGetPathFailureBuilder))
            {
                return (moduleReference, new(new UriResolutionError(moduleGetPathFailureBuilder, false)));
            }

            if (forceModulesRestore)
            {
                //override the status to force restore
                return (moduleReference, new(new UriResolutionError(x => x.ModuleRequiresRestore(moduleReference.FullyQualifiedReference), true)));
            }

            var restoreStatus = moduleDispatcher.GetArtifactRestoreStatus(moduleReference, out var restoreErrorBuilder);
            switch (restoreStatus)
            {
                case ArtifactRestoreStatus.Unknown:
                    // we have not yet attempted to restore the module, so let's do it
                    return (moduleReference, new(new UriResolutionError(x => x.ModuleRequiresRestore(moduleReference.FullyQualifiedReference), true)));
                case ArtifactRestoreStatus.Failed:
                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    return (moduleReference, new(new UriResolutionError(restoreErrorBuilder ?? (x => x.ModuleRestoreFailed(moduleReference.FullyQualifiedReference)), false)));
                default:
                    break;
            }

            return (moduleReference, new(moduleFileUri));
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.TryUnwrap())
                .WhereNotNull()
                .SelectMany(sourceFile => GetReferenceSourceNodes(sourceFile)
                    .SelectMany(moduleDeclaration => this.uriResultByArtifactReference.Values.Select(f => f.TryGetValue(moduleDeclaration)?.TryUnwrap()))
                    .WhereNotNull()
                    .Select(fileUri => this.fileResultByUri[fileUri].TryUnwrap())
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (file, uriResultByModuleForFile) in uriResultByArtifactReference)
            {
                foreach (var (statement, urlResult) in uriResultByModuleForFile)
                {
                    if (urlResult.IsSuccess(out var fileUri) &&
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
            BicepFile bicepFile => bicepFile.ProgramSyntax.Children.OfType<IArtifactReferenceSyntax>(),
            BicepParamFile paramsFile => paramsFile.ProgramSyntax.Children.OfType<IArtifactReferenceSyntax>(),
            _ => Enumerable.Empty<IArtifactReferenceSyntax>(),
        };

        private static IEnumerable<ModuleDeclarationSyntax> GetModuleDeclarations(ISourceFile sourceFile) => sourceFile is BicepFile bicepFile
            ? bicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>()
            : Enumerable.Empty<ModuleDeclarationSyntax>();
    }
}

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
using Bicep.Core.Modules;
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

        private readonly Dictionary<Uri, FileResolutionResult> fileResultByUri;
        private readonly ConcurrentDictionary<ISourceFile, Dictionary<IForeignArtifactReference, UriResolutionResult>> uriResultByModule;

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
            this.uriResultByModule = new();
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
            this.uriResultByModule = new(current.UriResultByModule.Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value.ToDictionary(p => p.Key, p => p.Value))));
            this.fileResultByUri = current.FileResultByUri.Where(x => x.Value.File is not null).ToDictionary(x => x.Key, x => x.Value);
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
            var isParamsFile = current.FileResultByUri[current.EntryFileUri].File is BicepParamFile;
            var modulesToRestore = current.GetModulesToRestore().ToHashSet();

            foreach (var (module, sourceFile) in modulesToRestore)
            {
                builder.uriResultByModule[sourceFile].Remove(module);
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

            if (fileResult.File is null)
            {
                // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                var failureBuilder = fileResult.ErrorBuilder ?? throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to provide failure diagnostics");
                var diagnostic = failureBuilder(ForDocumentStart());

                throw new ErrorDiagnosticException(diagnostic);
            }

            var sourceFileDependencies = this.ReportFailuresForCycles();

            return new SourceFileGrouping(
                fileResolver,
                entryFileUri,
                fileResultByUri.ToImmutableDictionary(),
                uriResultByModule.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableDictionary()),
                sourceFileDependencies.InvertLookup().ToImmutableDictionary());
        }

        private FileResolutionResult GetFileResolutionResult(Uri fileUri, ArtifactReference? moduleReference)
        {
            if (workspace.TryGetSourceFile(fileUri, out var sourceFile))
            {
                return new(fileUri, null, sourceFile);
            }

            if (!fileResolver.TryRead(fileUri, out var fileContents, out var failureBuilder))
            {
                return new(fileUri, failureBuilder, null);
            }

            sourceFile = SourceFileFactory.CreateSourceFile(fileUri, fileContents, moduleReference);

            return new(fileUri, null, sourceFile);
        }

        private FileResolutionResult GetFileResolutionResultWithCaching(Uri fileUri, ArtifactReference? moduleReference)
        {
            if (!fileResultByUri.TryGetValue(fileUri, out var resolutionResult))
            {
                resolutionResult = GetFileResolutionResult(fileUri, moduleReference);
                fileResultByUri[fileUri] = resolutionResult;
            }

            return resolutionResult;
        }

        private FileResolutionResult PopulateRecursive(Uri fileUri, ArtifactReference? moduleReference, ImmutableHashSet<ISourceFile>? sourceFileToRebuild, IFeatureProviderFactory featuresFactory)
        {
            var fileResult = GetFileResolutionResultWithCaching(fileUri, moduleReference);
            var features = featuresFactory.GetFeatureProvider(fileUri);
            switch (fileResult.File)
            {
                case BicepFile bicepFile:
                    {
                        foreach (var restorable in bicepFile.ProgramSyntax.Children.OfType<IForeignArtifactReference>())
                        {
                            // NOTE(asilverman): The below check is ugly but temporary until we have a better way to
                            // handle dynamic type loading in a way that is decoupled from modules.
                            if (restorable is ProviderDeclarationSyntax providerImport &&
                                (providerImport.Specification.Name != AzNamespaceType.BuiltInName || !features.DynamicTypeLoadingEnabled))
                            {
                                continue;
                            }
                            var (childModuleReference, uriResult) = GetModuleRestoreResult(fileUri, restorable);

                            uriResultByModule.GetOrAdd(bicepFile, f => new())[restorable] = uriResult;

                            if (uriResult.FileUri is null)
                            {
                                continue;
                            }

                            if (!fileResultByUri.TryGetValue(uriResult.FileUri, out var childResult) ||
                                (childResult.File is not null && sourceFileToRebuild is not null && sourceFileToRebuild.Contains(childResult.File)))
                            {
                                // only recurse if we've not seen this file before - to avoid infinite loops
                                childResult = PopulateRecursive(uriResult.FileUri, childModuleReference, sourceFileToRebuild, featuresFactory);
                            }

                            fileResultByUri[uriResult.FileUri] = childResult;
                        }
                        break;
                    }
                case BicepParamFile paramsFile:
                    {
                        foreach (var usingDeclaration in paramsFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>())
                        {
                            if (!SyntaxHelper.TryGetForeignTemplatePath(usingDeclaration, out var usingFilePath, out var errorBuilder))
                            {
                                uriResultByModule.GetOrAdd(paramsFile, f => new())[usingDeclaration] = new(null, false, errorBuilder);
                                continue;
                            }

                            if (fileResolver.TryResolveFilePath(fileUri, usingFilePath) is not { } usingFileUri)
                            {
                                uriResultByModule.GetOrAdd(paramsFile, f => new())[usingDeclaration] = new(null, false, x => x.FilePathCouldNotBeResolved(usingFilePath, fileUri.LocalPath));
                                continue;
                            }

                            uriResultByModule.GetOrAdd(paramsFile, f => new())[usingDeclaration] = new(usingFileUri, false, null);

                            if (!fileResultByUri.TryGetValue(usingFileUri, out var childResult))
                            {
                                // only recurse if we've not seen this file before - to avoid infinite loops
                                childResult = PopulateRecursive(usingFileUri, null, sourceFileToRebuild, featuresFactory);
                            }

                            fileResultByUri[usingFileUri] = childResult;
                        }
                        break;
                    }
            }

            return fileResult;
        }

        private (ArtifactReference? reference, UriResolutionResult result) GetModuleRestoreResult(Uri parentFileUri, IForeignArtifactReference foreignTemplateReference)
        {
            if (!moduleDispatcher.TryGetModuleReference(foreignTemplateReference, parentFileUri, out var moduleReference, out var referenceResolutionError))
            {
                // module reference is not valid
                return (null, new(null, false, referenceResolutionError));
            }

            if (!moduleDispatcher.TryGetLocalModuleEntryPointUri(moduleReference, out var moduleFileUri, out var moduleGetPathFailureBuilder))
            {
                return (moduleReference, new(null, false, moduleGetPathFailureBuilder));
            }

            if (forceModulesRestore)
            {
                //override the status to force restore
                return (moduleReference, new(moduleFileUri, true, x => x.ModuleRequiresRestore(moduleReference.FullyQualifiedReference)));
            }

            var restoreStatus = moduleDispatcher.GetArtifactRestoreStatus(moduleReference, out var restoreErrorBuilder);
            switch (restoreStatus)
            {
                case ArtifactRestoreStatus.Unknown:
                    // we have not yet attempted to restore the module, so let's do it
                    return (moduleReference, new(moduleFileUri, true, x => x.ModuleRequiresRestore(moduleReference.FullyQualifiedReference)));
                case ArtifactRestoreStatus.Failed:
                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    return (moduleReference, new(null, false, restoreErrorBuilder));
                default:
                    break;
            }

            return (moduleReference, new(moduleFileUri, false, null));
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.File)
                .WhereNotNull()
                .SelectMany(sourceFile => GetReferenceSourceNodes(sourceFile)
                    .SelectMany(moduleDeclaration => this.uriResultByModule.Values.Select(f => f.TryGetValue(moduleDeclaration)?.FileUri))
                    .Select(fileUri => fileUri != null ? this.fileResultByUri[fileUri].File : null)
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (file, uriResultByModuleForFile) in uriResultByModule)
            {
                foreach (var (statement, urlResult) in uriResultByModuleForFile)
                {
                    if (urlResult.FileUri is not null &&
                        fileResultByUri[urlResult.FileUri].File is { } sourceFile &&
                        cycles.TryGetValue(sourceFile, out var cycle))
                    {
                        if (cycle.Length == 1)
                        {
                            uriResultByModuleForFile[statement] = cycle[0] switch
                            {
                                BicepParamFile => new(null, false, x => x.CyclicParametersSelfReference()),
                                _ => new(null, false, x => x.CyclicModuleSelfReference())
                            };
                        }
                        else
                        {
                            // the error message is generic so it should work for either bicep module or params
                            uriResultByModuleForFile[statement] = new(null, false, x => x.CyclicFile(cycle.Select(u => u.FileUri.LocalPath)));
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
        private static IEnumerable<IForeignArtifactReference> GetReferenceSourceNodes(ISourceFile sourceFile) => sourceFile switch
        {
            BicepFile bicepFile => bicepFile.ProgramSyntax.Children.OfType<IForeignArtifactReference>(),
            BicepParamFile paramsFile => paramsFile.ProgramSyntax.Children.OfType<IForeignArtifactReference>(),
            _ => Enumerable.Empty<IForeignArtifactReference>(),
        };

        private static IEnumerable<ModuleDeclarationSyntax> GetModuleDeclarations(ISourceFile sourceFile) => sourceFile is BicepFile bicepFile
            ? bicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>()
            : Enumerable.Empty<ModuleDeclarationSyntax>();
    }
}

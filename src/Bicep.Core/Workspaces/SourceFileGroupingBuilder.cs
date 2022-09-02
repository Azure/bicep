// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGroupingBuilder
    {
        private readonly IFileResolver fileResolver;
        private readonly IModuleDispatcher moduleDispatcher;
        private readonly IReadOnlyWorkspace workspace;

        private readonly Dictionary<Uri, FileResolutionResult> fileResultByUri;
        private readonly Dictionary<StatementSyntax, UriResolutionResult> uriResultByModule;

        private readonly RootConfiguration configuration;

        private readonly bool forceModulesRestore;

        private SourceFileGroupingBuilder(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, RootConfiguration configuration, bool forceforceModulesRestore = false)
        {
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.configuration = configuration;
            this.uriResultByModule = new();
            this.fileResultByUri = new();
            this.forceModulesRestore = forceforceModulesRestore;
        }

        private SourceFileGroupingBuilder(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, SourceFileGrouping current, RootConfiguration configuration, bool forceforceModulesRestore = false)
        {
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.configuration = configuration;
            this.uriResultByModule = new(current.UriResultByModule);
            this.fileResultByUri = current.FileResultByUri.Where(x => x.Value.File is not null).ToDictionary(x => x.Key, x => x.Value);
            this.forceModulesRestore = forceforceModulesRestore;
        }

        public static SourceFileGrouping Build(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, Uri entryFileUri, RootConfiguration configuration, bool forceModulesRestore = false)
        {
            var builder = new SourceFileGroupingBuilder(fileResolver, moduleDispatcher, workspace, configuration, forceModulesRestore);

            return builder.Build(entryFileUri);
        }

        public static SourceFileGrouping Rebuild(IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, SourceFileGrouping current, RootConfiguration configuration)
        {
            var builder = new SourceFileGroupingBuilder(current.FileResolver, moduleDispatcher, workspace, current, configuration);
            var isParamsFile = current.FileResultByUri[current.EntryFileUri].File is BicepParamFile;
            var modulesToRestore = current.GetModulesToRestore().ToHashSet();

            foreach (var module in modulesToRestore)
            {
                builder.uriResultByModule.Remove(module);
            }

            // Rebuild source files that contains external module references restored during the inital build.
            var sourceFilesToRebuild = current.SourceFiles
                .Where(sourceFile => GetModuleDeclarations(sourceFile).Any(moduleDeclaration => modulesToRestore.Contains(moduleDeclaration)))
                .ToImmutableHashSet();

            return builder.Build(current.EntryPoint.FileUri, sourceFilesToRebuild);
        }

        private SourceFileGrouping Build(Uri entryFileUri, ImmutableHashSet<ISourceFile>? sourceFilesToRebuild = null)
        {
            var fileResult = this.PopulateRecursive(entryFileUri, null, sourceFilesToRebuild);

            if (fileResult.File is null)
            {
                // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                var failureBuilder = fileResult.ErrorBuilder ?? throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to provide failure diagnostics");
                var diagnostic = failureBuilder(ForPosition(new TextSpan(0, 0)));

                throw new ErrorDiagnosticException(diagnostic);
            }

            var sourceFileDependencies = this.ReportFailuresForCycles();

            return new SourceFileGrouping(
                fileResolver,
                entryFileUri,
                fileResultByUri.ToImmutableDictionary(),
                uriResultByModule.ToImmutableDictionary(),
                sourceFileDependencies.InvertLookup().ToImmutableDictionary());
        }

        private FileResolutionResult GetFileResolutionResult(Uri fileUri, ModuleReference? moduleReference)
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

        private FileResolutionResult GetFileResolutionResultWithCaching(Uri fileUri, ModuleReference? moduleReference)
        {
            if (!fileResultByUri.TryGetValue(fileUri, out var resolutionResult))
            {
                resolutionResult = GetFileResolutionResult(fileUri, moduleReference);
                fileResultByUri[fileUri] = resolutionResult;
            }

            return resolutionResult;
        }

        private FileResolutionResult PopulateRecursive(Uri fileUri, ModuleReference? moduleReference, ImmutableHashSet<ISourceFile>? sourceFileToRebuild)
        {
            var fileResult = GetFileResolutionResultWithCaching(fileUri, moduleReference);

            switch (fileResult.File)
            {
                case BicepFile bicepFile:
                {
                    foreach (var childModule in bicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>())
                    {
                        var (childModuleReference, uriResult) = GetModuleRestoreResult(fileUri, childModule);

                        uriResultByModule[childModule] = uriResult;
                        if (uriResult.FileUri is null)
                        {
                            continue;
                        }

                        if (!fileResultByUri.TryGetValue(uriResult.FileUri, out var childResult) ||
                            (childResult.File is not null && sourceFileToRebuild is not null && sourceFileToRebuild.Contains(childResult.File)))
                        {
                            // only recurse if we've not seen this file before - to avoid infinite loops
                            childResult = PopulateRecursive(uriResult.FileUri, childModuleReference, sourceFileToRebuild);
                        }

                        fileResultByUri[uriResult.FileUri] = childResult;
                    }
                    break;
                }
                case BicepParamFile paramsFile:
                {
                    foreach (var usingDeclaration in paramsFile.ProgramSyntax.Children.OfType<UsingDeclarationSyntax>())
                    {
                        if (SyntaxHelper.TryGetUsingPath(usingDeclaration, out var errorBuilder) is not {} usingFilePath)
                        {
                            uriResultByModule[usingDeclaration] = new(usingDeclaration, null, false, errorBuilder);
                            continue;
                        }

                        if (fileResolver.TryResolveFilePath(fileUri, usingFilePath) is not {} usingFileUri)
                        {
                            uriResultByModule[usingDeclaration] = new(usingDeclaration, null, false, x => x.FilePathCouldNotBeResolved(usingFilePath, fileUri.LocalPath));
                            continue;
                        }

                        uriResultByModule[usingDeclaration] = new(usingDeclaration, usingFileUri, false, null);

                        if (!fileResultByUri.TryGetValue(usingFileUri, out var childResult))
                        {
                            // only recurse if we've not seen this file before - to avoid infinite loops
                            childResult = PopulateRecursive(usingFileUri, null, sourceFileToRebuild);
                        }

                        fileResultByUri[usingFileUri] = childResult;
                    }
                    break;
                }
            }

            return fileResult;
        }

        private (ModuleReference? reference, UriResolutionResult result) GetModuleRestoreResult(Uri parentFileUri, ModuleDeclarationSyntax module)
        {
            if (moduleDispatcher.TryGetModuleReference(module, configuration, out var referenceResolutionError) is not {} moduleReference)
            {
                // module reference is not valid
                return (null, new(module, null, false, referenceResolutionError));
            }

            var moduleFileUri = moduleDispatcher.TryGetLocalModuleEntryPointUri(parentFileUri, moduleReference, configuration, out var moduleGetPathFailureBuilder);
            if (moduleFileUri is null)
            {
                return (moduleReference, new(module, null, false, moduleGetPathFailureBuilder));
            }

            if (forceModulesRestore)
            {
                //override the status to force restore
                return (moduleReference, new(module, moduleFileUri, true, x => x.ModuleRequiresRestore(moduleReference.FullyQualifiedReference)));
            }

            var restoreStatus = moduleDispatcher.GetModuleRestoreStatus(moduleReference, configuration, out var restoreErrorBuilder);
            switch (restoreStatus)
            {
                case ModuleRestoreStatus.Unknown:
                    // we have not yet attempted to restore the module, so let's do it
                    return (moduleReference, new(module, moduleFileUri, true, x => x.ModuleRequiresRestore(moduleReference.FullyQualifiedReference)));
                case ModuleRestoreStatus.Failed:
                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    return (moduleReference, new(module, null, false, restoreErrorBuilder));
                default:
                    break;
            }

            return (moduleReference, new(module, moduleFileUri, false, null));
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.fileResultByUri.Values
                .Select(x => x.File)
                .WhereNotNull()
                .SelectMany(sourceFile => GetModuleDeclarations(sourceFile)
                    .Select(moduleDeclaration => this.uriResultByModule.TryGetValue(moduleDeclaration)?.FileUri)
                    .Select(fileUri => fileUri != null ? this.fileResultByUri[fileUri].File : null)
                    .WhereNotNull()
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (statement, urlResult) in uriResultByModule)
            {
                if (urlResult.FileUri is not null &&
                    fileResultByUri[urlResult.FileUri].File is {} sourceFile &&
                    cycles.TryGetValue(sourceFile, out var cycle))
                {
                    if (cycle.Length == 1)
                    {
                        uriResultByModule[statement] = new(statement, null, false, x => x.CyclicModuleSelfReference());
                    }
                    else
                    {
                        uriResultByModule[statement] = new(statement, null, false, x => x.CyclicModule(cycle.Select(u => u.FileUri.LocalPath)));
                    }
                }
            }

            return sourceFileGraph;
        }

        private static IEnumerable<ModuleDeclarationSyntax> GetModuleDeclarations(ISourceFile sourceFile) => sourceFile is BicepFile bicepFile
            ? bicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>()
            : Enumerable.Empty<ModuleDeclarationSyntax>();
    }
}

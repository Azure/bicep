// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        private readonly Dictionary<ModuleDeclarationSyntax, ISourceFile> sourceFilesByModuleDeclaration;
        private readonly Dictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> errorBuildersByModuleDeclaration;

        private readonly HashSet<ModuleDeclarationSyntax> modulesToRestore;

        // uri -> successfully loaded syntax tree
        private readonly Dictionary<Uri, ISourceFile> sourceFilesByUri;

        // uri -> syntax tree load failure 
        private readonly Dictionary<Uri, ErrorBuilderDelegate> errorBuildersByUri;

        private SourceFileGroupingBuilder(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace)
        {
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.workspace = workspace;
            this.sourceFilesByModuleDeclaration = new();
            this.errorBuildersByModuleDeclaration = new();
            this.modulesToRestore = new();
            this.sourceFilesByUri = new();
            this.errorBuildersByUri = new();
        }

        private SourceFileGroupingBuilder(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, SourceFileGrouping current)
        {
            this.fileResolver = fileResolver;
            this.moduleDispatcher = moduleDispatcher;
            this.workspace = workspace;

            this.sourceFilesByModuleDeclaration = new(current.SourceFilesByModuleDeclaration);
            this.errorBuildersByModuleDeclaration = new(current.ErrorBuildersByModuleDeclaration);

            this.modulesToRestore = new();
            
            this.sourceFilesByUri = current.SourceFiles.ToDictionary(tree => tree.FileUri);
            this.errorBuildersByUri = new();
        }

        public static SourceFileGrouping Build(IFileResolver fileResolver, IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, Uri entryFileUri)
        {
            var builder = new SourceFileGroupingBuilder(fileResolver, moduleDispatcher, workspace);

            return builder.Build(entryFileUri);
        }

        public static SourceFileGrouping Rebuild(IModuleDispatcher moduleDispatcher, IReadOnlyWorkspace workspace, SourceFileGrouping current)
        {
            var builder = new SourceFileGroupingBuilder(current.FileResolver, moduleDispatcher, workspace, current);

            foreach (var module in current.ModulesToRestore)
            {
                builder.sourceFilesByModuleDeclaration.Remove(module);
                builder.errorBuildersByModuleDeclaration.Remove(module);
            }

            return builder.Build(current.EntryPoint.FileUri);
        }

        private SourceFileGrouping Build(Uri entryFileUri)
        {
            var sourceFile = this.PopulateRecursive(entryFileUri, null, out var entryPointLoadFailureBuilder);

            if (sourceFile is null)
            {
                // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                var failureBuilder = entryPointLoadFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to provide failure diagnostics");
                var diagnostic = failureBuilder(ForPosition(new TextSpan(0, 0)));

                throw new ErrorDiagnosticException(diagnostic);
            }

            if (sourceFile is not BicepFile entryPoint)
            {
                throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to return a BicepFile.");
            }

            var sourceFileDependencies = this.ReportFailuresForCycles();

            return new SourceFileGrouping(
                fileResolver,
                entryPoint,
                sourceFilesByUri.Values.ToImmutableHashSet(),
                sourceFilesByModuleDeclaration.ToImmutableDictionary(),
                sourceFileDependencies.InvertLookup().ToImmutableDictionary(),
                errorBuildersByModuleDeclaration.ToImmutableDictionary(),
                modulesToRestore.ToImmutableHashSet());
        }

        private ISourceFile? TryGetSourceFile(Uri fileUri, ModuleReference? moduleReference, out ErrorBuilderDelegate? failureBuilder)
        {
            if (workspace.TryGetSourceFile(fileUri, out var sourceFile))
            {
                failureBuilder = null;
                sourceFilesByUri[fileUri] = sourceFile;
                return sourceFile;
            }

            if (sourceFilesByUri.TryGetValue(fileUri, out sourceFile))
            {
                failureBuilder = null;
                return sourceFile;
            }

            if (errorBuildersByUri.TryGetValue(fileUri, out failureBuilder))
            {
                return null;
            }

            if (!fileResolver.TryRead(fileUri, out var fileContents, out failureBuilder))
            {
                errorBuildersByUri[fileUri] = failureBuilder;
                return null;
            }

            failureBuilder = null;
            return AddSourceFile(fileUri, fileContents, moduleReference);
        }

        private ISourceFile AddSourceFile(Uri fileUri, string fileContents, ModuleReference? moduleReference)
        {
            var sourceFile = SourceFileFactory.CreateSourceFile(fileUri, fileContents, moduleReference);

            sourceFilesByUri[fileUri] = sourceFile;

            return sourceFile;
        }

        private ISourceFile? PopulateRecursive(Uri fileUri, ModuleReference? moduleReference, out ErrorBuilderDelegate? failureBuilder)
        {
            var sourceFile = this.TryGetSourceFile(fileUri, moduleReference, out var getSourceFileFailureBuilder);

            if (sourceFile is null)
            {
                failureBuilder = getSourceFileFailureBuilder;
                return null;
            }

            if (sourceFile is not BicepFile bicepFile)
            {
                // The source file must be a JSON template.
                failureBuilder = null;
                return sourceFile;
            }

            foreach (var childModule in GetModuleDeclarations(bicepFile))
            {
                var childModuleReference = this.moduleDispatcher.TryGetModuleReference(childModule, out var parseReferenceFailureBuilder);
                if(childModuleReference is null)
                {
                    // module reference is not valid
                    errorBuildersByModuleDeclaration[childModule] = parseReferenceFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(IModuleDispatcher.TryGetModuleReference)} to provide failure diagnostics.");
                    continue;
                }

                var restoreStatus = this.moduleDispatcher.GetModuleRestoreStatus(childModuleReference, out var restoreErrorBuilder);
                if (restoreStatus != ModuleRestoreStatus.Succeeded)
                {
                    if(restoreStatus == ModuleRestoreStatus.Unknown)
                    {
                        // we have not yet attempted to restore the module, so let's do it
                        modulesToRestore.Add(childModule);
                    }

                    // the module has not yet been restored or restore failed
                    // in either case, set the error
                    errorBuildersByModuleDeclaration[childModule] = restoreErrorBuilder ?? throw new InvalidOperationException($"Expected {nameof(IModuleDispatcher.GetModuleRestoreStatus)} to provide failure diagnostics.");
                    continue;
                }

                var childModuleFileUri = this.moduleDispatcher.TryGetLocalModuleEntryPointUri(fileUri, childModuleReference, out var moduleGetPathFailureBuilder);
                if (childModuleFileUri is null)
                {
                    // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                    errorBuildersByModuleDeclaration[childModule] = moduleGetPathFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(moduleDispatcher.TryGetLocalModuleEntryPointUri)} to provide failure diagnostics.");
                    continue;
                }

                // only recurse if we've not seen this module before - to avoid infinite loops
                if (!sourceFilesByUri.TryGetValue(childModuleFileUri, out var childModuleFile))
                {
                    childModuleFile = PopulateRecursive(childModuleFileUri, childModuleReference, out var modulePopulateFailureBuilder);
                    
                    if (childModuleFile is null)
                    {
                        // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                        errorBuildersByModuleDeclaration[childModule] = modulePopulateFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to provide failure diagnostics.");
                        continue;
                    }
                }

                if (childModuleFile is null)
                {
                    continue;
                }

                sourceFilesByModuleDeclaration[childModule] = childModuleFile;
            }

            failureBuilder = null;
            return sourceFile;
        }

        private ILookup<ISourceFile, ISourceFile> ReportFailuresForCycles()
        {
            var sourceFileGraph = this.sourceFilesByUri.Values
                .SelectMany(sourceFile => GetModuleDeclarations(sourceFile)
                    .Where(this.sourceFilesByModuleDeclaration.ContainsKey)
                    .Select(moduleDeclaration => this.sourceFilesByModuleDeclaration[moduleDeclaration])
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var (moduleDeclaration, moduleSourceFile) in sourceFilesByModuleDeclaration)
            {
                if (cycles.TryGetValue(moduleSourceFile, out var cycle))
                {
                    if (cycle.Length == 1)
                    {
                        errorBuildersByModuleDeclaration[moduleDeclaration] = x => x.CyclicModuleSelfReference();
                    }
                    else
                    {
                        errorBuildersByModuleDeclaration[moduleDeclaration] = x => x.CyclicModule(cycle.Select(x => x.FileUri.LocalPath));
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

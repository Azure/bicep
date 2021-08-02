// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
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

        private readonly HashSet<ModuleDeclarationSyntax> modulesToInit;

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
            this.modulesToInit = new();
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

            this.modulesToInit = new();
            
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
            var sourceFile = this.PopulateRecursive(entryFileUri, isEntryFile: true, out var entryPointLoadFailureBuilder);

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

            this.ReportFailuresForCycles();

            return new SourceFileGrouping(
                fileResolver,
                entryPoint,
                sourceFilesByUri.Values.ToImmutableHashSet(),
                sourceFilesByModuleDeclaration.ToImmutableDictionary(),
                errorBuildersByModuleDeclaration.ToImmutableDictionary(),
                modulesToInit.ToImmutableHashSet());
        }

        private ISourceFile? TryGetSourceFile(Uri fileUri, bool isEntryFile, out ErrorBuilderDelegate? failureBuilder)
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
            return AddSourceFile(fileUri, fileContents, isEntryFile);
        }

        private ISourceFile AddSourceFile(Uri fileUri, string fileContents, bool isEntryFile)
        {
            var sourceFile = SourceFileFactory.CreateSourceFile(fileUri, fileContents, isEntryFile);

            sourceFilesByUri[fileUri] = sourceFile;

            return sourceFile;
        }

        private ISourceFile? PopulateRecursive(Uri fileUri, bool isEntryFile, out ErrorBuilderDelegate? failureBuilder)
        {
            var sourceFile = this.TryGetSourceFile(fileUri, isEntryFile, out var getSourceFileFailureBuilder);

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

            foreach (var module in GetModuleDeclarations(bicepFile))
            {
                if(!this.moduleDispatcher.ValidateModuleReference(module, out var parseReferenceFailureBuilder))
                {
                    // module reference is not valid
                    errorBuildersByModuleDeclaration[module] = parseReferenceFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(IModuleDispatcher.ValidateModuleReference)} to provide failure diagnostics.");
                    continue;
                }

                if(!this.moduleDispatcher.IsModuleAvailable(module, out var restoreErrorBuilder))
                {
                    errorBuildersByModuleDeclaration[module] = restoreErrorBuilder ?? throw new InvalidOperationException($"Expected {nameof(IModuleDispatcher.IsModuleAvailable)} to provide failure diagnostics.");
                    modulesToInit.Add(module);
                    continue;
                }

                var moduleFileUri = this.moduleDispatcher.TryGetLocalModuleEntryPointUri(fileUri, module, out var moduleGetPathFailureBuilder);
                if (moduleFileUri is null)
                {
                    // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                    errorBuildersByModuleDeclaration[module] = moduleGetPathFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(moduleDispatcher.TryGetLocalModuleEntryPointUri)} to provide failure diagnostics.");
                    continue;
                }

                // only recurse if we've not seen this module before - to avoid infinite loops
                if (!sourceFilesByUri.TryGetValue(moduleFileUri, out var moduleFile))
                {
                    moduleFile = PopulateRecursive(moduleFileUri, isEntryFile: false, out var modulePopulateFailureBuilder);
                    
                    if (moduleFile is null)
                    {
                        // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                        errorBuildersByModuleDeclaration[module] = modulePopulateFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to provide failure diagnostics.");
                        continue;
                    }
                }

                if (moduleFile is null)
                {
                    continue;
                }

                sourceFilesByModuleDeclaration[module] = moduleFile;
            }

            failureBuilder = null;
            return sourceFile;
        }

        private void ReportFailuresForCycles()
        {
            var sourceFileGraph = this.sourceFilesByUri.Values
                .SelectMany(sourceFile => GetModuleDeclarations(sourceFile)
                    .Where(this.sourceFilesByModuleDeclaration.ContainsKey)
                    .Select(moduleDeclaration => this.sourceFilesByModuleDeclaration[moduleDeclaration])
                    .Distinct()
                    .Select(referencedFile => (sourceFile, referencedFile)))
                .ToLookup(x => x.sourceFile, x => x.referencedFile);

            var cycles = CycleDetector<ISourceFile>.FindCycles(sourceFileGraph);
            foreach (var kvp in sourceFilesByModuleDeclaration)
            {
                if (cycles.TryGetValue(kvp.Value, out var cycle))
                {
                    if (cycle.Length == 1)
                    {
                        errorBuildersByModuleDeclaration[kvp.Key] = x => x.CyclicModuleSelfReference();
                    }
                    else
                    {
                        errorBuildersByModuleDeclaration[kvp.Key] = x => x.CyclicModule(cycle.Select(x => x.FileUri.LocalPath));
                    }
                }
            }
        }

        private static IEnumerable<ModuleDeclarationSyntax> GetModuleDeclarations(ISourceFile sourceFile) => sourceFile is BicepFile bicepFile
            ? bicepFile.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>()
            : Enumerable.Empty<ModuleDeclarationSyntax>();
    }
}

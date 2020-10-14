// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parser;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;

namespace Bicep.Core.Syntax
{
    public class SyntaxTreeGroupingBuilder
    {
        private readonly IFileResolver fileResolver;
        private readonly IDictionary<ModuleDeclarationSyntax, SyntaxTree> moduleLookup;
        private readonly IDictionary<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate> moduleFailureLookup;
        private readonly IDictionary<string, SyntaxTree?> syntaxTrees;

        private SyntaxTreeGroupingBuilder(IFileResolver fileResolver)
        {
            this.fileResolver = fileResolver;
            this.moduleLookup = new Dictionary<ModuleDeclarationSyntax, SyntaxTree>();
            this.moduleFailureLookup = new Dictionary<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate>();
            this.syntaxTrees = new Dictionary<string, SyntaxTree?>();
        }

        public static SyntaxTreeGrouping Build(IFileResolver fileResolver, string entryFileName)
        {
            var builder = new SyntaxTreeGroupingBuilder(fileResolver);
            var normalizedFileName = fileResolver.GetNormalizedFileName(entryFileName);

            return builder.Build(normalizedFileName);
        }

        public static SyntaxTreeGrouping BuildWithPreloadedFile(IFileResolver fileResolver, string entryFileName, string fileContents)
        {
            var builder = new SyntaxTreeGroupingBuilder(fileResolver);
            var normalizedFileName = fileResolver.GetNormalizedFileName(entryFileName);
            builder.AddSyntaxTree(normalizedFileName, fileContents);

            return builder.Build(normalizedFileName);
        }

        private SyntaxTreeGrouping Build(string entryFileName)
        {
            var entryPoint = PopulateRecursive(entryFileName, out var entryPointLoadFailureBuilder);
            if (entryPoint == null)
            {
                // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                var failureBuilder = entryPointLoadFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to provide failure diagnostics");
                var diagnostic = failureBuilder(DiagnosticBuilder.ForPosition(new TextSpan(0, 0)));

                throw new ErrorDiagnosticException(diagnostic);
            }

            ReportFailuresForCycles();

            return new SyntaxTreeGrouping(
                entryPoint,
                syntaxTrees.Values.OfType<SyntaxTree>().ToImmutableHashSet(), 
                moduleLookup.ToImmutableDictionary(),
                moduleFailureLookup.ToImmutableDictionary());
        }

        private SyntaxTree? TryGetSyntaxTree(string fullFileName, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var normalizedFileName = fileResolver.GetNormalizedFileName(fullFileName);
 
            if (syntaxTrees.TryGetValue(normalizedFileName, out var syntaxTree))
            {
                failureBuilder = null;
                return syntaxTree;
            }

            var fileContents = fileResolver.TryRead(normalizedFileName, out var failureMessage);
            if (fileContents == null)
            {
                // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                var concreteFailureMessage = failureMessage ?? throw new InvalidOperationException($"Expected {nameof(fileResolver.TryRead)} to provide failure diagnostics");

                syntaxTrees[normalizedFileName] = null;
                failureBuilder = x => x.ErrorOccurredLoadingModule(concreteFailureMessage);
                return null;
            }

            failureBuilder = null;
            return AddSyntaxTree(normalizedFileName, fileContents);
        }

        private SyntaxTree AddSyntaxTree(string normalizedFileName, string fileContents)
        {
            var syntaxTree = SyntaxTree.Create(normalizedFileName, fileContents);
            syntaxTrees[normalizedFileName] = syntaxTree;

            return syntaxTree;
        }

        private SyntaxTree? PopulateRecursive(string fileName, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var syntaxTree = TryGetSyntaxTree(fileName, out var getSyntaxTreeFailureBuilder);
            if (syntaxTree == null)
            {
                failureBuilder = getSyntaxTreeFailureBuilder;
                return null;
            }

            foreach (var module in GetModuleSyntaxes(syntaxTree))
            {
                var moduleFileName = TryGetNormalizedModulePath(fileName, module, out var moduleGetPathFailureBuilder);
                if (moduleFileName == null)
                {
                    // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                    moduleFailureLookup[module] = moduleGetPathFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(TryGetNormalizedModulePath)} to provide failure diagnostics");
                    continue;
                }

                // only recurse if we've not seen this module before - to avoid infinite loops
                if (!syntaxTrees.TryGetValue(moduleFileName, out var moduleSyntaxTree))
                {
                    moduleSyntaxTree = PopulateRecursive(moduleFileName, out var modulePopulateFailureBuilder);
                    
                    if (moduleSyntaxTree == null)
                    {
                        // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                        moduleFailureLookup[module] = modulePopulateFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to provide failure diagnostics");
                        continue;
                    }
                }

                if (moduleSyntaxTree == null)
                {
                    continue;
                }

                moduleLookup[module] = moduleSyntaxTree;
            }

            failureBuilder = null;
            return syntaxTree;
        }

        private string? TryGetNormalizedModulePath(string parentFileName, ModuleDeclarationSyntax moduleDeclarationSyntax, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var pathName = SyntaxHelper.TryGetModulePath(moduleDeclarationSyntax, out var getModulePathFailureBuilder);
            if (pathName == null)
            {
                failureBuilder = getModulePathFailureBuilder;
                return null;
            }

            var fullPath = fileResolver.TryResolveModulePath(parentFileName, pathName);
            if (fullPath == null)
            {
                failureBuilder = x => x.ModulePathCouldNotBeResolved(pathName, parentFileName);
                return null;
            }

            failureBuilder = null;
            return fullPath;
        }

        private void ReportFailuresForCycles()
        {
            var syntaxTreeGraph = syntaxTrees.Values.OfType<SyntaxTree>()
                .SelectMany(tree => GetModuleSyntaxes(tree).Where(moduleLookup.ContainsKey).Select(x => moduleLookup[x]).Distinct().Select(x => (tree, x)))
                .ToLookup(x => x.Item1, x => x.Item2);

            var cycles = CycleDetector<SyntaxTree>.FindCycles(syntaxTreeGraph);
            foreach (var kvp in moduleLookup)
            {
                if (cycles.TryGetValue(kvp.Value, out var cycle))
                {
                    if (cycle.Length == 1)
                    {
                        moduleFailureLookup[kvp.Key] = x => x.CyclicModuleSelfReference();
                    }
                    else
                    {
                        moduleFailureLookup[kvp.Key] = x => x.CyclicModule(cycle.Select(x => x.FilePath));
                    }
                }
            }
        }

        private static IEnumerable<ModuleDeclarationSyntax> GetModuleSyntaxes(SyntaxTree syntaxTree)
            => syntaxTree.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>();
    }
}
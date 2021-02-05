// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Syntax
{
    public class SyntaxTreeGroupingBuilder
    {
        private readonly IFileResolver fileResolver;
        private readonly IReadOnlyWorkspace workspace;
        private readonly IDictionary<ModuleDeclarationSyntax, SyntaxTree> moduleLookup;
        private readonly IDictionary<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate> moduleFailureLookup;
        private readonly IDictionary<Uri, SyntaxTree> syntaxTrees;
        private readonly IDictionary<Uri, DiagnosticBuilder.ErrorBuilderDelegate> syntaxTreeLoadFailures;

        private SyntaxTreeGroupingBuilder(IFileResolver fileResolver, IReadOnlyWorkspace workspace)
        {
            this.fileResolver = fileResolver;
            this.workspace = workspace;
            this.moduleLookup = new Dictionary<ModuleDeclarationSyntax, SyntaxTree>();
            this.moduleFailureLookup = new Dictionary<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate>();
            this.syntaxTrees = new Dictionary<Uri, SyntaxTree>();
            this.syntaxTreeLoadFailures = new Dictionary<Uri, DiagnosticBuilder.ErrorBuilderDelegate>();
        }

        public static SyntaxTreeGrouping Build(IFileResolver fileResolver, IReadOnlyWorkspace workspace, Uri entryFileUri)
        {
            var builder = new SyntaxTreeGroupingBuilder(fileResolver, workspace);

            return builder.Build(entryFileUri);
        }

        private SyntaxTreeGrouping Build(Uri entryFileUri)
        {
            var entryPoint = PopulateRecursive(entryFileUri, out var entryPointLoadFailureBuilder);
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
                syntaxTrees.Values.ToImmutableHashSet(), 
                moduleLookup.ToImmutableDictionary(),
                moduleFailureLookup.ToImmutableDictionary());
        }

        private SyntaxTree? TryGetSyntaxTree(Uri fileUri, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (workspace.TryGetSyntaxTree(fileUri, out var syntaxTree))
            {
                failureBuilder = null;
                syntaxTrees[fileUri] = syntaxTree;
                return syntaxTree;
            }

            if (syntaxTrees.TryGetValue(fileUri, out syntaxTree))
            {
                failureBuilder = null;
                return syntaxTree;
            }

            if (syntaxTreeLoadFailures.TryGetValue(fileUri, out failureBuilder))
            {
                return null;
            }

            if (!fileResolver.TryRead(fileUri, out var fileContents, out failureBuilder))
            {
                syntaxTreeLoadFailures[fileUri] = failureBuilder;
                return null;
            }

            failureBuilder = null;
            return AddSyntaxTree(fileUri, fileContents);
        }

        private SyntaxTree AddSyntaxTree(Uri fileUri, string fileContents)
        {
            var syntaxTree = SyntaxTree.Create(fileUri, fileContents);
            syntaxTrees[fileUri] = syntaxTree;

            return syntaxTree;
        }

        private SyntaxTree? PopulateRecursive(Uri fileUri, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var syntaxTree = TryGetSyntaxTree(fileUri, out var getSyntaxTreeFailureBuilder);
            if (syntaxTree == null)
            {
                failureBuilder = getSyntaxTreeFailureBuilder;
                return null;
            }

            foreach (var module in GetModuleSyntaxes(syntaxTree))
            {
                var moduleFileName = TryGetNormalizedModulePath(fileUri, module, out var moduleGetPathFailureBuilder);
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

        private static readonly ImmutableHashSet<char> forbiddenPathChars = "<>:\"\\|?*".ToImmutableHashSet();
        private static readonly ImmutableHashSet<char> forbiddenPathTerminatorChars = " .".ToImmutableHashSet();
        private static bool IsInvalidPathControlCharacter(char pathChar)
        {
            // TODO: Revisit when we add unicode support to Bicep

            // The following are disallowed as path chars on Windows, so we block them to avoid cross-platform compilation issues.
            // Note that we're checking this range explicitly, as char.IsControl() includes some characters that are valid path characters.
            return pathChar >= 0 && pathChar <= 31;
        }

        public static bool ValidateModulePath(string pathName, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            if (pathName.Length == 0)
            {
                failureBuilder = x => x.ModulePathIsEmpty();
                return false;
            }

            if (pathName.First() == '/')
            {
                failureBuilder = x => x.ModulePathBeginsWithForwardSlash();
                return false;
            }

            foreach (var pathChar in pathName)
            {
                if (pathChar == '\\')
                {
                    // enforce '/' rather than '\' for module paths for cross-platform compatibility
                    failureBuilder = x => x.ModulePathContainsBackSlash();
                    return false;
                }

                if (forbiddenPathChars.Contains(pathChar))
                {
                    failureBuilder = x => x.ModulePathContainsForbiddenCharacters(forbiddenPathChars);
                    return false;
                }

                if (IsInvalidPathControlCharacter(pathChar))
                {
                    failureBuilder = x => x.ModulePathContainsControlChars();
                    return false;
                }
            }

            if (forbiddenPathTerminatorChars.Contains(pathName.Last()))
            {
                failureBuilder = x => x.ModulePathHasForbiddenTerminator(forbiddenPathTerminatorChars);
                return false;
            }

            failureBuilder = null;
            return true;
        }

        private Uri? TryGetNormalizedModulePath(Uri parentFileUri, ModuleDeclarationSyntax moduleDeclarationSyntax, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            var pathName = SyntaxHelper.TryGetModulePath(moduleDeclarationSyntax, out var getModulePathFailureBuilder);
            if (pathName == null)
            {
                failureBuilder = getModulePathFailureBuilder;
                return null;
            }

            if (!ValidateModulePath(pathName, out var validateModulePathFailureBuilder))
            {
                failureBuilder = validateModulePathFailureBuilder;
                return null;
            }

            var moduleUri = fileResolver.TryResolveModulePath(parentFileUri, pathName);
            if (moduleUri == null)
            {
                failureBuilder = x => x.ModulePathCouldNotBeResolved(pathName, parentFileUri.LocalPath);
                return null;
            }

            failureBuilder = null;
            return moduleUri;
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
                        moduleFailureLookup[kvp.Key] = x => x.CyclicModule(cycle.Select(x => x.FileUri.LocalPath));
                    }
                }
            }
        }

        private static IEnumerable<ModuleDeclarationSyntax> GetModuleSyntaxes(SyntaxTree syntaxTree)
            => syntaxTree.ProgramSyntax.Declarations.OfType<ModuleDeclarationSyntax>();
    }
}
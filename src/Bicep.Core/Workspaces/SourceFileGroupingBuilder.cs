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
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGroupingBuilder
    {
        private readonly IFileResolver fileResolver;
        private readonly IReadOnlyWorkspace workspace;
        private readonly IDictionary<ModuleDeclarationSyntax, ISourceFile> sourceFilesByModuleDeclaration;
        private readonly IDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> errorBuildersByModuleDeclaration;
        private readonly IDictionary<Uri, ISourceFile> sourceFilesByUri;
        private readonly IDictionary<Uri, ErrorBuilderDelegate> errorBuildersByUri;

        private SourceFileGroupingBuilder(IFileResolver fileResolver, IReadOnlyWorkspace workspace)
        {
            this.fileResolver = fileResolver;
            this.workspace = workspace;
            this.sourceFilesByModuleDeclaration = new Dictionary<ModuleDeclarationSyntax, ISourceFile>();
            this.errorBuildersByModuleDeclaration = new Dictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate>();
            this.sourceFilesByUri = new Dictionary<Uri, ISourceFile>();
            this.errorBuildersByUri = new Dictionary<Uri, ErrorBuilderDelegate>();
        }

        public static SourceFileGrouping Build(IFileResolver fileResolver, IReadOnlyWorkspace workspace, Uri entryFileUri)
        {
            var builder = new SourceFileGroupingBuilder(fileResolver, workspace);

            return builder.Build(entryFileUri);
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
                errorBuildersByModuleDeclaration.ToImmutableDictionary());
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

            if (sourceFile == null)
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
                var moduleUri = this.TryGetNormalizedModuleUri(fileUri, module, out var moduleGetPathFailureBuilder);
                if (moduleUri == null)
                {
                    // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                    errorBuildersByModuleDeclaration[module] = moduleGetPathFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(TryGetNormalizedModuleUri)} to provide failure diagnostics");
                    continue;
                }

                // only recurse if we've not seen this module before - to avoid infinite loops
                if (!sourceFilesByUri.TryGetValue(moduleUri, out var moduleFile))
                {
                    moduleFile = PopulateRecursive(moduleUri, isEntryFile: false, out var modulePopulateFailureBuilder);

                    if (moduleFile == null)
                    {
                        // TODO: If we upgrade to netstandard2.1, we should be able to use the following to hint to the compiler that failureBuilder is non-null:
                        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
                        errorBuildersByModuleDeclaration[module] = modulePopulateFailureBuilder ?? throw new InvalidOperationException($"Expected {nameof(PopulateRecursive)} to provide failure diagnostics");
                        continue;
                    }
                }

                if (moduleFile == null)
                {
                    continue;
                }

                sourceFilesByModuleDeclaration[module] = moduleFile;
            }

            failureBuilder = null;
            return sourceFile;
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

        public static bool ValidateFilePath(string pathName, [NotNullWhen(false)] out ErrorBuilderDelegate? failureBuilder)
        {
            if (string.IsNullOrWhiteSpace(pathName))
            {
                failureBuilder = x => x.FilePathIsEmpty();
                return false;
            }

            if (pathName.First() == '/')
            {
                failureBuilder = x => x.FilePathBeginsWithForwardSlash();
                return false;
            }

            foreach (var pathChar in pathName)
            {
                if (pathChar == '\\')
                {
                    // enforce '/' rather than '\' for module paths for cross-platform compatibility
                    failureBuilder = x => x.FilePathContainsBackSlash();
                    return false;
                }

                if (forbiddenPathChars.Contains(pathChar))
                {
                    failureBuilder = x => x.FilePathContainsForbiddenCharacters(forbiddenPathChars);
                    return false;
                }

                if (IsInvalidPathControlCharacter(pathChar))
                {
                    failureBuilder = x => x.FilePathContainsControlChars();
                    return false;
                }
            }

            if (forbiddenPathTerminatorChars.Contains(pathName.Last()))
            {
                failureBuilder = x => x.FilePathHasForbiddenTerminator(forbiddenPathTerminatorChars);
                return false;
            }

            failureBuilder = null;
            return true;
        }

        private Uri? TryGetNormalizedModuleUri(Uri parentFileUri, ModuleDeclarationSyntax moduleDeclarationSyntax, out ErrorBuilderDelegate? failureBuilder)
        {
            var pathName = SyntaxHelper.TryGetModulePath(moduleDeclarationSyntax, out var getModulePathFailureBuilder);
            if (pathName == null)
            {
                failureBuilder = getModulePathFailureBuilder;
                return null;
            }

            if (!ValidateFilePath(pathName, out var validateModulePathFailureBuilder))
            {
                failureBuilder = validateModulePathFailureBuilder;
                return null;
            }

            var moduleUri = fileResolver.TryResolveFilePath(parentFileUri, pathName);
            if (moduleUri == null)
            {
                failureBuilder = x => x.FilePathCouldNotBeResolved(pathName, parentFileUri.LocalPath);
                return null;
            }

            failureBuilder = null;
            return moduleUri;
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

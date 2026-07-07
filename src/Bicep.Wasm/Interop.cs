// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Highlighting;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Text;
using Bicep.Decompiler;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.Wasm.LanguageHelpers;
using Microsoft.JSInterop;

namespace Bicep.Wasm
{
    public partial class Interop
    {
        private const string QuickstartsRootPath = "/quickstarts/";

        public record DecompileResult(string? bicepFile, string? error);

        public record CompileResult(string template, object diagnostics);

        private readonly IJSRuntime jsRuntime;
        private readonly IServiceProvider serviceProvider;

        public Interop(IJSRuntime jsRuntime, IServiceProvider serviceProvider)
        {
            this.jsRuntime = jsRuntime;
            this.serviceProvider = serviceProvider;
        }

        [JSInvokable]
        public async Task<CompileResult> CompileAndEmitDiagnostics(string content, string? sourcePath)
        {
            try
            {
                var compilation = await GetCompilation(content, sourcePath);
                var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

                var stringWriter = new StringWriter();
                var emitResult = emitter.Emit(stringWriter);
                var diagnostics = emitResult.Diagnostics
                    .Select(d => ToMonacoDiagnostic(d, compilation.SourceFileGrouping))
                    .ToArray();

                if (emitResult.Status != EmitStatus.Failed)
                {
                    // compilation was successful or had warnings - return the compiled template
                    return new(stringWriter.ToString(), diagnostics);
                }

                // compilation failed
                return new("Compilation failed!", diagnostics);
            }
            catch (Exception exception)
            {
                return new(exception.ToString(), Enumerable.Empty<object>());
            }
        }

        [JSInvokable]
        public async Task<DecompileResult> Decompile(string jsonContent)
        {
            using var serviceScope = serviceProvider.CreateScope();
            var decompiler = serviceScope.ServiceProvider.GetRequiredService<BicepDecompiler>();

            try
            {
                var (entrypointUri, filesToSave) = await decompiler.Decompile(DummyFileHandle.Default.Uri, jsonContent);

                return new DecompileResult(filesToSave[entrypointUri], null);
            }
            catch (Exception exception)
            {
                return new DecompileResult(null, exception.Message);
            }
        }

        [JSInvokable]
        public object GetSemanticTokensLegend()
        {
            var tokenTypes = Enum.GetValues(typeof(SemanticTokenType)).Cast<SemanticTokenType>();
            var tokenStrings = tokenTypes.OrderBy(t => (int)t).Select(t => t.ToString().ToLowerInvariant());

            return new
            {
                tokenModifiers = new string[] { },
                tokenTypes = tokenStrings.ToArray(),
            };
        }

        [JSInvokable]
        public async Task<object> GetSemanticTokens(string content)
        {
            var compilation = await GetCompilation(content);
            var tokens = GetTokenPositions(compilation.GetEntrypointSemanticModel());

            var data = new List<int>();
            TokenPosition? prevToken = null;
            foreach (var token in tokens)
            {
                if (prevToken == null)
                {
                    data.Add(token.Line);
                    data.Add(token.Character);
                    data.Add(token.Length);
                }
                else if (prevToken.Line != token.Line)
                {
                    data.Add(token.Line - prevToken.Line);
                    data.Add(token.Character);
                    data.Add(token.Length);
                }
                else
                {
                    data.Add(0);
                    data.Add(token.Character - prevToken.Character);
                    data.Add(token.Length);
                }

                data.Add((int)token.TokenType);
                data.Add(0);

                prevToken = token;
            }

            return new
            {
                data = data.ToArray(),
            };
        }

        private async Task<Compilation> GetCompilation(string fileContents, string? sourcePath = null)
        {
            using var serviceScope = serviceProvider.CreateScope();
            var compiler = serviceScope.ServiceProvider.GetRequiredService<BicepCompiler>();
            var fileExplorer = serviceScope.ServiceProvider.GetRequiredService<IFileExplorer>();

            var fileUri = string.IsNullOrEmpty(sourcePath)
                ? IOUri.FromFilePath("/main.bicep")
                : IOUri.FromFilePath($"{QuickstartsRootPath}{sourcePath.TrimStart('/')}");

            await WriteFileAsync(fileExplorer, fileUri, fileContents);

            if (!string.IsNullOrEmpty(sourcePath))
            {
                await WriteModuleFilesRecursively(fileExplorer, fileUri, fileContents, [fileUri]);
            }

            return await compiler.CreateCompilation(fileUri);
        }

        private async Task WriteModuleFilesRecursively(IFileExplorer fileExplorer, IOUri sourceFileUri, string sourceContent, HashSet<IOUri> loadedUris)
        {
            foreach (var modulePath in GetLocalModulePaths(sourceContent))
            {
                var moduleUri = sourceFileUri.Resolve(modulePath);

                if (!moduleUri.Path.StartsWith(QuickstartsRootPath, StringComparison.Ordinal) ||
                    !loadedUris.Add(moduleUri))
                {
                    continue;
                }

                string? moduleContents;
                var moduleFile = fileExplorer.GetFile(moduleUri);

                if (moduleFile.Exists())
                {
                    moduleContents = await moduleFile.ReadAllTextAsync();
                }
                else
                {
                    var quickstartsPath = moduleUri.Path[QuickstartsRootPath.Length..];
                    moduleContents = await jsRuntime.InvokeAsync<string?>("LoadQuickstartsFile", quickstartsPath);

                    if (moduleContents is null)
                    {
                        continue;
                    }

                    await WriteFileAsync(fileExplorer, moduleUri, moduleContents);
                }

                await WriteModuleFilesRecursively(fileExplorer, moduleUri, moduleContents, loadedUris);
            }
        }

        private static async Task WriteFileAsync(IFileExplorer fileExplorer, IOUri fileUri, string fileContents)
        {
            var file = fileExplorer.GetFile(fileUri);
            file.GetParent().EnsureExists();

            await file.WriteAllTextAsync(fileContents);
        }

        private static IEnumerable<string> GetLocalModulePaths(string sourceContent)
            => GetModulePathRegex().Matches(sourceContent)
                .Select(match => match.Groups["path"].Value)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Where(path => !path.StartsWith("br:", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.StartsWith("ts:", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.StartsWith("az:", StringComparison.OrdinalIgnoreCase))
                .Where(path => !HasAbsoluteSchemePrefix(path));

        private static bool HasAbsoluteSchemePrefix(string path) => GetAbsoluteSchemePrefixRegex().IsMatch(path);

        [GeneratedRegex("^\\s*module\\s+\\w+\\s+'(?<path>[^']+)'", RegexOptions.Multiline)]
        private static partial Regex GetModulePathRegex();

        [GeneratedRegex("^[A-Za-z][A-Za-z0-9+.-]*:")]
        private static partial Regex GetAbsoluteSchemePrefixRegex();

        private static object ToMonacoDiagnostic(IDiagnostic diagnostic, SourceFileGrouping sourceFileGrouping)
        {
            var diagnosticUri = diagnostic.Uri?.ToIOUri();
            var diagnosticSourceFile = GetSourceFile(sourceFileGrouping, diagnosticUri);

            if (diagnosticSourceFile is not null &&
                diagnosticSourceFile.FileHandle.Uri != sourceFileGrouping.EntryPoint.FileHandle.Uri)
            {
                var sourcePath = GetDiagnosticSourcePath(diagnosticSourceFile.FileHandle.Uri);

                // The playground editor only displays markers in the entrypoint model.
                // For diagnostics from referenced module files, pin a marker to line 1
                // and include the file path in the message so the error is still visible.
                return new
                {
                    code = diagnostic.Code,
                    message = $"{sourcePath}: {diagnostic.Message}",
                    severity = ToMonacoSeverity(diagnostic.Level),
                    startLineNumber = 1,
                    startColumn = 1,
                    endLineNumber = 1,
                    endColumn = 1,
                };
            }

            var lineStarts = diagnosticSourceFile?.LineStarts ?? sourceFileGrouping.EntryPoint.LineStarts;
            var (startLine, startChar) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);
            var (endLine, endChar) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.GetEndPosition());

            return new
            {
                code = diagnostic.Code,
                message = diagnostic.Message,
                severity = ToMonacoSeverity(diagnostic.Level),
                startLineNumber = startLine + 1,
                startColumn = startChar + 1,
                endLineNumber = endLine + 1,
                endColumn = endChar + 1,
            };
        }

        private static BicepSourceFile? GetSourceFile(SourceFileGrouping sourceFileGrouping, IOUri? diagnosticUri)
        {
            if (diagnosticUri is null ||
                !sourceFileGrouping.SourceFileLookup.TryGetValue(diagnosticUri, out var sourceFileResult) ||
                !sourceFileResult.IsSuccess(out var sourceFile) ||
                sourceFile is not BicepSourceFile bicepSourceFile)
            {
                return null;
            }

            return bicepSourceFile;
        }

        private static string GetDiagnosticSourcePath(IOUri diagnosticUri)
        {
            var sourcePath = diagnosticUri.Path;

            if (sourcePath.StartsWith(QuickstartsRootPath, StringComparison.Ordinal))
            {
                sourcePath = sourcePath[QuickstartsRootPath.Length..];
            }

            return sourcePath;
        }

        private static int ToMonacoSeverity(DiagnosticLevel level)
            => level switch
            {
                DiagnosticLevel.Info => 2,
                DiagnosticLevel.Warning => 4,
                DiagnosticLevel.Error => 8,
                _ => throw new ArgumentException($"Unrecognized level {level}"),
            };


        private static IEnumerable<TokenPosition> GetTokenPositions(SemanticModel model)
        {
            var tokens = SemanticTokenVisitor.Build(model);

            // the builder is fussy about ordering. tokens are visited out of order, we need to call build after visiting everything
            foreach (var (positionable, tokenType) in tokens.OrderBy(t => t.Positionable.GetPosition()))
            {
                var tokenRanges = positionable.ToRangeSpanningLines(model.SourceFile.LineStarts);
                foreach (var tokenRange in tokenRanges)
                {
                    yield return new(tokenRange.Start.Line, tokenRange.Start.Character, tokenRange.End.Character - tokenRange.Start.Character, tokenType);
                }
            }
        }
    }
}

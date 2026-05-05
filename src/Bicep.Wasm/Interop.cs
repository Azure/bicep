// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
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
            var fileSystem = serviceScope.ServiceProvider.GetRequiredService<IFileSystem>();

            var fileUri = string.IsNullOrEmpty(sourcePath)
                ? new Uri("file:///main.bicep")
                : new Uri($"file://{QuickstartsRootPath}{sourcePath.TrimStart('/')}");

            EnsureParentDirectoryExists(fileSystem, fileUri.LocalPath);
            await fileSystem.File.WriteAllTextAsync(fileUri.LocalPath, fileContents);

            if (!string.IsNullOrEmpty(sourcePath))
            {
                await WriteModuleFilesRecursively(fileSystem, fileUri, fileContents, [fileUri.LocalPath]);
            }

            return await compiler.CreateCompilation(fileUri.ToIOUri());
        }

        private async Task WriteModuleFilesRecursively(IFileSystem fileSystem, Uri sourceFileUri, string sourceContent, HashSet<string> loadedPaths)
        {
            foreach (var modulePath in GetLocalModulePaths(sourceContent))
            {
                var moduleUri = new Uri(sourceFileUri, modulePath);

                if (!moduleUri.LocalPath.StartsWith(QuickstartsRootPath, StringComparison.Ordinal) ||
                    !loadedPaths.Add(moduleUri.LocalPath))
                {
                    continue;
                }

                string? moduleContents;

                if (fileSystem.File.Exists(moduleUri.LocalPath))
                {
                    moduleContents = await fileSystem.File.ReadAllTextAsync(moduleUri.LocalPath);
                }
                else
                {
                    var quickstartsPath = moduleUri.LocalPath[QuickstartsRootPath.Length..];
                    moduleContents = await jsRuntime.InvokeAsync<string?>("LoadQuickstartsFile", quickstartsPath);

                    if (moduleContents is null)
                    {
                        continue;
                    }

                    EnsureParentDirectoryExists(fileSystem, moduleUri.LocalPath);
                    await fileSystem.File.WriteAllTextAsync(moduleUri.LocalPath, moduleContents);
                }

                await WriteModuleFilesRecursively(fileSystem, moduleUri, moduleContents, loadedPaths);
            }
        }

        private static void EnsureParentDirectoryExists(IFileSystem fileSystem, string filePath)
        {
            var parentDirectory = fileSystem.Path.GetDirectoryName(filePath);

            if (parentDirectory is not null)
            {
                fileSystem.Directory.CreateDirectory(parentDirectory);
            }
        }

        private static IEnumerable<string> GetLocalModulePaths(string sourceContent)
            => GetModulePathRegex().Matches(sourceContent)
                .Select(match => match.Groups["path"].Value)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Where(path => !path.StartsWith("br:", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.StartsWith("ts:", StringComparison.OrdinalIgnoreCase))
                .Where(path => !path.StartsWith("az:", StringComparison.OrdinalIgnoreCase))
                .Where(path => !Uri.TryCreate(path, UriKind.Absolute, out _));

        [GeneratedRegex("^\\s*module\\s+\\w+\\s+'(?<path>[^']+)'", RegexOptions.Multiline)]
        private static partial Regex GetModulePathRegex();

        private static object ToMonacoDiagnostic(IDiagnostic diagnostic, SourceFileGrouping sourceFileGrouping)
        {
            if (diagnostic.Uri is { } diagnosticUri &&
                diagnosticUri != sourceFileGrouping.EntryPoint.Uri)
            {
                var sourcePath = GetDiagnosticSourcePath(diagnosticUri);

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

            var lineStarts = GetLineStarts(sourceFileGrouping, diagnostic.Uri)
                ?? sourceFileGrouping.EntryPoint.LineStarts;
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

        private static IReadOnlyList<int>? GetLineStarts(SourceFileGrouping sourceFileGrouping, Uri? diagnosticUri)
        {
            if (diagnosticUri is null ||
                !sourceFileGrouping.SourceFileLookup.TryGetValue(diagnosticUri.ToIOUri(), out var sourceFileResult) ||
                !sourceFileResult.IsSuccess(out var sourceFile) ||
                sourceFile is not BicepSourceFile bicepSourceFile)
            {
                return null;
            }

            return bicepSourceFile.LineStarts;
        }

        private static string GetDiagnosticSourcePath(Uri diagnosticUri)
        {
            var sourcePath = diagnosticUri.LocalPath;

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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Highlighting;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Decompiler;
using Bicep.IO.InMemory;
using Bicep.Wasm.LanguageHelpers;
using Microsoft.JSInterop;

namespace Bicep.Wasm
{
    public class Interop
    {
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
        public async Task<CompileResult> CompileAndEmitDiagnostics(string content)
        {
            try
            {
                var compilation = await GetCompilation(content);
                var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;
                var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

                var stringWriter = new StringWriter();
                var emitResult = emitter.Emit(stringWriter);

                if (emitResult.Status != EmitStatus.Failed)
                {
                    // compilation was successful or had warnings - return the compiled template
                    return new(stringWriter.ToString(), emitResult.Diagnostics.Select(d => ToMonacoDiagnostic(d, lineStarts)));
                }

                // compilation failed
                return new("Compilation failed!", emitResult.Diagnostics.Select(d => ToMonacoDiagnostic(d, lineStarts)));
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

        private async Task<Compilation> GetCompilation(string fileContents)
        {
            using var serviceScope = serviceProvider.CreateScope();
            var compiler = serviceScope.ServiceProvider.GetRequiredService<BicepCompiler>();
            var fileSystem = serviceScope.ServiceProvider.GetRequiredService<IFileSystem>();

            var fileUri = new Uri("file:///main.bicep");
            await fileSystem.File.WriteAllTextAsync(fileUri.LocalPath, fileContents);

            return await compiler.CreateCompilation(fileUri.ToIOUri());
        }

        private static object ToMonacoDiagnostic(IDiagnostic diagnostic, IReadOnlyList<int> lineStarts)
        {
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

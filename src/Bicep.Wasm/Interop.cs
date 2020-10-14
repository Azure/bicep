// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.JSInterop;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;
using Bicep.Core.Emit;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Wasm.LanguageHelpers;
using System.Linq;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.FileSystem;

namespace Bicep.Wasm
{
    public class Interop
    {
        private static readonly IResourceTypeProvider resourceTypeProvider = new AzResourceTypeProvider();

        private readonly IJSRuntime jsRuntime;

        public Interop(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        [JSInvokable]
        public object CompileAndEmitDiagnostics(string content)
        {
            var (output, diagnostics) = CompileInternal(content);
            
            return new
            {
                template = output,
                diagnostics = diagnostics,
            };
        }

        [JSInvokable]
        public object GetSemanticTokensLegend()
        {
            var tokenTypes = Enum.GetValues(typeof(SemanticTokenType)).Cast<SemanticTokenType>();
            var tokenStrings = tokenTypes.OrderBy(t => (int)t).Select(t => t.ToString().ToLowerInvariant());

            return new {
                tokenModifiers = new string[] { },
                tokenTypes = tokenStrings.ToArray(),
            };
        }

        [JSInvokable]
        public object GetSemanticTokens(string content)
        {
            var compilation = GetCompilation(content);
            var tokens = SemanticTokenVisitor.BuildSemanticTokens(compilation.SyntaxTreeGrouping.EntryPoint);

            var data = new List<int>();
            SemanticToken? prevToken = null;
            foreach (var token in tokens) {
                if (prevToken == null) {
                    data.Add(token.Line);
                    data.Add(token.Character);
                    data.Add(token.Length);
                } else if (prevToken.Line != token.Line) {
                    data.Add(token.Line - prevToken.Line);
                    data.Add(token.Character);
                    data.Add(token.Length);
                } else {
                    data.Add(0);
                    data.Add(token.Character - prevToken.Character);
                    data.Add(token.Length);
                }

                data.Add((int)token.TokenType);
                data.Add(0);

                prevToken = token;
            }

            return new {
                data = data.ToArray(),
            };
        }

        private static (string, IEnumerable<dynamic>) CompileInternal(string content)
        {
            try
            {
                var lineStarts = TextCoordinateConverter.GetLineStarts(content);
                var compilation = GetCompilation(content);
                var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());

                // memory stream is not ideal for frequent large allocations
                using var stream = new MemoryStream();
                var emitResult = emitter.Emit(stream);

                if (emitResult.Status != EmitStatus.Failed)
                {
                    // compilation was successful or had warnings - return the compiled template
                    stream.Position = 0;
                    return (ReadStreamToEnd(stream), emitResult.Diagnostics.Select(d => ToMonacoDiagnostic(d, lineStarts)));
                }

                // compilation failed
                return ("Compilation failed!", emitResult.Diagnostics.Select(d => ToMonacoDiagnostic(d, lineStarts)));
            }
            catch (Exception exception)
            {
                return (exception.ToString(), Enumerable.Empty<dynamic>());
            }
        }

        private static Compilation GetCompilation(string text)
        {
            var fileName = "/main.bicep";
            var fileResolver = new InMemoryFileResolver(new Dictionary<string, string> { [fileName] = text });
            var syntaxTreeGrouping = SyntaxTreeGroupingBuilder.Build(fileResolver, fileName);

            return new Compilation(resourceTypeProvider, syntaxTreeGrouping);
        }

        private static string ReadStreamToEnd(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static object ToMonacoDiagnostic(Diagnostic diagnostic, IReadOnlyList<int> lineStarts)
        {
            var (startLine, startChar) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position);
            var (endLine, endChar) = TextCoordinateConverter.GetPosition(lineStarts, diagnostic.Span.Position + diagnostic.Span.Length);

            return new {
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
            => level switch {
                DiagnosticLevel.Info => 2,
                DiagnosticLevel.Warning => 4,
                DiagnosticLevel.Error => 8,
                _ => throw new ArgumentException($"Unrecognized level {level}"),
            };
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using Bicep.Core.SemanticModel;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer
{
    public class BicepCompilationManager : ICompilationManager
    {
        private readonly ILanguageServer server;

        // represents compilations of open bicep files
        private readonly ConcurrentDictionary<Uri, CompilationContext> activeContexts = new ConcurrentDictionary<Uri, CompilationContext>();

        public BicepCompilationManager(ILanguageServer server)
        {
            this.server = server;
        }

        public CompilationContext? UpsertCompilation(Uri uri, long version, string text)
        {
            try
            {
                var context = CreateContext(text);

                // there shouldn't be concurrent upsert requests (famous last words...), so a simple overwrite should be sufficient
                this.activeContexts[uri] = context;

                // convert all the errors to LSP diagnostics
                var diagnostics = GetErrorsFromContext(context).ToDiagnostics(context.LineStarts);

                // publish all the errors
                this.PublishDocumentDiagnostics(uri, version, diagnostics);

                return context;
            }
            catch (Exception exception)
            {
                // this is a fatal error likely due to a code defect
                
                // the file is no longer in a state that can be parsed
                // clear all info to prevent cascading failures elsewhere
                this.activeContexts.TryRemove(uri, out _);

                // publish a single fatal error diagnostic to tell the user something horrible has occurred
                // TODO: Tell user how to create an issue on GitHub.
                this.PublishDocumentDiagnostics(uri, version, new[]
                {
                    new Diagnostic
                    {
                        Range = new Range
                        {
                            Start = new Position(0, 0),
                            End = new Position(1, 0),
                        },
                        Severity = DiagnosticSeverity.Error,
                        Message = exception.Message,
                        Code = new DiagnosticCode("Fatal")
                    }
                });

                return null;
            }
        }

        public void CloseCompilation(Uri uri)
        {
            // remove the active compilation
            this.activeContexts.TryRemove(uri, out _);

            // clear diagnostics for the file
            // if upsert failed to create a compilation due to a fatal error, we still need to clean up the diagnostics
            PublishDocumentDiagnostics(uri, 0, Enumerable.Empty<Diagnostic>());
        }

        public CompilationContext? GetCompilation(Uri uri)
        {
            this.activeContexts.TryGetValue(uri, out var context);
            return context;
        }

        // TODO: Remove the lexer part when we stop it from emitting errors
        private IEnumerable<Error> GetErrorsFromContext(CompilationContext context) => context.Lexer.GetErrors().Concat(context.Compilation.GetSemanticModel().GetAllDiagnostics());

        private void PublishDocumentDiagnostics(Uri uri, long version, IEnumerable<Diagnostic> diagnostics)
        {
            server.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Version = version,
                Diagnostics = new Container<Diagnostic>(diagnostics)
            });
        }

        private CompilationContext CreateContext(string text)
        {
            var lineStarts = PositionHelper.GetLineStarts(text);

            var lexer = new Lexer(new SlidingTextWindow(text));
            lexer.Lex();

            var parser = new Parser(lexer.GetTokens());
            var program = parser.Parse();

            var compilation = new Compilation(program);

            return new CompilationContext(lexer, compilation, lineStarts);
        }
    }
}
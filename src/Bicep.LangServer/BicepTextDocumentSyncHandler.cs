using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Parser;
using Bicep.Core.TypeSystem;
using Bicep.Core.Visitors;
using Bicep.LanguageServer.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Server.ILanguageServer;

namespace Bicep.LanguageServer
{
    class BicepTextDocumentSyncHandler : TextDocumentSyncHandler
    {
        private readonly ILogger<BicepTextDocumentSyncHandler> logger;
        private readonly ILanguageServerConfiguration configuration;
        private readonly ILanguageServer server;

        private static TextDocumentSaveRegistrationOptions GetSaveRegistrationOptions()
            => new TextDocumentSaveRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage("bicep"),
                IncludeText = true,
            };

        public BicepTextDocumentSyncHandler(ILogger<BicepTextDocumentSyncHandler> logger, ILanguageServerConfiguration configuration, ILanguageServer server)
            : base(TextDocumentSyncKind.Full, GetSaveRegistrationOptions())
        {
            this.logger = logger;
            this.configuration = configuration;
            this.server = server;
        }

        public override TextDocumentAttributes GetTextDocumentAttributes(Uri uri)
        {
            return new TextDocumentAttributes(uri, "bicep");
        }

        public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken token)
        {
            // we have full sync enabled, so apparently first change is the whole document
            var contents = request.ContentChanges.First().Text;

            server.Document.PublishDiagnostics(new PublishDiagnosticsParams{
                Uri = request.TextDocument.Uri,
                Version = request.TextDocument.Version,
                Diagnostics = new Container<Diagnostic>(this.GetDiagnostics(contents))
            });

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            //await configuration.GetScopedConfiguration(request.TextDocument.Uri);

            server.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = request.TextDocument.Uri,
                Version = request.TextDocument.Version,
                Diagnostics = GetDiagnostics(request.TextDocument.Text)
            });

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            //if (configuration.TryGetScopedConfiguration(request.TextDocument.Uri, out var disposable))
            //{
            //    disposable.Dispose();
            //}

            server.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = request.TextDocument.Uri,
                Diagnostics = new Container<Diagnostic>()
            });

            return Unit.Task;
        }

        private List<Diagnostic> GetDiagnostics(string contents)
        {
            var newLinePositions = new List<int>();
            newLinePositions.Add(0);
            for (var i = 0; i < contents.Length; i++)
            {
                if (contents[i] == '\n')
                {
                    newLinePositions.Add(i + 1);
                }
            }

            var diagnostics = new List<Diagnostic>();
            try
            {
                var lexer = new Lexer(new SlidingTextWindow(contents));
                lexer.Lex();

                foreach (var error in lexer.GetErrors())
                {
                    diagnostics.Add(new Diagnostic
                    {
                        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range
                        {
                            Start = PositionHelper.GetPosition(newLinePositions, error.Span.Position),
                            End = PositionHelper.GetPosition(newLinePositions, error.Span.Position + error.Span.Length),
                        },
                        Severity = DiagnosticSeverity.Error,
                        Message = error.Message,
                    });
                }

                var tokens = lexer.GetTokens();

                var parser = new Core.Parser.Parser(tokens);
                var program = parser.Parse();

                var errors = new List<Error>();
                var checker = new CheckVisitor(errors, new TypeCache());
                checker.Visit(program);

                foreach (var error in errors)
                {
                    diagnostics.Add(new Diagnostic
                    {
                        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range
                        {
                            Start = PositionHelper.GetPosition(newLinePositions, error.Span.Position),
                            End = PositionHelper.GetPosition(newLinePositions, error.Span.Position + error.Span.Length),
                        },
                        Severity = DiagnosticSeverity.Error,
                        Message = error.Message,
                    });
                }
            }
            catch (Exception exception)
            {
                diagnostics.Add(new Diagnostic
                {
                    Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range
                    {
                        Start = new Position(0, 0),
                        End = new Position(1, 0),
                    },
                    Severity = DiagnosticSeverity.Error,
                    Message = exception.Message,
                    Code = new DiagnosticCode("LexException"),
                });
            }

            return diagnostics;
        }
    }
}
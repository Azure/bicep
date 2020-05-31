using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Parser;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
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
                DocumentSelector = DocumentSelector.ForPattern("**/*.arm"),
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

        private static Position GetPosition(IReadOnlyList<int> newlines, int position)
        {
            var prevIndex = 0;
            for (var i = 0; i < newlines.Count; i++)
            {
                if (newlines[i] > position)
                {
                    break;
                }

                prevIndex = i;
            }

            return new Position(prevIndex + 1, position - newlines[prevIndex]);
        }

        public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken token)
        {
            var contents = request.ContentChanges.First().Text;

            var newLinePositions = new List<int>();
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
                            Start = GetPosition(newLinePositions, error.Span.Position),
                            End = GetPosition(newLinePositions, error.Span.Position + error.Span.Length),
                        },
                        Severity = DiagnosticSeverity.Error,
                        Message = error.Message,
                    });
                }

                var tokens = lexer.GetTokens();

                var parser = new Core.Parser.Parser(tokens);
                var program = parser.Parse();
                
                foreach (var error in parser.GetErrors())
                {
                    diagnostics.Add(new Diagnostic
                    {
                        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range
                        {
                            Start = GetPosition(newLinePositions, error.Span.Position),
                            End = GetPosition(newLinePositions, error.Span.Position + error.Span.Length),
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
                    Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range{
                        Start = new Position(0, 0),
                        End = new Position(1, 0),
                    },
                    Severity = DiagnosticSeverity.Error,
                    Message = exception.Message,
                    Code = new DiagnosticCode("LexException"),
                });
            }

            server.Document.PublishDiagnostics(new PublishDiagnosticsParams{
                Uri = request.TextDocument.Uri,
                Version = request.TextDocument.Version,
                Diagnostics = new Container<Diagnostic>(diagnostics),
            });

            return Unit.Task;
        }

        public override async Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Hello world!");
            await configuration.GetScopedConfiguration(request.TextDocument.Uri);
            return Unit.Value;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            if (configuration.TryGetScopedConfiguration(request.TextDocument.Uri, out var disposable))
            {
                disposable.Dispose();
            }

            return Unit.Task;
        }
    }
}
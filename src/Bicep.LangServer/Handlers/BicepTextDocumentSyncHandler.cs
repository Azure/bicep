using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.LanguageServer.CompilationManager;
using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Bicep.LanguageServer.Handlers
{
    class BicepTextDocumentSyncHandler : TextDocumentSyncHandler
    {
        private readonly ICompilationManager compilationManager;
        private readonly ILogger<BicepTextDocumentSyncHandler> logger;
        private readonly ILanguageServerConfiguration configuration;
        private readonly ILanguageServer server;

        private static TextDocumentSaveRegistrationOptions GetSaveRegistrationOptions()
            => new TextDocumentSaveRegistrationOptions
            {
                DocumentSelector = DocumentSelector.ForLanguage(LanguageServerConstants.LanguageId),
                IncludeText = true,
            };

        public BicepTextDocumentSyncHandler(
            ICompilationManager compilationManager,
            ILogger<BicepTextDocumentSyncHandler> logger,
            ILanguageServerConfiguration configuration,
            ILanguageServer server)
            : base(TextDocumentSyncKind.Full, GetSaveRegistrationOptions())
        {
            this.compilationManager = compilationManager;
            this.logger = logger;
            this.configuration = configuration;
            this.server = server;
        }

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            return new TextDocumentAttributes(uri, LanguageServerConstants.LanguageId);
        }

        public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken token)
        {
            // we have full sync enabled, so apparently first change is the whole document
            var contents = request.ContentChanges.First().Text;

            this.compilationManager.UpsertCompilation(request.TextDocument.Uri, request.TextDocument.Version, contents);

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            //await configuration.GetScopedConfiguration(request.TextDocument.Uri);

            this.compilationManager.UpsertCompilation(request.TextDocument.Uri, request.TextDocument.Version, request.TextDocument.Text);
            
            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            // nothing needs to be done when the document is saved
            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            //if (configuration.TryGetScopedConfiguration(request.TextDocument.Uri, out var disposable))
            //{
            //    disposable.Dispose();
            //}

            this.compilationManager.CloseCompilation(request.TextDocument.Uri);
            return Unit.Task;
        }
    }
}
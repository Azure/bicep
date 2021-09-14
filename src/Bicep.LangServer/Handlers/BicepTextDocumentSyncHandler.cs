// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepTextDocumentSyncHandler : TextDocumentSyncHandlerBase
    {
        private readonly ICompilationManager compilationManager;

        public BicepTextDocumentSyncHandler(ICompilationManager compilationManager)
        {
            this.compilationManager = compilationManager;
        }

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            return new TextDocumentAttributes(uri, LanguageConstants.LanguageId);
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
            this.compilationManager.UpsertCompilation(request.TextDocument.Uri, request.TextDocument.Version, request.TextDocument.Text, request.TextDocument.LanguageId);

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            // nothing needs to be done when the document is saved
            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            this.compilationManager.CloseCompilation(request.TextDocument.Uri);
            return Unit.Task;
        }

        protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            Change = TextDocumentSyncKind.Full,
            DocumentSelector = DocumentSelectorFactory.CreateForTextDocumentSync()
        };
    }
}

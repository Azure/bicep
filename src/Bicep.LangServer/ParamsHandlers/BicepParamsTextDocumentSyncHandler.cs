// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Features;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Utils;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.ParamsHandlers
{
    internal class BicepParamsTextDocumentSyncHandler : TextDocumentSyncHandlerBase
    {
        private readonly IParamsCompilationManager paramsCompilationManager;
        private readonly IFeatureProvider features;

        public BicepParamsTextDocumentSyncHandler(IParamsCompilationManager paramsCompilationManager, IFeatureProvider features)
        {
            this.paramsCompilationManager = paramsCompilationManager;
            this.features = features;
        }

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            return new TextDocumentAttributes(uri, LanguageConstants.ParamsLanguageId);
        }

        public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
        {
            // we have full sync enabled, so apparently first change is the whole document
            var contents = request.ContentChanges.First().Text;

            var documentUri = request.TextDocument.Uri;

            if (features.ParamsFilesEnabled)
            {
                this.paramsCompilationManager.UpsertCompilation(documentUri, request.TextDocument.Version, contents);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            if (features.ParamsFilesEnabled)
            {
                this.paramsCompilationManager.UpsertCompilation(request.TextDocument.Uri, request.TextDocument.Version, request.TextDocument.Text, request.TextDocument.LanguageId);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) => Unit.Task;

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            if (features.ParamsFilesEnabled)
            {
                this.paramsCompilationManager.CloseCompilation(request.TextDocument.Uri);
            }

            return Unit.Task;
        }

        protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            Change = TextDocumentSyncKind.Full,
            DocumentSelector = DocumentSelectorFactory.CreateForParamsOnly()
        };
    }
}

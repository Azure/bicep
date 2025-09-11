// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Configuration;
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
        private readonly IBicepConfigChangeHandler bicepConfigChangeHandler;
        private readonly DocumentSelectorFactory documentSelectorFactory;

        public BicepTextDocumentSyncHandler(ICompilationManager compilationManager, IBicepConfigChangeHandler bicepConfigChangeHandler, DocumentSelectorFactory documentSelectorFactory)
        {
            this.bicepConfigChangeHandler = bicepConfigChangeHandler;
            this.compilationManager = compilationManager;
            this.documentSelectorFactory = documentSelectorFactory;
        }

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            if (ConfigurationHelper.IsBicepConfigFile(uri))
            {
                return new TextDocumentAttributes(uri, LanguageConstants.JsoncLanguageId);
            }

            if (uri.ToIOUri().HasBicepParamExtension())
            {
                return new TextDocumentAttributes(uri, LanguageConstants.ParamsLanguageId);
            }

            return new TextDocumentAttributes(uri, LanguageConstants.LanguageId);
        }

        public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken token)
        {
            // we have full sync enabled, so apparently first change is the whole document
            var contents = request.ContentChanges.First().Text;

            var documentUri = request.TextDocument.Uri;

            if (ConfigurationHelper.IsBicepConfigFile(documentUri))
            {
                bicepConfigChangeHandler.HandleBicepConfigChangeEvent(documentUri);
            }
            else
            {
                this.compilationManager.UpdateCompilation(documentUri, request.TextDocument.Version, contents);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json, we'll add an entry to activeBicepConfigCache.
            if (ConfigurationHelper.IsBicepConfigFile(documentUri)) //potentially copy this for bicep params
            {
                bicepConfigChangeHandler.HandleBicepConfigOpenEvent(documentUri);
            }
            else
            {
                this.compilationManager.OpenCompilation(documentUri, request.TextDocument.Version, request.TextDocument.Text, request.TextDocument.LanguageId);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            if (ConfigurationHelper.IsBicepConfigFile(documentUri))
            {
                bicepConfigChangeHandler.HandleBicepConfigSaveEvent(documentUri);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json, we'll remove the entry from activeBicepConfigCache.
            if (ConfigurationHelper.IsBicepConfigFile(documentUri))
            {
                bicepConfigChangeHandler.HandleBicepConfigCloseEvent(documentUri);
            }
            else
            {
                this.compilationManager.CloseCompilation(request.TextDocument.Uri);
            }

            return Unit.Task;
        }

        protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            Change = TextDocumentSyncKind.Full,
            DocumentSelector = documentSelectorFactory.CreateForAllSupportedLangIds()
        };
    }
}

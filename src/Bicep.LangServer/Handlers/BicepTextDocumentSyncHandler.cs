// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
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

        public BicepTextDocumentSyncHandler(ICompilationManager compilationManager, IBicepConfigChangeHandler bicepConfigChangeHandler) 
        {
            this.bicepConfigChangeHandler = bicepConfigChangeHandler;
            this.compilationManager = compilationManager;
        }

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            if(ConfigurationHelper.IsBicepConfigFile(uri))
            {
                return new TextDocumentAttributes(uri, LanguageConstants.JsoncLanguageId);
            }

            return new TextDocumentAttributes(uri, LanguageConstants.LanguageId);
        }

        public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken token)
        {
            // we have full sync enabled, so apparently first change is the whole document
            var contents = request.ContentChanges.First().Text;

            var documentUri = request.TextDocument.Uri;

            this.compilationManager.UpsertCompilation(documentUri, request.TextDocument.Version, contents);

            // Handle scenario where the bicepconfig.json file was opened prior to
            // language service activation. If the config file was opened before the language server
            // activation, there won't be an entry for it in the cache. We'll capture the state of the
            // config file on disk when it's changed and cache it.
            if (ConfigurationHelper.IsBicepConfigFile(documentUri))
            {
                bicepConfigChangeHandler.HandleBicepConfigChangeEvent(documentUri);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json, we'll add an entry to activeBicepConfigCache.
            if (ConfigurationHelper.IsBicepConfigFile(documentUri)) //potentialy copy this for bicep params
            {
                bicepConfigChangeHandler.HandleBicepConfigOpenEvent(documentUri);
            }

            this.compilationManager.UpsertCompilation(documentUri, request.TextDocument.Version, request.TextDocument.Text, request.TextDocument.LanguageId, triggeredByFileOpenEvent: true);

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json and there's an entry in activeBicepConfigCache,
            // we'll use the last known configuration and the one from currently saved config file to figure out
            // if we need to send out telemetry information regarding the config change.
            // We'll also update the entry in activeBicepConfigCache.
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

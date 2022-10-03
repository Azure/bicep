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
        private readonly IFeatureProviderFactory featureProviderFactory;

        public BicepParamsTextDocumentSyncHandler(IParamsCompilationManager paramsCompilationManager, IFeatureProviderFactory featureProviderFactory)
        {
            this.paramsCompilationManager = paramsCompilationManager;
            this.featureProviderFactory = featureProviderFactory;
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

            if (featureProviderFactory.GetFeatureProvider(documentUri.ToUri()).ParamsFilesEnabled)
            {
                this.paramsCompilationManager.UpsertCompilation(documentUri, request.TextDocument.Version, contents);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;
            if (featureProviderFactory.GetFeatureProvider(documentUri.ToUri()).ParamsFilesEnabled)
            {
                this.paramsCompilationManager.UpsertCompilation(documentUri, request.TextDocument.Version, request.TextDocument.Text, request.TextDocument.LanguageId);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken) => Unit.Task;

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;
            if (featureProviderFactory.GetFeatureProvider(documentUri.ToUri()).ParamsFilesEnabled)
            {
                this.paramsCompilationManager.CloseCompilation(documentUri);
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

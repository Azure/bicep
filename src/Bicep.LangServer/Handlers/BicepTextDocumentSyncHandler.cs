// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Configuration;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Telemetry;
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
        private readonly IConfigurationManager configurationManager;
        private readonly ILinterRulesProvider LinterRulesProvider;
        private readonly ITelemetryProvider TelemetryProvider;

        private readonly ConcurrentDictionary<DocumentUri, RootConfiguration> activeBicepConfigCache = new ConcurrentDictionary<DocumentUri, RootConfiguration>();

        public BicepTextDocumentSyncHandler(ICompilationManager compilationManager, IConfigurationManager configurationManager, ITelemetryProvider telemetryProvider, ILinterRulesProvider linterRulesProvider)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;

            this.LinterRulesProvider = linterRulesProvider;
            this.TelemetryProvider = telemetryProvider;
        }

        public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
        {
            return new TextDocumentAttributes(uri, LanguageConstants.LanguageId);
        }

        public override Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken token)
        {
            // we have full sync enabled, so apparently first change is the whole document
            var contents = request.ContentChanges.First().Text;

            var documentUri = request.TextDocument.Uri;

            this.compilationManager.UpsertCompilation(request.TextDocument.Uri, request.TextDocument.Version, contents);

            if (IsBicepConfigFile(documentUri))
            {
                var configuration = configurationManager.GetConfiguration(documentUri.ToUri());
                activeBicepConfigCache.AddOrUpdate(documentUri,
                                                   (documentUri) => configuration,
                                                   (documentUri, prevConfiguration) => configuration);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            if (IsBicepConfigFile(documentUri))
            {
                var configuration = configurationManager.GetConfiguration(documentUri.ToUri());
                activeBicepConfigCache.TryAdd(documentUri, configuration);
            }

            this.compilationManager.UpsertCompilation(documentUri, request.TextDocument.Version, request.TextDocument.Text, request.TextDocument.LanguageId);

            return Unit.Task;
        }

        private bool IsBicepConfigFile(DocumentUri documentUri)
        {
            return string.Equals(Path.GetFileName(documentUri.Path), LanguageConstants.BicepConfigurationFileName);
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            if (IsBicepConfigFile(documentUri) &&
                activeBicepConfigCache.TryRemove(documentUri, out RootConfiguration? prevBicepConfiguration) &&
                prevBicepConfiguration != null)
            {
                var curConfiguration = configurationManager.GetConfiguration(documentUri.ToUri());
                TelemetryHelper.SendTelemetryOnBicepConfigChange(prevBicepConfiguration, curConfiguration, LinterRulesProvider, TelemetryProvider);
            }

            // nothing needs to be done when the document is saved
            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            if (IsBicepConfigFile(documentUri))
            {
                activeBicepConfigCache.TryRemove(documentUri, out _);
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

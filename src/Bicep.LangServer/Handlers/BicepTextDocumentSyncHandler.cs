// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Configuration;
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
        private readonly ILinterRulesProvider linterRulesProvider;
        private readonly ITelemetryProvider telemetryProvider;

        private readonly ConcurrentDictionary<DocumentUri, RootConfiguration> activeBicepConfigCache = new ConcurrentDictionary<DocumentUri, RootConfiguration>();

        public BicepTextDocumentSyncHandler(ICompilationManager compilationManager, IConfigurationManager configurationManager, ITelemetryProvider telemetryProvider, ILinterRulesProvider linterRulesProvider)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
            this.linterRulesProvider = linterRulesProvider;
            this.telemetryProvider = telemetryProvider;
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

            this.compilationManager.UpsertCompilation(documentUri, request.TextDocument.Version, contents);

            // Handle scenario where the bicepconfig.json file was opened prior to
            // language service activation. If the config file was opened before the language server
            // activation, there won't be an entry for it in the cache. We'll capture the state of the
            // config file on disk when it's changes and cache it.
            if (ConfigurationHelper.IsBicepConfigFile(documentUri) &&
                !activeBicepConfigCache.ContainsKey(documentUri) &&
                ConfigurationHelper.TryGetConfiguration(configurationManager, documentUri, out RootConfiguration? configuration))
            {
                activeBicepConfigCache.TryAdd(documentUri, configuration);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json, we'll add an entry to activeBicepConfigCache.
            if (ConfigurationHelper.IsBicepConfigFile(documentUri) &&
                ConfigurationHelper.TryGetConfiguration(configurationManager, documentUri, out RootConfiguration? configuration))
            {
                activeBicepConfigCache.TryAdd(documentUri, configuration);
            }

            this.compilationManager.UpsertCompilation(documentUri, request.TextDocument.Version, request.TextDocument.Text, request.TextDocument.LanguageId);

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json and there's an entry in activeBicepConfigCache,
            // we'll use the last known configuration and the one from currently saved config file to figure out
            // if we need to send out telemetry information regarding the config change.
            // We'll also update the entry in activeBicepConfigCache.
            if (ConfigurationHelper.IsBicepConfigFile(documentUri) &&
                activeBicepConfigCache.TryGetValue(documentUri, out RootConfiguration? prevBicepConfiguration) &&
                prevBicepConfiguration != null &&
                ConfigurationHelper.TryGetConfiguration(configurationManager, documentUri, out RootConfiguration? curConfiguration))
            {
                TelemetryHelper.SendTelemetryOnBicepConfigChange(prevBicepConfiguration, curConfiguration, linterRulesProvider, telemetryProvider);
                activeBicepConfigCache.AddOrUpdate(documentUri, (documentUri) => curConfiguration, (documentUri, prevConfiguration) => curConfiguration);
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json, we'll remove the entry from activeBicepConfigCache.
            if (ConfigurationHelper.IsBicepConfigFile(documentUri))
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

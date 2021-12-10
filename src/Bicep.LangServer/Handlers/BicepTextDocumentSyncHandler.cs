// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
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

            if (IsBicepConfigFile(documentUri))
            {
                try
                {
                    if (!activeBicepConfigCache.ContainsKey(documentUri))
                    {
                        // Handle scenario where the bicepconfig.json file was opened prior to
                        // language service activation. If the config file was opened before the language server
                        // activation, there won't be an entry for it in the cache. We'll capture the state of the
                        // config file on disk when it's changes and cache it.
                        var configuration = configurationManager.GetConfiguration(documentUri.ToUri());
                        activeBicepConfigCache.TryAdd(documentUri, configuration);
                    }
                }
                catch (Exception)
                {
                    // If there was an issue getting RootConfguration, we'll not do anything.
                }
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json, we'll add an entry to activeBicepConfigCache.
            if (IsBicepConfigFile(documentUri))
            {
                try
                {
                    var configuration = configurationManager.GetConfiguration(documentUri.ToUri());
                    activeBicepConfigCache.TryAdd(documentUri, configuration);
                }
                catch (Exception)
                {
                    // If there was an issue getting RootConfguration, we'll not do anything.
                }
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
            if (IsBicepConfigFile(documentUri) &&
                activeBicepConfigCache.TryGetValue(documentUri, out RootConfiguration? prevBicepConfiguration) &&
                prevBicepConfiguration != null)
            {
                try
                {
                    var curConfiguration = configurationManager.GetConfiguration(documentUri.ToUri());
                    TelemetryHelper.SendTelemetryOnBicepConfigChange(prevBicepConfiguration, curConfiguration, linterRulesProvider, telemetryProvider);

                    activeBicepConfigCache.AddOrUpdate(documentUri, (documentUri) => curConfiguration, (documentUri, prevConfiguration) => curConfiguration);
                }
                catch (Exception)
                {
                    // If there was an issue getting RootConfguration, we'll fail silently and not do anything.
                }
            }

            return Unit.Task;
        }

        public override Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
        {
            var documentUri = request.TextDocument.Uri;

            // If the documentUri corresponds to bicepconfig.json, we'll remove the entry from activeBicepConfigCache.
            if (IsBicepConfigFile(documentUri))
            {
                activeBicepConfigCache.TryRemove(documentUri, out _);
            }

            this.compilationManager.CloseCompilation(request.TextDocument.Uri);
            return Unit.Task;
        }

        private bool IsBicepConfigFile(DocumentUri documentUri)
        {
            try
            {
                return string.Equals(Path.GetFileName(documentUri.Path), LanguageConstants.BicepConfigurationFileName, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                // If we encounter any issues while getting file name, we'll return false.
                return false;
            }
        }

        protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            Change = TextDocumentSyncKind.Full,
            DocumentSelector = DocumentSelectorFactory.CreateForTextDocumentSync()
        };
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using LspDiagnostic = OmniSharp.Extensions.LanguageServer.Protocol.Models.Diagnostic;

namespace Bicep.LanguageServer.BicepConfig
{
    public class BicepConfigLifecycleManager : IBicepConfigLifecycleManager
    {
        private readonly ICompilationManager compilationManager;
        private readonly IBicepConfigurationManager bicepConfigurationManager;
        private readonly ILanguageServerFacade server;

        public BicepConfigLifecycleManager(ICompilationManager compilationManager,
                           IBicepConfigurationManager bicepConfigurationManager,
                           ILanguageServerFacade server)
        {
            this.compilationManager = compilationManager;
            this.bicepConfigurationManager = bicepConfigurationManager;
            this.server = server;
        }

        public void RefreshCompilationOfSourceFilesInWorkspace()
        {
            bicepConfigurationManager.PurgeAllCaches();
            // We shouldn't need to reload auxiliary files if a configuration file has changed.
            compilationManager.RefreshAllActiveCompilations(forceReloadAuxiliaryFiles: false);
        }

        public void HandleBicepConfigOpenEvent(DocumentUri documentUri)
        {
            HandleBicepConfigOpenOrChangeEvent(documentUri);
            PublishConfigDiagnostics(documentUri);
        }

        public void HandleBicepConfigChangeEvent(DocumentUri documentUri)
        {
            // A change event can represent file creation, modification, or deletion.
            // Creation and deletion change config file discovery (the lookup cache), so we
            // must do a full purge rather than a targeted invalidation.
            bicepConfigurationManager.PurgeAllCaches();
            HandleBicepConfigOpenOrChangeEvent(documentUri);
        }

        private void HandleBicepConfigOpenOrChangeEvent(DocumentUri documentUri)
            => bicepConfigurationManager.PurgeCacheForAffectedChains(documentUri.ToIOUri());

        public void HandleBicepConfigSaveEvent(DocumentUri documentUri)
        {
            bicepConfigurationManager.PurgeCacheForAffectedChains(documentUri.ToIOUri());
            PublishConfigDiagnostics(documentUri);
        }

        public void HandleBicepConfigCloseEvent(DocumentUri documentUri)
        {
            bicepConfigurationManager.PurgeCacheForAffectedChains(documentUri.ToIOUri());

            // Clear squiggles when the file is closed.
            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = documentUri,
                Diagnostics = new Container<LspDiagnostic>()
            });
        }

        private void PublishConfigDiagnostics(DocumentUri documentUri)
        {
            var chain = bicepConfigurationManager.GetConfigurationChain(documentUri.ToIOUri());

            // Always publish to documentUri first — covers chain-level errors (cycle, too deep)
            // stored on the built-in fallback layer, and clears stale squiggles on the active file.
            var effectiveDiagnostics = chain.GetEffectiveConfiguration().GetDiagnostics();
            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = documentUri,
                Diagnostics = new Container<LspDiagnostic>(effectiveDiagnostics.Select(d => new LspDiagnostic
                {
                    Severity = ToLspSeverity(d.Level),
                    Code = new DiagnosticCode(d.Code),
                    Message = d.Message,
                    Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 0, 0, 0),
                    Source = "bicep"
                }))
            });

            // Publish diagnostics for every user-layer config file in the chain — including empty
            // arrays so the client clears stale squiggles for files that no longer have errors.
            foreach (var (fileUri, diagnostics) in chain.EnumerateDiagnosticsPerFile())
            {
                server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
                {
                    Uri = DocumentUri.From(fileUri.ToUri()),
                    Diagnostics = new Container<LspDiagnostic>(diagnostics.Select(d => new LspDiagnostic
                    {
                        Severity = ToLspSeverity(d.Level),
                        Code = new DiagnosticCode(d.Code),
                        Message = d.Message,
                        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 0, 0, 0),
                        Source = "bicep"
                    }))
                });
            }
        }

        private static DiagnosticSeverity ToLspSeverity(DiagnosticLevel level) => level switch
        {
            DiagnosticLevel.Error => DiagnosticSeverity.Error,
            DiagnosticLevel.Warning => DiagnosticSeverity.Warning,
            DiagnosticLevel.Info => DiagnosticSeverity.Information,
            _ => DiagnosticSeverity.Hint
        };
    }
}

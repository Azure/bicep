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
        private readonly ConfigurationManager configurationManager;
        private readonly IBicepConfigurationManager bicepConfigurationManager;
        private readonly ILanguageServerFacade server;

        public BicepConfigLifecycleManager(ICompilationManager compilationManager,
                           ConfigurationManager configurationManager,
                           IBicepConfigurationManager bicepConfigurationManager,
                           ILanguageServerFacade server)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
            this.bicepConfigurationManager = bicepConfigurationManager;
            this.server = server;
        }

        public void RefreshCompilationOfSourceFilesInWorkspace()
        {
            configurationManager.PurgeCache();
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
            configurationManager.PurgeCache();
            HandleBicepConfigOpenOrChangeEvent(documentUri);
            PublishConfigDiagnostics(documentUri);
        }

        private void HandleBicepConfigOpenOrChangeEvent(DocumentUri documentUri)
            => configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri());

        public void HandleBicepConfigSaveEvent(DocumentUri documentUri)
        {
            configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri());
            PublishConfigDiagnostics(documentUri);
        }

        public void HandleBicepConfigCloseEvent(DocumentUri documentUri)
        {
            configurationManager.RemoveConfigCacheEntry(documentUri.ToIOUri());

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
            var diagnostics = chain.GetEffectiveConfiguration().GetDiagnostics();

            var lspDiagnostics = diagnostics.Select(d => new LspDiagnostic
            {
                Severity = ToLspSeverity(d.Level),
                Code = new DiagnosticCode(d.Code),
                Message = d.Message,
                Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 0, 0, 0),
                Source = "bicep"
            });

            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = documentUri,
                Diagnostics = new Container<LspDiagnostic>(lspDiagnostics)
            });
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

namespace Bicep.LanguageServer.BicepConfig
{
    public class BicepConfigLifecycleManager : IBicepConfigLifecycleManager
    {
        private readonly ICompilationManager compilationManager;
        private readonly ConfigurationManager configurationManager;
<<<<<<< HEAD

        public BicepConfigLifecycleManager(ICompilationManager compilationManager,
                           ConfigurationManager configurationManager)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
=======
        private readonly IBicepConfigurationManager bicepConfigurationManager;
        private readonly ILanguageServerFacade server;
        private readonly ILinterRulesProvider linterRulesProvider;
        private readonly ITelemetryProvider telemetryProvider;
        private readonly IActiveSourceFileSet workspace;

        public BicepConfigLifecycleManager(ICompilationManager compilationManager,
                           ConfigurationManager configurationManager,
                           IBicepConfigurationManager bicepConfigurationManager,
                           ILanguageServerFacade server,
                           ILinterRulesProvider linterRulesProvider,
                           ITelemetryProvider telemetryProvider,
                           IActiveSourceFileSet workspace)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
            this.bicepConfigurationManager = bicepConfigurationManager;
            this.server = server;
            this.linterRulesProvider = linterRulesProvider;
            this.telemetryProvider = telemetryProvider;
            this.workspace = workspace;
>>>>>>> af7513a9a (add 'extends' schema support and surface config chain diagnostics in VS Code)
        }

        public void RefreshCompilationOfSourceFilesInWorkspace()
        {
            configurationManager.PurgeCache();
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
            configurationManager.PurgeCache();
            HandleBicepConfigOpenOrChangeEvent(documentUri);
            PublishConfigDiagnostics(documentUri);
        }

        private void HandleBicepConfigOpenOrChangeEvent(DocumentUri documentUri)
            => configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri());

        public void HandleBicepConfigSaveEvent(DocumentUri documentUri)
<<<<<<< HEAD
            => configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri());
=======
        {
            if (configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri()) is { } update)
            {
                TelemetryHelper.SendTelemetryOnBicepConfigChange(update.prevConfiguration, update.newConfiguration, linterRulesProvider, telemetryProvider);
            }

            PublishConfigDiagnostics(documentUri);
        }
>>>>>>> af7513a9a (add 'extends' schema support and surface config chain diagnostics in VS Code)

        public void HandleBicepConfigCloseEvent(DocumentUri documentUri)
        {
            configurationManager.RemoveConfigCacheEntry(documentUri.ToIOUri());

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
            var diagnostics = chain.GetEffectiveConfiguration().GetDiagnostics();

            var lspDiagnostics = diagnostics.Select(d => new LspDiagnostic
            {
                Severity = ToLspSeverity(d.Level),
                Code = new DiagnosticCode(d.Code),
                Message = d.Message,
                Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 0, 0, 0),
                Source = "bicep"
            });

            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = documentUri,
                Diagnostics = new Container<LspDiagnostic>(lspDiagnostics)
            });
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

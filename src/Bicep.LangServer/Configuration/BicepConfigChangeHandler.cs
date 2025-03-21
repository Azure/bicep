// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.SourceGraph;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Telemetry;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandler : IBicepConfigChangeHandler
    {
        private readonly ICompilationManager compilationManager;
        private readonly ConfigurationManager configurationManager;
        private readonly ILinterRulesProvider linterRulesProvider;
        private readonly ITelemetryProvider telemetryProvider;
        private readonly IWorkspace workspace;

        public BicepConfigChangeHandler(ICompilationManager compilationManager,
                                        ConfigurationManager configurationManager,
                                        ILinterRulesProvider linterRulesProvider,
                                        ITelemetryProvider telemetryProvider,
                                        IWorkspace workspace)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
            this.linterRulesProvider = linterRulesProvider;
            this.telemetryProvider = telemetryProvider;
            this.workspace = workspace;
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
        }

        public void HandleBicepConfigChangeEvent(DocumentUri documentUri)
        {
            HandleBicepConfigOpenOrChangeEvent(documentUri);
            // The change may have rendered a config file invalid, or the event itself may have represented a file creation or deletion.
            // In either case, the lookup cache would be stale.
            configurationManager.PurgeLookupCache();
        }

        private void HandleBicepConfigOpenOrChangeEvent(DocumentUri documentUri)
            => configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri());

        public void HandleBicepConfigSaveEvent(DocumentUri documentUri)
        {
            if (configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri()) is { } update)
            {
                TelemetryHelper.SendTelemetryOnBicepConfigChange(update.prevConfiguration, update.newConfiguration, linterRulesProvider, telemetryProvider);
            }
        }

        public void HandleBicepConfigCloseEvent(DocumentUri documentUri)
            => configurationManager.RemoveConfigCacheEntry(documentUri.ToIOUri());
    }
}

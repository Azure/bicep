// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.LanguageServer.Telemetry;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.Configuration
{
    public class BicepConfigChangeHandlingConfigurationManager : ConfigurationManager, IBicepConfigChangeHandler
    {
        private readonly ILinterRulesProvider linterRulesProvider;
        private readonly ITelemetryProvider telemetryProvider;

        public BicepConfigChangeHandlingConfigurationManager(
            ILinterRulesProvider linterRulesProvider,
            ITelemetryProvider telemetryProvider,
            IFileSystem fileSystem) : base(fileSystem)
        {
            this.linterRulesProvider = linterRulesProvider;
            this.telemetryProvider = telemetryProvider;
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
            PurgeLookupCache();
        }

        private void HandleBicepConfigOpenOrChangeEvent(DocumentUri documentUri) => RefreshConfigCacheEntry(documentUri.ToUri());

        public void HandleBicepConfigSaveEvent(DocumentUri documentUri)
        {
            if (RefreshConfigCacheEntry(documentUri.ToUri()) is {} update)
            {
                TelemetryHelper.SendTelemetryOnBicepConfigChange(update.prevConfiguration, update.newConfiguration, linterRulesProvider, telemetryProvider);
            }
            // It's possible the saved document is invalid, which could render the lookup cache invalid as well.
            PurgeLookupCache();
        }

        public void HandleBicepConfigCloseEvent(DocumentUri documentUri) 
            => RemoveConfigCacheEntry(documentUri.ToUri());
    }
}

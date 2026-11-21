// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.BicepConfig
{
    /// <summary>
    /// Publishes diagnostics for open configuration files. Cached configuration keeps itself current
    /// (see <see cref="Bicep.Core.Configuration.BicepConfigurationManager"/>), and compilations are refreshed when it changes.
    /// </summary>
    public interface IBicepConfigLifecycleManager
    {
        void HandleBicepConfigOpenEvent(DocumentUri documentUri);

        void HandleBicepConfigChangeEvent(DocumentUri documentUri);

        void HandleBicepConfigSaveEvent(DocumentUri documentUri);

        void HandleBicepConfigCloseEvent(DocumentUri documentUri);
    }
}

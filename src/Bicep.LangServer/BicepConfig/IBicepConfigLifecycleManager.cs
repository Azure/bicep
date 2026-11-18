// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.BicepConfig
{
    public interface IBicepConfigLifecycleManager
    {
        /// <summary>
        /// Invalidates cached configuration affected by files created, changed or deleted on disk, and refreshes
        /// active compilations if any configuration they use may have changed.
        /// </summary>
        void HandleFileChanges(IEnumerable<FileEvent> fileEvents);

        void HandleBicepConfigOpenEvent(DocumentUri documentUri);

        void HandleBicepConfigChangeEvent(DocumentUri documentUri);

        void HandleBicepConfigSaveEvent(DocumentUri documentUri);

        void HandleBicepConfigCloseEvent(DocumentUri documentUri);
    }
}

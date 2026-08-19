// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.LanguageServer.Compilation;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LanguageServer.BicepConfig
{
    public class BicepConfigLifecycleManager : IBicepConfigLifecycleManager
    {
        private readonly ICompilationManager compilationManager;
        private readonly ConfigurationManager configurationManager;

        public BicepConfigLifecycleManager(ICompilationManager compilationManager,
                           ConfigurationManager configurationManager)
        {
            this.compilationManager = compilationManager;
            this.configurationManager = configurationManager;
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
            // A change event can represent file creation, modification, or deletion.
            // Creation and deletion change config file discovery (the lookup cache), so we
            // must do a full purge rather than a targeted invalidation.
            configurationManager.PurgeCache();
            HandleBicepConfigOpenOrChangeEvent(documentUri);
        }

        private void HandleBicepConfigOpenOrChangeEvent(DocumentUri documentUri)
            => configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri());

        public void HandleBicepConfigSaveEvent(DocumentUri documentUri)
            => configurationManager.RefreshConfigCacheEntry(documentUri.ToIOUri());

        public void HandleBicepConfigCloseEvent(DocumentUri documentUri)
            => configurationManager.RemoveConfigCacheEntry(documentUri.ToIOUri());
    }
}

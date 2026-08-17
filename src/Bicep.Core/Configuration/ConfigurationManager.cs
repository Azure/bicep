// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly static DiagnosticBuilder.DiagnosticBuilderInternal ConfigDiagnosticBuilder = DiagnosticBuilder.ForDocumentStart();
        private readonly ConcurrentDictionary<IFileHandle, ResultWithDiagnostic<RootConfiguration>> loadedConfigCache = new();
        private readonly IFileExplorer fileExplorer;
        private readonly IBicepConfigurationManager bicepConfigurationManager;

        public ConfigurationManager(IFileExplorer fileExplorer, IBicepConfigurationManager bicepConfigurationManager)
        {
            this.fileExplorer = fileExplorer;
            this.bicepConfigurationManager = bicepConfigurationManager;
        }

        public RootConfiguration GetConfiguration(IOUri sourceFileUri)
            => this.bicepConfigurationManager.GetMergedConfiguration(sourceFileUri);

        public void PurgeCache()
        {
            loadedConfigCache.Clear();
            this.bicepConfigurationManager.PurgeChainCache();
        }

        public void PurgeLookupCache() => this.bicepConfigurationManager.PurgeChainCache();

        public (RootConfiguration prevConfiguration, RootConfiguration newConfiguration)? RefreshConfigCacheEntry(IOUri configFileIdentifier)
        {
            (RootConfiguration, RootConfiguration)? returnVal = null;
            var configFileHandle = this.fileExplorer.GetFile(configFileIdentifier);
            loadedConfigCache.AddOrUpdate(configFileHandle, LoadConfiguration, (handle, prev) =>
            {
                var reloaded = LoadConfiguration(handle);
                if (prev.IsSuccess(out var prevConfig) && reloaded.IsSuccess(out var newConfig))
                {
                    returnVal = (prevConfig, newConfig);
                }
                return reloaded;
            });

            // Targeted invalidation: only chains that include this file are stale.
            this.bicepConfigurationManager.PurgeCacheForAffectedChains(configFileIdentifier);

            return returnVal;
        }

        public void RemoveConfigCacheEntry(IOUri configFileUri)
        {
            var configFileHandle = this.fileExplorer.GetFile(configFileUri);
            loadedConfigCache.TryRemove(configFileHandle, out _);

            // Targeted invalidation: only chains that include this file are stale.
            this.bicepConfigurationManager.PurgeCacheForAffectedChains(configFileUri);
        }

        private static ResultWithDiagnostic<RootConfiguration> LoadConfiguration(IFileHandle configFileHandle)
        {
            try
            {
                using var stream = configFileHandle.OpenRead();
                var element = IConfigurationManager.BuiltInConfigurationElement.Merge(JsonElementFactory.CreateElementFromStream(stream));

                return RootConfiguration.Bind(element, configFileHandle.Uri);
            }
            catch (ConfigurationException exception)
            {
                return new(ConfigDiagnosticBuilder.InvalidBicepConfigFile(configFileHandle.Uri, exception.Message));
            }
            catch (JsonException exception)
            {
                return new(ConfigDiagnosticBuilder.UnparsableBicepConfigFile(configFileHandle.Uri, exception.Message));
            }
            catch (Exception exception)
            {
                return new(ConfigDiagnosticBuilder.UnloadableBicepConfigFile(configFileHandle.Uri, exception.Message));
            }
        }
    }
}

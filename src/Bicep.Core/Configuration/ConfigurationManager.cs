// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Security;
using System.ServiceModel;
using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using Bicep.Core.TypeSystem;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly static DiagnosticBuilder.DiagnosticBuilderInternal ConfigDiagnosticBuilder = DiagnosticBuilder.ForDocumentStart();
        private readonly ConcurrentDictionary<IDirectoryHandle, ResultWithDiagnostic<IFileHandle?>> configFileLookupCache = new(); // Source file directory handle -> config file handle.
        private readonly ConcurrentDictionary<IFileHandle, ResultWithDiagnostic<RootConfiguration>> loadedConfigCache = new();     // Config file handle -> RootConfiguration.
        private readonly IFileExplorer fileExplorer;

        public ConfigurationManager(IFileExplorer fileExplorer)
        {
            this.fileExplorer = fileExplorer;
        }

        public RootConfiguration GetConfiguration(IOUri sourceFileUri)
        {
            if (!sourceFileUri.IsFile)
            {
                return GetDefaultConfiguration();
            }

            var sourceDirectory = this.fileExplorer.GetFile(sourceFileUri).GetParent();

            if (!configFileLookupCache.GetOrAdd(sourceDirectory, LookupConfigurationFile).IsSuccess(out var configFileHandle, out var lookupDiagnostic))
            {
                return GetDefaultConfiguration().With(diagnostics: [lookupDiagnostic]);
            }

            if (configFileHandle is null)
            {
                return GetDefaultConfiguration();
            }

            if (!loadedConfigCache.GetOrAdd(configFileHandle, LoadConfiguration).IsSuccess(out var configuration, out var loadDiagnostic))
            {
                return GetDefaultConfiguration().With(diagnostics: [loadDiagnostic]);
            }

            return configuration;
        }

        public void PurgeCache()
        {
            PurgeLookupCache();
            loadedConfigCache.Clear();
        }

        public void PurgeLookupCache() => configFileLookupCache.Clear();

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

            return returnVal;
        }

        public void RemoveConfigCacheEntry(IOUri configFileUri)
        {
            var configFileHandle = this.fileExplorer.GetFile(configFileUri);
            if (loadedConfigCache.TryRemove(configFileHandle, out _))
            {
                // If a config file has been removed from a workspace, the lookup cache is no longer valid.
                PurgeLookupCache();
            }
        }

        private static RootConfiguration GetDefaultConfiguration() => IConfigurationManager.GetBuiltInConfiguration();

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

        private ResultWithDiagnostic<IFileHandle?> LookupConfigurationFile(IDirectoryHandle? directoryToLookup)
        {
            try
            {
                while (directoryToLookup is not null)
                {
                    var configFileHandle = directoryToLookup.GetFile(LanguageConstants.BicepConfigurationFileName);

                    if (configFileHandle.Exists())
                    {
                        return new(configFileHandle);
                    }

                    directoryToLookup = directoryToLookup.GetParent();
                }
            }
            catch (IOException exception)
            {
                return new(ConfigDiagnosticBuilder.PotentialConfigDirectoryCouldNotBeScanned(directoryToLookup?.Uri, exception.Message));
            }

            return new((IFileHandle?)null);
        }
    }
}

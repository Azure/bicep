// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security;
using System.Text.Json;

namespace Bicep.Core.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ConcurrentDictionary<Uri, (RootConfiguration? config, DiagnosticBuilder.DiagnosticBuilderDelegate? loadError)> configCache = new();
        private readonly ConcurrentDictionary<Uri, ConfigLookupResult> configLookupCache = new();
        private readonly IFileSystem fileSystem;

        public ConfigurationManager(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public RootConfiguration GetConfiguration(Uri sourceFileUri)
        {
            var (config, diagnosticBuilders) = GetConfigurationFromCache(sourceFileUri);
            return diagnosticBuilders.Count > 0
                ? new(config.Cloud, config.ModuleAliases, config.Analyzers, config.CacheRootDirectory, config.ExperimentalFeaturesEnabled, config.ConfigurationPath, diagnosticBuilders)
                : config;
        }

        public (RootConfiguration, List<DiagnosticBuilder.DiagnosticBuilderDelegate>) GetConfigurationFromCache(Uri sourceFileUri)
        {
            List<DiagnosticBuilder.DiagnosticBuilderDelegate> diagnostics = new();
            var loadErrorEncountered = false;

            var (configFileUri, lookupDiagnostic) = configLookupCache.GetOrAdd(sourceFileUri, LookupConfiguration);
            if (lookupDiagnostic is not null)
            {
                diagnostics.Add(lookupDiagnostic);
            }

            if (configFileUri is not null)
            {
                var (config, loadError) = configCache.GetOrAdd(configFileUri, LoadConfiguration);
                if (loadError is not null)
                {
                    loadErrorEncountered = true;
                    diagnostics.Add(loadError);
                }

                if (config is not null)
                {
                    return (config, diagnostics);
                }
            }

            return (GetDefaultConfiguration(loadErrorEncountered), diagnostics);
        }

        protected virtual RootConfiguration GetDefaultConfiguration(bool loadErrorEncountered) => IConfigurationManager.GetBuiltInConfiguration();

        protected void PurgeLookupCache() => configLookupCache.Clear();

        protected (RootConfiguration prevConfiguration, RootConfiguration newConfiguration)? RefreshConfigCacheEntry(Uri configUri)
        {
            (RootConfiguration, RootConfiguration)? returnVal = default;
            configCache.AddOrUpdate(configUri, LoadConfiguration, (uri, prev) => {
                var reloaded = LoadConfiguration(uri);
                if (prev.config is {} prevConfig && reloaded.Item1 is {} newConfig)
                {
                    returnVal = (prevConfig, newConfig);
                }
                return reloaded;
            });

            return returnVal;
        }

        protected void RemoveConfigCacheEntry(Uri configUri)
        {
            if (configCache.TryRemove(configUri, out _))
            {
                // If a config file has been removed from a workspace, the lookup cache is no longer valid.
                PurgeLookupCache();
            }
        }

        private (RootConfiguration?, DiagnosticBuilder.DiagnosticBuilderDelegate?) LoadConfiguration(Uri configurationUri)
        {
            try
            {
                using var stream = fileSystem.FileStream.Create(configurationUri.LocalPath, FileMode.Open, FileAccess.Read);
                var element = IConfigurationManager.BuiltInConfigurationElement.Merge(JsonElementFactory.CreateElement(stream));

                return (RootConfiguration.Bind(element, configurationUri.LocalPath), default);
            } catch (ConfigurationException exception)
            {
                return (default, x => x.InvalidBicepConfigFile(configurationUri.LocalPath, exception.Message));
            }
            catch (JsonException exception)
            {
                return (default, x => x.UnparsableBicepConfigFile(configurationUri.LocalPath, exception.Message));
            }
            catch (Exception exception)
            {
                return (default, x => x.UnloadableBicepConfigFile(configurationUri.LocalPath, exception.Message));
            }
        }

        private ConfigLookupResult LookupConfiguration(Uri sourceFileUri)
        {
            DiagnosticBuilder.DiagnosticBuilderDelegate? lookupDiagnostic = default;
            if (sourceFileUri.Scheme == Uri.UriSchemeFile)
            {
                string? currentDirectory = fileSystem.Path.GetDirectoryName(sourceFileUri.LocalPath);
                while (!string.IsNullOrEmpty(currentDirectory))
                {
                    var configurationPath = this.fileSystem.Path.Combine(currentDirectory, LanguageConstants.BicepConfigurationFileName);

                    if (this.fileSystem.File.Exists(configurationPath))
                    {
                        return new(PathHelper.FilePathToFileUrl(configurationPath), lookupDiagnostic);
                    }

                    try
                    {
                        // Catching Directory.GetParent alone because it is the only one that throws IO related exceptions.
                        // Path.Combine only throws ArgumentNullException which indicates a bug in our code.
                        // File.Exists will not throw exceptions regardless the existence of path or if the user has permissions to read the file.
                        currentDirectory = this.fileSystem.Directory.GetParent(currentDirectory)?.FullName;
                    }
                    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException)
                    {
                        // The exception could happen in senarios where users may not have read permission on the parent folder.
                        // We should not throw ConfigurationException in such cases since it will block compilation.
                        lookupDiagnostic = x => x.PotentialConfigDirectoryCouldNotBeScanned(currentDirectory, exception.Message);
                        break;
                    }
                }
            }

            return new(default, lookupDiagnostic);
        }

        private record ConfigLookupResult(Uri? configFileUri = default, DiagnosticBuilder.DiagnosticBuilderDelegate? lookupDiagnostic = default);
    }
}

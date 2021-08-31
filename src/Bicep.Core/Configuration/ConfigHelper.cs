// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Bicep.Core.Configuration
{
    public class ConfigHelper
    {
        private const string bicepConfigResourceName = "Bicep.Core.Configuration.bicepconfig.json";

        /// <summary>
        /// Property exposes the configuration root
        /// that is currently loaded
        /// </summary>
        public IConfigurationRoot Config { get; private set; }

        public ConfigHelper(bool loadDefaultConfig = false, BicepConfig? bicepConfig = null, string? localFolder = null)
        {
            if (loadDefaultConfig)
            {
                this.Config = BuildConfig((string?)null);
            }
            else
            {
                localFolder = localFolder ?? Directory.GetCurrentDirectory();
                this.Config = BuildConfig(localFolder, bicepConfig);
            }
        }

        private IConfigurationRoot BuildConfig(Uri fileUri)
        {
            string? localFolder = fileUri.IsFile ? fileUri.LocalPath : null;
            return BuildConfig(localFolder);
        }

        private IConfigurationRoot BuildConfig(string? localFolder, BicepConfig? bicepConfig = null)
        {
            var configBuilder = new ConfigurationBuilder();

            // load the default settings from file embedded as resource
            var assembly = Assembly.GetExecutingAssembly();

            // keep this stream open until after Build() call
            using (var defaultConfigStream = assembly.GetManifestResourceStream(bicepConfigResourceName))
            {
                Debug.Assert(defaultConfigStream != null, "Default configuration file should exist as embedded resource.");
                configBuilder.AddJsonStream(defaultConfigStream);

                // last added json settings take precedent - add local settings last
                if (DiscoverLocalConfigurationFile(localFolder) is string localConfig)
                {
                    // bicepconfig.json file was modified but not saved. If the
                    // file path matches, we'll rebuild the config with updated contents
                    if (bicepConfig is not null &&
                        !string.IsNullOrWhiteSpace(bicepConfig.Contents) &&
                        bicepConfig.Uri.LocalPath is string localPath &&
                        string.Equals(localPath, localConfig, StringComparison.OrdinalIgnoreCase))
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(bicepConfig.Contents);
                        MemoryStream stream = new MemoryStream(byteArray);
                        configBuilder.AddJsonStream(stream);
                    }
                    else
                    {
                        // we must set reloadOnChange to false here - if it set to true, then ConfigurationBuilder will initialize 
                        // a FileSystem.Watcher instance - which has severe performance impact on non-Windows OSes (https://github.com/dotnet/runtime/issues/42036)
                        configBuilder.AddJsonFile(localConfig, optional: true, reloadOnChange: false);
                    }

                    this.CustomSettingsFileName = localConfig;
                }
                else
                {
                    this.CustomSettingsFileName = default;
                }

                try
                {
                    var config = configBuilder.Build();

                    foreach (var kvp in SettingOverrides)
                    {
                        config[kvp.Key] = kvp.Value.ToString();
                    }

                    return this.Config = config;
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        string.Format("Could not load configuration file. {0}", ex.InnerException?.Message ?? ex.Message));
                }
            }
        }

        private string? DiscoverLocalConfigurationFile(string? nextDir)
        {
            try
            {
                while (!string.IsNullOrEmpty(nextDir))
                {
                    while (!string.IsNullOrEmpty(nextDir))
                    {
                        var fileName = Path.Combine(nextDir, LanguageConstants.BicepConfigSettingsFileName);
                        if (File.Exists(fileName))
                        {
                            return fileName;
                        }
                        nextDir = Directory.GetParent(nextDir)?.FullName;
                    }
                }
            }
            catch (Exception)
            {
            }

            return default;
        }

        internal void LoadDefaultConfiguration()
        {
            this.Config = BuildConfig((string?)null);
        }

        internal void LoadConfigurationForSourceFile(Uri fileUri)
        {
            this.Config = BuildConfig(fileUri);
        }

        /// <summary>
        /// Contains path to any custom bicepconfig.json file
        /// that is currently in effect
        /// </summary>
        public string? CustomSettingsFileName { get; private set; }

        /// <summary>
        /// Get setting value from underlying Config
        /// </summary>
        /// <param name="settingPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string settingPath, T defaultValue)
            => ConfigurationBinder.GetValue<T>(this.Config, settingPath, defaultValue);


        #region internal config management for unit tests

        private Dictionary<string, object> SettingOverrides = new Dictionary<string, object>();

        /// <summary>
        /// For unit testing we want to force setting overrides
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ConfigHelper OverrideSetting(string name, object value)
        {
            SettingOverrides[name] = value;
            this.Config = BuildConfig(Directory.GetCurrentDirectory());
            return this;
        }

        internal bool TryGetOverrideSettingValue<T>(string name, out T? value)
        {
            if (SettingOverrides.TryGetValue(name, out var overrideValue)
                && overrideValue is T typedValue)
            {
                value = typedValue;
                return true;
            }
            value = default;
            return false;
        }

        #endregion

    }
}

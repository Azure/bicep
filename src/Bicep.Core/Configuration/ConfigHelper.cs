// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bicep.Core.Configuration
{
    internal class ConfigHelper
    {
        private const string SettingsFileName = "bicepsettings.json";

        /// <summary>
        /// Property exposes the configuration root
        /// that is currently loaded
        /// </summary>
        public IConfigurationRoot Config { get; private set; }

        internal ConfigHelper()
        {
            this.Config = BuildConfig(Directory.GetCurrentDirectory());
        }

        private IConfigurationRoot BuildConfig(string localFolder)
        {
            var configBuilder = new ConfigurationBuilder();

            // load the default settings from file embedded as resource
            var assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            var defaultConfigResourceName = names.FirstOrDefault(n => n.EndsWith(SettingsFileName));

            // keep this stream open until after Build() call
            using (var defaultConfigStream = assembly.GetManifestResourceStream(defaultConfigResourceName))
            {
                Debug.Assert(defaultConfigStream != null, "Default configuration file should exist as embedded resource.");
                configBuilder.AddJsonStream(defaultConfigStream);

                // last added json settings take precedent - add local settings last
                if (DiscoverLocalConfigurationFile(localFolder) is string localConfig)
                {
                    configBuilder.AddJsonFile(localConfig, true, true);
                    this.CustomSettingsFileName = localConfig;
                }
                else
                {
                    this.CustomSettingsFileName = default;
                }

                return configBuilder.Build();
            }
        }

        private string? DiscoverLocalConfigurationFile(string? nextDir)
        {
            while (nextDir != default)
            {
                var fileName = Path.Combine(nextDir, SettingsFileName);
                if (File.Exists(fileName))
                {
                    return fileName;
                }
                nextDir = Directory.GetParent(nextDir)?.FullName;
            }
            return default;
        }

        internal void LoadConfiguration(Uri fileUri)
        {
            var localFile = Path.GetDirectoryName(fileUri.LocalPath);
            this.Config = BuildConfig(localFile);
        }

        /// <summary>
        /// Contains path to any custom bicepsettings.json file
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

    }
}

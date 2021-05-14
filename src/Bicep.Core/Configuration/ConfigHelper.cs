// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bicep.Core.Configuration
{
    internal class ConfigHelper : IDisposable
    {
        private const string settingsfile = "bicepsettings.json";
        private bool disposedValue;

        private readonly IConfigurationBuilder ConfigBuilder;
        private FileSystemWatcher? ConfigWatcher;

        public IConfigurationRoot Config { get; }

        /// <summary>
        /// Subscribe to be notified if the local configuration
        /// file is changed
        /// </summary>
        public event EventHandler? ConfigFileChanged;

        internal ConfigHelper()
        {
            ConfigBuilder = new ConfigurationBuilder();

            // initialize with Global config setting from installation path
            var executePath = Assembly.GetExecutingAssembly().Location;
            var globalConfig = Path.Combine(Path.GetDirectoryName(executePath), settingsfile);
            if (File.Exists(globalConfig))
            {
                this.ConfigBuilder.AddJsonFile(globalConfig, false, true);
            }

            // last added json settings take precedent - add local settings last
            var localConfig = Path.Join(Directory.GetCurrentDirectory(), settingsfile);
            if (File.Exists(localConfig))
            {
                this.ConfigBuilder.AddJsonFile(localConfig, false, true);
                this.ConfigWatcher = new FileSystemWatcher(localConfig);
                this.ConfigWatcher.Changed += ConfigWatcher_Changed;
            }

            this.Config = this.ConfigBuilder.Build();
        }

        /// <summary>
        /// handle filewatcher change notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (this.ConfigFileChanged != null)
            {
                this.ConfigFileChanged(this, EventArgs.Empty);
            }
        }

        public bool GetValue(string name, bool defaultValue)
        {
            if (Config?[name] is string configValue
                && bool.TryParse(configValue, out bool configBool))
            {
                return configBool;
            }
            return defaultValue;
        }

        public int GetValue(string name, int defaultValue)
        {
            if (Config?[name] is string configValue
                && int.TryParse(configValue, out int configInt))
            {
                return configInt;
            }
            return defaultValue;
        }

        public string GetValue(string name, string defaultValue)
            => Config[name] ?? defaultValue;

        public IEnumerable<string> GetValue(string name, IEnumerable<string> defaultValue)
        {
            var values = new List<string>();
            this.Config.Bind(name, values);
            if (values.Any())
            {
                return values;
            }
            return defaultValue;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.ConfigWatcher != default)
                    {
                        this.ConfigWatcher.Changed -= this.ConfigWatcher_Changed;
                        this.ConfigWatcher = default;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ConfigHelper()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

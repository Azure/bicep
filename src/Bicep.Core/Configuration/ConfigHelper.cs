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
    internal class ConfigHelper
    {
        private const string settingsfile = "bicepsettings.json";
        private IConfigurationBuilder ConfigBuilder { get; }
        public IConfigurationRoot Config { get; }

        internal ConfigHelper()
        {
            ConfigBuilder = new ConfigurationBuilder();

            // initialize with Global config setting from installation path
            var executePath = Assembly.GetExecutingAssembly().Location;
            var globalConfig = Path.Combine(Path.GetDirectoryName(executePath), settingsfile);
            this.ConfigBuilder.AddJsonFile(globalConfig, false, true);

            // last added json settings take precedent - add local settings last
            var localConfig = Path.Join(Directory.GetCurrentDirectory(), settingsfile);
            this.ConfigBuilder.AddJsonFile(localConfig, false, true);

            this.Config = this.ConfigBuilder.Build();
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
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;

namespace Bicep.Core.Configuration
{
    public class RootConfiguration
    {
        private RootConfiguration(CloudConfiguration cloud, ModuleAliasesConfiguration moduleAliases, AnalyzersConfiguration analyzers, string? configurationPath)
        {
            this.Cloud = cloud;
            this.ModuleAliases = moduleAliases;
            this.Analyzers = analyzers;
            this.ConfigurationPath = configurationPath;
        }

        public static RootConfiguration Bind(IConfiguration rawConfiguration, string? configurationPath = null, bool disableAnalyzers = false)
        {
            var cloud = CloudConfiguration.Bind(rawConfiguration.GetSection("cloud"), configurationPath);
            var moduleAliases = ModuleAliasesConfiguration.Bind(rawConfiguration.GetSection("moduleAliases"), configurationPath);
            var analyzers = new AnalyzersConfiguration(disableAnalyzers ? null : rawConfiguration.GetSection("analyzers"));

            return new(cloud, moduleAliases, analyzers, configurationPath);
        }

        public CloudConfiguration Cloud { get; }

        public ModuleAliasesConfiguration ModuleAliases { get; }

        public AnalyzersConfiguration Analyzers { get; }

        /// <summary>
        /// Gets the built-in configuraiton manifest resource name if the configuration is a built-in configuraiton,
        /// or a path to a bicepconfig.json file if the configuration is a custom one.
        /// </summary>
        public string? ConfigurationPath { get; }

        public bool IsBuiltIn => ConfigurationPath is null;
    }
}

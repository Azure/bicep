// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;

namespace Bicep.Core.Configuration
{
    public class RootConfiguration
    {
        private RootConfiguration(CloudConfiguration cloud, ModuleAliasesConfiguration moduleAliases, AnalyzersConfiguration analyzers, string resourceName)
        {
            this.Cloud = cloud;
            this.ModuleAliases = moduleAliases;
            this.Analyzers = analyzers;
            this.ResourceName = resourceName;
        }

        public static RootConfiguration Bind(IConfiguration rawConfiguration, string resourceName, bool disableAnalyzers = false)
        {
            var cloud = CloudConfiguration.Bind(rawConfiguration.GetSection("cloud"));
            var moduleAliases = ModuleAliasesConfiguration.Bind(rawConfiguration.GetSection("moduleAliases"));
            var analyzers = new AnalyzersConfiguration(disableAnalyzers ? null : rawConfiguration.GetSection("analyzers"));

            return new(cloud, moduleAliases, analyzers, resourceName);
        }

        public CloudConfiguration Cloud { get; }

        public ModuleAliasesConfiguration ModuleAliases { get; }

        public AnalyzersConfiguration Analyzers { get; }

        /// <summary>
        /// Gets the built-in configuraiton manifest resource name if the configuration is a built-in configuraiton,
        /// or a path to a bicepconfig.json file if the configuration is a custom one.
        /// </summary>
        public string ResourceName { get; }

        public bool IsBuiltIn => ResourceName == ConfigurationManager.BuiltInConfigurationResourceName;
    }
}

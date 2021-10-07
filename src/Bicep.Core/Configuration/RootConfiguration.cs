// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Configuration
{
    public class RootConfiguration
    {
        public RootConfiguration(CloudConfiguration cloud, ModuleAliasesConfiguration moduleAliases, AnalyzersConfiguration analyzers, string? configurationPath)
        {
            this.Cloud = cloud;
            this.ModuleAliases = moduleAliases;
            this.Analyzers = analyzers;
            this.ConfigurationPath = configurationPath;
        }

        public static RootConfiguration Bind(JsonElement element, string? configurationPath = null, bool disableAnalyzers = false)
        {
            var cloud = CloudConfiguration.Bind(element.GetProperty("cloud"), configurationPath);
            var moduleAliases = ModuleAliasesConfiguration.Bind(element.GetProperty("moduleAliases"), configurationPath);
            var analyzers = disableAnalyzers ? AnalyzersConfiguration.Empty : new AnalyzersConfiguration(element.GetProperty("analyzers"));

            return new(cloud, moduleAliases, analyzers, configurationPath);
        }

        public CloudConfiguration Cloud { get; }

        public ModuleAliasesConfiguration ModuleAliases { get; }

        public AnalyzersConfiguration Analyzers { get; }

        public string? ConfigurationPath { get; }

        public bool IsBuiltIn => ConfigurationPath is null;
    }
}

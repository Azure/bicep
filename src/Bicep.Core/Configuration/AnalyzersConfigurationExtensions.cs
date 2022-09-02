// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public static class AnalyzersConfigurationExtensions
    {
        public static AnalyzersConfiguration WithAllAnalyzersDisabled(this AnalyzersConfiguration analyzersConfiguration)
        {
            return AnalyzersConfiguration.Empty;
        }

        public static AnalyzersConfiguration WithAnalyzersDisabled(this AnalyzersConfiguration analyzersConfiguration, params string[] analyzerCodesToDisable)
        {
            var config = analyzersConfiguration;
            foreach (string code in analyzerCodesToDisable)
            {
                config = config.SetValue($"core.rules.{code}.level", "off");
            }

            return config;
        }

        public static RootConfiguration WithAllAnalyzersDisabled(this RootConfiguration analyzersConfiguration)
        {
            return new RootConfiguration(
                analyzersConfiguration.Cloud,
                analyzersConfiguration.ModuleAliases,
                analyzersConfiguration.Analyzers.WithAllAnalyzersDisabled(),
                analyzersConfiguration.ConfigurationPath);
        }

        public static RootConfiguration  WithAnalyzersDisabled(this RootConfiguration analyzersConfiguration, params string[] analyzerCodesToDisable)
        {
            return new RootConfiguration(
                           analyzersConfiguration.Cloud,
                           analyzersConfiguration.ModuleAliases,
                           analyzersConfiguration.Analyzers.WithAnalyzersDisabled(analyzerCodesToDisable),
                           analyzersConfiguration.ConfigurationPath);
        }
    }
}

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

        public static RootConfiguration WithAnalyzers(this RootConfiguration analyzersConfiguration, AnalyzersConfiguration anayzersConfiguration)
        {
            return new RootConfiguration(
                analyzersConfiguration.Cloud,
                analyzersConfiguration.ModuleAliases,
                analyzersConfiguration.ProviderAliases,
                analyzersConfiguration.ProvidersConfig,
                analyzersConfiguration.ImplicitProvidersConfig,
                anayzersConfiguration,
                analyzersConfiguration.CacheRootDirectory,
                analyzersConfiguration.ExperimentalFeaturesEnabled,
                analyzersConfiguration.Formatting,
                analyzersConfiguration.ConfigFileUri,
                analyzersConfiguration.DiagnosticBuilders);
        }

        public static RootConfiguration WithAllAnalyzersDisabled(this RootConfiguration analyzersConfiguration)
        {
            return new RootConfiguration(
                analyzersConfiguration.Cloud,
                analyzersConfiguration.ModuleAliases,
                analyzersConfiguration.ProviderAliases,
                analyzersConfiguration.ProvidersConfig,
                analyzersConfiguration.ImplicitProvidersConfig,
                analyzersConfiguration.Analyzers.WithAllAnalyzersDisabled(),
                analyzersConfiguration.CacheRootDirectory,
                analyzersConfiguration.ExperimentalFeaturesEnabled,
                analyzersConfiguration.Formatting,
                analyzersConfiguration.ConfigFileUri,
                analyzersConfiguration.DiagnosticBuilders);
        }

        public static RootConfiguration WithAnalyzersDisabled(this RootConfiguration analyzersConfiguration, params string[] analyzerCodesToDisable)
        {
            return new RootConfiguration(
                analyzersConfiguration.Cloud,
                analyzersConfiguration.ModuleAliases,
                analyzersConfiguration.ProviderAliases,
                analyzersConfiguration.ProvidersConfig,
                analyzersConfiguration.ImplicitProvidersConfig,
                analyzersConfiguration.Analyzers.WithAnalyzersDisabled(analyzerCodesToDisable),
                analyzersConfiguration.CacheRootDirectory,
                analyzersConfiguration.ExperimentalFeaturesEnabled,
                analyzersConfiguration.Formatting,
                analyzersConfiguration.ConfigFileUri,
                analyzersConfiguration.DiagnosticBuilders);
        }
    }
}

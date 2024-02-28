// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public static class AnalyzersConfigurationExtensions
    {
        public static AnalyzersConfiguration WithAllAnalyzersDisabled(this AnalyzersConfiguration _)
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

        public static RootConfiguration WithAnalyzers(this RootConfiguration current, AnalyzersConfiguration analyzersConfiguration)
        {
            return new RootConfiguration(
                current.Cloud,
                current.ModuleAliases,
                current.ProviderAliases,
                current.ProvidersConfig,
                current.ImplicitProvidersConfig,
                analyzersConfiguration,
                current.CacheRootDirectory,
                current.ExperimentalFeaturesEnabled,
                current.Formatting,
                current.ConfigFileUri,
                current.DiagnosticBuilders);
        }

        public static RootConfiguration WithAllAnalyzersDisabled(this RootConfiguration current)
        {
            return new RootConfiguration(
                current.Cloud,
                current.ModuleAliases,
                current.ProviderAliases,
                current.ProvidersConfig,
                current.ImplicitProvidersConfig,
                current.Analyzers.WithAllAnalyzersDisabled(),
                current.CacheRootDirectory,
                current.ExperimentalFeaturesEnabled,
                current.Formatting,
                current.ConfigFileUri,
                current.DiagnosticBuilders);
        }

        public static RootConfiguration WithAnalyzersDisabled(this RootConfiguration current, params string[] analyzerCodesToDisable)
        {
            return new RootConfiguration(
                current.Cloud,
                current.ModuleAliases,
                current.ProviderAliases,
                current.ProvidersConfig,
                current.ImplicitProvidersConfig,
                current.Analyzers.WithAnalyzersDisabled(analyzerCodesToDisable),
                current.CacheRootDirectory,
                current.ExperimentalFeaturesEnabled,
                current.Formatting,
                current.ConfigFileUri,
                current.DiagnosticBuilders);
        }
    }
}

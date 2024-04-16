// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;

namespace Bicep.Core.Configuration
{
    public static class AnalyzersConfigurationExtensions
    {
        public static AnalyzersConfiguration WithAllAnalyzersDisabled(this AnalyzersConfiguration _) =>
            AnalyzersConfiguration.Empty;

        public static AnalyzersConfiguration WithAnalyzersDisabled(this AnalyzersConfiguration analyzersConfiguration, params string[] analyzerCodesToDisable)
        {
            var config = analyzersConfiguration;
            foreach (string code in analyzerCodesToDisable)
            {
                config = config.SetValue($"core.rules.{code}.level", "off");
            }

            return config;
        }

        public static AnalyzersConfiguration WithAllAnalyzers(this AnalyzersConfiguration analyzersConfiguration)
        {
            var config = analyzersConfiguration;
            foreach (string code in new LinterAnalyzer().GetRuleSet().Where(r => r.DefaultDiagnosticLevel == Diagnostics.DiagnosticLevel.Off).Select(r => r.Code))
            {
                config = config.SetValue($"core.rules.{code}.level", "warning");
            }

            return config;
        }

        public static RootConfiguration WithAnalyzersConfiguration(this RootConfiguration current, AnalyzersConfiguration analyzersConfiguration) =>
            new(
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

        public static RootConfiguration WithAllAnalyzersDisabled(this RootConfiguration current) =>
            current.WithAnalyzersConfiguration(current.Analyzers.WithAllAnalyzersDisabled());

        public static RootConfiguration WithAnalyzersDisabled(this RootConfiguration current, params string[] analyzerCodesToDisable) =>
            current.WithAnalyzersConfiguration(current.Analyzers.WithAnalyzersDisabled(analyzerCodesToDisable));

        public static RootConfiguration WithAllAnalyzers(this RootConfiguration current) =>
            current.WithAnalyzersConfiguration(current.Analyzers.WithAllAnalyzers());
    }
}

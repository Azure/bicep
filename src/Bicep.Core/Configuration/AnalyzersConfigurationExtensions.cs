// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager.Resources.Models;
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
            foreach (var (code, ruleInfo) in new LinterRulesProvider().GetLinterRules())
            {
                if (ruleInfo.defaultDiagnosticLevel == Diagnostics.DiagnosticLevel.Off)
                {
                    config = config.SetValue($"core.rules.{code}.level", "warning");
                }
            }

            return config;
        }

        public static RootConfiguration WithAnalyzersConfiguration(this RootConfiguration current, AnalyzersConfiguration analyzersConfiguration) =>
            new(
                current.Cloud,
                current.ModuleAliases,
                current.Extensions,
                current.ImplicitExtensions,
                analyzersConfiguration,
                current.CacheRootDirectory,
                current.ExperimentalFeaturesEnabled,
                current.Formatting,
                current.ConfigFileUri,
                current.Diagnostics);

        public static RootConfiguration WithAllAnalyzersDisabled(this RootConfiguration current) =>
            current.WithAnalyzersConfiguration(current.Analyzers.WithAllAnalyzersDisabled());

        public static RootConfiguration WithAnalyzersDisabled(this RootConfiguration current, params string[] analyzerCodesToDisable) =>
            current.WithAnalyzersConfiguration(current.Analyzers.WithAnalyzersDisabled(analyzerCodesToDisable));

        public static RootConfiguration WithAllAnalyzers(this RootConfiguration current) =>
            current.WithAnalyzersConfiguration(current.Analyzers.WithAllAnalyzers());

        public static RootConfiguration WithCloudConfiguration(this RootConfiguration current, CloudConfiguration cloudConfiguration) =>
        new(
            cloudConfiguration,
            current.ModuleAliases,
            current.Extensions,
            current.ImplicitExtensions,
            current.Analyzers,
            current.CacheRootDirectory,
            current.ExperimentalFeaturesEnabled,
            current.Formatting,
            current.ConfigFileUri,
            current.Diagnostics);

    }
}

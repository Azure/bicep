// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;

namespace Bicep.Testing.Extensions
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
                if (ruleInfo.defaultDiagnosticLevel == DiagnosticLevel.Off)
                {
                    config = config.SetValue($"core.rules.{code}.level", "warning");
                }
            }

            return config;
        }

        public static IBicepConfiguration WithAnalyzersConfiguration(this IBicepConfiguration current, AnalyzersConfiguration analyzersConfiguration) =>
            current.With(analyzers: analyzersConfiguration);

        public static IBicepConfiguration WithAllAnalyzersDisabled(this IBicepConfiguration current) =>
            current.WithAnalyzersConfiguration(((AnalyzersConfiguration)current.Analyzers).WithAllAnalyzersDisabled());

        public static IBicepConfiguration WithAnalyzersDisabled(this IBicepConfiguration current, params string[] analyzerCodesToDisable) =>
            current.WithAnalyzersConfiguration(((AnalyzersConfiguration)current.Analyzers).WithAnalyzersDisabled(analyzerCodesToDisable));

        public static IBicepConfiguration WithAllAnalyzers(this IBicepConfiguration current) =>
            current.WithAnalyzersConfiguration(((AnalyzersConfiguration)current.Analyzers).WithAllAnalyzers());

        public static IBicepConfiguration WithCloudConfiguration(this IBicepConfiguration current, CloudConfiguration cloudConfiguration) =>
            current.With(cloud: cloudConfiguration);

    }
}

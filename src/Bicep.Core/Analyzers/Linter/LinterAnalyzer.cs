// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Bicep.Core.Analyzers.Linter
{
    internal class LinterAnalyzer : IBicepAnalyzer
    {
        public const string SettingsRoot = "analyzers";
        public const string AnalyzerName = "core";
        public static string LinterEnabledSetting => $"{SettingsRoot}:{AnalyzerName}:enabled";
        public static string LinterVerboseSetting => $"{SettingsRoot}:{AnalyzerName}:verbose";

        private ConfigHelper configHelper = new ConfigHelper();
        private ImmutableArray<IBicepAnalyzerRule> RuleSet;

        public LinterAnalyzer()
        {
            RuleSet = CreateLinterRules().ToImmutableArray();
        }

        private bool LinterEnabled => this.configHelper.GetValue(LinterEnabledSetting, true);
        private bool LinterVerbose => this.configHelper.GetValue(LinterVerboseSetting, true);

        private IEnumerable<IBicepAnalyzerRule> CreateLinterRules()
        {
            var ruleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IBicepAnalyzerRule).IsAssignableFrom(t)
                            && t.IsClass
                            && t.IsPublic
                            && t.GetConstructor(Type.EmptyTypes) != null);

            foreach (var ruleType in ruleTypes)
            {
                yield return (IBicepAnalyzerRule)Activator.CreateInstance(ruleType);
            }
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => RuleSet;

        public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel semanticModel) => Analyze(semanticModel, default);

        internal IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel semanticModel, ConfigHelper? overrideConfig = default)
        {
            // check for configuration overrides
            /// typically only used in unit tests
            var configHelp = overrideConfig ?? this.configHelper;

            var diagnostics = new List<IBicepAnalyzerDiagnostic>();

            configHelp.LoadConfiguration(semanticModel.SyntaxTree.FileUri);
            this.RuleSet.ForEach(r => r.Configure(configHelp.Config));
            if (this.LinterEnabled)
            {
                // add an info diagnostic for local configuration reporting
                if (this.LinterVerbose)
                {
                    diagnostics.Add(GetConfigurationDiagnostic());
                }

                diagnostics.AddRange(RuleSet.Where(rule => rule.IsEnabled())
                                     .SelectMany(r => r.Analyze(semanticModel)));
            }
            else
            {
                if (this.LinterVerbose)
                {
                    diagnostics.Add(
                        new AnalyzerDiagnostic(AnalyzerName,
                                new TextSpan(0, 0),
                                DiagnosticLevel.Info,
                                "Linter Disabled",
                                string.Format(CoreResources.LinterDisabledFormatMessage, this.configHelper.CustomSettingsFileName)));
                }
            }
            return diagnostics;
        }

        private IBicepAnalyzerDiagnostic GetConfigurationDiagnostic()
        {
            var configMessage = this.configHelper.CustomSettingsFileName == default ?
                                    CoreResources.BicepConfigNoCustomSettingsMessage
                                    : string.Format(CoreResources.BicepConfigCustomSettingsFoundFormatMessage, this.configHelper.CustomSettingsFileName);

            return new AnalyzerDiagnostic(AnalyzerName,
                                                    new TextSpan(0, 0),
                                                    DiagnosticLevel.Info,
                                                    "Bicep Linter Configuration",
                                                    configMessage);
        }

        /// <summary>
        /// Internal method intended to allow eash configuration
        /// override in Unit Testing
        /// </summary>
        /// <param name="overrideConfig"></param>
        internal void OverrideConfig(ConfigHelper overrideConfig)
        {
            this.configHelper = overrideConfig;
        }
    }
}

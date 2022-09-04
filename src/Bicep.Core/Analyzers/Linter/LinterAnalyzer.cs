// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter
{
    public class LinterAnalyzer : IBicepAnalyzer
    {
        public const string AnalyzerName = "core";

        public static string LinterEnabledSetting => $"{AnalyzerName}.enabled";

        public static string LinterVerboseSetting => $"{AnalyzerName}.verbose";

        private readonly RootConfiguration configuration;

        private readonly LinterRulesProvider linterRulesProvider;

        private readonly ImmutableArray<IBicepAnalyzerRule> ruleSet;

        private readonly ImmutableArray<IDiagnostic> ruleCreationErrors;

        public LinterAnalyzer(RootConfiguration configuration)
        {
            this.configuration = configuration;
            this.linterRulesProvider = new LinterRulesProvider();
            (this.ruleSet, this.ruleCreationErrors) = CreateLinterRules();
        }

        private bool LinterEnabled => this.configuration.Analyzers.GetValue(LinterEnabledSetting, false); // defaults to true in base bicepconfig.json file

        private bool LinterVerbose => this.configuration.Analyzers.GetValue(LinterVerboseSetting, false);

        private (ImmutableArray<IBicepAnalyzerRule> rules, ImmutableArray<IDiagnostic> errors) CreateLinterRules()
        {
            var errors = new List<IDiagnostic>();
            var rules = new List<IBicepAnalyzerRule>();

            var ruleTypes = linterRulesProvider.GetRuleTypes();

            foreach (var ruleType in ruleTypes)
            {
                rules.Add(Activator.CreateInstance(ruleType) as IBicepAnalyzerRule ?? throw new InvalidOperationException($"Failed to create an instance of \"{ruleType.Name}\"."));
            }

            return (rules.ToImmutableArray(), errors.ToImmutableArray());
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => ruleSet;

        public IEnumerable<IDiagnostic> Analyze(SemanticModel semanticModel)
        {
            var diagnostics = new List<IDiagnostic>();

            this.ruleSet.ForEach(r => r.Configure(this.configuration.Analyzers));

            if (this.LinterEnabled)
            {
                // Add diaagnostics for rules that failed to load
                diagnostics.AddRange(ruleCreationErrors);

                // add an info diagnostic for local configuration reporting
                if (this.LinterVerbose)
                {
                    diagnostics.Add(GetConfigurationDiagnostic());
                }

                diagnostics.AddRange(ruleSet
                    .Where(rule => rule.IsEnabled())
                    .SelectMany(r => r.Analyze(semanticModel)));
            }
            else
            {
                if (this.LinterVerbose)
                {
                    diagnostics.Add(new AnalyzerDiagnostic(
                        AnalyzerName,
                        new TextSpan(0, 0),
                        DiagnosticLevel.Info,
                        "Linter Disabled",
                        string.Format(CoreResources.LinterDisabledFormatMessage, this.configuration.ConfigurationPath ?? ConfigurationManager.BuiltInConfigurationResourceName)));
                }
            }

            return diagnostics;
        }

        private IDiagnostic GetConfigurationDiagnostic()
        {
            var configMessage = this.configuration.IsBuiltIn
                ? CoreResources.BicepConfigNoCustomSettingsMessage
                : string.Format(CoreResources.BicepConfigCustomSettingsFoundFormatMessage, this.configuration.ConfigurationPath);

            return new AnalyzerDiagnostic(
                AnalyzerName,
                new TextSpan(0, 0),
                DiagnosticLevel.Info,
                "Bicep Linter Configuration",
                configMessage);
        }
    }
}

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

        private readonly LinterRulesProvider linterRulesProvider;

        private readonly ImmutableArray<IBicepAnalyzerRule> ruleSet;

        private readonly ImmutableArray<IDiagnostic> ruleCreationErrors;

        public LinterAnalyzer()
        {
            this.linterRulesProvider = new LinterRulesProvider();
            (this.ruleSet, this.ruleCreationErrors) = CreateLinterRules();
        }

        private bool LinterEnabled(SemanticModel model) => model.Configuration.Analyzers.GetValue(LinterEnabledSetting, false); // defaults to true in base bicepconfig.json file

        private bool LinterVerbose(SemanticModel model) => model.Configuration.Analyzers.GetValue(LinterVerboseSetting, false);

        private (ImmutableArray<IBicepAnalyzerRule> rules, ImmutableArray<IDiagnostic> errors) CreateLinterRules()
        {
            var errors = new List<IDiagnostic>();
            var rules = new List<IBicepAnalyzerRule>();

            var ruleTypes = linterRulesProvider.GetRuleTypes();

            foreach (var ruleType in ruleTypes)
            {
                if (Activator.CreateInstance(ruleType) is IBicepAnalyzerRule rule)
                {
                    rules.Add(rule);
                } else 
                {
                    errors.Add(DiagnosticBuilder.ForDocumentStart().RuleFailedToLoad(ruleType.Name));
                }
            }

            return (rules.ToImmutableArray(), errors.ToImmutableArray());
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => ruleSet;

        public IEnumerable<IDiagnostic> Analyze(SemanticModel semanticModel)
        {
            var diagnostics = new List<IDiagnostic>();

            this.ruleSet.ForEach(r => r.Configure(semanticModel.Configuration.Analyzers));

            if (this.LinterEnabled(semanticModel))
            {
                // Add diagnostics for rules that failed to load
                diagnostics.AddRange(ruleCreationErrors);

                // add an info diagnostic for local configuration reporting
                if (this.LinterVerbose(semanticModel))
                {
                    diagnostics.Add(GetConfigurationDiagnostic(semanticModel));
                }

                diagnostics.AddRange(ruleSet
                    .Where(rule => rule.IsEnabled())
                    .SelectMany(r => r.Analyze(semanticModel)));
            }
            else
            {
                if (this.LinterVerbose(semanticModel))
                {
                    diagnostics.Add(new AnalyzerDiagnostic(
                        AnalyzerName,
                        TextSpan.TextDocumentStart,
                        DiagnosticLevel.Info,
                        "Linter Disabled",
                        string.Format(CoreResources.LinterDisabledFormatMessage, semanticModel.Configuration.ConfigurationPath ?? IConfigurationManager.BuiltInConfigurationResourceName)));
                }
            }

            return diagnostics;
        }

        private IDiagnostic GetConfigurationDiagnostic(SemanticModel model)
        {
            var configMessage = model.Configuration.IsBuiltIn
                ? CoreResources.BicepConfigNoCustomSettingsMessage
                : string.Format(CoreResources.BicepConfigCustomSettingsFoundFormatMessage, model.Configuration.ConfigurationPath);

            return new AnalyzerDiagnostic(
                AnalyzerName,
                TextSpan.TextDocumentStart,
                DiagnosticLevel.Info,
                "Bicep Linter Configuration",
                configMessage);
        }
    }
}

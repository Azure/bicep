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

namespace Bicep.Core.Analyzers.Linter
{
    public class LinterAnalyzer : IBicepAnalyzer
    {
        public const string AnalyzerName = "core";

        public static string LinterEnabledSetting => $"{AnalyzerName}.enabled";

        public static string LinterVerboseSetting => $"{AnalyzerName}.verbose";

        private readonly LinterRulesProvider linterRulesProvider;

        private readonly ImmutableArray<IBicepAnalyzerRule> ruleSet;

        public LinterAnalyzer()
        {
            this.linterRulesProvider = new LinterRulesProvider();
            this.ruleSet = CreateLinterRules();
        }

        private bool LinterEnabled(SemanticModel model) => model.Configuration.Analyzers.GetValue(LinterEnabledSetting, false); // defaults to true in base bicepconfig.json file

        private bool LinterVerbose(SemanticModel model) => model.Configuration.Analyzers.GetValue(LinterVerboseSetting, false);

        private ImmutableArray<IBicepAnalyzerRule> CreateLinterRules()
        {
            var rules = new List<IBicepAnalyzerRule>();

            var ruleTypes = linterRulesProvider.GetRuleTypes();

            foreach (var ruleType in ruleTypes)
            {
                rules.Add(Activator.CreateInstance(ruleType) as IBicepAnalyzerRule ?? throw new InvalidOperationException($"Failed to create an instance of \"{ruleType.Name}\"."));
            }

            return rules.ToImmutableArray();
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => ruleSet;

        public IEnumerable<IDiagnostic> Analyze(SemanticModel semanticModel)
        {
            var diagnostics = new List<IDiagnostic>();

            if (this.LinterEnabled(semanticModel))
            {
                // add an info diagnostic for local configuration reporting
                if (this.LinterVerbose(semanticModel))
                {
                    diagnostics.Add(GetConfigurationDiagnostic(semanticModel));
                }

                diagnostics.AddRange(ruleSet.SelectMany(r => r.Analyze(semanticModel)));
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

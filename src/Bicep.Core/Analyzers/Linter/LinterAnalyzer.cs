// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry.Catalog;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Analyzers.Linter
{
    public class LinterAnalyzer : IBicepAnalyzer
    {
        public const string AnalyzerName = "core";

        public static string LinterEnabledSetting => $"{AnalyzerName}.enabled";

        public static string LinterVerboseSetting => $"{AnalyzerName}.verbose";

        private readonly LinterRulesProvider linterRulesProvider;

        private readonly ImmutableArray<IBicepAnalyzerRule> ruleSet;

        private readonly IServiceProvider serviceProvider;

        public LinterAnalyzer(IServiceProvider serviceProvider)
        {
            this.linterRulesProvider = new LinterRulesProvider();
            this.ruleSet = CreateLinterRules();
            this.serviceProvider = serviceProvider;
        }

        private bool LinterEnabled(SemanticModel model) => model.Configuration.Analyzers.GetValue(LinterEnabledSetting, false); // defaults to true in base bicepconfig.json file

        private bool LinterVerbose(SemanticModel model) => model.Configuration.Analyzers.GetValue(LinterVerboseSetting, false);


        [UnconditionalSuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "List of types comes from a source generator.")]
        private ImmutableArray<IBicepAnalyzerRule> CreateLinterRules()
        {
            var rules = new List<IBicepAnalyzerRule>();

            var ruleTypes = linterRulesProvider.GetRuleTypes();

            foreach (var ruleType in ruleTypes)
            {
                rules.Add(Activator.CreateInstance(ruleType) as IBicepAnalyzerRule ?? throw new InvalidOperationException($"Failed to create an instance of \"{ruleType.Name}\"."));
            }

            return [.. rules];
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

                diagnostics.AddRange(ruleSet.SelectMany(r => r.Analyze(semanticModel, this.serviceProvider)));
            }
            else
            {
                if (this.LinterVerbose(semanticModel))
                {
                    diagnostics.Add(new Diagnostic(
                        TextSpan.TextDocumentStart,
                        DiagnosticLevel.Info,
                        DiagnosticSource.CoreLinter,
                        "Linter Disabled",
                        string.Format(CoreResources.LinterDisabledFormatMessage, semanticModel.Configuration.ConfigFileUri?.ToString() ?? IConfigurationManager.BuiltInConfigurationResourceName)));
                }
            }

            return diagnostics;
        }

        private IDiagnostic GetConfigurationDiagnostic(SemanticModel model)
        {
            var configMessage = model.Configuration.IsBuiltIn
                ? CoreResources.BicepConfigNoCustomSettingsMessage
                : string.Format(CoreResources.BicepConfigCustomSettingsFoundFormatMessage, model.Configuration.ConfigFileUri?.ToString());

            return new Diagnostic(
                TextSpan.TextDocumentStart,
                DiagnosticLevel.Info,
                DiagnosticSource.CoreLinter,
                "Bicep Linter Configuration",
                configMessage);
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Azure.Deployments.Core.Extensions;
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

        public static string LinterEnabledSetting => $"{AnalyzerName}:enabled";

        public static string LinterVerboseSetting => $"{AnalyzerName}:verbose";

        private readonly RootConfiguration configuration;

        private ImmutableArray<IBicepAnalyzerRule> ruleSet;

        private ImmutableArray<IDiagnostic> ruleCreationErrors;

        // TODO: This should be controlled by a core component, not an analyzer
        public const string FailedRuleCode = "linter-internal-error";

        public LinterAnalyzer(RootConfiguration configuration)
        {
            this.configuration = configuration;
            (this.ruleSet, this.ruleCreationErrors) = CreateLinterRules();
        }

        private bool LinterEnabled => this.configuration.Analyzers.GetValue(LinterEnabledSetting, false);

        private bool LinterVerbose => this.configuration.Analyzers.GetValue(LinterVerboseSetting, false);

        private (ImmutableArray<IBicepAnalyzerRule> rules, ImmutableArray<IDiagnostic> errors) CreateLinterRules()
        {
            var errors = new List<IDiagnostic>();
            var rules = new List<IBicepAnalyzerRule>();

            var ruleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IBicepAnalyzerRule).IsAssignableFrom(t)
                            && t.IsClass
                            && t.IsPublic
                            && t.GetConstructor(Type.EmptyTypes) != null);

            foreach (var ruleType in ruleTypes)
            {
                try
                {
                    rules.Add((IBicepAnalyzerRule)Activator.CreateInstance(ruleType));
                }
                catch (Exception ex)
                {
                    string message = string.Format("Analyzer '{0}' could not instantiate rule '{1}'. {2}",
                        AnalyzerName,
                        ruleType.Name,
                        ex.InnerException?.Message ?? ex.Message);
                    errors.Add(CreateInternalErrorDiagnostic(AnalyzerName, message));
                }
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
                        string.Format(CoreResources.LinterDisabledFormatMessage, this.configuration.ResourceName)));
                }
            }

            return diagnostics;
        }

        private IDiagnostic GetConfigurationDiagnostic()
        {
            var configMessage = this.configuration.IsBuiltIn
                ? CoreResources.BicepConfigNoCustomSettingsMessage
                : string.Format(CoreResources.BicepConfigCustomSettingsFoundFormatMessage, this.configuration.ResourceName);

            return new AnalyzerDiagnostic(
                AnalyzerName,
                new TextSpan(0, 0),
                DiagnosticLevel.Info,
                "Bicep Linter Configuration",
                configMessage);
        }

        private static IDiagnostic CreateInternalErrorDiagnostic(string analyzerName, string message) => new AnalyzerDiagnostic(
            analyzerName,
            new TextSpan(0, 0),
            DiagnosticLevel.Warning,
            LinterAnalyzer.FailedRuleCode,
            message);
    }
}

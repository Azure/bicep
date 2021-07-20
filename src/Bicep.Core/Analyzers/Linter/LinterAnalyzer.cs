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
using Bicep.Core.Syntax;

namespace Bicep.Core.Analyzers.Linter
{
    public class LinterAnalyzer : IBicepAnalyzer
    {
        public const string SettingsRoot = "analyzers";
        public const string AnalyzerName = "core";
        public static string LinterEnabledSetting => $"{SettingsRoot}:{AnalyzerName}:enabled";
        public static string LinterVerboseSetting => $"{SettingsRoot}:{AnalyzerName}:verbose";

        private readonly ConfigHelper defaultConfigHelper = new ConfigHelper();
        private ConfigHelper activeConfigHelper;
        private ImmutableArray<IBicepAnalyzerRule> RuleSet;
        private ImmutableArray<IDiagnostic> RuleCreationErrors;

        // TODO: This should be controlled by a core component, not an analyzer
        public const string FailedRuleCode = "linter-internal-error";

        public LinterAnalyzer()
        {
            this.activeConfigHelper = this.defaultConfigHelper;
            (RuleSet, RuleCreationErrors) = CreateLinterRules();
        }

        private bool LinterEnabled => this.activeConfigHelper.GetValue(LinterEnabledSetting, true);
        private bool LinterVerbose => this.activeConfigHelper.GetValue(LinterVerboseSetting, true);

        private (ImmutableArray<IBicepAnalyzerRule> rules, ImmutableArray<IDiagnostic> errors) CreateLinterRules()
        {
            List<IDiagnostic> errors = new List<IDiagnostic>();
            List<IBicepAnalyzerRule> rules = new List<IBicepAnalyzerRule>();

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

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => RuleSet;

        public IEnumerable<IDiagnostic> Analyze(SemanticModel semanticModel) => Analyze(semanticModel, default);

        internal IEnumerable<IDiagnostic> Analyze(SemanticModel semanticModel, ConfigHelper? overrideConfig = default)
        {
            // check for configuration overrides
            /// typically only used in unit tests
            this.activeConfigHelper = overrideConfig ?? this.defaultConfigHelper;

            var diagnostics = new List<IDiagnostic>();

            try
            {
                this.activeConfigHelper.LoadConfigurationForSourceFile(semanticModel.SourceFile.FileUri);
            }
            catch (Exception ex)
            {
                diagnostics.Add(CreateInternalErrorDiagnostic(AnalyzerName, ex.InnerException?.Message ?? ex.Message));
                // Build a default config to continue with.  This should not fail.
                this.activeConfigHelper.LoadDefaultConfiguration();
            }

            this.RuleSet.ForEach(r => r.Configure(this.activeConfigHelper.Config));

            if (this.LinterEnabled)
            {
                // Add diaagnostics for rules that failed to load
                diagnostics.AddRange(RuleCreationErrors);

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
                                string.Format(CoreResources.LinterDisabledFormatMessage, this.activeConfigHelper.CustomSettingsFileName),
                                null, null));
                }
            }
            return diagnostics;
        }

        private IDiagnostic GetConfigurationDiagnostic()
        {
            var configMessage = this.activeConfigHelper.CustomSettingsFileName == default ?
                                    CoreResources.BicepConfigNoCustomSettingsMessage
                                    : string.Format(CoreResources.BicepConfigCustomSettingsFoundFormatMessage, this.activeConfigHelper.CustomSettingsFileName);

            return new AnalyzerDiagnostic(AnalyzerName,
                                            new TextSpan(0, 0),
                                            DiagnosticLevel.Info,
                                            "Bicep Linter Configuration",
                                            configMessage,
                                            null, null);
        }

        internal IDiagnostic CreateInternalErrorDiagnostic(string analyzerName, string message)
        {
            return new AnalyzerDiagnostic(
                    analyzerName,
                    new TextSpan(0, 0),
                    DiagnosticLevel.Warning,
                    LinterAnalyzer.FailedRuleCode,
                    message,
                    null,
                    null);
        }
    }
}

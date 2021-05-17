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
        public const string SettingsRoot = "Analyzers";
        public const string AnalyzerName = "Bicep Core Linter";
        private readonly ConfigHelper configHelper = new ConfigHelper();
        private readonly string InvocationHost;
        private ImmutableArray<IBicepAnalyzerRule> RuleSet;

        public LinterAnalyzer(string host = "unknown")
        {
            InvocationHost = host;
            RuleSet = CreateLinterRules().ToImmutableArray();
        }

        private bool LinterEnabled => this.configHelper.GetValue($"{SettingsRoot}:{AnalyzerName}:Enabled", true);

        private IEnumerable<IBicepAnalyzerRule> CreateLinterRules()
        {
            var ruleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IBicepAnalyzerRule).IsAssignableFrom(t)
                            && !t.IsInterface && !t.IsAbstract); // exlude the interface and the base class

            foreach (var ruleType in ruleTypes)
            {
                if (typeof(IBicepAnalyzerRule).IsAssignableFrom(ruleType))
                {
                    yield return (IBicepAnalyzerRule)Activator.CreateInstance(ruleType);
                }
            }
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => RuleSet;

        public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel semanticModel)
        {
            System.Diagnostics.Debugger.Launch();
            var diagnostics = new List<IBicepAnalyzerDiagnostic>();

            //TODO:  remove diagnostic stopwatches
            var configSW = new Stopwatch();
            configSW.Start();

            this.configHelper.LoadConfiguration(semanticModel.SyntaxTree.FileUri);
            this.RuleSet.ForEach(r => r.Configure(this.configHelper.Config));

            configSW.Stop();
            diagnostics.Add(new AnalyzerDiagnostic("Linter", new TextSpan(0, 0), DiagnosticLevel.Info, "Timing Configuration", $"Loaded configuration in: {configSW.ElapsedMilliseconds}ms"));

            if (this.LinterEnabled)
            {
                var analyzeSW = new Stopwatch();
                analyzeSW.Start();
                diagnostics.Add(GetConfigurationDiagnostic());

                diagnostics.AddRange(RuleSet.Where(rule => rule.Enabled)
                                     .SelectMany(r => r.Analyze(semanticModel)));
                analyzeSW.Stop();
                diagnostics.Add(new AnalyzerDiagnostic("Linter", new TextSpan(0, 0), DiagnosticLevel.Info, "Timing Linter Analyze", $"Linter Analyzer completed in: {analyzeSW.ElapsedMilliseconds}ms"));
            }
            else
            {
                diagnostics.Add(new AnalyzerDiagnostic(AnalyzerName,
                                                        new TextSpan(0, 0),
                                                        DiagnosticLevel.Info,
                                                        "Linter Disabled",
                                                        string.Format(CoreResources.LinterDisabledFormatMessage, this.configHelper.CustomSettingsFileName)));
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
    }
}

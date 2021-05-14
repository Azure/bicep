// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Bicep.Core.Analyzers.Linter
{
    internal class LinterAnalyzer : IBicepAnalyzer, IDisposable
    {
        public const string AnalyzerName = "Bicep Internal Linter";
        private readonly ConfigHelper configHelper = new ConfigHelper();
        private readonly string InvocationHost;
        private ImmutableArray<IBicepAnalyzerRule> RuleSet;
        private bool disposedValue;

        public LinterAnalyzer(string host = "unknown")
        {
            InvocationHost = host;
            RuleSet = CreateLinterRules().ToImmutableArray();

            // attach to event telling when the config changes
            this.configHelper.ConfigFileChanged += OnConfigFileChanged;
        }

        /// <summary>
        /// Reload ruleset when the config has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConfigFileChanged(object sender, EventArgs e)
        {
            RuleSet = CreateLinterRules().ToImmutableArray();
        }

        private IEnumerable<IBicepAnalyzerRule> CreateLinterRules()
        {
            if (this.configHelper.GetValue("Linter:Core:Enabled", true))
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
            else
            {
                yield break;
            }
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => RuleSet;

        public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel semanticModel)
            => RuleSet.Where(rule => rule.Enabled)
                .SelectMany(r => r.Analyze(semanticModel))
                .ToArray();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.configHelper.ConfigFileChanged -= this.OnConfigFileChanged;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

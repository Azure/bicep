// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Bicep.Core.Analyzers.Linter
{
    internal class LinterAnalyzer : IBicepAnalyzer
    {
        public const string AnalyzerName = "Bicep Internal Linter";
        private readonly string InvocationHost;
        private readonly ImmutableArray<IBicepAnalyzerRule> RuleSet;

        public LinterAnalyzer(string host="unknown")
        {
            InvocationHost = host;
            RuleSet = CreateLinterRules().ToImmutableArray();
        }

        private IEnumerable<IBicepAnalyzerRule> CreateLinterRules()
        {
            var ruleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IBicepAnalyzerRule).IsAssignableFrom(t)
                            && !t.IsInterface && !t.IsAbstract); // exlude the interface and the base class

            foreach (var ruleType in ruleTypes)
            {
                if(typeof(IBicepAnalyzerRule).IsAssignableFrom(ruleType))
                {
                    yield return (IBicepAnalyzerRule) Activator.CreateInstance(ruleType);
                }
            }
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => RuleSet;

        public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel semanticModel)
            => RuleSet.Where(rule => rule.Enabled)
                .SelectMany(r => r.Analyze(semanticModel))
                .ToArray();
    }
}

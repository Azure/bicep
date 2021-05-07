// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Bicep.Core.Analyzers.Linter
{
    internal class LinterAnalyzer : IBicepAnalyzer
    {
        public static string AnalyzerName => "Bicep Internal Linter";
        public static readonly bool IsCliInvoked;

        private static readonly ImmutableArray<Type> RuleTypes;
        private static readonly ImmutableArray<IBicepAnalyzerRule> RuleSet;

        static LinterAnalyzer()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            IsCliInvoked = entryAssembly.Location.EndsWith("bicep.dll", true, CultureInfo.CurrentCulture);

            RuleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IBicepAnalyzerRule).IsAssignableFrom(t)
                            && !t.IsInterface && !t.IsAbstract) // exlude the interface and the base class
                .ToImmutableArray();

            RuleSet = CreateLinterRules().ToImmutableArray();
        }

        internal static  IEnumerable<IBicepAnalyzerRule> CreateLinterRules()
        {
            foreach(var ruleType in RuleTypes)
            {
                if(typeof(IBicepAnalyzerRule).IsAssignableFrom(ruleType))
                {
                    yield return (IBicepAnalyzerRule) Activator.CreateInstance(ruleType);
                }
            }
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => RuleSet;

        public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel semanticModel)
            => RuleSet.Where(rule => (IsCliInvoked && rule.EnabledForCLI) || (!IsCliInvoked && rule.EnabledForEditing))
                .SelectMany(r => r.Analyze(semanticModel))
                .ToArray();
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Diagnostics;
using Bicep.RoslynAnalyzers;

namespace Bicep.Core.Analyzers.Linter
{
    public partial class LinterRulesProvider : ILinterRulesProvider
    {
        private readonly Lazy<ImmutableDictionary<string, (string diagnosticLevelConfigProperty, DiagnosticLevel defaultDiagnosticLevel)>> linterRulesLazy;

        public LinterRulesProvider()
        {
            this.linterRulesLazy = new(() => GetLinterRulesInternal().ToImmutableDictionary());
        }

        [UnconditionalSuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "List of types comes from a source analyzer")]
        private Dictionary<string, (string diagnosticLevelConfigProperty, DiagnosticLevel defaultDiagnosticLevel)> GetLinterRulesInternal()
        {
            var rules = new Dictionary<string, (string diagnosticLevelConfigProperty, DiagnosticLevel defaultDiagnosticLevel)>();
            var ruleTypes = GetRuleTypes();

            foreach (var ruleType in ruleTypes)
            {
                IBicepAnalyzerRule? rule = Activator.CreateInstance(ruleType) as IBicepAnalyzerRule;
                if (rule is not null)
                {
                    rules.Add(rule.Code, ($"core.rules.{rule.Code}.level", rule.DefaultDiagnosticLevel));
                }
            }

            return rules;
        }

        [LinterRuleTypesGenerator]
        public partial IEnumerable<Type> GetRuleTypes();

        public ImmutableDictionary<string, (string diagnosticLevelConfigProperty, DiagnosticLevel defaultDiagnosticLevel)> GetLinterRules() => linterRulesLazy.Value;
    }
}

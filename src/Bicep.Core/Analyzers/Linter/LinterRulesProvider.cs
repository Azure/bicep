// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.RoslynAnalyzers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.Analyzers.Linter
{
    public partial class LinterRulesProvider : ILinterRulesProvider
    {
        private readonly Lazy<ImmutableDictionary<string, string>> linterRulesLazy;

        public LinterRulesProvider()
        {
            this.linterRulesLazy = new Lazy<ImmutableDictionary<string, string>>(() => GetLinterRulesInternal().ToImmutableDictionary());
        }

        [UnconditionalSuppressMessage("Trimming", "IL2072:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The return value of the source method does not have matching annotations.", Justification = "List of types comes from a source analyzer")]
        private Dictionary<string, string> GetLinterRulesInternal()
        {
            var rules = new Dictionary<string, string>();
            var ruleTypes = GetRuleTypes();

            foreach (var ruleType in ruleTypes)
            {
                IBicepAnalyzerRule? rule = Activator.CreateInstance(ruleType) as IBicepAnalyzerRule;
                if (rule is not null)
                {
                    var code = rule.Code;
                    rules.Add(code, $"core.rules.{code}.level");
                }
            }

            return rules;
        }

        [LinterRuleTypesGenerator]
        public partial IEnumerable<Type> GetRuleTypes();

        public ImmutableDictionary<string, string> GetLinterRules() => linterRulesLazy.Value;
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Bicep.Core.Analyzers.Interfaces;

namespace Bicep.Core.Analyzers.Linter
{
    public class LinterRulesProvider : ILinterRulesProvider
    {
        private readonly Lazy<ImmutableDictionary<string, string>> linterRulesLazy;

        public LinterRulesProvider()
        {
            this.linterRulesLazy = new Lazy<ImmutableDictionary<string, string>>(() => GetLinterRulesInternal().ToImmutableDictionary());
        }

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

        public IEnumerable<Type> GetRuleTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IBicepAnalyzerRule).IsAssignableFrom(t)
                            && t.IsClass
                            && t.IsPublic
                            && t.GetConstructor(Type.EmptyTypes) != null);
        }

        public ImmutableDictionary<string, string> GetLinterRules() => linterRulesLazy.Value;
    }
}

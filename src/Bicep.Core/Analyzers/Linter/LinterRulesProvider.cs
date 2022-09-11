// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;

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
            // Can't use reflection to get this list because the output dotnet executable is trimmed,
            //   and dependencies that can't be determined at compile time get removed.
            return new[] {
                typeof(AdminUsernameShouldNotBeLiteralRule),
                typeof(ArtifactsParametersRule),
                typeof(ExplicitValuesForLocationParamsRule),
                typeof(MaxNumberOutputsRule),
                typeof(MaxNumberParametersRule),
                typeof(MaxNumberResourcesRule),
                typeof(MaxNumberVariablesRule),
                typeof(NoHardcodedEnvironmentUrlsRule),
                typeof(NoHardcodedLocationRule),
                typeof(NoLocationExprOutsideParamsRule),
                typeof(NoUnnecessaryDependsOnRule),
                typeof(NoUnusedExistingResourcesRule),
                typeof(NoUnusedParametersRule),
                typeof(NoUnusedVariablesRule),
                typeof(OutputsShouldNotContainSecretsRule),
                typeof(PreferInterpolationRule),
                typeof(PreferUnquotedPropertyNamesRule),
                typeof(SecretsInParamsMustBeSecureRule),
                typeof(SecureParameterDefaultRule),
                typeof(SecureParamsInNestedDeploymentsRule),
                typeof(SimplifyInterpolationRule),
                typeof(ProtectCommandToExecuteSecretsRule),
                typeof(UseRecentApiVersionRule),
                typeof(UseResourceIdFunctionsRule),
                typeof(UseStableResourceIdentifiersRule),
                typeof(UseStableVMImageRule),
            };
        }

        public ImmutableDictionary<string, string> GetLinterRules() => linterRulesLazy.Value;
    }
}

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
        public static string AnalyzerName => "Bicep Internal Linter";
        private static readonly IEnumerable<Type> RuleTypes;
        private static readonly ImmutableArray<IBicepAnalyzerRule> RuleSet;


        static LinterAnalyzer()
        {
            System.Diagnostics.Debugger.Launch();

            RuleTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(LinterRule)));

            RuleSet = CreateLinterRules().ToImmutableArray();
        }

        private static  IEnumerable<IBicepAnalyzerRule> CreateLinterRules()
        {
            //yield return new BCPL1000();
            //yield return new BCPL1010();
            //yield return new BCPL1020();
            //yield return new BCPL1030();
            //yield return new BCPL1040();
            //yield return new BCPL1050();
            //yield return new BCPL1060();
            //yield return new BCPL1070();
            //yield return new BCPL1080();

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
            => RuleSet.SelectMany(r => r.Analyze(semanticModel));

    }
}

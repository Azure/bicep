// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Semantics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.Analyzers.Linter
{
    internal class LinterAnalyzer : IBicepAnalyzer
    {
        public static string AnalyzerName => "Bicep Internal Linter";
        readonly List<IBicepAnalyzerRule> RuleSet;

        public LinterAnalyzer()
        {
            RuleSet = CreateLinterRules().ToList();
        }

        public IEnumerable<IBicepAnalyzerRule> CreateLinterRules()
        {
            yield return new BCPL1000();
            yield return new BCPL1010();
            yield return new BCPL1020();
            yield return new BCPL1030();
            yield return new BCPL1040();
            yield return new BCPL1050();
            yield return new BCPL1060();
            yield return new BCPL1070();
        }

        public IEnumerable<IBicepAnalyzerRule> GetRuleSet() => this.RuleSet;

        public IEnumerable<IBicepAnalyzerDiagnostic> Analyze(SemanticModel semanticModel)
            => this.RuleSet.SelectMany(r => r.Analyze(semanticModel));

    }
}

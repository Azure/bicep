// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class LinterAnalyzerTests
    {
        [TestMethod]
        public void HasBuiltInRules()
        {
            var linter = new LinterAnalyzer();
            linter.GetRuleSet().Should().NotBeEmpty();
        }

        [DataTestMethod]
        [DataRow("BCPL1000")]
        [DataRow("BCPL1010")]
        [DataRow("BCPL1020")]
        [DataRow("BCPL1030")]
        [DataRow("BCPL1040")]
        [DataRow("BCPL1050")]
        [DataRow("BCPL1060")]
        [DataRow("BCPL1070")]
        [DataRow("BCPL1080")]
        public void BuiltInRulesExist(string ruleCode)
        {
            var linter = new LinterAnalyzer();
            linter.GetRuleSet().Should().Contain(r => r.Code == ruleCode);
        }

        [TestMethod]
        public void AllRulesHaveUniqueDetails()
        {
            var analyzer = new LinterAnalyzer();
            var ruleSet = analyzer.GetRuleSet();

            var codeSet = ruleSet.Select(r => r.Code).ToHashSet();
            codeSet.Should().HaveSameCount(ruleSet);

            var descSet = ruleSet.Select(r => r.Description).ToHashSet();
            descSet.Should().HaveSameCount(ruleSet);

            var nameSet = ruleSet.Select(r => r.RuleName).ToHashSet();
            nameSet.Should().HaveSameCount(ruleSet);
        }

        [TestMethod]
        public void AllRulesEnabledByDefault()
        {
            var analyzer = new LinterAnalyzer();
            var ruleSet = analyzer.GetRuleSet();
            ruleSet.Should().OnlyContain( r => r.Enabled);
        }

        [TestMethod]
        public void AllRulesHaveDescription()
        {
            var analyzer = new LinterAnalyzer();
            var ruleSet = analyzer.GetRuleSet();
            ruleSet.Should().OnlyContain(r => r.Description.Length > 0);
        }

        public class LinterThrowsTestRule : LinterRuleBase
        {
            public LinterThrowsTestRule() : base("ThrowsRule", "NotImplementedRule", "Throws an exception when used", "http:\\none", DiagnosticLevel.Warning) { }

            override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model) =>
                throw new System.NotImplementedException();
        }

        [TestMethod]
        public void TestRuleThrowingExceptionReturnsOneDiagnostic()
        {
            var text = @"
@secure()
param param1 string = 'val'";
            var compilationResult = CompilationHelper.Compile(text);
            var semanticModel = compilationResult.Compilation.GetSemanticModel(compilationResult.SyntaxTree);

            var throwRule = new LinterThrowsTestRule();
            var diagnostics = throwRule.Analyze(semanticModel);
            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics.Should().HaveCount(1);

            var diag = diagnostics.First();
            diag.Should().NotBeNull();

            diag.Code.Should().NotBeNull();
            diag.Code.Should().Match(CoreResources.LinterRuleExceptionCode);

            diag.Span.Should().NotBeNull();
            diag.Span.Position.Should().Equals(0);
        }

    }
}

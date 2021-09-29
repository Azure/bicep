// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Analyzers;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class LinterAnalyzerTests
    {
        private readonly RootConfiguration configuration = BicepTestConstants.BuiltInConfiguration;

        [TestMethod]
        public void HasBuiltInRules()
        {
            var linter = new LinterAnalyzer(configuration);
            linter.GetRuleSet().Should().NotBeEmpty();
        }

        [DataTestMethod]
        [DataRow(NoHardcodedEnvironmentUrlsRule.Code)]
        [DataRow(PreferInterpolationRule.Code)]
        [DataRow(NoUnusedParametersRule.Code)]
        [DataRow(SecureParameterDefaultRule.Code)]
        [DataRow(SimplifyInterpolationRule.Code)]
        [DataRow(NoUnusedVariablesRule.Code)]
        public void BuiltInRulesExist(string ruleCode)
        {
            var linter = new LinterAnalyzer(configuration);
            linter.GetRuleSet().Should().Contain(r => r.Code == ruleCode);
        }

        [TestMethod]
        public void AllRulesHaveUniqueDetails()
        {
            var analyzer = new LinterAnalyzer(configuration);
            var ruleSet = analyzer.GetRuleSet();

            var codeSet = ruleSet.Select(r => r.Code).ToHashSet();
            codeSet.Should().HaveSameCount(ruleSet);

            var descSet = ruleSet.Select(r => r.Description).ToHashSet();
            descSet.Should().HaveSameCount(ruleSet);
        }

        [TestMethod]
        public void AllRulesEnabledByDefault()
        {
            var analyzer = new LinterAnalyzer(configuration);
            var ruleSet = analyzer.GetRuleSet();
            ruleSet.Should().OnlyContain(r => r.IsEnabled());
        }

        [TestMethod]
        public void AllRulesHaveDescription()
        {
            var analyzer = new LinterAnalyzer(configuration);
            var ruleSet = analyzer.GetRuleSet();
            ruleSet.Should().OnlyContain(r => r.Description.Length > 0);
        }

        public class LinterThrowsTestRule : LinterRuleBase
        {
            public LinterThrowsTestRule() : base("ThrowsRule", "Throws an exception when used", null, DiagnosticLevel.Warning) { }

            public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
            {
                // Have a yield return to force this method to return an iterator like the real rules
                yield return new AnalyzerDiagnostic(this.AnalyzerName,
                                                    new TextSpan(0, 0),
                                                    DiagnosticLevel.Warning,
                                                    "fakeRule",
                                                    "Fake Rule",
                                                    null,
                                                    null);
                // Now throw an exception
                throw new System.NotImplementedException();
            }
        }

        [TestMethod]
        public void TestRuleThrowingExceptionReturnsOneDiagnostic()
        {
            var text = @"
@secure()
param param1 string = 'val'";
            var compilationResult = CompilationHelper.Compile(text);
            var semanticModel = compilationResult.Compilation.GetSemanticModel(compilationResult.BicepFile);

            var throwRule = new LinterThrowsTestRule();
            var diagnostics = throwRule.Analyze(semanticModel);
            diagnostics.Should().NotBeNullOrEmpty();
            diagnostics.Should().HaveCount(1);

            var diag = diagnostics.First();
            diag.Should().NotBeNull();

            diag.Code.Should().NotBeNull();
            diag.Code.Should().Match("linter-internal-error");

            diag.Span.Should().NotBeNull();
            diag.Span.Position.Should().Be(0);
        }

    }
}

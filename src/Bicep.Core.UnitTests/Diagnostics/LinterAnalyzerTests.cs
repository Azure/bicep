// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Analyzers;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
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

        // No need to add new rules here
        [DataTestMethod]
        [DataRow(AdminUsernameShouldNotBeLiteralRule.Code)]
        [DataRow(ExplicitValuesForLocationParamsRule.Code)]
        [DataRow(NoHardcodedEnvironmentUrlsRule.Code)]
        [DataRow(NoHardcodedLocationRule.Code)]
        public void BuiltInRulesExistSanityCheck(string ruleCode)

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
        public void MostRulesEnabledByDefault()
        {
            var analyzer = new LinterAnalyzer(configuration);
            var ruleSet = analyzer.GetRuleSet();
            var numberEnabled = ruleSet.Where(r => r.IsEnabled()).Count();
            numberEnabled.Should().BeGreaterThan(ruleSet.Count() / 2, "most rules should probably be enabled by default");
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
                                                    null);
                // Now throw an exception
                throw new System.ArgumentOutOfRangeException();
            }
        }

        [TestMethod]
        public void TestRuleThrowingException()
        {
            var text = @"
@secure()
param param1 string = 'val'";
            var compilationResult = CompilationHelper.Compile(text);
            var semanticModel = compilationResult.Compilation.GetSemanticModel(compilationResult.BicepFile);

            var throwRule = new LinterThrowsTestRule();
            var test = () => throwRule.Analyze(semanticModel).ToArray();
            test.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}

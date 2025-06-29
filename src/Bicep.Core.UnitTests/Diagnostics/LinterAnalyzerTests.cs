// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Bicep.Core.Analyzers;
using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class LinterAnalyzerTests
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class TestDataAttribute : Attribute, ITestDataSource
        {
            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                var analyzer = new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider);
                var ruleSet = analyzer.GetRuleSet().ToArray();

                return ruleSet.Select(rule => new object[] { rule });
            }

            public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
            {
                var baselineData = (data?[0] as IBicepAnalyzerRule)!;

                return $"{methodInfo.Name} ({baselineData.Code})";
            }
        }

        [TestMethod]
        public void HasBuiltInRules()
        {
            var linter = new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider);
            linter.GetRuleSet().Should().NotBeEmpty();
        }

        // No need to add new rules here, just checking a few known ones
        [DataTestMethod]
        [DataRow(AdminUsernameShouldNotBeLiteralRule.Code)]
        [DataRow(ExplicitValuesForLocationParamsRule.Code)]
        [DataRow(NoHardcodedEnvironmentUrlsRule.Code)]
        [DataRow(NoHardcodedLocationRule.Code)]
        public void BuiltInRulesExistSanityCheck(string ruleCode)

        {
            var linter = new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider);
            linter.GetRuleSet().Should().Contain(r => r.Code == ruleCode);
        }

        [TestMethod]
        public void AllDefinedRulesAreListedInLinterRulesProvider()
        {
            var linter = new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider);
            var ruleTypes = linter.GetRuleSet().Select(r => r.GetType()).ToArray();

            var expectedRuleTypes = typeof(LinterAnalyzer).Assembly
                .GetTypes()
                .Where(t => typeof(IBicepAnalyzerRule).IsAssignableFrom(t)
                            && t.IsClass
                            && t.IsPublic
                            && t.GetConstructor(Type.EmptyTypes) != null);

            var actualTypeNames = ruleTypes.Select(t => t.FullName ?? throw new ArgumentNullException("bad type"));
            var expectedTypeNames = expectedRuleTypes.Select(t => t.FullName ?? throw new ArgumentNullException("bad type"));

            actualTypeNames.Should().BeEquivalentTo(expectedTypeNames, "Please verify that the {nameof(LinterRuleTypeGenerator)} source generator is working correctly");
        }

        [TestMethod]
        public void AllRulesHaveUniqueDetails()
        {
            var analyzer = new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider);
            var ruleSet = analyzer.GetRuleSet().ToArray();

            var codeSet = ruleSet.Select(r => r.Code).ToHashSet();
            codeSet.Should().HaveSameCount(ruleSet);

            var descSet = ruleSet.Select(r => r.Description).ToHashSet();
            descSet.Should().HaveSameCount(ruleSet);
        }

        [TestMethod]
        public void MostRulesEnabledByDefault()
        {
            var analyzer = new LinterAnalyzer(BicepTestConstants.EmptyServiceProvider);
            var ruleSet = analyzer.GetRuleSet().ToArray();
            var numberEnabled = ruleSet.Where(r => r.DefaultDiagnosticLevel != DiagnosticLevel.Off).Count();
            numberEnabled.Should().BeGreaterThan(ruleSet.Length / 2, "most rules should probably be enabled by default");
        }

        [DataTestMethod]
        [TestData]
        public void AllRulesHaveDescription(IBicepAnalyzerRule rule)
        {
            rule.Description.Length.Should().BeGreaterThan(0);
        }

        [DataTestMethod()]
        [TestData]
        public void RulesShouldNotSpecifyOverriddenDiagnosticLevel_UnlessDifferingFromCategoryDefault(IBicepAnalyzerRule rule)
        {
            if (rule is LinterRuleBase ruleBase)
            {
                if (ruleBase.OverrideCategoryDefaultDiagnosticLevel.HasValue)
                {
                    ruleBase.DefaultDiagnosticLevel.Should().NotBe(LinterRuleBase.GetDefaultDiagosticLevelForCategory(ruleBase.Category),
                        "Do not specify a value for OverrideCategoryDefaultDiagnosticLevel unless it is overriding the default diagnostic level for that rule's category " +
                            "(and usually that should not be done).");

                    ruleBase.DefaultDiagnosticLevel.Should().Be(DiagnosticLevel.Off,
                        "I think the reason for overriding the default diagnostic level of a rule's category should only be to turn it to Off by default " +
                            "(if there turn out to be valid reasons for something different, this test will need to be changed)");
                }
            }
        }

        public class LinterThrowsTestRule : LinterRuleBase
        {
            public LinterThrowsTestRule() : base("ThrowsRule", "Throws an exception when used", LinterRuleCategory.Style) { }

            public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                // Have a yield return to force this method to return an iterator like the real rules
                yield return new Diagnostic(
                    TextSpan.TextDocumentStart,
                    diagnosticLevel,
                    DiagnosticSource.CoreLinter,
                    "fakeRule",
                    "Fake Rule");
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
            var test = () => throwRule.Analyze(semanticModel, BicepTestConstants.EmptyServiceProvider).ToArray();
            test.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}

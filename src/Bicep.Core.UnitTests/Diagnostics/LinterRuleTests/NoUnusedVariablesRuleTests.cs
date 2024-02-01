// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class NoUnusedVariablesRuleTests : LinterRuleTestsBase
    {

        [TestMethod]
        public void VariableNameInFormattedMessage()
        {
            var ruleToTest = new NoUnusedVariablesRule();
            ruleToTest.GetMessage(nameof(ruleToTest)).Should().Be($"Variable \"{nameof(ruleToTest)}\" is declared but never used.");
        }

        private void CompileAndTest(string text, params string[] unusedVars)
        {
            CompileAndTest(text, new(OnCompileErrors.IncludeErrors), unusedVars);
        }

        private void CompileAndTest(string text, Options options, params string[] unusedVars)
        {
            AssertLinterRuleDiagnostics(NoUnusedVariablesRule.Code, text, diags =>
                {
                    if (unusedVars.Any())
                    {
                        var rule = new NoUnusedVariablesRule();
                        string[] expectedMessages = unusedVars.Select(p => rule.GetMessage(p)).ToArray();
                        diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                    }
                    else
                    {
                        diags.Should().BeEmpty();
                    }
                },
                options);
        }

        [DataRow(@"
            var password = 'PLACEHOLDER'
            var sum = 1 + 3
            output sub int = sum
            ",
            "password")]
        [DataRow(@"
            var var1 = 'var1'
            var var2 = 'var2'
            var var3 = 'var3'
            var sum = 1 + 3
            output sub int = sum
            ",
            "var1", "var2", "var3")]
        [DataRow(@"
            var var1 = 'var1'
            var var2 = 4
            var var3 = resourceGroup().location
            var sum = 1 + 3
            output sub int = sum + var2
            ",
            "var1", "var3")]
        [DataRow(@"
            param param2 int = 4
            var sum = 1 + 3
            output sub int = sum + param2
            ")]
        [DataRow(@"
            var sum = 1 + 3
            output sub int = sum
            ")]
        [DataTestMethod]
        public void TestRule(string text, params string[] unusedVars)
        {
            CompileAndTest(text, unusedVars);
        }

        [TestMethod]
        public void Exported_variables_are_not_reported_as_unused()
        {
            CompileAndTest("""
                @export()
                var foo = 'foo'
                """,
                new Options(OnCompileErrors.IncludeErrors));
        }

        [DataRow(@"
            // Syntax errors
            var string =
            var a string
            resource abc 'Microsoft.AAD/domainServices@2021-03-01'
                    ",
            "a")]
        [DataTestMethod]
        public void SyntaxErrors(string text, params string[] unusedVars)
        {
            CompileAndTest(text, new(OnCompileErrors.Ignore), unusedVars);
        }

        [DataRow(@"var")] // Don't show as unused - no param name
        [DataRow(@"var // whoops")] // Don't show as unused - no param name
        [DataTestMethod]
        public void Errors(string text, params string[] unusedVars)
        {
            CompileAndTest(text, new(OnCompileErrors.Ignore), unusedVars);
        }
    }
}

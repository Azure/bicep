// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
            CompileAndTest(text, OnCompileErrors.IncludeErrors, unusedVars);
        }

        private void CompileAndTest(string text, OnCompileErrors onCompileErrors, params string[] unusedVars)
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
                new Options(onCompileErrors));
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
            CompileAndTest(text, OnCompileErrors.Ignore, unusedVars);
        }

        [DataRow(@"var")] // Don't show as unused - no param name
        [DataRow(@"var // whoops")] // Don't show as unused - no param name
        [DataTestMethod]
        public void Errors(string text, params string[] unusedParams)
        {
            CompileAndTest(text, OnCompileErrors.Ignore);
        }
    }
}

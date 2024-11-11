// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class MaxNumberVariablesRuleTests : MaxNumberTestsBase
    {
        [TestMethod]
        public void LimitShouldBeInFormattedMessage()
        {
            var ruleToTest = new MaxNumberVariablesRule();
            ruleToTest.GetMessage(1).Should().Be("Too many variables. Number of variables is limited to 1.");
        }

        [DataRow(
            1, 512, "var v% = %",
            new string[] { })]
        [DataRow(
            2, 513, "var v% = %",
            new string[] { })]
        [DataRow(
            1, 513, "var v% = %",
            new string[] {
                "Too many variables. Number of variables is limited to 512."
            })]
        [DataRow(
            2, 514, "var v% = %",
            new string[] {
                "Too many variables. Number of variables is limited to 512."
            })]
        [DataTestMethod]
        public void TooManyVariables(int i, int j, string pattern, string[] expectedMessages)
        {
            CompileAndTest(GenerateText(i, j, pattern), MaxNumberVariablesRule.Code, DiagnosticLevel.Error, expectedMessages);
        }
    }
}

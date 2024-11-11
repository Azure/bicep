// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class MaxNumberParametersRuleTests : MaxNumberTestsBase
    {
        [TestMethod]
        public void LimitShouldBeInFormattedMessage()
        {
            var ruleToTest = new MaxNumberParametersRule();
            ruleToTest.GetMessage(1).Should().Be("Too many parameters. Number of parameters is limited to 1.");
        }

        [DataRow(
            1, 256, "param p% string",
            new string[] { })]
        [DataRow(
            2, 257, "param p% int = %",
            new string[] { })]
        [DataRow(
            1, 257, "param p% int = %",
            new string[] { "Too many parameters. Number of parameters is limited to 256." })]
        [DataTestMethod]
        public void TestRule(int i, int j, string pattern, string[] expectedMessages)
        {
            CompileAndTest(GenerateText(i, j, pattern), MaxNumberParametersRule.Code, Core.Diagnostics.DiagnosticLevel.Error, expectedMessages);
        }
    }
}

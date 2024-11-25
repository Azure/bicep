// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class MaxNumberOutputsRuleTests : MaxNumberTestsBase
    {
        [TestMethod]
        public void LimitShouldBeInFormattedMessage()
        {
            var ruleToTest = new MaxNumberOutputsRule();
            ruleToTest.GetMessage(123).Should().Be("Too many outputs. Number of outputs is limited to 123.");
        }

        [DataRow(
            1, 64, "output o% string = 'o%'",
            new string[] { })]
        [DataRow(
            2, 65, "output o% string = 'o%'",
            new string[] { })]
        [DataRow(
            1, 65, "output o% string = 'o%'",
            new string[] { "Too many outputs. Number of outputs is limited to 64." })]
        [DataTestMethod]
        public void TooManyOutputs(int i, int j, string pattern, string[] expectedMessages)
        {
            CompileAndTest(GenerateText(i, j, pattern), MaxNumberOutputsRule.Code, Core.Diagnostics.DiagnosticLevel.Error, expectedMessages);
        }
    }
}

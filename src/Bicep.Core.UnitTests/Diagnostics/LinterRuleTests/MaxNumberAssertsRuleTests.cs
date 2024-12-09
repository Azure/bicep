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
    public class MaxNumberAssertsRuleTests : MaxNumberTestsBase
    {
        [TestMethod]
        public void LimitShouldBeInFormattedMessage()
        {
            var ruleToTest = new MaxNumberAssertsRule();
            ruleToTest.GetMessage(1).Should().Be("Too many predeployment conditions. Number of 'assert' statements is limited to 1.");
        }

        [DataRow(
            1, 32, "assert a% = true",
            new string[] { })]
        [DataRow(
            1, 33, "assert a% = true",
            new string[] { "Too many predeployment conditions. Number of 'assert' statements is limited to 32." })]
        [DataTestMethod]
        public void TooManyAsserts(int i, int j, string pattern, string[] expectedMessages)
        {
            CompileAndTest(GenerateText(i, j, pattern), MaxNumberAssertsRule.Code, Core.Diagnostics.DiagnosticLevel.Error, expectedMessages, new Options() { OnCompileErrors = OnCompileErrors.Ignore });
        }
    }
}

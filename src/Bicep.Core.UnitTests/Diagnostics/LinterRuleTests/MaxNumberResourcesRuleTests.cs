// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class MaxNumberResourcesRuleTests : MaxNumberTestsBase
    {
        [TestMethod]
        public void LimitShouldBeInFormattedMessage()
        {
            var ruleToTest = new MaxNumberResourcesRule();
            ruleToTest.GetMessage(1).Should().Be("Too many resources. Number of resources is limited to 1.");
        }

        [DataRow(
            1, 800, """
                resource r% 'Microsoft.Network/virtualNetworks@2021-05-01' = {
                    name: 'r%'
                }
                """,
            new string[] { })]
        [DataRow(
            1, 801, """
                resource r% 'Microsoft.Network/virtualNetworks@2021-05-01' = {
                    name: 'r%'
                }
                """,
            new string[] { "Too many resources. Number of resources is limited to 800." })]
        [DataTestMethod]
        public void TooManyResources(int i, int j, string pattern, string[] expectedMessages)
        {
            CompileAndTest(GenerateText(i, j, pattern), MaxNumberResourcesRule.Code, Core.Diagnostics.DiagnosticLevel.Error, expectedMessages);
        }
    }
}

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
    public class NoWritingReadonlyResourcesRuleTests : LinterRuleTestsBase
    {

        [TestMethod]
        public void ResourceTypeInFormattedMessage()
        {
            var ruleToTest = new NoWritingReadonlyResourcesRule();
            ruleToTest.GetMessage(nameof(ruleToTest)).Should().Be($"Resources of type '{nameof(ruleToTest)}' can only be used with the 'existing' keyword.");
        }

        private void CompileAndTest(string text, params string[] invalidResourceTypes)
        {
            CompileAndTest(text, OnCompileErrors.Fail, invalidResourceTypes);
        }

        private void CompileAndTest(string text, OnCompileErrors onCompileErrors, params string[] invalidResourceTypes)
        {
            AssertLinterRuleDiagnostics(NoWritingReadonlyResourcesRule.Code, text, onCompileErrors, diags =>
            {
                if (invalidResourceTypes.Any())
                {
                    var rule = new NoWritingReadonlyResourcesRule();
                    string[] expectedMessages = invalidResourceTypes.Select(p => rule.GetMessage(p)).ToArray();
                    diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                }
                else
                {
                    diags.Should().BeEmpty();
                }
            });
        }

        [DataRow(@"
            targetScope = 'subscription'

            resource priceSheet 'Microsoft.Consumption/pricesheets@2021-05-01' = {
                name: 'default'
            }
            ",
            "Microsoft.Consumption/pricesheets@2021-05-01")]
        [DataTestMethod]
        public void TestRule(string text, params string[] invalidResourceTypes)
        {
            CompileAndTest(text, invalidResourceTypes);
        }

        [DataRow(@"
            targetScope = 'subscription'

            resource priceSheet 'Microsoft.Consumption/pricesheets@2021-05-01' existing = {
                name: 'default'
            }
            ")]
        [DataTestMethod]
        public void TestRuleNotTriggeredForExistingResource(string text, params string[] invalidResourceTypes)
        {
            CompileAndTest(text, invalidResourceTypes);
        }

        [DataRow(@"
            // Syntax errors
            targetScope = 'subscription'
            var string =
            resource priceSheet 'Microsoft.Consumption/pricesheets@2021-05-01' = {
                name: 'default'
            }",
            "Microsoft.Consumption/pricesheets@2021-05-01")]
        [DataTestMethod]
        public void SyntaxErrors(string text, params string[] invalidResourceTypes)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, invalidResourceTypes);
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.Core.Analyzers.Linter.Rules;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class UseRecentApiVersionRuleTests : LinterRuleTestsBase
    {
        private void CompileAndTest(string text, params string[] useRecentApiVersions)
        {
            AssertRuleCodeDiagnostics(UseRecentApiVersionRule.Code, text, diags =>
            {
                if (useRecentApiVersions.Any())
                {
                    var rule = new UseRecentApiVersionRule();
                    string[] expectedMessages = useRecentApiVersions.Select(p => rule.GetMessage(p)).ToArray();
                    diags.Select(e => e.Message).Should().ContainInOrder(expectedMessages);
                }
                else
                {
                    diags.Should().BeEmpty();
                }
            });
        }

        [DataRow(@"
            resource dnsZone 'Microsoft.Network/dnsZones@2015-10-01-preview' = {
              name: 'name'
              location: resourceGroup().location
            }",
            "2018-05-01")]
        [DataTestMethod]
        public void TestRule(string text, params string[] useRecentApiVersions)
        {
            CompileAndTest(text, useRecentApiVersions);
        }
    }
}

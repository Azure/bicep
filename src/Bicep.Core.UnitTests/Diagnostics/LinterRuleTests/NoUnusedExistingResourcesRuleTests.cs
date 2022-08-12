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
    public class NoUnusedExistingResourcesRuleTests : LinterRuleTestsBase
    {

        [TestMethod]
        public void ResourceNameInFormattedMessage()
        {
            var ruleToTest = new NoUnusedExistingResourcesRule();
            ruleToTest.GetMessage(nameof(ruleToTest)).Should().Be($"Existing resource \"{nameof(ruleToTest)}\" is declared but never used.");
        }

        private void CompileAndTest(string text, params string[] unusedExistingResources)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, unusedExistingResources);
        }

        private void CompileAndTest(string text, OnCompileErrors onCompileErrors, params string[] unusedExistingResources)
        {
            AssertLinterRuleDiagnostics(NoUnusedExistingResourcesRule.Code, text, diags =>
                {
                    if (unusedExistingResources.Any())
                    {
                        var rule = new NoUnusedExistingResourcesRule();
                        string[] expectedMessages = unusedExistingResources.Select(p => rule.GetMessage(p)).ToArray();
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
            resource app 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app'
            }
            ",
            "app")]
        [DataRow(@"
            resource app1 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app1'
            }
            resource app2 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app2'
            }
            ",
            "app1", "app2")]
        [DataRow(@"
            resource app 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app'
            }
            var appName = app.name
            ")]
        [DataRow(@"
            resource app1 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app1'
            }
            resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2017-05-01-preview' = {
                name: 'diagnosticSettings'
                scope: app1
            }
            ")]
        [DataRow(@"
            resource app1 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app1'
            }
            resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2017-05-01-preview' = {
                name: 'diagnosticSettings'
                scope: app1
            }

            resource app2 'Microsoft.Web/sites@2021-03-01' existing = {
                name: 'app2'
            }
            ",
            "app2")]
        [DataRow(@"
            resource newApp 'Microsoft.Web/sites@2021-03-01' = {
                name: 'newApp'
            }
            ")]
        [DataTestMethod]
        public void TestRule(string text, params string[] unusedExistingResources)
        {
            CompileAndTest(text, unusedExistingResources);
        }

        [DataRow(@"resource abc1 'Microsoft.Web/sites@2021-03-01' existing", "abc1")]
        [DataRow(@"resource abc2 'Microsoft.Web/sites@2021-03-01' existing =", "abc2")]
        [DataRow(@"resource abc3 'Microsoft.Web/sites@2021-03-01' existing = {", "abc3")]
        [DataRow(@"resource abc4 'Microsoft.Web/sites@2021-03-01' existing = {}", "abc4")]
        [DataTestMethod]
        public void SyntaxErrors(string text, params string[] unusedExistingResources)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, unusedExistingResources);
        }

        [DataRow(@"resource")]
        [DataRow(@"resource abc1")]
        [DataRow(@"resource abc1 existing")]
        [DataTestMethod]
        public void Errors(string text)
        {
            CompileAndTest(text, OnCompileErrors.Ignore);
        }
    }
}

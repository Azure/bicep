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
    public class NoUnusedParametersRuleTests : LinterRuleTestsBase
    {
        [TestMethod]
        public void ParameterNameInFormattedMessage()
        {
            var ruleToTest = new NoUnusedParametersRule();
            ruleToTest.GetMessage(nameof(ruleToTest)).Should().Be($"Parameter \"{nameof(ruleToTest)}\" is declared but never used.");
        }

        private void CompileAndTest(string text, params string[] unusedParams)
        {
            CompileAndTest(text, OnCompileErrors.IncludeErrors, unusedParams);
        }

        private void CompileAndTest(string text, OnCompileErrors onCompileErrors, params string[] unusedParams)
        {
            AssertLinterRuleDiagnostics(NoUnusedParametersRule.Code, text, diags =>
                {
                    if (unusedParams.Any())
                    {
                        var rule = new NoUnusedParametersRule();
                        string[] expectedMessages = unusedParams.Select(p => rule.GetMessage(p)).ToArray();
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
            param password string
            var sum = 1 + 3
            output sub int = sum
            ",
            "password")]
        [DataRow(@"
            param param1 string
            param param2 string
            param param3 string
            var sum = 1 + 3
            output sub int = sum
            ",
            "param1", "param2", "param3")]
        [DataRow(@"
            param param1 string
            param param2 int = 4
            param param3 string
            var sum = 1 + 3
            output sub int = sum + param2
            ",
            "param1", "param3")]
        [DataRow(@"
            param param2 int = 4
            var sum = 1 + 3
            output sub int = sum + param2
            ")]
        [DataRow(@"
            var sum = 1 + 3
            output sub int = sum
            ")]
        [DataRow(@"
            // Syntax errors
            resource abc 'Microsoft.AAD/domainServices@2021-03-01'
            param
            param p1
            param p2 =
                    ",
            "p1", "p2")]
        [DataTestMethod]
        public void TestRule(string text, params string[] unusedParams)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, unusedParams);
        }

        [DataRow(@"
            @minLength(3)
            @maxLength(11)
            param namePrefix string
            param location string = resourceGroup().location

            module stgModule './storageAccount.bicep' = {
              name: 'storageDeploy'
              params: {
                storagePrefix: namePrefix
                location: location
              }
            }

            output storageEndpoint object = stgModule.outputs.storageEndpoint
            ")]
        [DataRow(@"
            @minLength(3)
            @maxLength(11)
            param namePrefix string
            param location string = resourceGroup().location
            param unusedparam string

            module stgModule './storageAccount.bicep' = {
              name: 'storageDeploy'
              params: {
                storagePrefix: namePrefix
                location: location
              }
            }

            output storageEndpoint object = stgModule.outputs.storageEndpoint
            ",
            "unusedparam")]
        [DataTestMethod]
        public void Modules(string text, params string[] unusedParams)
        {
            CompileAndTest(text, OnCompileErrors.Ignore, unusedParams);
        }

        [DataRow(@"
            param location string

            resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = if (false) {
              name: 'myZone'
              location: location
            }
            ")]
        [DataRow(@"
            param location string
            param size int

            resource dnsZone 'Microsoft.Network/dnszones@2018-05-01' = if (false) {
              name: 'myZone'
              location: location
            }
            ",
            "size")]
        [DataTestMethod]
        public void Conditions(string text, params string[] unusedParams)
        {
            CompileAndTest(text, unusedParams);
        }

        [DataRow(@"param")] // Don't show as unused - no param name
        [DataRow(@"param // whoops")] // Don't show as unused - no param name
        [DataTestMethod]
        public void Errors(string text, params string[] unusedParams)
        {
            CompileAndTest(text, OnCompileErrors.Ignore);
        }
    }
}

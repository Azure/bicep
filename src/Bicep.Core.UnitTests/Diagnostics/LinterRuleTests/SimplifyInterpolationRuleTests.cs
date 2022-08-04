// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class SimplifyInterpolationRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string text, Options? options = null)
        {
            AssertLinterRuleDiagnostics(SimplifyInterpolationRule.Code, text, diags =>
                {
                    diags.Should().HaveCount(0, $"expecting linter rule to pass");
                },
                options);
        }

        private void ExpectDiagnosticWithFix(string text, string expectedFix)
        {
            AssertLinterRuleDiagnostics(SimplifyInterpolationRule.Code, text, diags =>
            {
                diags.Should().HaveCount(1, $"expected one fix per testcase");

                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.Should().HaveCount(1);
                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.Should().HaveCount(1);
                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.First().Text.Should().Be(expectedFix);
            });
        }

        [DataRow(
            @"
            param p1 string
            var v1 = '${p1}'
            ",
            "p1"
        )]
        [DataRow(
            @"
                param AutomationAccountName string
                resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                    name: '${AutomationAccountName}'
                    location: resourceGroup().location
                }",
            "AutomationAccountName"
        )]
        [DataRow(
            @"
                param p1 string
                resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                    name: 'name'
                    location: resourceGroup().location
                    properties: {
                        encryption: '${p1}'
                    }
                }",
            "p1"
        )]
        [DataRow(
            @"
                param p1 string = 'a'
                resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                    name: 'name'
                    location: resourceGroup().location
                    properties: {
                        encryption: '${p1}'
                    }
                }",
            "p1"
        )]
        [DataRow(@"
                @secure()
                param ssVal string
                var stringVal = '${ssVal}'
            ",
            "ssVal"
        )]
        [DataTestMethod]
        public void ParameterReference(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(
            @"
                var AutomationAccountName = 'mystring'
                resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                    name: '${AutomationAccountName}'
                }
            ",
            "AutomationAccountName"
        )]
        [DataRow(@"
                var ssVal = 'mystring'
                var stringVal = '${ssVal}'
            ",
            "ssVal"
        )]
        [DataRow(@"
                var ssVal = concat('a', 'b')
                var stringVal = '${ssVal}'
            ",
            "ssVal"
        )]
        [DataTestMethod]
        public void VariableReference(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(
            @"
            var s = '${concat('a', 'b')}'
            ",
            "concat('a', 'b')"
        )]
        [DataRow(
            @"
                resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                    name: 'name'
                    location: '${resourceGroup().location}'
                    properties: {
                        encryption: 'a'
                    }
                }",
            "resourceGroup().location"
        )]
        [DataTestMethod]
        public void StringExpression(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(@"
                    param AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: '${AutomationAccountName}text'
                        location: resourceGroup().location
                    }"
        )]
        [DataRow(@"
                    param AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: ' ${AutomationAccountName}'
                        location: resourceGroup().location
                    }"
        )]
        [DataRow(@"
                    param AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: ' ${AutomationAccountName} '
                        location: resourceGroup().location
                    }"
        )]
        [DataRow(@"
                    var AutomationAccountName = 'hello'
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: '${AutomationAccountName}${AutomationAccountName}'
                        location: resourceGroup().location
                    }"
        )]
        [DataTestMethod]
        public void InterpolationMoreThanJustParamOrVar_Passes(string text)
        {
            ExpectPass(text);
        }

        [DataRow(@"
                    param AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: AutomationAccountName
                        location: resourceGroup().location
                    }"
        )]
        [DataTestMethod]
        public void DoesntHaveInterpolation_Passes(string text)
        {
            ExpectPass(text);
        }

        [DataRow(@"
            param scriptToExecute string
            param location string
            param uamiId string
            param currentTime string

            resource dScript 'Microsoft.Resources/deploymentScripts@2019-10-01-preview' = {
                name: 'scriptWithStorage'
                location: location
                kind: 'AzureCLI'
                identity: {
                    type: 'UserAssigned'
                    userAssignedIdentities: {
                    '${uamiId}': {}  // <-- string interpolation inside string property should not fail test
                    }
                }
                properties: {
                    azCliVersion: '2.0.80'
                    storageAccountSettings: {
                    storageAccountName: stg.name
                    storageAccountKey: listKeys(stg.id, stg.apiVersion).keys[0].value
                    }
                    scriptContent: scriptToExecute
                    cleanupPreference: 'OnSuccess'
                    retentionInterval: 'P1D'
                    forceUpdateTag: currentTime // ensures script will run every time
                }
            }

            resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
                name: 'name'
}
            "
        )]
        [DataTestMethod]
        public void StringInterpolationInsidePropertyNames_Passes(string text)
        {
            ExpectPass(text);
        }

        [DataRow(@"
            var val = {}
            var stringVal = '${val}' // is a string '123'
            "
        )]
        [DataRow(@"
            var intVal = 123
            var stringVal = '${intVal}' // is a string '123'
            "
        )]
        [DataRow(@"
            var arrayOfStrings = [
                'a'
                'b'
            ]
            var stringVal = '${arrayOfStrings}'
        ")]
        [DataRow(@"
            var arrayOne = [
                'a'
                'b'
            ]
            var arrayTwo = [
                'c'
            ]

            var stringVal = '${concat(arrayOne, arrayTwo)}'
        ")]
        [DataRow(@"
            var stringVal = '${max(1, 2)}'
        ")]
        [DataRow(@"
            var stringVal = '${resourceGroup().tags}'
        ")]
        [DataTestMethod]
        public void TypeIsNotString_Passes(string text)
        {
            ExpectPass(text);
        }

        [DataRow(
            @"
                param untypedParam string
                output s string = '${untypedParam}
            "
        )]
        [DataRow(
            @"
                var s = '${unknown}'
            "
        )]
        [DataRow(
            @"
                param untypedParam // syntax error
                var stringVal = '${untypedParam}'
            "
        )]
        [DataTestMethod]
        public void SyntaxErrors_ExpectNoFixes(string text)
        {
            ExpectPass(text, new Options(OnCompileErrors.Ignore));
        }
    }
}

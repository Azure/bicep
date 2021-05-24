// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class SimplifyInterpolationRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string text)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(SimplifyInterpolationRule.Code, text);
                errors.Should().HaveCount(0, $"expecting linter rule to pass");
            }
        }

        private void ExpectDiagnosticWithFix(string text, string expectedFix)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(SimplifyInterpolationRule.Code, text);
                errors.Should().HaveCount(1, $"expected one fix per testcase");

                errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.Should().HaveCount(1);
                errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.Should().HaveCount(1);
                errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.First().Text.Should().Be(expectedFix);
            }
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
                }",
            "AutomationAccountName"
        )]
        [DataRow(
            @"
                param p1 string
                resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
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
                    properties: {
                        encryption: '${p1}'
                    }
                }",
            "p1"
        )]
        [DataRow(@"
                @secure
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
                @secure
                var ssVal = 'mystring'
                var stringVal = '${ssVal}'
            ",
            "ssVal"
        )]
        [DataRow(@"
                @secure
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

        [DataRow(@"
                    param AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: '${AutomationAccountName}text'
                    }"
        )]
        [DataRow(@"
                    param AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: ' ${AutomationAccountName}'
                    }"
        )]
        [DataRow(@"
                    param AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: ' ${AutomationAccountName} '
                    }"
        )]
        [DataRow(@"
                    var AutomationAccountName = 'hello'
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: '${AutomationAccountName}${AutomationAccountName}'
                    }"
        )]
        [DataRow(
            @"
            var s = '${concat('a','b')}'
            "
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
                    }"
        )]
        [DataTestMethod]
        public void DoesntHaveInterpolation_Passes(string text)
        {
            ExpectPass(text);
        }

        [DataRow(@"
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
            var arrayOfStrings = [ 'a', 'b' ]
            var stringVal = '${arrayOfStrings}'
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
            ExpectPass(text);
        }
    }
}


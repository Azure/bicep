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
            var errors = GetDiagnostics(SimplifyInterpolationRule.Code, text);
            errors.Should().HaveCount(0);
        }

        private void ExpectDiagnosticWithFix(string text, string expectedFix)
        {
            var errors = GetDiagnostics(SimplifyInterpolationRule.Code, text);
            errors.Should().HaveCount(1);

            errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.Should().HaveCount(1);
            errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.Should().HaveCount(1);
            errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.First().Text.Should().Equals(expectedFix);
        }

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
            param p1 string
            var v1 = '${p1}'
            ",
            "p1"
        )]
        [DataTestMethod]
        public void ParameterReference(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [TestMethod]
        public void VariableReference()
        {
            ExpectDiagnosticWithFix(
                @"
                    var AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: '${AutomationAccountName}'
                    }",
                "AutomationAccountName"
            );
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
                    param AutomationAccountName string
                    resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                        name: '${AutomationAccountName}${AutomationAccountName}'
                    }"
        )]
        [DataTestMethod]
        public void InterpolationMoreThanJustParam_Passes(string text)
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
    }
}

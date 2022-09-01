// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests
{
    [TestClass]
    public class PreferUnquotedPropertyNamesRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string text)
        {
            AssertLinterRuleDiagnostics(PreferUnquotedPropertyNamesRule.Code, text, Array.Empty<string>());
        }

        private void ExpectDiagnosticWithFix(string text, string expectedFix)
        {
            AssertLinterRuleDiagnostics(PreferUnquotedPropertyNamesRule.Code, text, diags =>
            {
                diags.Should().HaveCount(1, $"expected one fix per testcase");

                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.Should().HaveCount(1);
                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.Should().HaveCount(1);
                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.First().Text.Should().Be(expectedFix);
            });
        }

        [DataRow(
            @"
                var v1 = {
                    'myProp': {}
                }",
            "myProp"
        )]
        [DataRow(
            @"
                var v1 = {
                    'my_property': {}
                }",
            "my_property"
        )]
        [DataRow(
            @"
                var v1 = {
                    'myProp1': {}
                }",
            "myProp1"
        )]
        [DataTestMethod]
        public void ObjectPropertyDeclaration(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(
            @"
                var v1 = {
                    '1': {}
                }"
        )]
        [DataRow(
            @"
                var v1 = {
                    'my-property': {}
                }"
        )]
        [DataTestMethod]
        public void ObjectPropertyDeclaration_NotValidIdentifier(string text)
        {
            ExpectPass(text);
        }

        [DataRow(
            @"
                var v1 = {
                    v1: {}
                }"
        )]
        [DataRow(
            @"
                var v1 = {
                    myProperty: {}
                }"
        )]
        [DataTestMethod]
        public void ObjectPropertyDeclaration_AlreadyBare(string text)
        {
            ExpectPass(text);
        }

        [DataRow(
            @"
                param AnObject object
                var v1 = AnObject['myProp']",
            ".myProp"
        )]
        [DataRow(
            @"
                param AnObject object
                var v1 = AnObject['my_property']",
            ".my_property"
        )]
        [DataRow(
            @"
                param AnObject object
                var v1 = AnObject['myProp1']",
            ".myProp1"
        )]
        [DataTestMethod]
        public void ObjectPropertyDereference(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(
            @"
                param AnObject object
                var v1 = AnObject['1']"
        )]
        [DataRow(
            @"
                param AnObject object
                var v1 = AnObject['my-property']"
        )]
        [DataTestMethod]
        public void ObjectPropertyDereference_NotValidIdentifier(string text)
        {
            ExpectPass(text);
        }

        [DataRow(
            @"
                param AutomationAccountName string
                param AutomationAccountLocation string
                resource AutomationAccount 'Microsoft.Automation/automationAccounts@2020-01-13-preview' = {
                    name: AutomationAccountName
                    location: AutomationAccountLocation
                }"
        )]
        [DataTestMethod]
        public void NoPropertyDeclarationOrDereference_Passes(string text)
        {
            ExpectPass(text);
        }
    }
}

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
    public class SimplifyJsonNullRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string text, Options? options = null)
        {
            AssertLinterRuleDiagnostics(SimplifyJsonNullRule.Code, text, diags =>
            {
                diags.Should().HaveCount(0, $"expecting linter rule to pass");
            },
            options);
        }

        private void ExpectDiagnosticWithFix(string text, string expectedFix)
        {
            AssertLinterRuleDiagnostics(SimplifyJsonNullRule.Code, text, diags =>
            {
                diags.Should().HaveCount(1, $"expected one fix per testcase");

                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.Should().HaveCount(1);
                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.Should().HaveCount(1);
                diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.First().Text.Should().Be(expectedFix);
            });
        }

        [DataRow(
            @"
                param myParam object = {
                    prop1: json('null')
                }
            ",
            "null"
        )]
        [DataRow(
            @"
                var myVar = json('null')
            ",
            "null"
        )]
        [DataRow(
            @"
                var myVar = json(' null  ')
            ",
            "null"
        )]
        [DataRow(
            @"
                var myVar = json( 'null')
            ",
            "null"
        )]
        [DataRow(
            @"
                resource stgAcct 'Microsoft.Storage/storageAccounts@2022-09-01' = {
                    name: 'myStorageAccount'
                    properties: {
                        azureFilesIdentityBasedAuthentication: json('null')
                    }
                }
            ",
            "null"
        )]
        [DataRow(
            @"
                output myOutput object = {
                    prop1: json('null')
                }
            ",
            "null"
        )]
        [DataTestMethod]
        public void Rule_ProducesDiagnosticWithFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [TestMethod]
        public void DoesntHaveJsonNull_Passes()
        {
            ExpectPass("var test = 'test'");
        }

        [DataTestMethod]
        [DataRow(
            @"
                var a = json(null)
            "
        )]
        [DataRow(
            @"
                var a = json('nullx')
            "
        )]
        [DataRow(
            @"
                var a = 'json('null')'
            "
        )]
        public void SyntaxErrors_ExpectNoFixes(string text)
        {
            ExpectPass(text, new Options(OnCompileErrors.Ignore));
        }

        [TestMethod]
        public void FunctionAppearsInStringLiteratl_ExpectNoFixes()
        {
            ExpectPass("var a = '''json('null')'''");
        }
    }
}
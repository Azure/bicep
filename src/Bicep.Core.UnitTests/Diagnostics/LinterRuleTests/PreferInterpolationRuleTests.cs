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
    public class PreferInterpolationRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string text, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        {
            AssertLinterRuleDiagnostics(PreferInterpolationRule.Code, text, diags =>
                {
                    diags.Should().HaveCount(0, $"expecting linter rule to pass");
                },
                new Options(onCompileErrors));
        }

        private void ExpectDiagnosticWithFix(string text, string expectedFix, Options? options = null)
        {
            ExpectDiagnosticWithFix(text, new string[] { expectedFix }, options);
        }

        private void ExpectDiagnosticWithFix(string text, string[] expectedFixes, Options? options = null)
        {
            AssertLinterRuleDiagnostics(PreferInterpolationRule.Code, text, diags =>
                {
                    diags.Should().HaveCount(expectedFixes.Length, $"expecting one fix per testcase");

                    diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.Should().HaveCount(1);
                    diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.Should().HaveCount(1);
                    var a = diags.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.SelectMany(f => f.Replacements.SelectMany(r => r.Text));
                },
                options);
        }

        [DataRow(@"
                param suffix string = '001'
                var vnetName = concat('vnet-', suffix)
                resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                    name: vnetName
                }
            ",
            "'vnet-${suffix}'"
        )]
        [DataTestMethod]
        public void VariableValue_HasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(@"
                param suffix string = '001'
                param vnetName string = concat('vnet-', suffix)
                resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                    name: vnetName
                }
            ",
            "'vnet-${suffix}'"
        )]
        [DataTestMethod]
        public void ParameterValue_HasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(@"
                param suffix string = '001'
                param vnetName string = concat('vnet-', suffix)
                resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                    name: vnetName
                }
            ",
            "'vnet-${suffix}'"
        )]
        [DataTestMethod]
        public void ResourceProperty_HasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(@"
                param p1 string
                param p2 string

                resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                    name: 'vnet'
                    properties: {
                        addressSpace: concat(p1, p2)
                    }
                }
            ",
            "'${p1}${p2}'"
        )]
        [DataTestMethod]
        public void ResourceDeepProperty_HasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(
            @"
                var v1 = 'v1'
                // Note that this has multiple issues flagged in the same line
                var v2 = startsWith(concat(v1, '-variable'), concat('a', 'b'))
            ",
             "'${v1}-variable'",
              "'ab'"
        )]
        [DataRow(
            @"
                param vNetName string
                param projectName string
                var v1 = resourceId('Microsoft.Network/virtualNetworks/subnets', vNetName, concat(projectName, 'main'))
            ",
            "'${projectName}main'"
        )]
        [DataTestMethod]
        public void ConcatDeepInExpression_HasFix(string text, params string[] expectedFixes)
        {
            ExpectDiagnosticWithFix(text, expectedFixes);
        }

        [DataRow(@"
                var v1 = concat('abc', 'def')
            ",
            "'abcdef'"
        )]
        [DataRow(@"
                var v1 = concat('abc', 'def', 'ghi')
            ",
            "'abcdefghi'"
        )]
        [DataRow(@"
                var v3 = concat(concat('pre'), concat('abc', 'def'), concat('ghi', 'jkl', 'mno'))
            ",
            "'preabcdefghijklmno'"
        )]
        [DataTestMethod]
        public void JustLiterals_HasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat('a', v1)
            ",
            "'a${v1}'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat(v1, 'a')
            ",
            "'${v1}a'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat('a', v1, 'b')
            ",
            "'a${v1}b'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat(v1, 'a', v2)
            ",
            "'${v1}a${v2}'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat('a', v1, 'b', v2)
            ",
            "'a${v1}b${v2}'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat(concat('abc', v1), concat('ghi', v2, 'jkl'))
            ",
            "'abc${v1}ghi${v2}jkl'"
        )]
        [DataTestMethod]
        public void MixedLiteralsAndExpressions_HasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(@"
                var v1 = 'abc'
                var v2 = 'def'
                var v3 = concat(v1, v2)
            ",
            "'${v1}${v2}'"
        )]
        [DataRow(@"
                var v1 = 'abc'
                var v2 = 'def'
                var v3 = concat(v1, v2)
            ",
            "'${v1}${v2}'"
        )]
        [DataRow(@"
                var v1 = 'abc'
                var v2 = 'def'
                var v3 = concat(v1, concat(v1, v2))
            ",
            "'${v1}${v1}${v2}'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = concat(concat('abc', 'def'), concat('ghi', v1, 'jkl'))
            ",
            "'abcdefghi${v1}jkl'"
        )]
        [DataRow(
            @"
                var a = [
                    'a'
                    123
                ]
                var c1 = concat('a', '${a}', 'b')
            ",
            "'a${a}b'"
        )]
        [DataRow(
            @"
                var a = [
                    'a'
                    123
                ]
                var c2 = concat('${a}', '${a}', uniqueString('${a}'))
            ",
            "'${a}${a}${uniqueString('${a}')}'"
        )]
        [DataRow(
            @"
                var a = [
                    'a'
                    123
                ]
                var b = {}
                var c3 = concat('${a}', '${b}', uniqueString('${a}'))
            ",
            "'${a}${b}${uniqueString('${a}')}'"
        )]
        [DataTestMethod]
        public void StringFolding_HasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(@"
                var v1 = 'v1'
                var v2 = concat(concat('abc', 'def'), concat(1 + 2, v1, 'jkl'))
            ",
            "'abcdef${1 + 2}${v1}jkl'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat(startsWith(concat('abc', v1), 'hello'), v2, 1 + 2, v1, 'jkl')
            ",
            "'${startsWith(concat('abc', v1), 'hello')}${v2}${1 + 2}${v1}jkl'"
        )]
        [DataTestMethod]
        public void NestedComplexExpressions_HasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, expectedFix);
        }

        [DataRow(
            @"
                var a = [
                    'a'
                    'b'
                    123
                ]
                var b = [
                    'c'
                    456
                ]
                output test2 array = concat(a, b) // Valid expression, but result would be an array, not a string, so no suggestions
                output test3 string = '${a}${b}' // Valid, but already a string interpolation, so no suggested fix
            "
        )]
        [DataRow(
            @"
                var myObj = {
                    val: true
                }
                var myArray = [
                    12
                    34
                ]
                output testa object = concat(myObj, myArray) // This results in a compiler error (type mismatch), should not suggest fixes
                output test string = '${myObj}..${myArray}'  // Valid but already a string interpolation
            ",
            OnCompileErrors.Ignore
        )]
        [DataRow(
            @"
                var myObj = {
                val: true
                }
                output testa object = concat(myObj, myObj) // Another type mismatch error
            ",
            OnCompileErrors.Ignore
        )]
        [DataRow(
            @"
                var myObj = {
                val: true
                }
                output testa object = concat(myObj, 'a') // Another type mismatch error
            ",
            OnCompileErrors.Ignore
        )]
        [DataRow(
            @"
                var a = [
                    'a'
                    123
                ]
                var b = [
                    'c'
                ]
                var c = concat(a, a) // Two arrays - result is array
            "
        )]
        [DataRow(
            @"
                var a = [
                    'a'
                    123
                ]
                var b = {}
                var c = concat(a, b, uniqueString('${a}')) // type mismaatch
            ",
            OnCompileErrors.Ignore
        )]
        [DataRow(
            @"
                var a = [
                    'a'
                    123
                ]
                var b
                var c = concat('a', b, uniqueString('${a}')) // b is in error
            ",
            OnCompileErrors.Ignore
        )]
        [DataRow(
            @"
                var a = 'a'
                output c string = concat(, a) // syntax error
            ",
            OnCompileErrors.Ignore
        )]
        [DataRow(
            @"
                var a = []
                var b = concat(a) // array - no interpolate recommended - this is also ignored because we require more than 1 arg
            "
        )]
        [DataRow(
            @"
                var a1 = []
                var a2 = []
                var a3 = []
                var a4 = []
                var b = concat(a1, a2, a3, a4) // arrays - no interpolate recommended
            "
        )]
        [DataTestMethod]
        public void ArgsNotStrings_DoNotSuggestFix(string text, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        {
            ExpectPass(text, onCompileErrors);
        }

        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat('abc', 'DEF'), 'hello')
            ",
            "'abcDEF'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = 'v2'
                var v3 = concat( 'abc', 'DEF', 'hello'
            ",
            null
        )]
        [DataRow(@"
                var v1 = concat()
            ",
            null
        )]
        [DataRow(@"
                var v3 = concat(1 +, 'hi')
            ",
            null
        )]
        [DataRow(@"
                module abc concat('a', 'b') = {
                    name: 'name'
                }
            ",
            null
        )]
        [DataRow(
            @"
                var a = 'a'
                output c string = concat(, a) // syntax error here shouldn't affect us suggesting a fix in the next line
                output d string = concat('a', 'b')
            ",
            "'ab'"
        )]
        [DataTestMethod]
        public void HandlesSyntaxErrors(string text, string? expectedFix)
        {
            if (expectedFix == null)
            {
                ExpectPass(text, OnCompileErrors.Ignore);
            }
            else
            {
                ExpectDiagnosticWithFix(text, expectedFix, new Options(OnCompileErrors.Ignore));
            }
        }

        [DataRow(@"
                var v1 = concat('abc')
            "
        )]
        [DataRow(@"
                var a = 'text'
                var b = concat(a) // by definition concat must have multiple arguments before we recommend interpolation
            "
        )]
        [DataTestMethod]
        public void SingleArgs_DontSuggestFix(string text)
        {
            ExpectPass(text);
        }
    }
}

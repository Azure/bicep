
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
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
    public class InterpolateNotConcatRuleTests : LinterRuleTestsBase
    {
        private void ExpectPass(string text)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(InterpolateNotConcatRule.Code, text);
                errors.Should().HaveCount(0, $"Expecting linter rule to pass. Text: {text}");
            }
        }

        private void ExpectDiagnosticWithFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFix(text, new string[] { expectedFix });
        }

        private void ExpectDiagnosticWithFix(string text, string[] expectedFixes)
        {
            using (new AssertionScope($"linter errors for this code:\n{text}\n"))
            {
                var errors = GetDiagnostics(InterpolateNotConcatRule.Code, text);
                errors.Should().HaveCount(expectedFixes.Length, $"expected one fix per testcase.  Text: {text}");

                errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.Should().HaveCount(1);
                errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.First().Replacements.Should().HaveCount(1);
                var a = errors.First().As<IBicepAnalyerFixableDiagnostic>().Fixes.SelectMany(f => f.Replacements.SelectMany(r => r.Text));
            }
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
                var v1 = concat('abc')
            ",
            "'abc'"
        )]
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
                var v2 = concat(concat('abc', v1), concat('ghi', v2, 'jkl'))
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
                var v3 = CONCAT(v1, concat(v1, v2))
            ",
            "'${v1}${v1}${v2}'"
        )]
        [DataRow(@"
                var v1 = 'v1'
                var v2 = concat(concat('abc', 'def'), concat('ghi', v1, 'jkl'))
            ",
            "'abcdefghi${v1}jkl'"
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
            "''"
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
        [DataTestMethod]
        public void HandlesSyntaxErrors(string text, string? expectedFix) // TODO: more
        {
            if (expectedFix == null)
            {
                ExpectPass(text);
            }
            else
            {
                ExpectDiagnosticWithFix(text, expectedFix);
            }
        }
    }
}

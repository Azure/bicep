// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoConflictingMetadataRuleTests : LinterRuleTestsBase
{
    private void CompileAndTest(string text, int expectedDiagnosticCount, Options? options = null)
    {
        AssertLinterRuleDiagnostics(NoConflictingMetadataRule.Code, text, expectedDiagnosticCount, options);
    }

    [DataRow("""
        @metadata({
            description: 'Description set in metadata'
        })
        @description('Description set via decorator')
        param p string
        """)]
    [DataTestMethod]
    public void If_UsesBothMetadataPropertyAndConflictingDecorator_ShouldRaise(string text)
    {
        CompileAndTest(text, 1);
    }

    [DataRow("""
        @metadata({
            description: 'Description set in metadata'
        })
        param p string
        """)]
    [DataTestMethod]
    public void If_UsesBothMetadataPropertyWithoutConflictingDecorator_ShouldNotRaise(string text)
    {
        CompileAndTest(text, 0);
    }
}

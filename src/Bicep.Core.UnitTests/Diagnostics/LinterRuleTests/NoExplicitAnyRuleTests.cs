// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoExplicitAnyRuleTests : LinterRuleTestsBase
{
    [DataRow("type foo = any")]
    [DataRow("param foo any")]
    [DataRow("output foo any = 'foo'")]
    [DataRow("type foo = (any)")]
    [DataRow("type foo = any!")]
    [DataRow("type foo = any?")]
    [DataRow("type foo = any[]")]
    [DataRow("type foo = [any]")]
    [DataRow("type foo = { prop: any }")]
    [DataRow("type foo = { *: any }")]
    [DataRow("type foo = sys.any")]
    [DataTestMethod]
    public void Should_raise_diagnostic_when_any_used(string text, int diagnosticCount = 1)
        => CompileAndTest(text, diagnosticCount);

    [TestMethod]
    public void Should_not_raise_if_user_defined_type_named_any_is_used()
    {
        CompileAndTest(
            """
            type any = string
            type foo = any
            """,
            0);
    }

    private void CompileAndTest(string text, int expectedErrorCount, Options? options = null)
    {
        AssertLinterRuleDiagnostics(NoExplicitAnyRule.Code, text, expectedErrorCount, options);
    }
}

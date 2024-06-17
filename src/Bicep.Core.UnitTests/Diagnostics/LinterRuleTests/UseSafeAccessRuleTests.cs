// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseSafeAccessRuleTests : LinterRuleTestsBase
{
    private void AssertCodeFix(string inputFile, string resultFile)
        => AssertCodeFix(UseSafeAccessRule.Code, "Use the safe access (.?) operator", inputFile, resultFile);

    private void AssertNoDiagnostics(string inputFile)
        => AssertLinterRuleDiagnostics(UseSafeAccessRule.Code, inputFile, [], new(OnCompileErrors.Ignore, IncludePosition.None));

    [TestMethod]
    public void Codefix_fixes_syntax_which_can_be_simplified() => AssertCodeFix("""
param foo object
var test = contai|ns(foo, 'bar') ? foo.bar : 'baz'
""", """
param foo object
var test = foo.?bar ?? 'baz'
""");

    [TestMethod]
    public void Codefix_fixes_syntax_which_can_be_simplified_array_access() => AssertCodeFix("""
param foo object
param target string
var test = contai|ns(foo, target) ? foo[target] : 'baz'
""", """
param foo object
param target string
var test = foo[?target] ?? 'baz'
""");

    [TestMethod]
    public void Rule_ignores_syntax_which_cannot_be_simplified() => AssertNoDiagnostics("""
param foo object
var test = contains(foo, 'bar') ? foo.baz : 'baz'
""");

    [TestMethod]
    public void Rule_ignores_syntax_which_cannot_be_simplified_2() => AssertNoDiagnostics("""
param foo object
param target string
param notTarget string
var test = contains(foo, target) ? bar[notTarget] : 'baz'
""");

    [TestMethod]
    public void Rule_ignores_syntax_which_cannot_be_simplified_array_access() => AssertNoDiagnostics("""
param foo object
var test = contains(foo, 'bar') ? bar.bar : 'baz'
""");
}

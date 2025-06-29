// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseUserDefinedTypesRuleTests : LinterRuleTestsBase
{
    private void AssertDiagnostics(string inputFile, int expectedCount = 1)
        => AssertLinterRuleDiagnostics(UseUserDefinedTypesRule.Code, inputFile, expectedCount);

    private void AssertNoDiagnostics(string inputFile)
        => AssertLinterRuleDiagnostics(UseUserDefinedTypesRule.Code, inputFile, [], new(OnCompileErrors.Ignore, IncludePosition.None));

    [TestMethod]
    public void Rule_ignores_user_defined_types() => AssertNoDiagnostics("""
param foo {
  bar: string
}
""");

    [TestMethod]
    public void Rule_defaults_to_off()
    {
        var result = CompilationHelper.Compile("""
param foo object
""");
        result.ExcludingDiagnostics("no-unused-params").Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Rule_flags_usage_of_object() => AssertDiagnostics("""
param foo object
""");

    [TestMethod]
    public void Rule_flags_usage_of_array() => AssertDiagnostics("""
param foo object
""");
}

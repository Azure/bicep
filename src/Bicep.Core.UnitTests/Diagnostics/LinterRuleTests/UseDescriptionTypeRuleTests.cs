// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseDescriptionTypeRuleTests : LinterRuleTestsBase
{
    private static readonly Options RuleOptions = new(ConfigurationPatch: EnableRule);

    private static RootConfiguration EnableRule(RootConfiguration configuration) =>
        configuration.WithAnalyzersConfiguration(
            configuration.Analyzers.SetValue($"core.rules.{UseDescriptionTypeRule.Code}.level", "warning"));

    private void AssertDiagnostics(string inputFile, int expectedCount = 1)
        => AssertLinterRuleDiagnostics(UseDescriptionTypeRule.Code, inputFile, expectedCount, RuleOptions);

    private void AssertDiagnostics(string inputFile, string[] expectedMessages)
        => AssertLinterRuleDiagnostics(UseDescriptionTypeRule.Code, inputFile, expectedMessages, RuleOptions);

    private void AssertNoDiagnostics(string inputFile, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        => AssertLinterRuleDiagnostics(
            UseDescriptionTypeRule.Code,
            inputFile,
            [],
            RuleOptions with
            {
                OnCompileErrors = onCompileErrors,
                IncludePosition = IncludePosition.None,
            });

    [TestMethod]
    public void Rule_defaults_to_off()
    {
        var result = CompilationHelper.Compile("""
            @export()
            type myType = string
            """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Types_without_descriptions_are_reported()
    {
        AssertDiagnostics(
            """
            @export()
            type first = string

            @export()
            type second = int
            """,
            [
                """[2] Type "first" must have a non-empty description.""",
                """[5] Type "second" must have a non-empty description.""",
            ]);
    }

    [DataRow("""
        @export()
        @description('Type description.')
        type myType = string
        """)]
    [DataRow("""
        @export()
        @sys.description('Type description.')
        type myType = string
        """)]
    [DataTestMethod]
    public void Non_empty_descriptions_are_accepted(string text)
    {
        AssertNoDiagnostics(text);
    }

    [DataRow("""
        @export()
        @description('')
        type myType = string
        """)]
    [DataRow("""
        @export()
        @description('   ')
        type myType = string
        """)]
    [DataTestMethod]
    public void Empty_and_whitespace_descriptions_are_reported(string text)
    {
        AssertDiagnostics(text);
    }

    [TestMethod]
    public void Descriptions_on_type_properties_do_not_satisfy_the_rule()
    {
        AssertDiagnostics(
            """
            @export()
            type myType = {
              @description('Property description.')
              name: string
            }
            """,
            ["""[2] Type "myType" must have a non-empty description."""]);
    }

    [DataRow("""
        @description('Parameter description.')
        param input string
        """)]
    [DataRow("""
        @description('Output description.')
        output result string = 'value'
        """)]
    [DataTestMethod]
    public void Descriptions_on_other_declarations_are_ignored(string text)
    {
        AssertNoDiagnostics(text);
    }

    [TestMethod]
    public void Malformed_type_without_a_name_is_ignored()
    {
        AssertNoDiagnostics("type", OnCompileErrors.Ignore);
    }
}

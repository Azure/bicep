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
public class UseDescriptionVarsRuleTests : LinterRuleTestsBase
{
    private static readonly Options RuleOptions = new(ConfigurationPatch: EnableRule);

    private static RootConfiguration EnableRule(RootConfiguration configuration) =>
        configuration.WithAnalyzersConfiguration(
            configuration.Analyzers.SetValue($"core.rules.{UseDescriptionVarsRule.Code}.level", "warning"));

    private void AssertDiagnostics(string inputFile, int expectedCount = 1)
        => AssertLinterRuleDiagnostics(UseDescriptionVarsRule.Code, inputFile, expectedCount, RuleOptions);

    private void AssertDiagnostics(string inputFile, string[] expectedMessages)
        => AssertLinterRuleDiagnostics(UseDescriptionVarsRule.Code, inputFile, expectedMessages, RuleOptions);

    private void AssertNoDiagnostics(string inputFile, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        => AssertLinterRuleDiagnostics(
            UseDescriptionVarsRule.Code,
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
            var input = 'value'
            """);

        result.ExcludingDiagnostics("no-unused-vars").Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Variables_without_descriptions_are_reported()
    {
        AssertDiagnostics(
            """
            var first = 'value'

            @export()
            var second = 'value'
            """,
            [
                """[1] Variable "first" must have a non-empty description.""",
                """[4] Variable "second" must have a non-empty description.""",
            ]);
    }

    [DataRow("""
        @description('Variable description.')
        var input = 'value'
        """)]
    [DataRow("""
        @sys.description('Variable description.')
        var input = 'value'
        """)]
    [DataTestMethod]
    public void Non_empty_descriptions_are_accepted(string text)
    {
        AssertNoDiagnostics(text);
    }

    [DataRow("""
        @description('')
        var input = 'value'
        """)]
    [DataRow("""
        @description('   ')
        var input = 'value'
        """)]
    [DataRow("""
        @sys.description('')
        var input = 'value'
        """)]
    [DataRow("""
        @sys.description('''

        ''')
        var input = 'value'
        """)]
    [DataTestMethod]
    public void Empty_and_whitespace_descriptions_are_reported(string text)
    {
        AssertDiagnostics(text);
    }

    [TestMethod]
    public void Loop_variables_are_reported()
    {
        AssertDiagnostics(
            """
            var items = [for i in range(0, 3): i]
            """,
            ["""[1] Variable "items" must have a non-empty description."""]);
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
    public void Malformed_variable_without_a_name_is_ignored()
    {
        AssertNoDiagnostics("var", OnCompileErrors.Ignore);
    }
}

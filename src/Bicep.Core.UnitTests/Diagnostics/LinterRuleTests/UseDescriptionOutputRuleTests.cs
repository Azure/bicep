// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Testing.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseDescriptionOutputRuleTests : LinterRuleTestsBase
{
    private static readonly Options RuleOptions = new(ConfigurationPatch: EnableRule);

    private static IBicepConfiguration EnableRule(IBicepConfiguration configuration) =>
        configuration.WithAnalyzersConfiguration(
            ((AnalyzersConfiguration)configuration.Analyzers).SetValue($"core.rules.{UseDescriptionOutputRule.Code}.level", "warning"));

    private void AssertDiagnostics(string inputFile, int expectedCount = 1)
        => AssertLinterRuleDiagnostics(UseDescriptionOutputRule.Code, inputFile, expectedCount, RuleOptions);

    private void AssertDiagnostics(string inputFile, string[] expectedMessages)
        => AssertLinterRuleDiagnostics(UseDescriptionOutputRule.Code, inputFile, expectedMessages, RuleOptions);

    private void AssertNoDiagnostics(string inputFile, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        => AssertLinterRuleDiagnostics(
            UseDescriptionOutputRule.Code,
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
            output result string = 'value'
            """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Outputs_without_descriptions_are_reported()
    {
        AssertDiagnostics(
            """
            output first string = 'value'

            @secure()
            output second string = 'value'
            """,
            [
                """[1] Output "first" must have a non-empty description.""",
                """[4] Output "second" must have a non-empty description.""",
            ]);
    }

    [DataRow("""
        @description('Output description.')
        output result string = 'value'
        """)]
    [DataRow("""
        @sys.description('Output description.')
        output result string = 'value'
        """)]
    [DataTestMethod]
    public void Non_empty_descriptions_are_accepted(string text)
    {
        AssertNoDiagnostics(text);
    }

    [DataRow("""
        @description('')
        output result string = 'value'
        """)]
    [DataRow("""
        @description('   ')
        output result string = 'value'
        """)]
    [DataRow("""
        @sys.description('''

        ''')
        output result string = 'value'
        """)]
    [DataTestMethod]
    public void Empty_and_whitespace_descriptions_are_reported(string text)
    {
        AssertDiagnostics(text);
    }

    [TestMethod]
    public void Metadata_description_does_not_satisfy_the_rule()
    {
        AssertDiagnostics(
            """
            @metadata({ description: 'Metadata description.' })
            output result string = 'value'
            """,
            ["""[2] Output "result" must have a non-empty description."""]);
    }

    [DataRow("""
        @description('Parameter description.')
        param input string
        """)]
    [DataRow("""
        @description('Variable description.')
        var value = 'value'
        """)]
    [DataTestMethod]
    public void Descriptions_on_other_declarations_are_ignored(string text)
    {
        AssertNoDiagnostics(text);
    }

    [TestMethod]
    public void Malformed_output_without_a_name_is_ignored()
    {
        AssertNoDiagnostics("output", OnCompileErrors.Ignore);
    }
}

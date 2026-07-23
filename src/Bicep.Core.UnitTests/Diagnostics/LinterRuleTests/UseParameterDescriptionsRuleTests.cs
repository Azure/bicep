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
public class UseParameterDescriptionsRuleTests : LinterRuleTestsBase
{
    private static readonly Options RuleOptions = new(ConfigurationPatch: EnableRule);

    private static RootConfiguration EnableRule(RootConfiguration configuration) =>
        configuration.WithAnalyzersConfiguration(
            configuration.Analyzers.SetValue($"core.rules.{UseParameterDescriptionsRule.Code}.level", "warning"));

    private void AssertDiagnostics(string inputFile, int expectedCount = 1)
        => AssertLinterRuleDiagnostics(UseParameterDescriptionsRule.Code, inputFile, expectedCount, RuleOptions);

    private void AssertDiagnostics(string inputFile, string[] expectedMessages)
        => AssertLinterRuleDiagnostics(UseParameterDescriptionsRule.Code, inputFile, expectedMessages, RuleOptions);

    private void AssertNoDiagnostics(string inputFile, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        => AssertLinterRuleDiagnostics(
            UseParameterDescriptionsRule.Code,
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
            param input string
            """);

        result.ExcludingDiagnostics("no-unused-params").Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Parameters_without_descriptions_are_reported()
    {
        AssertDiagnostics(
            """
            param first string

            @secure()
            param second string
            """,
            [
                """[1] Parameter "first" must have a non-empty description.""",
                """[4] Parameter "second" must have a non-empty description.""",
            ]);
    }

    [DataRow("""
        @description('Parameter description.')
        param input string
        """)]
    [DataRow("""
        @sys.description('Parameter description.')
        param input string
        """)]
    [DataTestMethod]
    public void Non_empty_descriptions_are_accepted(string text)
    {
        AssertNoDiagnostics(text);
    }

    [DataRow("""
        @description('')
        param input string
        """)]
    [DataRow("""
        @description('   ')
        param input string
        """)]
    [DataRow("""
        @sys.description('')
        param input string
        """)]
    [DataRow("""
        @sys.description('''

        ''')
        param input string
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
            param input string
            """,
            ["""[2] Parameter "input" must have a non-empty description."""]);
    }

    [DataRow("""
        @description('Variable description.')
        var value = 'value'
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
    public void Malformed_parameter_without_a_name_is_ignored()
    {
        AssertNoDiagnostics("param", OnCompileErrors.Ignore);
    }
}

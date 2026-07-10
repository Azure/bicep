// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class UseParameterDescriptionsRuleTests : LinterRuleTestsBase
{
    private static readonly Options RuleOptions = new(
        OnCompileErrors.Ignore,
        ConfigurationPatch: EnableRule);

    private static RootConfiguration EnableRule(RootConfiguration configuration) =>
        configuration.WithAnalyzersConfiguration(
            configuration.Analyzers.SetValue($"core.rules.{UseParameterDescriptionsRule.Code}.level", "warning"));

    [TestMethod]
    public void Rule_defaults_to_warning()
    {
        new UseParameterDescriptionsRule().DefaultDiagnosticLevel.Should().Be(DiagnosticLevel.Warning);
    }

    [TestMethod]
    public void Parameters_without_descriptions_are_reported()
    {
        AssertLinterRuleDiagnostics(
            UseParameterDescriptionsRule.Code,
            """
            param first string

            @secure()
            param second string
            """,
            [
                """[1] Parameter "first" must have a non-empty description.""",
                """[4] Parameter "second" must have a non-empty description.""",
            ],
            RuleOptions);
    }

    [TestMethod]
    public void Unqualified_and_sys_qualified_descriptions_are_accepted()
    {
        AssertLinterRuleDiagnostics(
            UseParameterDescriptionsRule.Code,
            """
            @description('First parameter.')
            param first string

            @sys.description('Second parameter.')
            param second string
            """,
            0,
            RuleOptions);
    }

    [TestMethod]
    public void Empty_and_whitespace_descriptions_are_reported_for_both_decorator_forms()
    {
        AssertLinterRuleDiagnostics(
            UseParameterDescriptionsRule.Code,
            """
            @description('')
            param empty string

            @description('   ')
            param spaces string

            @sys.description('')
            param qualifiedEmpty string

            @sys.description('''

            ''')
            param qualifiedNewline string
            """,
            4,
            RuleOptions);
    }

    [TestMethod]
    public void Metadata_description_does_not_satisfy_the_rule()
    {
        AssertLinterRuleDiagnostics(
            UseParameterDescriptionsRule.Code,
            """
            @metadata({ description: 'Metadata description.' })
            param input string
            """,
            ["""[2] Parameter "input" must have a non-empty description."""],
            RuleOptions);
    }

    [TestMethod]
    public void Descriptions_on_other_declarations_are_ignored()
    {
        AssertLinterRuleDiagnostics(
            UseParameterDescriptionsRule.Code,
            """
            @description('Variable description.')
            var value = 'value'

            @description('Output description.')
            output result string = value
            """,
            0,
            RuleOptions);
    }

    [TestMethod]
    public void Malformed_parameter_without_a_name_is_ignored()
    {
        AssertLinterRuleDiagnostics(
            UseParameterDescriptionsRule.Code,
            "param",
            0,
            RuleOptions);
    }
}

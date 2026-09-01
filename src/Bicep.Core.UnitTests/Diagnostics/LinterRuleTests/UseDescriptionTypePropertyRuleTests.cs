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
public class UseDescriptionTypePropertyRuleTests : LinterRuleTestsBase
{
    private static readonly Options RuleOptions = new(ConfigurationPatch: EnableRule);

    private static IBicepConfiguration EnableRule(IBicepConfiguration configuration) =>
        configuration.WithAnalyzersConfiguration(
            ((AnalyzersConfiguration)configuration.Analyzers).SetValue($"core.rules.{UseDescriptionTypePropertyRule.Code}.level", "warning"));

    private void AssertDiagnostics(string inputFile, int expectedCount = 1)
        => AssertLinterRuleDiagnostics(UseDescriptionTypePropertyRule.Code, inputFile, expectedCount, RuleOptions);

    private void AssertDiagnostics(string inputFile, string[] expectedMessages)
        => AssertLinterRuleDiagnostics(UseDescriptionTypePropertyRule.Code, inputFile, expectedMessages, RuleOptions);

    private void AssertNoDiagnostics(string inputFile, OnCompileErrors onCompileErrors = OnCompileErrors.IncludeErrors)
        => AssertLinterRuleDiagnostics(
            UseDescriptionTypePropertyRule.Code,
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
            type myType = {
              name: string
            }
            """);

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Properties_without_descriptions_are_reported()
    {
        AssertDiagnostics(
            """
            @export()
            type myType = {
              first: string
              second: int
            }
            """,
            [
                """[3] Type property "first" must have a non-empty description.""",
                """[4] Type property "second" must have a non-empty description.""",
            ]);
    }

    [DataRow("""
        @export()
        type myType = {
          @description('Property description.')
          name: string
        }
        """)]
    [DataRow("""
        @export()
        type myType = {
          @sys.description('Property description.')
          name: string
        }
        """)]
    [DataTestMethod]
    public void Non_empty_descriptions_are_accepted(string text)
    {
        AssertNoDiagnostics(text);
    }

    [DataRow("""
        @export()
        type myType = {
          @description('')
          name: string
        }
        """)]
    [DataRow("""
        @export()
        type myType = {
          @description('   ')
          name: string
        }
        """)]
    [DataTestMethod]
    public void Empty_and_whitespace_descriptions_are_reported(string text)
    {
        AssertDiagnostics(text);
    }

    [TestMethod]
    public void Nested_object_type_properties_are_reported()
    {
        AssertDiagnostics(
            """
            @export()
            type myType = {
              @description('Outer property description.')
              outer: {
                inner: string
              }
            }
            """,
            ["""[5] Type property "inner" must have a non-empty description."""]);
    }

    [TestMethod]
    public void Quoted_property_names_are_reported()
    {
        AssertDiagnostics(
            """
            @export()
            type myType = {
              'my-property': string
            }
            """,
            ["""[3] Type property "my-property" must have a non-empty description."""]);
    }

    [TestMethod]
    public void Description_on_the_type_itself_does_not_satisfy_the_rule()
    {
        AssertDiagnostics(
            """
            @export()
            @description('Type description.')
            type myType = {
              name: string
            }
            """,
            ["""[4] Type property "name" must have a non-empty description."""]);
    }

    [TestMethod]
    public void Additional_properties_without_descriptions_are_reported()
    {
        AssertDiagnostics(
            """
            @export()
            type myType = {
              *: string
            }
            """,
            ["""[3] Type property "*" must have a non-empty description."""]);
    }

    [TestMethod]
    public void Additional_properties_with_descriptions_are_accepted()
    {
        AssertNoDiagnostics("""
            @export()
            type myType = {
              @description('Any additional value.')
              *: string
            }
            """);
    }

    [TestMethod]
    public void Additional_properties_with_empty_descriptions_are_reported()
    {
        AssertDiagnostics("""
            @export()
            type myType = {
              @description('  ')
              *: string
            }
            """);
    }

    [TestMethod]
    public void Properties_of_object_types_in_a_discriminated_union_are_reported()
    {
        AssertDiagnostics(
            """
            @export()
            @description('Foo config.')
            type fooConfig = {
              @description('Discriminator value.')
              type: 'foo'
            }

            @export()
            @discriminator('type')
            type serviceConfig = fooConfig | {
              type: 'baz'
              *: string
            }
            """,
            [
                """[11] Type property "type" must have a non-empty description.""",
                """[12] Type property "*" must have a non-empty description.""",
            ]);
    }

    [TestMethod]
    public void Object_type_properties_outside_type_declarations_are_ignored()
    {
        AssertNoDiagnostics("""
            param input {
              name: string
            } = {
              name: 'value'
            }

            output result object = input
            """);
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.CodeAction;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class NoHardcodedOutputsRuleTests : LinterRuleTestsBase
{
    private static readonly Options RuleOptions = new(
        ConfigurationPatch: EnableRule);

    private static RootConfiguration EnableRule(RootConfiguration configuration) =>
        configuration.WithAnalyzersConfiguration(
            configuration.Analyzers.SetValue($"core.rules.{NoHardcodedOutputsRule.Code}.level", "warning"));

    [TestMethod]
    public void Hardcoded_string_output_is_reported_with_codefix_inserted_after_last_variable()
    {
        AssertRuleCodeFix(
            NoHardcodedOutputsRule.Code,
            "Create exported variable 'apiVersion' and remove output 'apiVersion'",
            """
            var existing = 'value'

            output apiVersion string = |'2024-01-01'
            """,
            """
            var existing = 'value'
            @export()
            var apiVersion = '2024-01-01'
            """);
    }

    [TestMethod]
    public void Hardcoded_output_codefix_inserts_after_parameters_when_file_has_no_variables()
    {
        AssertRuleCodeFix(
            NoHardcodedOutputsRule.Code,
            "Create exported variable 'mode' and remove output 'mode'",
            """
            param name string

            output mode string = |'prod'
            """,
            """
            param name string

            @export()
            var mode = 'prod'
            """);
    }

    [TestMethod]
    public void Hardcoded_output_codefix_inserts_after_parameters_and_before_resources_when_file_has_no_variables()
    {
        AssertRuleCodeFix(
            NoHardcodedOutputsRule.Code,
            "Create exported variable 'mode' and remove output 'mode'",
            """
            param name string

            resource storageAccount 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: name
            }

            output mode string = |'prod'
            """,
            """
            param name string

            @export()
            var mode = 'prod'

            resource storageAccount 'Microsoft.Storage/storageAccounts@2025-01-01' = {
              name: name
            }
            """);
    }

    [TestMethod]
    public void Hardcoded_output_codefix_uses_unused_variable_name()
    {
        AssertRuleCodeFix(
            NoHardcodedOutputsRule.Code,
            "Create exported variable 'mode2' and remove output 'mode'",
            """
            var mode = 'dev'

            output mode string = |'prod'
            """,
            """
            var mode = 'dev'
            @export()
            var mode2 = 'prod'
            """);
    }

    [TestMethod]
    public void Hardcoded_literal_outputs_are_reported()
    {
        var text = """
            param name string

            output stringValue string = 'literal'
            output intValue int = 42
            output boolValue bool = true
            output arrayValue array = [
              'one'
              2
            ]
            output objectValue object = {
              first: 'one'
              nested: [
                false
              ]
            }
            output interpolated string = 'literal-${name}'
            output expression string = toLower('LITERAL')
            output reference string = name
            """;

        AssertLinterRuleDiagnostics(
            NoHardcodedOutputsRule.Code,
            text,
            [
                "[3] Output 'stringValue' uses a hard-coded value. Use an exported variable for constants that need to be imported by other files.",
                "[4] Output 'intValue' uses a hard-coded value. Use an exported variable for constants that need to be imported by other files.",
                "[5] Output 'boolValue' uses a hard-coded value. Use an exported variable for constants that need to be imported by other files.",
                "[6] Output 'arrayValue' uses a hard-coded value. Use an exported variable for constants that need to be imported by other files.",
                "[10] Output 'objectValue' uses a hard-coded value. Use an exported variable for constants that need to be imported by other files.",
            ],
            RuleOptions);
    }

    [TestMethod]
    public void Expression_based_outputs_are_not_reported()
    {
        var text = """
            param name string

            output reference string = name
            output interpolated string = 'literal-${name}'
            output functionResult string = toLower('LITERAL')
            output arrayWithExpression array = [
              'literal'
              name
            ]
            output objectWithExpression object = {
              literal: 'literal'
              reference: name
            }
            """;

        AssertLinterRuleDiagnostics(NoHardcodedOutputsRule.Code, text, 0, RuleOptions);
    }

    private static void AssertRuleCodeFix(string expectedCode, string expectedFixTitle, string inputFile, string resultFile)
    {
        var (file, cursor) = ParserHelper.GetFileWithSingleCursor(inputFile, '|');
        var result = CompilationHelper.Compile(new ServiceBuilder().WithConfigurationPatch(EnableRule), ("main.bicep", file));

        using (new AssertionScope().WithVisualCursor(result.Compilation.GetEntrypointSemanticModel().SourceFile, cursor))
        {
            var matchingDiagnostics = result.Diagnostics
                .Where(x => x.Source == DiagnosticSource.CoreLinter)
                .Where(x => x.Span.IsOverlapping(cursor));

            matchingDiagnostics.Should().ContainSingle(x => x.Code == expectedCode);
            var diagnostic = matchingDiagnostics.Single(x => x.Code == expectedCode);

            diagnostic.Fixes.Should().ContainSingle(x => x.Title == expectedFixTitle);
            var fix = diagnostic.Fixes.Single(x => x.Title == expectedFixTitle);
            fix.Kind.Should().Be(CodeFixKind.QuickFix);

            ApplyCodeFix(file, fix).Should().EqualIgnoringNewlines(resultFile);
        }
    }

    private static string ApplyCodeFix(string file, CodeFix fix)
    {
        var result = file;

        foreach (var replacement in fix.Replacements.OrderByDescending(replacement => replacement.Span.Position))
        {
            result = result.Remove(replacement.Span.Position, replacement.Span.Length);
            result = result.Insert(replacement.Span.Position, replacement.Text);
        }

        return result;
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Scenarios;

[TestClass]
public class NullabilityTests
{
    [DataTestMethod]
    [DynamicData(nameof(GetTemplatesWithSingleUnexpectedlyNullableValue), DynamicDataSourceType.Method)]
    public void Unexpectedly_nullable_types_raise_fixable_warning(string templateWithNullablyTypedValue, string templateWithNonNullAssertion, TypeSymbol expectedType, TypeSymbol actualType)
    {
        var result = CompilationHelper.Compile(templateWithNullablyTypedValue);
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, $@"Expected a value of type ""{expectedType}"" but the provided value is of type ""{actualType}""."),
        });
        result.Template.Should().NotBeNull();

        result.ExcludingLinterDiagnostics().Diagnostics.Single().Should().BeAssignableTo<IFixable>();
        result.ExcludingLinterDiagnostics().Diagnostics.Single().As<IFixable>().Fixes.Single().Should().HaveResult(templateWithNullablyTypedValue, templateWithNonNullAssertion);

        CompilationHelper.Compile(templateWithNonNullAssertion).ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    private static IEnumerable<object[]> GetTemplatesWithSingleUnexpectedlyNullableValue()
    {
        static object[] Case(string templateWithNullablyTypedValue, string templateWithNonNullAssertion, TypeSymbol expectedType, TypeSymbol actualType) =>
        [
            templateWithNullablyTypedValue,
            templateWithNonNullAssertion,
            expectedType,
            actualType,
        ];

        return new[]
        {
            // nullably typed property assignment
            Case(
@"
param input string

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: last(filter(split(input, '/'), x => true))
}
",
@"
param input string

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: last(filter(split(input, '/'), x => true))!
}
",
                LanguageConstants.String,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String)
            ),

            // nullably typed function argument
            Case(
@"
param input string

output out string = uniqueString(last(filter(split(input, '/'), x => true)))
",
@"
param input string

output out string = uniqueString(last(filter(split(input, '/'), x => true))!)
",
                LanguageConstants.String,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String)
            ),

            // nullably typed output value
            Case(
@"
param csv string

output firstPrefix string = first(filter(split(csv, ','), x => true))
",
@"
param csv string

output firstPrefix string = first(filter(split(csv, ','), x => true))!
",
                LanguageConstants.String,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String)
            ),

            // nullably typed ternary condition
            Case(
@"
param input array

var nullableAtIndex = [for i in input: i != null]

output eitherFooOrBar string = first(nullableAtIndex) ? 'foo' : 'bar'
",
@"
param input array

var nullableAtIndex = [for i in input: i != null]

output eitherFooOrBar string = first(nullableAtIndex) ! ? 'foo' : 'bar'
",
                LanguageConstants.Bool,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.Bool)
            ),

            // nullably typed if condition
            Case(
@"
param input array

var nullableAtIndex = [for i in input: i != null]

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' existing = if (first(nullableAtIndex)) {
  name: 'storageacct'
}
",
@"
param input array

var nullableAtIndex = [for i in input: i != null]

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' existing = if (first(nullableAtIndex)!) {
  name: 'storageacct'
}
",
                LanguageConstants.Bool,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.Bool)
            ),

            // nullably typed array indices
            Case(
@"
param input array

output something string = input[first(filter(range(0, length(input)), x => true))]
",
@"
param input array

output something string = input[first(filter(range(0, length(input)), x => true))!]
",
                LanguageConstants.Int,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.Int)
            ),

            // nullably typed object indices
            Case(
@"
param input object
param csv string

output something string = input[first(filter(split(csv, ','), x => true))]
",
@"
param input object
param csv string

output something string = input[first(filter(split(csv, ','), x => true))!]
",
                LanguageConstants.String,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String)
            ),

            // nullably typed list comprehension target
            Case(
@"
param input array

var arrayOfArrays = [for i in input: range(0, 10)]
var arrayOfOneBuzz = [for i in first(arrayOfArrays): 'buzz']
",
@"
param input array

var arrayOfArrays = [for i in input: range(0, 10)]
var arrayOfOneBuzz = [for i in first(arrayOfArrays)!: 'buzz']
",
                LanguageConstants.Array,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, new TypedArrayType(TypeFactory.CreateIntegerType(0, 10), default))
            ),

            // nullably typed parameter defaults
            Case(
@"
param csv string
param anotherParam string = first(filter(split(csv, ','), x => true))
",
@"
param csv string
param anotherParam string = first(filter(split(csv, ','), x => true))!
",
                LanguageConstants.String,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String)
            ),
        };
    }

    [TestMethod]
    public void Function_overload_with_multiple_nullability_violations_still_finds_match_and_raises_no_errors()
    {
        var result = CompilationHelper.Compile(@"
param input string[]

output out array = split(first(input), last(input))
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""string"" but the provided value is of type ""null | string""."),
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""array | string"" but the provided value is of type ""null | string""."),
        });
    }

    [TestMethod]
    public void Function_overload_mismatch_even_with_nullability_tweaks_raises_no_nullability_warnings()
    {
        var result = CompilationHelper.Compile(@"
param input string[]

output out array = split(first(input), 21)
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP070", DiagnosticLevel.Error, @"Argument of type ""21"" is not assignable to parameter of type ""array | string""."),
        });
    }

    // https://github.com/Azure/bicep/issues/17284
    [TestMethod]
    public void Issue_17284_nullable_operand_in_binary_operator_raises_fixable_warning_instead_of_error()
    {
        var result = CompilationHelper.Compile(@"
func hello(input int?) string? => input == null ? null : input > 60 ? 'Hello world ${input}' : 'Value is lower than 60'
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""input"" is of type ""int | null""."),
        });

        // ensure the emitted ARM expression is what the user actually intended (same as writing `input!` explicitly)
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['hello'].output.value",
            "[if(equals(parameters('input'), null()), null(), if(greater(parameters('input'), 60), format('Hello world {0}', parameters('input')), 'Value is lower than 60'))]");
    }

    [TestMethod]
    public void Nullable_operand_on_left_of_binary_comparison_raises_fixable_warning()
    {
        var templateWithNullable = @"
func compareLeft(input int?) bool => input == null ? false : input > 60
";
        var templateWithNonNullAssertion = @"
func compareLeft(input int?) bool => input == null ? false : input ! > 60
";

        var result = CompilationHelper.Compile(templateWithNullable);

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""input"" is of type ""int | null""."),
        });

        var warning = result.ExcludingLinterDiagnostics().Diagnostics.Single();
        warning.Should().BeAssignableTo<IFixable>();
        warning.As<IFixable>().Fixes.Single().Should().HaveResult(templateWithNullable, templateWithNonNullAssertion);

        // roundtrip: applying the fix yields a template that compiles without any diagnostics
        CompilationHelper.Compile(templateWithNonNullAssertion).ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        // the emitted expression preserves the null guard around the greater-than comparison
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['compareLeft'].output.value",
            "[if(equals(parameters('input'), null()), false(), greater(parameters('input'), 60))]");
    }

    [TestMethod]
    public void Nullable_operand_on_right_of_binary_comparison_raises_fixable_warning()
    {
        var templateWithNullable = @"
func compareRight(input int?) bool => input == null ? false : 60 > input
";
        var templateWithNonNullAssertion = @"
func compareRight(input int?) bool => input == null ? false : 60 > input!
";

        var result = CompilationHelper.Compile(templateWithNullable);

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""input"" is of type ""int | null""."),
        });

        var warning = result.ExcludingLinterDiagnostics().Diagnostics.Single();
        warning.Should().BeAssignableTo<IFixable>();
        warning.As<IFixable>().Fixes.Single().Should().HaveResult(templateWithNullable, templateWithNonNullAssertion);

        // roundtrip: applying the fix yields a template that compiles without any diagnostics
        CompilationHelper.Compile(templateWithNonNullAssertion).ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        // the emitted expression preserves the null guard around the greater-than comparison
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['compareRight'].output.value",
            "[if(equals(parameters('input'), null()), false(), greater(60, parameters('input')))]");
    }

    [TestMethod]
    public void Both_nullable_operands_in_binary_comparison_raise_two_fixable_warnings()
    {
        var result = CompilationHelper.Compile(@"
func compareBoth(cond bool, a int?, b int?) bool => cond ? a > b : false
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""a"" is of type ""int | null""."),
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""b"" is of type ""int | null""."),
        });

        // both operands still surface as parameters in the emitted expression, gated by the ternary condition
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['compareBoth'].output.value",
            "[if(parameters('cond'), greater(parameters('a'), parameters('b')), false())]");
    }

    [TestMethod]
    public void Nullable_operand_in_arithmetic_operator_raises_fixable_warning()
    {
        var result = CompilationHelper.Compile(@"
func addOne(input int?) int => input == null ? 0 : input + 1
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""input"" is of type ""int | null""."),
        });

        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['addOne'].output.value",
            "[if(equals(parameters('input'), null()), 0, add(parameters('input'), 1))]");
    }

    [TestMethod]
    public void Nullable_operand_in_binary_operator_with_non_null_assertion_raises_no_warning()
    {
        var result = CompilationHelper.Compile(@"
func compareLeft(input int?) bool => input! > 60
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Pure_null_operand_in_binary_operator_still_raises_error()
    {
        // pure `null` (not `T | null`) cannot be stripped by TryRemoveNullability, so BCP045 must still fire.
        var result = CompilationHelper.Compile(@"
var invalid = null + 's'
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP045", DiagnosticLevel.Error, @"Cannot apply operator ""+"" to operands of type ""null"" and ""'s'""."),
        });
    }

    [TestMethod]
    public void Nullable_operand_in_unary_not_operator_raises_fixable_warning()
    {
        var templateWithNullable = @"
func negate(input bool?) bool => input == null ? false : !input
";
        var templateWithNonNullAssertion = @"
func negate(input bool?) bool => input == null ? false : !input!
";

        var result = CompilationHelper.Compile(templateWithNullable);

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""bool"" but the provided value ""input"" is of type ""bool | null""."),
        });

        var warning = result.ExcludingLinterDiagnostics().Diagnostics.Single();
        warning.Should().BeAssignableTo<IFixable>();
        warning.As<IFixable>().Fixes.Single().Should().HaveResult(templateWithNullable, templateWithNonNullAssertion);

        // roundtrip: applying the fix yields a template that compiles without any diagnostics
        CompilationHelper.Compile(templateWithNonNullAssertion).ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        // the emitted expression preserves the null guard around the negation
        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['negate'].output.value",
            "[if(equals(parameters('input'), null()), false(), not(parameters('input')))]");
    }

    [TestMethod]
    public void Nullable_operand_in_unary_minus_operator_raises_fixable_warning()
    {
        var result = CompilationHelper.Compile(@"
func negate(input int?) int => input == null ? 0 : -input
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""input"" is of type ""int | null""."),
        });

        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['negate'].output.value",
            "[if(equals(parameters('input'), null()), 0, sub(0, parameters('input')))]");
    }

    [TestMethod]
    public void Nullable_operand_in_unary_operator_with_non_null_assertion_raises_no_warning()
    {
        var result = CompilationHelper.Compile(@"
func negate(input bool?) bool => !input!
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Pure_null_operand_in_unary_operator_still_raises_error()
    {
        // pure `null` (not `T | null`) cannot be stripped by TryRemoveNullability, so BCP044 must still fire.
        var result = CompilationHelper.Compile(@"
var invalid = !null
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP044", DiagnosticLevel.Error, @"Cannot apply operator ""!"" to operand of type ""null""."),
        });
    }

    [TestMethod]
    public void Coalesce_operator_on_nullable_left_does_not_raise_nullability_warning()
    {
        // the `??` operator's signature is (Any, Any) and it handles nullability internally, so the exploratory
        // strip-and-refold branch must never fire for it (no BCP321 false positives).
        var result = CompilationHelper.Compile(@"
func withDefault(input int?) int => input ?? 0
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['withDefault'].output.value",
            "[coalesce(parameters('input'), 0)]");
    }

    [TestMethod]
    public void Chained_nullable_arithmetic_raises_warnings_only_on_the_nullable_leaves()
    {
        // inner `a + b`: two nullable operands -> two BCP321s, folds to a non-nullable int type.
        // outer `<inner> + 1`: inner result is already non-nullable, so no extra warning.
        // this verifies the folded return type propagates as non-nullable up the expression tree.
        var result = CompilationHelper.Compile(@"
func sum(cond bool, a int?, b int?) int => cond ? a + b + 1 : 0
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""a"" is of type ""int | null""."),
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""b"" is of type ""int | null""."),
        });

        result.Template.Should().HaveValueAtPath(
            "$.functions[0].members['sum'].output.value",
            "[if(parameters('cond'), add(add(parameters('a'), parameters('b')), 1), 0)]");
    }

    [TestMethod]
    public void Multi_line_nullable_operand_source_text_is_rendered_on_a_single_line_in_the_diagnostic_message()
    {
        // The offending operand spans multiple lines in the source. The BCP321 message must render on a single line
        // (newlines replaced with spaces) so it stays readable in CLI/IDE output — inter-token whitespace is preserved
        // as-is, we just guarantee the message never contains a raw newline.
        var result = CompilationHelper.Compile(@"
func firstOrZero(input int[]) int => length(input) == 0 ? 0 : first(filter(
  input,
  x => x != 0
)) + 1
");

        result.Template.Should().NotBeNull();
        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP321", DiagnosticLevel.Warning, @"Expected a value of type ""int"" but the provided value ""first(filter(   input,   x => x != 0 ))"" is of type ""int | null""."),
        });
    }

    [TestMethod]
    public void Nullable_binary_operand_outside_ternary_branch_still_raises_BCP045_error()
    {
        // The strip-and-refold error->warning downgrade is intentionally gated on the operator being inside a
        // ternary branch (rough proxy for "user has a guard"). Outside a ternary, the pre-PR BCP045 error still fires
        // so the compiler doesn't silently succeed against a possibly-null runtime value.
        var result = CompilationHelper.Compile(@"
func addOne(input int?) int => input + 1
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP045", DiagnosticLevel.Error, @"Cannot apply operator ""+"" to operands of type ""int | null"" and ""1""."),
        });
    }

    [TestMethod]
    public void Nullable_unary_operand_outside_ternary_branch_still_raises_BCP044_error()
    {
        // Same gating applies to unary operators: outside a ternary branch, the pre-PR BCP044 error still fires.
        var result = CompilationHelper.Compile(@"
func negate(input bool?) bool => !input
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP044", DiagnosticLevel.Error, @"Cannot apply operator ""!"" to operand of type ""bool | null""."),
        });
    }
}

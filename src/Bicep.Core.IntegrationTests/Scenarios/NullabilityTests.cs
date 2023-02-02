// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Scenarios;

[TestClass]
public class NullabilityTests
{
    private static ServiceBuilder Services => new ServiceBuilder();

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
        static object[] Case(string templateWithNullablyTypedValue, string templateWithNonNullAssertion, TypeSymbol expectedType, TypeSymbol actualType) => new object[]
        {
            templateWithNullablyTypedValue,
            templateWithNonNullAssertion,
            expectedType,
            actualType,
        };

        return new[]
        {
            // nullably typed property assignment
            Case(
@"
param input string

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: last(split(input, '/'))
}
",
@"
param input string

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: last(split(input, '/'))!
}
",
                LanguageConstants.String,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String)
            ),

            // nullably typed function argument
            Case(
@"
param input string

output out string = uniqueString(last(split(input, '/')))
",
@"
param input string

output out string = uniqueString(last(split(input, '/'))!)
",
                LanguageConstants.String,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String)
            ),

            // nullably typed output value
            Case(
@"
param csv string

output firstPrefix string = first(split(csv, ','))
",
@"
param csv string

output firstPrefix string = first(split(csv, ','))!
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
  name: 'sa'
}
",
@"
param input array

var nullableAtIndex = [for i in input: i != null]

resource sa 'Microsoft.Storage/storageAccounts@2022-09-01' existing = if (first(nullableAtIndex)!) {
  name: 'sa'
}
",
                LanguageConstants.Bool,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.Bool)
            ),

            // nullably typed array indices
            Case(
@"
param input array

output something string = input[first(range(0, length(input)))]
",
@"
param input array

output something string = input[first(range(0, length(input)))!]
",
                LanguageConstants.Int,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.Int)
            ),

            // nullably typed object indices
            Case(
@"
param input object
param csv string

output something string = input[first(split(csv, ','))]
",
@"
param input object
param csv string

output something string = input[first(split(csv, ','))!]
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
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, new TypedArrayType(LanguageConstants.Int, default))
            ),

            // nullably typed parameter defaults
            Case(
@"
param csv string
param anotherParam string = first(split(csv, ','))
",
@"
param csv string
param anotherParam string = first(split(csv, ','))!
",
                LanguageConstants.String,
                TypeHelper.CreateTypeUnion(LanguageConstants.Null, LanguageConstants.String)
            ),
        };
    }

    [TestMethod]
    public void Function_overload_with_multiple_nullability_violations_still_finds_match_and_raises_no_errors()
    {
        var result = CompilationHelper.Compile(Services.WithFeatureOverrides(new(UserDefinedTypesEnabled: true)), @"
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
        var result = CompilationHelper.Compile(Services.WithFeatureOverrides(new(UserDefinedTypesEnabled: true)), @"
param input string[]

output out array = split(first(input), 21)
");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
        {
            ("BCP070", DiagnosticLevel.Error, @"Argument of type ""21"" is not assignable to parameter of type ""array | string""."),
        });
    }
}
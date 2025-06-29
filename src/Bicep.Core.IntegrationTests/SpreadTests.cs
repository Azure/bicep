// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class SpreadTests
{
    [TestMethod]
    public void Spread_operator_results_in_correct_codegen()
    {
        var result = CompilationHelper.Compile("""
var other = {
  bar: [1, ...[2, 3], 4]
}

output test object = {
  foo: 'foo'
  ...other
  baz: 'baz'  
}
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath("$.variables['other']['bar']", "[flatten(createArray(createArray(1), createArray(2, 3), createArray(4)))]");
        result.Template.Should().HaveValueAtPath("$.outputs['test'].value", "[shallowMerge(createArray(createObject('foo', 'foo'), variables('other'), createObject('baz', 'baz')))]");

        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();
        evaluated.Should().HaveJsonAtPath("$.outputs['test'].value", """
{
  "foo": "foo",
  "bar": [
    1,
    2,
    3,
    4
  ],
  "baz": "baz"
}
""");
    }

    [TestMethod]
    public void Spread_operator_works_on_single_line()
    {
        var result = CompilationHelper.Compile("""
param other object

var test = { foo: 'foo', ...other, baz: 'baz' }
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Spread_types_are_calculated_correctly_for_objects()
    {
        // positive case
        var result = CompilationHelper.Compile("""
param foo { foo: string }
param bar { bar: string }

output baz { foo: string, bar: string } = { ...foo, ...bar }
output qux { foo: string, bar: string } = { foo: 'foo', ...bar }
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        // negative case
        result = CompilationHelper.Compile("""
param foo { foo: string }
param bar { bar: string }

output baz { foo: string, baz: string } = { ...foo, ...bar }
output qux { foo: string, baz: string } = { foo: 'foo', ...bar }
""");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP089", Diagnostics.DiagnosticLevel.Warning, """The property "bar" is not allowed on objects of type "{ foo: string, baz: string }". Did you mean "baz"?"""),
            ("BCP089", Diagnostics.DiagnosticLevel.Warning, """The property "bar" is not allowed on objects of type "{ foo: string, baz: string }". Did you mean "baz"?"""),
        ]);
    }

    [TestMethod]
    public void Spread_types_are_calculated_correctly_for_arrays()
    {
        // positive case
        var result = CompilationHelper.Compile("""
param foo string[]
param bar string[]

output baz string[] = [ ...foo, ...bar ]
output qux string[] = [ 'foo', ...bar ]
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        // negative case
        result = CompilationHelper.Compile("""
param foo string[]
param bar string[]

output baz int[] = [ ...foo, ...bar ]
output qux int[] = [ 123, ...bar ]
""");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP403", Diagnostics.DiagnosticLevel.Error, """The enclosing array expects elements of type "int", but the array being spread contains elements of incompatible type "string"."""),
            ("BCP403", Diagnostics.DiagnosticLevel.Error, """The enclosing array expects elements of type "int", but the array being spread contains elements of incompatible type "string"."""),
            ("BCP403", Diagnostics.DiagnosticLevel.Error, """The enclosing array expects elements of type "int", but the array being spread contains elements of incompatible type "string"."""),
        ]);
    }

    [TestMethod]
    public void Array_spread_cannot_be_used_inside_object()
    {
        var result = CompilationHelper.Compile("""
var other = ['bar']

var test = {
  foo: 'foo'
  ...other
  baz: 'baz'  
}
""");

        result.ExcludingLinterDiagnostics().Should().ContainDiagnostic(
            "BCP402", Diagnostics.DiagnosticLevel.Error, """The spread operator "..." can only be used in this context for an expression assignable to type "object".""");
    }

    [TestMethod]
    public void Object_spread_cannot_be_used_inside_array()
    {
        var result = CompilationHelper.Compile("""
var other = {
  bar: 'bar'
}

var test = [
  'foo'
  ...other
  'baz'  
]
""");

        result.ExcludingLinterDiagnostics().Should().ContainDiagnostic(
            "BCP402", Diagnostics.DiagnosticLevel.Error, """The spread operator "..." can only be used in this context for an expression assignable to type "array".""");
    }

    [TestMethod]
    public void Spread_works_with_any()
    {
        var result = CompilationHelper.Compile("""
var badObj = {
  ...any(['foo'])
}

var badArray = [
  ...any({ foo: 'foo' })
]
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        // this will result in a runtime failure, but at least the codegen is correct.
        result.Template.Should().HaveValueAtPath("$.variables['badObj']", "[shallowMerge(createArray(createArray('foo')))]");
        result.Template.Should().HaveValueAtPath("$.variables['badArray']", "[flatten(createArray(createObject('foo', 'foo')))]");
    }

    [TestMethod]
    public void Spread_is_blocked_in_resource_body()
    {
        var result = CompilationHelper.Compile("""
var other = { location: 'westus' }

resource foo 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: 'foo'
  ...other
}
""");

        result.ExcludingLinterDiagnostics().Should().ContainDiagnostic(
            "BCP401", Diagnostics.DiagnosticLevel.Error, """The spread operator "..." is not permitted in this location.""");
    }

    [TestMethod]
    public void Spread_is_blocked_adjacent_to_for_loop()
    {
        // https://github.com/Azure/bicep/issues/16660
        var result = CompilationHelper.Compile("""
var test1 = true
var array1 =[]

resource ex 'Microsoft.Network/expressRouteCircuits@2024-05-01' = {
  name: 'test'
  properties: {
    ...test1 ? {
      gatewayManagerEtag: '1'
    } : {}
    authorizations: [for item in array1: {
      name: item
    }]
  }
}
""");

        result.ExcludingLinterDiagnostics().Should().ContainDiagnostic(
            "BCP417", Diagnostics.DiagnosticLevel.Error, """The spread operator "..." cannot be used inside objects with property for-expressions.""");
    }

    [TestMethod]
    public void Object_spread_edge_cases()
    {
        var result = CompilationHelper.Compile("""
output test1 object = {
  a: 0
  ...{ a: 1, b: 0 }
  c: 0
}

output test2 object = {
  'ABC': 0
  ...{ 'aBC': 1 }
}

output test3 object = {
  foo: 'bar'
  ...{ foo: null }
}
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

        var evaluated = TemplateEvaluator.Evaluate(result.Template).ToJToken();
        evaluated.Should().HaveJsonAtPath("$.outputs['test1'].value", """
{
  "a": 1,
  "b": 0,
  "c": 0
}
""");
        evaluated.Should().HaveJsonAtPath("$.outputs['test2'].value", """
{
  "ABC": 1
}
""");
        evaluated.Should().HaveJsonAtPath("$.outputs['test3'].value", """
{
  "foo": null
}
""");
    }
}

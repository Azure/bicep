// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class TypedVariableTests : TestBase
{
    private ServiceBuilder Services => new();

    [TestMethod]
    public void Simple_type_declarations_are_accepted()
    {
        var result = CompilationHelper.Compile(Services,
            ("main.bicep", """
var foo string = 'foo'
""")
        );

        result.ExcludingLinterDiagnostics().Diagnostics.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Complex_type_declarations_are_accepted()
    {
        var result = CompilationHelper.Compile(Services,
            ("main.bicep", """
var foo {
 abc: string
 def: int
 obj: {
   prop1: string
   prop2: int
 }
} = {
  abc: 'abc'
  def: 123
  obj: {
    prop1: 'prop1'
    prop2: 456
  }
}
""")
        );

        result.ExcludingLinterDiagnostics().Diagnostics.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Type_validation_runs_as_expected()
    {
        var result = CompilationHelper.Compile(Services,
            ("main.bicep", """
var foo object = 'invalidString'
""")
        );

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP033", DiagnosticLevel.Error, """Expected a value of type "object" but the provided value is of type "'invalidString'"."""),
        ]);
    }

    [TestMethod]
    public void Invalid_type_reference_raises_an_error()
    {
        var result = CompilationHelper.Compile(Services,
            ("main.bicep", """
var foo invalid = 'foo'
""")
        );

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP302", DiagnosticLevel.Error, """The name "invalid" is not a valid type. Please specify one of the following types: "any", "array", "bool", "int", "object", "resourceInput", "resourceOutput", "string"."""),
        ]);
    }

    [TestMethod]
    public void Type_information_is_persisted_across_bicep_modules()
    {
        var result = CompilationHelper.Compile(Services,
            ("mod.bicep", """
@export()
var exported {
  foo: string
} = any({})
"""),
            ("main.bicep", """
import { exported } from './mod.bicep'

output test {
  foo: int
} = exported
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP036", DiagnosticLevel.Error, """The property "foo" expected a value of type "int" but the provided value is of type "string"."""),
        ]);
    }

    [TestMethod]
    public void Type_information_is_persisted_across_json_modules()
    {
        var compiledModule = CompilationHelper.Compile(Services, """
@export()
var exported {
  foo: string
} = any({})
""");
        compiledModule.Template.Should().NotBeNull();

        var result = CompilationHelper.Compile(Services,
            ("main.bicep", """
import { exported } from './mod.json'

output test {
  foo: int
} = exported
"""),
            ("mod.json", compiledModule.Template!.ToString()));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP035", DiagnosticLevel.Error, """The specified "output" declaration is missing the following required properties: "foo"."""),
        ]);
    }

    [TestMethod]
    public void Typed_variables_can_be_used_in_bicepparam_files_with_imported_types()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("main.bicep", ""),
            ("types.bicep", """
@export()
type FooType = {
  foo: string
}
"""),
            ("parameters.bicepparam", """
import { FooType } from './types.bicep'

using './main.bicep'

var foo FooType = {
  foo: 123
}
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP036", DiagnosticLevel.Error, """The property "foo" expected a value of type "string" but the provided value is of type "123"."""),
        ]);
    }

    [TestMethod]
    public void Typed_variables_can_be_used_in_bicepparam_files_with_inline_types()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("main.bicep", ""),
            ("parameters.bicepparam", """
using './main.bicep'

var foo { foo: string } = {
  foo: 123
}
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP036", DiagnosticLevel.Error, """The property "foo" expected a value of type "string" but the provided value is of type "123"."""),
        ]);
    }

    [TestMethod]
    public void Typed_variables_can_be_used_in_bicepparam_files_with_type_declarations()
    {
        var result = CompilationHelper.CompileParams(Services,
            ("main.bicep", ""),
            ("parameters.bicepparam", """
using './main.bicep'

type FooType = {
  foo: string
}

var foo FooType = {
  foo: 123
}
"""));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP036", DiagnosticLevel.Error, """The property "foo" expected a value of type "string" but the provided value is of type "123"."""),
        ]);
    }
}

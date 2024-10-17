// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class DeprecationTests
{
    [NotNull] public TestContext? TestContext { get; set; }

    [TestMethod]
    public void Deprecation_is_represented_in_metadata()
    {
        var result = CompilationHelper.Compile("""
@deprecated('deprecated param')
param fooParam string?

@export()
@deprecated('deprecated var')
var fooVar = ''

@export()
@deprecated('deprecated func')
func fooFunc() string => ':('

@export()
@deprecated('deprecated type')
type fooType = {
  bar: string
}

@deprecated('deprecated output')
output fooOutput string = ':('
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        result.Template.Should().HaveValueAtPath($"$.parameters.fooParam.metadata.__bicep_deprecated!", "deprecated param");
        result.Template.Should().HaveValueAtPath($"$.metadata.__bicep_exported_variables![?(@.name == 'fooVar')].metadata.__bicep_deprecated!", "deprecated var");
        result.Template.Should().HaveValueAtPath($"$.functions[0].members.fooFunc.metadata.__bicep_deprecated!", "deprecated func");
        result.Template.Should().HaveValueAtPath($"$.definitions.fooType.metadata.__bicep_deprecated!", "deprecated type");
        result.Template.Should().HaveValueAtPath($"$.outputs.fooOutput.metadata.__bicep_deprecated!", "deprecated output");
    }

    [TestMethod]
    public void Type_properties_cannot_be_deprecated()
    {
        // This is a limitation of the current implementation - something we should try and fix longer-term
        var result = CompilationHelper.Compile("""
@export()
type fooType = {
  @deprecated('deprecated type property')
  bar: string
}
""");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP410", DiagnosticLevel.Error, $"""Function "deprecated" cannot be used as a type property decorator."""),
        ]);
    }

    [TestMethod]
    public void Deprecating_unexported_types_vars_and_funcs_is_blocked()
    {
        var result = CompilationHelper.Compile("""
@deprecated('deprecated var')
var fooVar = ''

@deprecated('deprecated func')
func fooFunc() string => ':('

@deprecated('deprecated type')
type fooType = {
  bar: string
}
""");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP411", DiagnosticLevel.Error, $"""This declaration cannot be marked as deprecated, because it has not been exported."""),
            ("BCP411", DiagnosticLevel.Error, $"""This declaration cannot be marked as deprecated, because it has not been exported."""),
            ("BCP411", DiagnosticLevel.Error, $"""This declaration cannot be marked as deprecated, because it has not been exported."""),
        ]);
    }

    [TestMethod]
    public void Deprecating_required_params_is_blocked()
    {
        var result = CompilationHelper.Compile("""
@deprecated('deprecated required')
param required string
""");

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
            ("BCP412", DiagnosticLevel.Error, $"""Parameters must either be nullable or have a default value to be marked as deprecated."""),
        ]);
    }

    [TestMethod]
    public void Deprecation_reason_is_optional()
    {
        var result = CompilationHelper.Compile("""
@deprecated()
param optional string = ''
""");

        result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Importing_deprecated_types_vars_and_funcs_is_flagged()
    {
        var result = CompilationHelper.Compile(("main.bicep", """
import { fooVar, fooFunc, fooType } from 'module.bicep'
"""), ("module.bicep", """
@export()
@deprecated('deprecated var')
var fooVar = ''

@export()
@deprecated('deprecated func')
func fooFunc() string => ':('

@export()
@deprecated('deprecated type')
type fooType = {
  bar: string
}
"""));

        result.Should().HaveDiagnostics([
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooVar' has been marked as deprecated, and should not be used. Reason: 'deprecated var'."""),
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooFunc' has been marked as deprecated, and should not be used. Reason: 'deprecated func'."""),
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooType' has been marked as deprecated, and should not be used. Reason: 'deprecated type'."""),
        ]);
    }

    [TestMethod]
    public void Importing_deprecated_types_vars_and_funcs_is_flagged_from_json()
    {
        var json = CompilationHelper.Compile("""
@export()
@deprecated('deprecated var')
var fooVar = ''

@export()
@deprecated('deprecated func')
func fooFunc() string => ':('

@export()
@deprecated('deprecated type')
type fooType = {
  bar: string
}
""").Template!.ToString();


        var result = CompilationHelper.Compile(("main.bicep", """
import { fooVar, fooFunc, fooType } from 'module.json'
"""), ("module.json", json));

        result.Should().HaveDiagnostics([
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooVar' has been marked as deprecated, and should not be used. Reason: 'deprecated var'."""),
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooFunc' has been marked as deprecated, and should not be used. Reason: 'deprecated func'."""),
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooType' has been marked as deprecated, and should not be used. Reason: 'deprecated type'."""),
        ]);
    }

    [TestMethod]
    public void Setting_params_and_accessing_outputs_is_flagged_if_deprecated()
    {
        var result = CompilationHelper.Compile(("main.bicep", """
module mod 'module.bicep' = {
  name: 'mod'
  params: {
    fooParam: ''
  }
}

var test = mod.outputs.fooOutput
"""), ("module.bicep", """
@deprecated('deprecated param')
param fooParam string?

@deprecated('deprecated output')
output fooOutput string = ':('
"""));

        result.ExcludingDiagnostics("no-unused-vars").Should().HaveDiagnostics([
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooParam' has been marked as deprecated, and should not be used. Reason: 'deprecated param'."""),
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooOutput' has been marked as deprecated, and should not be used. Reason: 'deprecated output'."""),
        ]);
    }

    [TestMethod]
    public void Setting_params_and_accessing_outputs_is_flagged_if_deprecated_from_json()
    {
        var json = CompilationHelper.Compile("""
@deprecated('deprecated param')
param fooParam string?

@deprecated('deprecated output')
output fooOutput string = ':('
""").Template!.ToString();

        var result = CompilationHelper.Compile(("main.bicep", """
module mod 'module.json' = {
  name: 'mod'
  params: {
    fooParam: ''
  }
}

var test = mod.outputs.fooOutput
"""), ("module.json", json));

        result.ExcludingDiagnostics("no-unused-vars").Should().HaveDiagnostics([
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooParam' has been marked as deprecated, and should not be used. Reason: 'deprecated param'."""),
            ("no-deprecated-dependencies", DiagnosticLevel.Warning, $"""Symbol 'fooOutput' has been marked as deprecated, and should not be used. Reason: 'deprecated output'."""),
        ]);
    }
}
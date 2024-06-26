// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests;

[TestClass]
public class ParameterFileTests
{
    [TestMethod]
    public void Parameters_file_cannot_reference_non_bicep_files()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using './foo.bicepparam'
"),
("foo.bicepparam", ""));

        result.Should().OnlyContainDiagnostic("BCP276", DiagnosticLevel.Error, "A using declaration can only reference a Bicep file.");
    }

    [TestMethod]
    public void Parameters_file_cannot_self_reference()
    {
        var result = CompilationHelper.CompileParams(("parameters.bicepparam", @"
using './parameters.bicepparam'
"));

        result.Should().OnlyContainDiagnostic("BCP278", DiagnosticLevel.Error, "This parameters file references itself, which is not allowed.");
    }

    [TestMethod]
    public void Parameters_file_cycles_should_be_detected()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using './one.bicepparam'
"),
("one.bicepparam", @"
using './two.bicepparam'
"),
("two.bicepparam", @"
using './one.bicepparam'
"));

        result.Diagnostics.Should().SatisfyRespectively(
            x =>
            {
                x.Code.Should().Be("BCP095");
                x.Level.Should().Be(DiagnosticLevel.Error);
                x.Message.Should().StartWith("The file is involved in a cycle (\"");
                x.Message.Should().EndWith("\").");
                x.Message.Should().ContainAll("one.bicepparam\" -> \"", "two.bicepparam\").");
            });
    }

    [TestMethod]
    public void Params_file_with_not_using_declaration_should_log_diagnostic()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
    param foo = 'bar'
    "),
("main.bicep", @"
    param foo string
    "));

        using (new AssertionScope())
        {
            result.Parameters.Should().BeNull();
            result.Diagnostics.Should().HaveDiagnostics(new[]
            {
                    ("BCP261", DiagnosticLevel.Error, "A using declaration must be present in this parameters file.")
                });
        }
    }

    [TestMethod]
    public void Parameters_file_cannot_reference_non_existing_env_variable()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using 'foo.bicep'
param fromEnv=readEnvironmentVariable('stringEnvVariable')
"),
("foo.bicep", @"param fromEnv string"));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]{
                ("BCP338", DiagnosticLevel.Error,
                "Failed to evaluate parameter \"fromEnv\": Environment variable \"stringEnvVariable\" does not exist, and no default value set.")});
    }

    [TestMethod]
    public void Parameters_file_cannot_reference_non_existing_env_variable_verbose()
    {
        var result = CompilationHelper.CompileParams(
("bicepconfig.json", """
{
  "analyzers": {
    "core": {
      "rules": {
      },
      "verbose": true
    }
  }
}
"""),
("parameters.bicepparam", @"
using 'foo.bicep'
param fromEnv=readEnvironmentVariable('stringEnvVariable')
"),
("foo.bicep", @"param fromEnv string"));

        result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]{
                ("BCP338", DiagnosticLevel.Error,
                "Failed to evaluate parameter \"fromEnv\": Environment variable \"stringEnvVariable\" does not exist, and no default value set."),
                ("Bicepparam ReadEnvironmentVariable function", DiagnosticLevel.Info,
                "Available environment variables are: "
                )});
    }
    [TestMethod]
    public void Parameters_file_can_use_variables()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using 'foo.bicep'

var abc = 'abc'
var abcWrapped = '>>${foo}<<'

param foo = abc
param bar = {
  abc: abc
  abcWrapped: abcWrapped
  abcUpper: toUpper(abc)
}

var list = 'FOO,BAR,BAZ'
param baz = join(map(range(0, 3), i => split(list, ',')[2 - i]), ',')
"),
("foo.bicep", @"
param foo string
param bar object
param baz string
"));

        result.Diagnostics.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateEvaluator.ParseParametersFile(result.Parameters);

        parameters["foo"].Should().DeepEqual("abc");
        parameters["bar"].Should().DeepEqual(new JObject
        {
            ["abc"] = "abc",
            ["abcWrapped"] = ">>abc<<",
            ["abcUpper"] = "ABC",
        });
        parameters["baz"].Should().DeepEqual("BAZ,BAR,FOO");
    }

    [TestMethod]
    public void Parameters_file_compilation_is_blocked_on_variable_errors()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using 'foo.bicep'

var abc = utcNow()
param foo = abc
"),
("foo.bicep", @"
param foo string
"));

        result.Should().NotGenerateParameters();
        result.Should().HaveDiagnostics(new[] {
            ("BCP065", DiagnosticLevel.Error, """Function "utcNow" is not valid at this location. It can only be used as a parameter default value."""),
            ("BCP062", DiagnosticLevel.Error, """The referenced declaration with name "abc" is not valid."""),
        });
    }

    [TestMethod]
    public void Parameters_file_permits_omission_of_optional_params()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using 'foo.bicep'

param foo = 'foo'
"),
("foo.bicep", @"
param foo string
param optionalBecauseNullable string?
param optionalBecauseDefault string = 'default'
"));

        result.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void Parameters_file_permits_nulling_out_of_optional_params()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using 'foo.bicep'

param foo = 'foo'
param optionalBecauseNullable = null
param optionalBecauseDefault = null
"),
("foo.bicep", @"
param foo string
param optionalBecauseNullable string?
param optionalBecauseDefault string = 'default'
"));

        result.Should().NotHaveAnyDiagnostics();
    }
}

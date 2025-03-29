// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Features;
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
    private ServiceBuilder ServicesWithExternalInputFunctionEnabled => 
        new ServiceBuilder()
            .WithFeatureOverrides(new FeatureProviderOverrides(TestContext, ExternalInputFunctionEnabled: true));

    [NotNull]
    public TestContext? TestContext { get; set; }
    
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

        result.Should().ContainDiagnostic("BCP338", DiagnosticLevel.Error, "Failed to evaluate parameter \"fromEnv\": Environment variable \"stringEnvVariable\" does not exist, and no default value set.");
        result.Should().ContainDiagnostic("Bicepparam ReadEnvironmentVariable function", DiagnosticLevel.Info, "Available environment variables are: ");
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
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);

        parameters["foo"].Value.Should().DeepEqual("abc");
        parameters["bar"].Value.Should().DeepEqual(new JObject
        {
            ["abc"] = "abc",
            ["abcWrapped"] = ">>abc<<",
            ["abcUpper"] = "ABC",
        });
        parameters["baz"].Value.Should().DeepEqual("BAZ,BAR,FOO");
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

    [TestMethod]
    public void Error_is_displayed_for_file_reference_with_errors()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", """
using 'main.bicep'
"""), ("main.bicep", """
invalid file
"""));

        result.Should().HaveDiagnostics(new[]
        {
            ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
        });
    }

    [TestMethod]
    public void ExternalInput_assigned_to_parameter()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
param foo = externalInput('sys.cli', 'foo')
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[externalInputs('0')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["options"] = "foo",
        });
    }

    [TestMethod]
    public void ExternalInput_multiple_with_different_parameters()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
param foo = externalInput('sys.cli', 'foo')
param bar = externalInput('sys.envVar', 'bar')
param baz = externalInput('custom.binding', '__BINDING__')
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);

        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[externalInputs('0')]""");

        parameters["bar"].Value.Should().BeNull();
        parameters["bar"].Expression.Should().DeepEqual("""[externalInputs('1')]""");

        parameters["baz"].Value.Should().BeNull();
        parameters["baz"].Expression.Should().DeepEqual("""[externalInputs('2')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["options"] = "foo",
        });
        externalInputs["1"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.envVar",
            ["options"] = "bar",
        });
        externalInputs["2"].Should().DeepEqual(new JObject
        {
            ["kind"] = "custom.binding",
            ["options"] = "__BINDING__",
        });
    }

    [TestMethod]
    public void ExternalInput_with_argument_referencing_variable()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
var bar = 'bar'
param foo = externalInput('sys.cli', bar)
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[externalInputs('0')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["options"] = "bar",
        });
    }

    [TestMethod]
    public void ExternalInput_assigned_to_parameter_wrapped_inside_builtin_function()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
param foo = bool(externalInput('sys.cli', 'foo'))
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[bool(externalInputs('0'))]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["options"] = "foo",
        });
    }

    [TestMethod]
    public void ExternalInput_variable_reference_on_parameter_assignment()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
var foo = bool(externalInput('sys.cli', 'foo'))
param bar = foo
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["bar"].Value.Should().BeNull();
        parameters["bar"].Expression.Should().DeepEqual("""[bool(externalInputs('0'))]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["options"] = "foo",
        });
    }

    [TestMethod]
    public void ExternalInput_with_object_config()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
param foo = externalInput('custom.tool', {
    path: '/path/to/file'
    isSecure: true
})
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[externalInputs('0')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "custom.tool",
            ["options"] = new JObject
            {
                ["path"] = "/path/to/file",
                ["isSecure"] = true,
            },
        });
    }

    [TestMethod]
    public void ExternalInput_variable_reference_chain()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
var a = externalInput('sys.cli', 'a')
var b = a
param c = b
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["c"].Value.Should().BeNull();
        parameters["c"].Expression.Should().DeepEqual("""[externalInputs('0')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["options"] = "a",
        });
    }

    [TestMethod]
    public void ExternalInput_param_reference_chain()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
param a = externalInput('sys.cli', 'a')
param b = a
param c = b
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);

        parameters["a"].Value.Should().BeNull();
        parameters["a"].Expression.Should().DeepEqual("""[externalInputs('0')]""");

        parameters["b"].Value.Should().BeNull();
        parameters["b"].Expression.Should().DeepEqual("""[externalInputs('0')]""");

        parameters["c"].Value.Should().BeNull();
        parameters["c"].Expression.Should().DeepEqual("""[externalInputs('0')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["options"] = "a",
        });
    }

    [TestMethod]
    public void ExternalInput_string_interpolation_of_param_references()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
param foo = int(externalInput('custom.binding', '__BINDING__'))
param bar = externalInput('custom.binding', {
    path: '/path/to/file'
    isSecure: true
})
param baz = '${foo} combined with ${bar}'
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);

        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[int(externalInputs('0'))]""");

        parameters["bar"].Value.Should().BeNull();
        parameters["bar"].Expression.Should().DeepEqual("""[externalInputs('1')]""");

        parameters["baz"].Value.Should().BeNull();
        parameters["baz"].Expression.Should().DeepEqual(
"""[format('{0} combined with {1}', int(externalInputs('0')), externalInputs('1'))]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "custom.binding",
            ["options"] = "__BINDING__",
        });
        externalInputs["1"].Should().DeepEqual(new JObject
        {
            ["kind"] = "custom.binding",
            ["options"] = new JObject
            {
                ["path"] = "/path/to/file",
                ["isSecure"] = true,
            },
        });
    }

    [TestMethod]
    public void ExternalInput_nested_variable_reference()
    {
        var result = CompilationHelper.CompileParams(
            ServicesWithExternalInputFunctionEnabled,
("parameters.bicepparam", @"
using none
var foo = '__BINDING__'
var bar = 'Binding: ${foo}, Value: ${json(externalInput('custom.binding', foo))}'
param baz = bar
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["baz"].Value.Should().BeNull();
        parameters["baz"].Expression.Should().DeepEqual(
"""[format('Binding: {0}, Value: {1}', '__BINDING__', json(externalInputs('0')))]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "custom.binding",
            ["options"] = "__BINDING__",
        });
    }
}

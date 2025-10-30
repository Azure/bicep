// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Web.Services.Description;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
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
            ("BCP427", DiagnosticLevel.Error,
                "Environment variable \"stringEnvVariable\" does not exist and there's no default value set.")
        });
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

        result.Should().ContainDiagnostic("BCP427", DiagnosticLevel.Error, "Environment variable \"stringEnvVariable\" does not exist and there's no default value set.");
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
    public void ExternalInput_assigned_to_parameter_without_config()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using none
param foo = externalInput('my.param.provider')
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[externalInputs('my_param_provider_0')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["my_param_provider_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "my.param.provider",
        });
    }

    [TestMethod]
    public void ExternalInput_assigned_to_parameter_with_config()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using none
param foo = externalInput('sys.cli', 'foo')
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[externalInputs('sys_cli_0')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["sys_cli_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["config"] = "foo",
        });
    }

    [TestMethod]
    public void ExternalInput_parameter_with_variable_references()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using none
var foo = externalInput('sys.cli', 'foo')
var foo2 = '${foo}-${externalInput('sys.cli', 'foo2')}'
param foo3 = foo2
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo3"].Value.Should().BeNull();
        parameters["foo3"].Expression.Should().DeepEqual("""[format('{0}-{1}', externalInputs('sys_cli_0'), externalInputs('sys_cli_1'))]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["sys_cli_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["config"] = "foo",
        });
        externalInputs["sys_cli_1"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["config"] = "foo2",
        });
    }

    [TestMethod]
    public void ExternalInput_parameter_with_param_references()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using none
param foo = externalInput('sys.cli', 'foo')
param foo2 = foo
param foo3 = foo2
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[externalInputs('sys_cli_0')]""");

        parameters["foo2"].Value.Should().BeNull();
        parameters["foo2"].Expression.Should().DeepEqual("""[externalInputs('sys_cli_0')]""");

        parameters["foo3"].Value.Should().BeNull();
        parameters["foo3"].Expression.Should().DeepEqual("""[externalInputs('sys_cli_0')]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["sys_cli_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cli",
            ["config"] = "foo",
        });
    }

    [TestMethod]
    public void ExternalInput_parameter_with_cyclic_references()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using none
param a = '${b}-${externalInput('sys.cli', 'a')}'
var b = '${c}-${a}'
param c = b
"));

        result.Should().NotGenerateParameters();
        result.Should().HaveDiagnostics(new[]
        {
            ("BCP062", DiagnosticLevel.Error, """The referenced declaration with name "b" is not valid."""),
            ("BCP080", DiagnosticLevel.Error, """The expression is involved in a cycle ("c" -> "b")."""),
            ("BCP080", DiagnosticLevel.Error, """The expression is involved in a cycle ("a" -> "b")."""),
            ("BCP080", DiagnosticLevel.Error, """The expression is involved in a cycle ("b" -> "c")."""),
        });
    }

    [TestMethod]
    public void ExternalInput_non_compile_time_constant_is_blocked()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using none
var myVar = 2 + 3
param foo = externalInput('sys.cli', myVar)
"));

        result.Should().NotGenerateParameters();
        result.Should().HaveDiagnostics(new[]
        {
            ("BCP032", DiagnosticLevel.Error, "The value must be a compile-time constant."),
        });
    }

    [TestMethod]
    public void ExternalInput_emits_top_level_expression()
    {
        var result = CompilationHelper.CompileParams(
("parameters.bicepparam", @"
using none
param foo = {
  bar: externalInput('my.param.provider')
}
"));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo"].Value.Should().BeNull();
        parameters["foo"].Expression.Should().DeepEqual("""[createObject('bar', externalInputs('my_param_provider_0'))]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["my_param_provider_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "my.param.provider",
        });
    }

    [TestMethod]
    public void ExternalInput_alternative_functions_also_generate_external_inputs()
    {
        var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, DeployCommandsEnabled: true));
        var result = CompilationHelper.CompileParams(
            services,
            ("parameters.bicepparam", """
                using 'main.bicep' with {
                  mode: 'deployment'
                  scope: '/subscriptions/foo/resourceGroups/bar'
                }
                var foo = readCliArg('foo')
                var foo2 = '${foo}-${readEnvVar('foo2')}'
                param foo3 = foo2
                """), ("main.bicep", """
                param foo3 string
                """));

        result.Should().NotHaveAnyDiagnostics();
        var parameters = TemplateHelper.ConvertAndAssertParameters(result.Parameters);
        parameters["foo3"].Value.Should().BeNull();
        parameters["foo3"].Expression.Should().DeepEqual("""[format('{0}-{1}', externalInputs('sys_cliArg_0'), externalInputs('sys_envVar_1'))]""");

        var externalInputs = TemplateHelper.ConvertAndAssertExternalInputs(result.Parameters);
        externalInputs["sys_cliArg_0"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.cliArg",
            ["config"] = "foo",
        });
        externalInputs["sys_envVar_1"].Should().DeepEqual(new JObject
        {
            ["kind"] = "sys.envVar",
            ["config"] = "foo2",
        });
    }
}

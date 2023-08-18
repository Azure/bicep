// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.IntegrationTests
{

    [TestClass]
    public class ValidateParamsCommandTests : TestBase
    {
        
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        [DoNotParallelize]
        public async Task Validate_params_with_correct_parameter_values_runs_successfully()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", """
param intParam int

@allowed([
    'foo'
    'bar'
])
param strParam string

param boolParam bool

param arrParam array

param objParam object
""");

            var paramsInput = """
{
  "intParam": 1,
  "strParam": "foo",
  "boolParam": true,
  "arrParam": [1, 2, 3],
  "objParam": {
    "firstProp": "foobar",
    "secondProp": [1, 2, 3] 
  }
}
""";       
            Environment.SetEnvironmentVariable("BICEP_PARAMETER_INPUT", paramsInput);

            var(output, error, result) = await Bicep("validate-params", bicepPath);

            result.Should().Be(0);
            output.Should().BeEmpty();
            AssertNoErrors(error);
        }


        [TestMethod]
        [DoNotParallelize]
        public async Task Validate_params_with_type_mismatch_fails_with_errors()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", """
param intParam int

@allowed(
  [
    'foo'
    'bar'
  ]
)
param strParam string
""");

            var paramsInput = """
{
  "intParam": "foo",
  "strParam": "baz"
}
""";

            Environment.SetEnvironmentVariable("BICEP_PARAMETER_INPUT", paramsInput);

            var(output, error, result) = await Bicep("validate-params", bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().NotBeEmpty();
            error.Should().Contain($"Error BCP368: Assigned type of parameter \"intParam\" does not match the declared type \"int\" in the bicep template");
            error.Should().Contain($"Error BCP368: Assigned type of parameter \"strParam\" does not match the declared type \"'bar' | 'foo'\" in the bicep template");        
        }

        [TestMethod]
        [DoNotParallelize]
        public async Task Validate_params_with_extra_parameters_fails_with_error()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", "param strParam string");

            var paramsInput = """
{
  "anotherStrParam": "foo",
}
""";
            Environment.SetEnvironmentVariable("BICEP_PARAMETER_INPUT", paramsInput);

            var(output, error, result) = await Bicep("validate-params", bicepPath);

            result.Should().Be(1);
            output.Should().BeEmpty();
            error.Should().NotBeEmpty();
            error.Should().Contain($"Error BCP369: A value for parameter \"anotherStrParam\" is provided but it is not declared in the bicep template \"{bicepPath}\"");
        }
    }
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands.Helpers.Deploy;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Services;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Utils;
using FluentAssertions;
using FluentAssertions.Common;
using Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Cli.UnitTests.Helpers.WhatIf;

[TestClass]
public class DeploymentProcessorTests
{
    private static readonly ServiceBuilder Services = new ServiceBuilder()
        .WithFeatureOverrides(new(DeployCommandsEnabled: true));

    [TestMethod]
    public async Task Parameters_with_deployments_config_can_be_parsed()
    {
        var environmentMock = StrictMock.Of<IEnvironment>();
        Dictionary<string, string> cliArgs = new()
        {
            ["subscription-id"] = "abc",
            ["resource-group"] = "def",
        };

        var result = CompilationHelper.CompileParams(Services, ("parameters.bicepparam", """
            var subscriptionId = readCliArg('subscription-id')
            var resourceGroup = readCliArg('resource-group')

            using 'main.bicep' with {
              name: 'asdf9uasd9'
              mode: 'deployment'
              scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
            }            

            param foo = 'bar'
            """), ("main.bicep", """
            param foo string
            """));

        result.Should().NotHaveAnyDiagnostics();

        var config = await DeploymentProcessor.GetDeployCommandsConfig(environmentMock.Object, cliArgs, result.Compilation.Emitter.Parameters());

        config.Template.Should().NotBeNull();
        config.Parameters.Should().NotBeNull();
        config.UsingConfig.Name.Should().Be("asdf9uasd9");
        config.UsingConfig.Scope.Should().Be("/subscriptions/abc/resourceGroups/def");
        config.UsingConfig.StacksConfig.Should().BeNull();
    }

    [TestMethod]
    public async Task Parameters_with_stacks_config_can_be_parsed()
    {
        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(m => m.GetVariable("SUBSCRIPTION_ID")).Returns("abc");
        environmentMock.Setup(m => m.GetVariable("RESOURCE_GROUP")).Returns("def");
        Dictionary<string, string> cliArgs = [];

        var result = CompilationHelper.CompileParams(Services, ("parameters.bicepparam", """
            var subscriptionId = readEnvVar('SUBSCRIPTION_ID')
            var resourceGroup = readEnvVar('RESOURCE_GROUP')

            using 'main.bicep' with {
              name: 'asdf9uasd9'
              mode: 'stack'
              scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
              description: 'my stack'
              actionOnUnmanage: {
                resources: 'delete'
              }
              denySettings: {
                mode: 'denyDelete'
              }
            }

            param foo = 'bar'
            """), ("main.bicep", """
            param foo string
            """));

        result.Should().NotHaveAnyDiagnostics();

        var config = await DeploymentProcessor.GetDeployCommandsConfig(environmentMock.Object, cliArgs, result.Compilation.Emitter.Parameters());

        config.Template.Should().NotBeNull();
        config.Parameters.Should().NotBeNull();
        config.UsingConfig.Name.Should().Be("asdf9uasd9");
        config.UsingConfig.Scope.Should().Be("/subscriptions/abc/resourceGroups/def");
        config.UsingConfig.StacksConfig.Should().BeEquivalentTo(new StacksConfig(
            Description: "my stack",
            ActionOnUnmanage: new(resources: DeploymentStacksDeleteDetachEnum.Delete),
            DenySettings: new(mode: DenySettingsMode.DenyDelete)
            {
                ApplyToChildScopes = false,
            }
        ));
    }

    [TestMethod]
    public async Task Missing_cli_args_throw_error()
    {
        var environmentMock = StrictMock.Of<IEnvironment>();
        Dictionary<string, string> cliArgs = new();

        var result = CompilationHelper.CompileParams(Services, ("parameters.bicepparam", """
            var subscriptionId = readCliArg('subscription-id')
            var resourceGroup = readCliArg('resource-group')

            using 'main.bicep' with {
              name: 'asdf9uasd9'
              mode: 'deployment'
              scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
            }            

            param foo = 'bar'
            """), ("main.bicep", """
            param foo string
            """));

        result.Should().NotHaveAnyDiagnostics();

        await FluentActions.Awaiting(async () => await DeploymentProcessor.GetDeployCommandsConfig(environmentMock.Object, cliArgs, result.Compilation.Emitter.Parameters()))
            .Should().ThrowAsync<CommandLineException>().WithMessage("CLI argument '--arg-subscription-id' must be provided.");
    }

    [TestMethod]
    public async Task Missing_env_vars_throw_error()
    {
        var environmentMock = StrictMock.Of<IEnvironment>();
        environmentMock.Setup(m => m.GetVariable("SUBSCRIPTION_ID")).Returns((string?)null);
        environmentMock.Setup(m => m.GetVariable("RESOURCE_GROUP")).Returns((string?)null);
        Dictionary<string, string> cliArgs = new();

        var result = CompilationHelper.CompileParams(Services, ("parameters.bicepparam", """
            var subscriptionId = readEnvVar('SUBSCRIPTION_ID')
            var resourceGroup = readEnvVar('RESOURCE_GROUP')

            using 'main.bicep' with {
              name: 'asdf9uasd9'
              mode: 'deployment'
              scope: '/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}'
            }            

            param foo = 'bar'
            """), ("main.bicep", """
            param foo string
            """));

        result.Should().NotHaveAnyDiagnostics();

        await FluentActions.Awaiting(async () => await DeploymentProcessor.GetDeployCommandsConfig(environmentMock.Object, cliArgs, result.Compilation.Emitter.Parameters()))
            .Should().ThrowAsync<CommandLineException>().WithMessage("Environment variable 'SUBSCRIPTION_ID' is not set.");
    }
}
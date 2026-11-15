// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Azure;
using Azure.Core;
using Azure.Deployments.Core.Definitions;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Mocking;
using Azure.ResourceManager.Resources.Models;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Azure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using StacksConfig = Bicep.Local.Deploy.Extensibility.NestedDeploymentExtension.StacksConfig;

namespace Bicep.Cli.IntegrationTests.Commands;

[TestClass]
public class OrchestrateCommandTests : TestBase
{
    [TestMethod]
    public async Task What_if_passes_resolved_inputs_to_the_deployment_provider()
    {
        var path = CreateFiles();
        var directory = Path.GetDirectoryName(path);
        FileHelper.SaveResultFile(TestContext, "main.bicep", """
            targetScope = 'orchestrator'
            stack app './app.bicepparam' = {
              region: 'westus'
              deploy: 'always'
              inputs: { enabled: false }
            }
            """, directory);
        FileHelper.SaveResultFile(TestContext, "app.bicepparam", """
            using './stack.bicep' with {
              mode: 'stack'
              name: 'app'
              scope: '/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/test'
              actionOnUnmanage: { resources: 'detach' }
              denySettings: { mode: 'none' }
            }
            param enabled = bool(externalInput('stack.setting', 'enabled'))
            """, directory);
        FileHelper.SaveResultFile(TestContext, "stack.bicep", """
            param enabled bool
            output enabled bool = enabled
            """, directory);
        var provider = StrictMock.Of<IArmDeploymentProvider>();
        provider.Setup(x => x.WhatIf(It.IsAny<IBicepConfiguration>(), It.IsAny<DeploymentLocator>(), "westus",
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<StacksConfig>(), It.IsAny<CancellationToken>()))
            .Callback<IBicepConfiguration, DeploymentLocator, string, string, string, StacksConfig, CancellationToken>((_, locator, _, template, parameters, config, _) =>
            {
                locator.SubscriptionId.Should().Be("00000000-0000-0000-0000-000000000001");
                locator.ResourceGroupName.Should().Be("test");
                locator.DeploymentName.Should().Be("app");
                JObject.Parse(parameters)["parameters"]!["enabled"]!["value"]!.Value<bool>().Should().BeFalse();
                template.Should().Contain("\"outputs\"");
                config.ActionOnUnmanage!.Resources.Should().Be(DeploymentStacksDeleteDetachEnum.Detach);
            })
            .ReturnsAsync([]);
        var armClient = StrictMock.Of<IArmClientProvider>();
        var result = await Bicep(Settings(), services => services.AddSingleton(provider.Object).AddSingleton(armClient.Object),
            TestContext.CancellationTokenSource.Token, ["orchestrate", "what-if", path]);
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        provider.VerifyAll();
        armClient.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task What_if_selects_the_same_stacks_as_deploy()
    {
        var path = CreateFiles();
        var inspected = new List<string>();
        var rawResponse = StrictMock.Of<Response>();
        rawResponse.SetupGet(x => x.Content).Returns(BinaryData.FromString("""{"status":"Succeeded","properties":{"changes":[]}}"""));
        var operation = StrictMock.Of<ArmOperation<WhatIfOperationResult>>();
        operation.Setup(x => x.GetRawResponse()).Returns(rawResponse.Object);
        var deployment = StrictMock.Of<ArmDeploymentResource>();
        deployment.Setup(x => x.WhatIfAsync(WaitUntil.Completed, It.IsAny<ArmDeploymentWhatIfContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(operation.Object);
        var resourcesClient = StrictMock.Of<MockableResourcesArmClient>();
        resourcesClient.Setup(x => x.GetArmDeploymentResource(It.IsAny<ResourceIdentifier>()))
            .Callback<ResourceIdentifier>(id => inspected.Add(id.Name))
            .Returns(deployment.Object);
        var armClient = StrictMock.Of<ArmClient>();
        armClient.Setup(x => x.GetCachedClient(It.IsAny<Func<ArmClient, MockableResourcesArmClient>>()))
            .Returns(resourcesClient.Object);
        var provider = StrictMock.Of<IArmClientProvider>();
        provider.Setup(x => x.CreateArmClient(It.IsAny<IBicepCloudConfiguration>(), null))
            .Returns(armClient.Object);

        var result = await Bicep(Settings(), services => services.AddSingleton(provider.Object),
            TestContext.CancellationTokenSource.Token,
            ["orchestrate", "what-if", path, "--label", "kind=app", "--label", "region=westus"]);
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        inspected.Should().BeEquivalentTo("cluster", "app");
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public async Task Deploy_selects_labels_and_prerequisites_through_the_engine(int selectionMode)
    {
        var path = CreateFiles();
        var started = new ConcurrentBag<string>();
        var regions = new ConcurrentBag<string>();
        var provider = StrictMock.Of<IArmDeploymentProvider>();
        provider.SetupGet(x => x.SupportsExternalExtensions).Returns(true);
        provider.Setup(x => x.CreateResourceGroup(It.IsAny<IBicepConfiguration>(), It.IsAny<DeploymentLocator>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<IBicepConfiguration, DeploymentLocator, string, CancellationToken>((_, _, region, _) => regions.Add(region))
            .Returns(Task.CompletedTask);
        provider.Setup(x => x.StartDeploymentStack(It.IsAny<IBicepConfiguration>(), It.IsAny<DeploymentLocator>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<StacksConfig>(), It.IsAny<CancellationToken>()))
            .Callback<IBicepConfiguration, DeploymentLocator, string, string, StacksConfig, CancellationToken>((_, locator, _, _, _, _) => started.Add(locator.DeploymentName))
            .Returns(Task.CompletedTask);
        provider.Setup(x => x.CheckDeploymentStack(It.IsAny<IBicepConfiguration>(), It.IsAny<DeploymentLocator>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LocalDeploymentResult(new()
            {
                Properties = new() { ProvisioningState = ProvisioningState.Succeeded },
            }, [], []));

        List<string> arguments = ["orchestrate", "deploy", path, "--format", "Json"];
        if (selectionMode > 0)
        {
            arguments.AddRange(["--label", "kind=app", "--label", "region=westus"]);
            if (selectionMode == 2)
            {
                arguments.AddRange(["--label", "region=eastus"]);
            }
        }
        var result = await Bicep(Settings(), services => services.AddSingleton(provider.Object),
            TestContext.CancellationTokenSource.Token,
            [.. arguments]);
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        string[] expectedNames = selectionMode == 1 ? ["cluster", "app"] : ["cluster", "app", "cluster", "app"];
        string[] expectedRegions = selectionMode == 1 ? ["westus", "westus"] : ["westus", "westus", "eastus", "eastus"];
        started.Should().BeEquivalentTo(expectedNames);
        regions.Should().BeEquivalentTo(expectedRegions);
    }

    [TestMethod]
    [DataRow("deploy", "kind")]
    [DataRow("what-if", "kind=app,")]
    [DataRow("teardown", "=app")]
    public async Task Malformed_labels_fail_before_compilation(string command, string selector)
    {
        var result = await Bicep(Settings(),
            ["orchestrate", command, "missing.bicepparam", "--label", selector]);
        result.Should().Fail();
        result.Stderr.Should().Contain("Invalid label selector");
    }

    [TestMethod]
    [DataRow("deploy")]
    [DataRow("what-if")]
    [DataRow("teardown")]
    public async Task No_matching_labels_fail_before_any_Azure_calls(string command)
    {
        var path = CreateFiles();
        var provider = StrictMock.Of<IArmDeploymentProvider>();
        var processor = StrictMock.Of<IDeploymentProcessor>();
        var result = await Bicep(Settings(), services => services
            .AddSingleton(provider.Object).AddSingleton(processor.Object),
            TestContext.CancellationTokenSource.Token,
            ["orchestrate", command, path, "--label", "kind=missing"]);
        result.Should().Fail();
        result.Stderr.Should().Contain("No stacks match");
        provider.VerifyNoOtherCalls();
        processor.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task Teardown_rejects_unselected_dependents()
    {
        var path = CreateFiles();
        var processor = StrictMock.Of<IDeploymentProcessor>();
        var result = await Bicep(Settings(), services => services.AddSingleton(processor.Object),
            TestContext.CancellationTokenSource.Token,
            ["orchestrate", "teardown", path, "--label", "kind=cluster"]);
        result.Should().Fail();
        result.Stderr.Should().Contain("unselected stack 'apps");
        processor.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task Teardown_uses_the_real_provider_with_delete_actions()
    {
        var path = CreateFiles();
        var deleted = new List<string>();
        var stack = StrictMock.Of<DeploymentStackResource>();
        stack.Setup(x => x.DeleteAsync(WaitUntil.Completed, UnmanageActionResourceMode.Delete,
                null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(StrictMock.Of<ArmOperation>().Object);
        var resources = StrictMock.Of<MockableResourcesArmClient>();
        resources.Setup(x => x.GetDeploymentStackResource(It.IsAny<ResourceIdentifier>()))
            .Callback<ResourceIdentifier>(id => deleted.Add(id.Name))
            .Returns(stack.Object);
        var client = StrictMock.Of<ArmClient>();
        client.Setup(x => x.GetCachedClient(It.IsAny<Func<ArmClient, MockableResourcesArmClient>>()))
            .Returns(resources.Object);
        var provider = StrictMock.Of<IArmClientProvider>();
        provider.Setup(x => x.CreateArmClient(It.IsAny<IBicepCloudConfiguration>(), null)).Returns(client.Object);
        var result = await Bicep(Settings(), services => services.AddSingleton(provider.Object),
            TestContext.CancellationTokenSource.Token,
            ["orchestrate", "teardown", path, "--label", "region=westus"]);
        result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
        deleted.Should().Equal("app", "cluster");
        stack.VerifyAll();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Teardown_orders_selected_stacks_and_stops_on_failure(bool fail)
    {
        var path = CreateFiles();
        var deleted = new ConcurrentQueue<string>();
        var provider = StrictMock.Of<IArmDeploymentProvider>();
        provider.Setup(x => x.TeardownStack(It.IsAny<IBicepConfiguration>(), It.IsAny<DeploymentLocator>(), It.IsAny<StacksConfig>(), It.IsAny<CancellationToken>()))
            .Returns<IBicepConfiguration, DeploymentLocator, StacksConfig, CancellationToken>((_, locator, _, _) =>
            {
                deleted.Enqueue(locator.DeploymentName);
                return fail ? Task.FromException(new InvalidOperationException("test failure")) : Task.CompletedTask;
            });
        var result = await Bicep(Settings(), services => services.AddSingleton(provider.Object),
            TestContext.CancellationTokenSource.Token,
            ["orchestrate", "teardown", path, "--label", "region=westus"]);
        if (fail)
        {
            result.Should().Fail();
            deleted.Should().Equal("app");
        }
        else
        {
            result.Should().Succeed("stdout: {0}; stderr: {1}", result.Stdout, result.Stderr);
            deleted.Should().Equal("app", "cluster");
        }
    }

    private InvocationSettings Settings() => CreateDefaultSettings() with
    {
        FeatureOverrides = CreateDefaultFeatureProviderOverrides() with
        {
            OrchestrationEnabled = true,
            DeployCommandsEnabled = true,
        },
    };

    private string CreateFiles()
    {
        var path = FileHelper.SaveResultFile(TestContext, "main.bicepparam", "using './main.bicep'");
        var directory = Path.GetDirectoryName(path);
        FileHelper.SaveResultFile(TestContext, "main.bicep", """
            targetScope = 'orchestrator'
            var regions = ['westus', 'eastus']
            stack clusters './cluster.bicepparam' = [for region in regions: {
              region: region
              deploy: 'always'
              labels: {
                kind: 'cluster'
                region: region
              }
              inputs: {}
            }]
            stack apps './app.bicepparam' = [for (region, i) in regions: {
              region: region
              deploy: 'always'
              labels: {
                kind: 'app'
                region: region
              }
              inputs: {}
              requires: [clusters[i]]
            }]
            """, directory);
        string[] stackNames = ["cluster", "app"];
        foreach (var name in stackNames)
        {
            FileHelper.SaveResultFile(TestContext, $"{name}.bicepparam", $$"""
            using './stack.bicep' with {
              mode: 'stack'
              name: '{{name}}'
              scope: '/subscriptions/00000000-0000-0000-0000-000000000001/resourceGroups/test'
              actionOnUnmanage: { resources: 'delete' }
              denySettings: { mode: 'none' }
            }
            """, directory);
        }
        FileHelper.SaveResultFile(TestContext, "stack.bicep", "output value string = 'example'", directory);
        return path;
    }
}

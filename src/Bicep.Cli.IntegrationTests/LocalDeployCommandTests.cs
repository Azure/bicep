// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Pipes;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Bicep.Cli.Rpc;
using Bicep.Cli.UnitTests.Assertions;
using Bicep.Core.Configuration;
using Bicep.Core.Json;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Extensibility;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace Bicep.Cli.IntegrationTests;

[TestClass]
public class LocalDeployCommandTests : TestBase
{
    private static ExtensionPackage GetMockLocalDeployPackage(BinaryData? tgzData = null)
    {
        tgzData ??= ExtensionResourceTypeHelper.GetHttpExtensionTypesTgz();

        var architecture = SupportedArchitectures.TryGetCurrent() ?? throw new InvalidOperationException("Failed to get current architecture");

        // this doesn't need to contain a real binary, because this test emulates the local binary connection
        ExtensionBinary binary = new(architecture, BinaryData.FromBytes([]));
        return new ExtensionPackage(tgzData, true, [binary]);
    }

    private static void RegisterExtensionMocks(
        IServiceCollection services,
        ILocalExtension? localExtension = null,
        IArmDeploymentProvider? armDeploymentProvider = null)
    {
        localExtension ??= StrictMock.Of<ILocalExtension>().Object;
        armDeploymentProvider ??= StrictMock.Of<IArmDeploymentProvider>().Object;

        var mockExtensionFactory = StrictMock.Of<ILocalExtensionFactory>();
        mockExtensionFactory.Setup(x => x.Start(It.IsAny<Uri>())).ReturnsAsync(localExtension);

        services
            .AddSingleton(mockExtensionFactory.Object)
            .AddSingleton(armDeploymentProvider);
    }

    [TestMethod]
    public async Task Local_deploy_should_succeed()
    {
        var paramFile = new EmbeddedFile(typeof(LocalDeployCommandTests).Assembly, "Files/LocalDeployCommandTests/weather/main.bicepparam");
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);

        var extensionMock = StrictMock.Of<ILocalExtension>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.IsAny<ResourceSpecification>(), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((req, _) =>
            {
                var outputProperties = req.Properties["uri"]?.ToString() switch
                {
                    "https://api.weather.gov/points/47.6363726,-122.1357068" => new JsonObject
                    {
                        ["body"] = """
                        {
                          "properties": {
                            "gridId": "SEW",
                            "gridX": 131,
                            "gridY": 68
                          }
                        }
                        """
                    },
                    "https://api.weather.gov/gridpoints/SEW/131,68/forecast" => new JsonObject
                    {
                        ["body"] = """
                        {
                          "properties": {
                            "periods": [
                              {
                                "name": "Tonight",
                                "temperature": 47
                              },
                              {
                                "name": "Wednesday",
                                "temperature": 64
                              },
                              {
                                "name": "Wednesday Night",
                                "temperature": 46
                              }
                            ]
                          }
                        }
                        """
                    },
                    _ => throw new InvalidOperationException("Unexpected URI"),
                };

                return Task.FromResult(new LocalExtensionOperationResponse(new Resource(req.Type, req.ApiVersion, req.Properties, (outputProperties as JsonObject)!, "Succeeded"), null));
            });


        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(GetMockLocalDeployPackage(), new(LocalDeployEnabled: true));
        var clientFactory = services.Build().Construct<IContainerRegistryClientFactory>();

        var result = await Bicep(
            new InvocationSettings(ClientFactory: clientFactory),
            services => RegisterExtensionMocks(services, extensionMock.Object),
            TestContext.CancellationTokenSource.Token,
            ["local-deploy", baselineFolder.EntryFile.OutputFilePath]);

        result.Should().Succeed();
        result.Stdout.Should().EqualIgnoringWhitespace("""
Output forecast: [
  {
    "name": "Tonight",
    "temperature": 47
  },
  {
    "name": "Wednesday",
    "temperature": 64
  },
  {
    "name": "Wednesday Night",
    "temperature": 46
  }
]
Output forecastString: "Forecast: Name"
Resource forecastReq (Create): Succeeded
Resource gridpointsReq (Create): Succeeded
Result: Succeeded

""");
    }

    [TestMethod]
    public async Task Local_deploy_with_azure_should_succeed()
    {
        var paramFile = new EmbeddedFile(typeof(LocalDeployCommandTests).Assembly, "Files/LocalDeployCommandTests/azure/main.bicepparam");
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, paramFile);

        var extensionMock = StrictMock.Of<ILocalExtension>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.IsAny<ResourceSpecification>(), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((req, _) =>
            {
                var outputProperties = req.Properties["uri"]?.ToString() switch
                {
                    "https://api.weather.gov/points/47.6363726,-122.1357068" => new JsonObject
                    {
                        ["body"] = """
                        {
                          "properties": {
                            "gridId": "SEW",
                            "gridX": 131,
                            "gridY": 68
                          }
                        }
                        """
                    },
                    _ => throw new InvalidOperationException("Unexpected URI"),
                };

                return Task.FromResult(new LocalExtensionOperationResponse(new Resource(req.Type, req.ApiVersion, req.Properties, (outputProperties as JsonObject)!, "Succeeded"), null));
            });

        var deploymentProviderMock = StrictMock.Of<IArmDeploymentProvider>();

        deploymentProviderMock.Setup(x => x.StartDeployment(It.IsAny<RootConfiguration>(), It.IsAny<DeploymentLocator>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<RootConfiguration, DeploymentLocator, string, string, CancellationToken>(async (config, locator, templateString, parametersString, _) =>
            {
                await Task.CompletedTask;

                var template = JsonSerializer.Deserialize<JsonElement>(templateString);
                var parameters = JsonSerializer.Deserialize<JsonElement>(parametersString);
            });

        deploymentProviderMock.Setup(x => x.CheckDeployment(It.IsAny<RootConfiguration>(), It.IsAny<DeploymentLocator>(), It.IsAny<CancellationToken>()))
            .Returns<RootConfiguration, DeploymentLocator, CancellationToken>(async (config, locator, _) =>
            {
                await Task.CompletedTask;

                return new(
                    new()
                    {
                        Properties = new()
                        {
                            ProvisioningState = ProvisioningState.Succeeded,
                            Outputs = new()
                            {
                                ["gridId"] = new()
                                {
                                    Value = "SEW",
                                }
                            }
                        },
                    },
                    []);
            });

        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(GetMockLocalDeployPackage(), new(LocalDeployEnabled: true));
        var clientFactory = services.Build().Construct<IContainerRegistryClientFactory>();

        var result = await Bicep(
            new InvocationSettings(ClientFactory: clientFactory),
            services => RegisterExtensionMocks(services, extensionMock.Object, deploymentProviderMock.Object),
            TestContext.CancellationTokenSource.Token,
            ["local-deploy", baselineFolder.EntryFile.OutputFilePath]);

        result.Should().Succeed();
        result.Stdout.Should().EqualIgnoringWhitespace("""
Output gridId: "SEW"
Resource gridpointsReq (Create): Succeeded
Resource gridCoords (Create): Succeeded
Result: Succeeded
""");
    }
}

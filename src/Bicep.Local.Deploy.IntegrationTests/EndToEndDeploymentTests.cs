// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Nodes;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Extensibility;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json.Linq;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Local.Deploy.IntegrationTests;

[TestClass]
public class EndToEndDeploymentTests : TestBase
{
    private static ExtensionPackage GetMockLocalDeployPackage(BinaryData? tgzData = null)
    {
        tgzData ??= ExtensionResourceTypeHelper.GetHttpExtensionTypesTgz();

        var architecture = SupportedArchitectures.TryGetCurrent() ?? throw new InvalidOperationException("Failed to get current architecture");

        // this doesn't need to contain a real binary, because this test emulates the local binary connection
        ExtensionBinary binary = new(architecture, BinaryData.FromBytes([]));
        return new ExtensionPackage(tgzData, true, [binary]);
    }

    [TestMethod]
    public async Task End_to_end_deployment_basic()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(GetMockLocalDeployPackage(), new(LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
targetScope = 'local'

extension http

param coords {
  latitude: string
  longitude: string
}

resource gridpointsReq 'request@v1' = {
  uri: 'https://api.weather.gov/points/${coords.latitude},${coords.longitude}'
  format: 'raw'
}

var gridpoints = json(gridpointsReq.body).properties

resource forecastReq 'request@v1' = {
  uri: 'https://api.weather.gov/gridpoints/${gridpoints.gridId}/${gridpoints.gridX},${gridpoints.gridY}/forecast'
  format: 'raw'
}

var forecast = json(forecastReq.body).properties

type forecastType = {
  name: string
  temperature: int
}

var val = 'Name'

func getForecast() string => 'Forecast: ${val}'

output forecast forecastType[] = map(forecast.periods, p => {
  name: p.name
  temperature: p.temperature
})

output forecastString string = getForecast()
"""),
            ("parameters.bicepparam", """
using 'main.bicep'

param coords = {
  latitude: '47.6363726'
  longitude: '-122.1357068'
}
"""));

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var extensionMock = StrictMock.Of<ILocalExtension>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.Is<ResourceSpecification>(req => req.Properties["uri"]!.ToString() == "https://api.weather.gov/points/47.6363726,-122.1357068"), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((req, _) =>
            {
                req.Type.Should().Be("request");
                req.ApiVersion.Should().Be("v1");
                req.Properties["body"] = """
{
  "properties": {
    "gridId": "SEW",
    "gridX": "131",
    "gridY": "68"
  }
}
""";
                return Task.FromResult(new LocalExtensionOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), null));
            });

        extensionMock.Setup(x => x.CreateOrUpdate(It.Is<ResourceSpecification>(req => req.Properties["uri"]!.ToString() == "https://api.weather.gov/gridpoints/SEW/131,68/forecast"), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((req, _) =>
            {
                req.Type.Should().Be("request");
                req.ApiVersion.Should().Be("v1");
                req.Properties["body"] = """
{
  "properties": {
    "periods": [
      {
        "number": 1,
        "name": "Tonight",
        "temperature": 47
      },
      {
        "number": 2,
        "name": "Wednesday",
        "temperature": 68
      }
    ]
  }
}
""";
                return Task.FromResult(new LocalExtensionOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), null));
            });

        var localDeployResult = await Deploy(services, extensionMock.Object, result);

        localDeployResult.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Succeeded);
        localDeployResult.Deployment.Properties.Outputs["forecastString"].Value.Should().DeepEqual("Forecast: Name");
        localDeployResult.Deployment.Properties.Outputs["forecast"].Value.Should().DeepEqual(JToken.Parse("""
[
  {
    "name": "Tonight",
    "temperature": 47
  },
  {
    "name": "Wednesday",
    "temperature": 68
  }
]
"""));
    }

    [TestMethod]
    public async Task Extension_returning_resource_and_error_data_should_fail()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(GetMockLocalDeployPackage(), new(LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
targetScope = 'local'

extension http

param coords {
  latitude: string
  longitude: string
}

resource gridpointsReq 'request@v1' = {
  uri: 'https://api.weather.gov/points/${coords.latitude},${coords.longitude}'
  format: 'raw'
}

var gridpoints = json(gridpointsReq.body).properties

resource forecastReq 'request@v1' = {
  uri: 'https://api.weather.gov/gridpoints/${gridpoints.gridId}/${gridpoints.gridX},${gridpoints.gridY}/forecast'
  format: 'raw'
}

var forecast = json(forecastReq.body).properties

type forecastType = {
  name: string
  temperature: int
}

output forecast forecastType[] = map(forecast.periods, p => {
  name: p.name
  temperature: p.temperature
})
"""),
            ("parameters.bicepparam", """
using 'main.bicep'

param coords = {
  latitude: '47.6363726'
  longitude: '-122.1357068'
}
"""));

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var extensionMock = StrictMock.Of<ILocalExtension>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.Is<ResourceSpecification>(req => req.Properties["uri"]!.ToString() == "https://api.weather.gov/points/47.6363726,-122.1357068"), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((req, _) =>
            {
                req.Properties["body"] = """
{
  "properties": {
    "gridId": "SEW",
    "gridX": "131",
    "gridY": "68"
  }
}
""";
                return Task.FromResult(new LocalExtensionOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), new ErrorData(new Error() { Code = "Code", Message = "Error message" })));
            });

        var localDeployResult = await Deploy(services, extensionMock.Object, result);

        localDeployResult.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Failed, because: $"Extension returned '{nameof(Resource)}' and '{nameof(ErrorData)}' as part of its response and it is not allowed. Extensions should return one or the other to indicate success or failure respectively.");
        localDeployResult.Deployment.Properties.Error.Should().NotBeNull();

        localDeployResult.Deployment.Properties.Error.Code.Should().Be("DeploymentFailed");
        localDeployResult.Deployment.Properties.Error.Details.Should().NotBeNullOrEmpty();
        localDeployResult.Deployment.Properties.Error.Details[0].Code.Should().Be("ResourceDeploymentFailure");
        localDeployResult.Deployment.Properties.Error.Details[0].Target.Should().Be("/resources/gridpointsReq", because: $"Expect a failure when mocking a response for \"/resources/gridpointsReq\" since it is returning '{nameof(Resource)}' and '{nameof(ErrorData)}' when only one type should be returned to indicate success or failure.");
    }

    [TestMethod]
    public async Task Extension_not_returning_resource_or_error_data_should_fail()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(GetMockLocalDeployPackage(), new(LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
targetScope = 'local'

extension http

param coords {
  latitude: string
  longitude: string
}

resource gridpointsReq 'request@v1' = {
  uri: 'https://api.weather.gov/points/${coords.latitude},${coords.longitude}'
  format: 'raw'
}

var gridpoints = json(gridpointsReq.body).properties

resource forecastReq 'request@v1' = {
  uri: 'https://api.weather.gov/gridpoints/${gridpoints.gridId}/${gridpoints.gridX},${gridpoints.gridY}/forecast'
  format: 'raw'
}

var forecast = json(forecastReq.body).properties

type forecastType = {
  name: string
  temperature: int
}

output forecast forecastType[] = map(forecast.periods, p => {
  name: p.name
  temperature: p.temperature
})
"""),
            ("parameters.bicepparam", """
using 'main.bicep'

param coords = {
  latitude: '47.6363726'
  longitude: '-122.1357068'
}
"""));

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var extensionMock = StrictMock.Of<ILocalExtension>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.Is<ResourceSpecification>(req => req.Properties["uri"]!.ToString() == "https://api.weather.gov/points/47.6363726,-122.1357068"), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((req, _) =>
            {
                req.Properties["body"] = """
{
  "properties": {
    "gridId": "SEW",
    "gridX": "131",
    "gridY": "68"
  }
}
""";
                return Task.FromResult(new LocalExtensionOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), new ErrorData(new Error() { Code = "Code", Message = "Error message" })));
            });

        var localDeployResult = await Deploy(services, extensionMock.Object, result);

        localDeployResult.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Failed, because: $"Extension did not return '{nameof(Resource)}' or '{nameof(ErrorData)}' as part of its response. Extensions should return one or the other to indicate success or failure respectively.");
        localDeployResult.Deployment.Properties.Error.Should().NotBeNull();

        localDeployResult.Deployment.Properties.Error.Code.Should().Be("DeploymentFailed");
        localDeployResult.Deployment.Properties.Error.Details.Should().NotBeNullOrEmpty();
        localDeployResult.Deployment.Properties.Error.Details[0].Code.Should().Be("ResourceDeploymentFailure");
        localDeployResult.Deployment.Properties.Error.Details[0].Target.Should().Be("/resources/gridpointsReq", because: $"Expect a failure when mocking a response for \"/resources/gridpointsReq\" because extension it is not returning '{nameof(Resource)}' or '{nameof(ErrorData)}' and one must be returned to indicate success or failure.");
    }

    [TestMethod]
    public async Task Extension_returning_error_data_should_fail()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(GetMockLocalDeployPackage(), new(LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
targetScope = 'local'

extension http

param coords {
  latitude: string
  longitude: string
}

resource gridpointsReq 'request@v1' = {
  uri: 'https://api.weather.gov/points/${coords.latitude},${coords.longitude}'
  format: 'raw'
}

var gridpoints = json(gridpointsReq.body).properties

resource forecastReq 'request@v1' = {
  uri: 'https://api.weather.gov/gridpoints/${gridpoints.gridId}/${gridpoints.gridX},${gridpoints.gridY}/forecast'
  format: 'raw'
}

var forecast = json(forecastReq.body).properties

type forecastType = {
  name: string
  temperature: int
}

output forecast forecastType[] = map(forecast.periods, p => {
  name: p.name
  temperature: p.temperature
})
"""),
            ("parameters.bicepparam", """
using 'main.bicep'

param coords = {
  latitude: '47.6363726'
  longitude: '-122.1357068'
}
"""));

        var extensionMock = StrictMock.Of<ILocalExtension>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.Is<ResourceSpecification>(req => req.Properties["uri"]!.ToString() == "https://api.weather.gov/points/47.6363726,-122.1357068"), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((_, _) =>
            {
                return Task.FromResult(new LocalExtensionOperationResponse(null, new ErrorData(new Error() { Code = "Code", Message = "Error message" })));
            });

        var localDeployResult = await Deploy(services, extensionMock.Object, result);

        localDeployResult.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Failed, because: "Extension returned a failure when attempting to create a resource.");
        localDeployResult.Deployment.Properties.Error.Should().NotBeNull();

        localDeployResult.Deployment.Properties.Error.Code.Should().Be("DeploymentFailed");
        localDeployResult.Deployment.Properties.Error.Details.Should().NotBeNullOrEmpty();
        localDeployResult.Deployment.Properties.Error.Details[0].Code.Should().Be("Code");
        localDeployResult.Deployment.Properties.Error.Details[0].Message.Should().Be("Error message");
    }

    [TestMethod]
    public async Task Extension_config_is_passed_via_extensibility_request()
    {
        var package = GetMockLocalDeployPackage(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration());
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(package, new(LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "foo": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
targetScope = 'local'

extension foo with {
  namespace: 'ThirdPartyNamespace'
  config: 'Some path to config file'
  context: 'Some ThirdParty context'
}

resource dadJoke 'fooType@v1' = {
  identifier: 'foo'
  joke: 'dad joke'
}

output joke string = dadJoke.joke
"""),
            ("parameters.bicepparam", """
using 'main.bicep'
"""));

        JsonObject identifiers = new()
        {
            ["identifier"] = "foo",
        };

        var extensionMock = StrictMock.Of<ILocalExtension>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.IsAny<ResourceSpecification>(), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((req, _) =>
            {
                req.Config!.ToJsonString().FromJson<JToken>().Should().DeepEqual(JToken.Parse("""
                    {
                      "namespace": "ThirdPartyNamespace",
                      "config": "Some path to config file",
                      "context": "Some ThirdParty context"
                    }
                    """));

                return Task.FromResult(new LocalExtensionOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), null));
            });

        var localDeployResult = await Deploy(services, extensionMock.Object, result);

        localDeployResult.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Succeeded);
    }

    [TestMethod]
    public async Task Extension_config_is_passed_via_extensibility_request_v2()
    {
        var package = GetMockLocalDeployPackage(ExtensionResourceTypeHelper.GetTestTypesTgzWithFallbackAndConfiguration(allConfigPropertiesOptional: true));
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(package, new(LocalDeployEnabled: true, ModuleExtensionConfigsEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(
            services,
            ("bicepconfig.json",
                // language=json
                """
                {
                  "extensions": {
                    "foo": "br:example.azurecr.io/extensions/foo:1.2.3"
                  },
                  "experimentalFeaturesEnabled": {
                    "extensibility": true,
                    "moduleExtensionConfigs": true,
                    "localDeploy": true
                  }
                }
                """),
            ("main.bicep",
                """
                targetScope = 'local'

                extension foo

                resource dadJoke 'fooType@v1' = {
                  identifier: 'foo'
                  joke: 'dad joke'
                }

                output joke string = dadJoke.joke
                """),
            ("parameters.bicepparam",
                """
                using 'main.bicep'

                extensionConfig foo with {
                  namespace: 'paramsFileNs'
                  config: 'paramsFileConfig'
                  context: 'paramsFileContext'
                }
                """));

        JsonObject identifiers = new()
        {
            ["identifier"] = "foo",
        };

        var extensionMock = StrictMock.Of<ILocalExtension>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.IsAny<ResourceSpecification>(), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((req, _) =>
            {
                req.Config!.ToJsonString()
                    .FromJson<JToken>()
                    .Should()
                    .DeepEqual(
                        JObject.Parse(
                            """
                            {
                              "namespace": "paramsFileNs",
                              "config": "paramsFileConfig",
                              "context": "paramsFileContext"
                            }
                            """));

                return Task.FromResult(new LocalExtensionOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), null));
            });

        var localDeployResult = await Deploy(services, extensionMock.Object, result);

        localDeployResult.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Succeeded);
    }

    private async Task<LocalDeploymentResult> Deploy(ServiceBuilder services, ILocalExtension extension, ParamsCompilationResult result)
    {
        result.Should().NotHaveAnyDiagnostics();

        var parametersFile = result.Compilation.Emitter.Parameters().Parameters!;
        var templateFile = result.Compilation.Emitter.Parameters().Template!.Template!;

        var mockExtensionFactory = StrictMock.Of<ILocalExtensionFactory>();
        mockExtensionFactory.Setup(x => x.Start(It.IsAny<IOUri>())).ReturnsAsync(extension);
        var serviceProvider = services.Build().Construct<IServiceProvider>();
        var moduleDispatcher = BicepTestConstants.CreateModuleDispatcher(serviceProvider);

        await using LocalExtensionDispatcher extensionDispatcher = new(
            serviceProvider.GetRequiredService<IConfigurationManager>(),
            mockExtensionFactory.Object,
            StrictMock.Of<IArmDeploymentProvider>().Object);

        await extensionDispatcher.InitializeExtensions(result.Compilation);

        return await extensionDispatcher.Deploy(templateFile, parametersFile, state => { }, TestContext.CancellationTokenSource.Token);
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Text;
using System.Text.Json.Nodes;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Engine.Host.Azure.ExtensibilityV2.Contract.Models;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Core.Configuration;
using Bicep.Core.Features;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Extensibility;
using Bicep.Local.Extension;
using FluentAssertions;
using Json.Pointer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy.IntegrationTests;

[TestClass]
public class EndToEndDeploymentTests : TestBase
{
    [TestMethod]
    public async Task End_to_end_deployment_basic()
    {
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ThirdPartyTypeHelper.GetHttpExtensionTypesTgz(), new(ExtensibilityEnabled: true, LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
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

        result.Should().NotHaveAnyDiagnostics();

        var parametersFile = result.Compilation.Emitter.Parameters().Parameters!;
        var templateFile = result.Compilation.Emitter.Parameters().Template!.Template!;

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var extensionMock = StrictMock.Of<LocalExtensibilityHost>();
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
                return Task.FromResult(new LocalExtensibilityOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), null));
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
                return Task.FromResult(new LocalExtensibilityOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), null));
            });

        var dispatcher = BicepTestConstants.CreateModuleDispatcher(services.Build().Construct<IServiceProvider>());
        await using LocalExtensibilityHostManager extensibilityHandler = new(dispatcher, uri => Task.FromResult(extensionMock.Object));
        await extensibilityHandler.InitializeExtensions(result.Compilation);

        var localDeployResult = await LocalDeployment.Deploy(extensibilityHandler, templateFile, parametersFile, TestContext.CancellationTokenSource.Token);

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
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ThirdPartyTypeHelper.GetHttpExtensionTypesTgz(), new(ExtensibilityEnabled: true, LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
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

        result.Should().NotHaveAnyDiagnostics();

        var parametersFile = result.Compilation.Emitter.Parameters().Parameters!;
        var templateFile = result.Compilation.Emitter.Parameters().Template!.Template!;

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var extensionMock = StrictMock.Of<LocalExtensibilityHost>();
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
                return Task.FromResult(new LocalExtensibilityOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), new ErrorData(new Error() { Code = "Code", Message = "Error message" })));
            });

        var dispatcher = BicepTestConstants.CreateModuleDispatcher(services.Build().Construct<IServiceProvider>());
        await using LocalExtensibilityHostManager extensibilityHandler = new(dispatcher, uri => Task.FromResult(extensionMock.Object));
        await extensibilityHandler.InitializeExtensions(result.Compilation);

        var localDeployResult = await LocalDeployment.Deploy(extensibilityHandler, templateFile, parametersFile, TestContext.CancellationTokenSource.Token);

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
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ThirdPartyTypeHelper.GetHttpExtensionTypesTgz(), new(ExtensibilityEnabled: true, LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
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

        result.Should().NotHaveAnyDiagnostics();

        var parametersFile = result.Compilation.Emitter.Parameters().Parameters!;
        var templateFile = result.Compilation.Emitter.Parameters().Template!.Template!;

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var extensionMock = StrictMock.Of<LocalExtensibilityHost>();
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
                return Task.FromResult(new LocalExtensibilityOperationResponse(new Resource(req.Type, req.ApiVersion, identifiers, req.Properties, "Succeeded"), new ErrorData(new Error() { Code = "Code", Message = "Error message" })));
            });

        var dispatcher = BicepTestConstants.CreateModuleDispatcher(services.Build().Construct<IServiceProvider>());
        await using LocalExtensibilityHostManager extensibilityHandler = new(dispatcher, uri => Task.FromResult(extensionMock.Object));
        await extensibilityHandler.InitializeExtensions(result.Compilation);

        var localDeployResult = await LocalDeployment.Deploy(extensibilityHandler, templateFile, parametersFile, TestContext.CancellationTokenSource.Token);

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
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(ThirdPartyTypeHelper.GetHttpExtensionTypesTgz(), new(ExtensibilityEnabled: true, LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "extensions": {
    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
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

        result.Should().NotHaveAnyDiagnostics();

        var parametersFile = result.Compilation.Emitter.Parameters().Parameters!;
        var templateFile = result.Compilation.Emitter.Parameters().Template!.Template!;

        JsonObject identifiers = new()
                {
                    { "name", "someName" },
                    { "namespace", "someNamespace" }
                };

        var extensionMock = StrictMock.Of<LocalExtensibilityHost>();
        extensionMock.Setup(x => x.CreateOrUpdate(It.Is<ResourceSpecification>(req => req.Properties["uri"]!.ToString() == "https://api.weather.gov/points/47.6363726,-122.1357068"), It.IsAny<CancellationToken>()))
            .Returns<ResourceSpecification, CancellationToken>((_, _) =>
            {
                return Task.FromResult(new LocalExtensibilityOperationResponse(null, new ErrorData(new Error() { Code = "Code", Message = "Error message" })));
            });

        var dispatcher = BicepTestConstants.CreateModuleDispatcher(services.Build().Construct<IServiceProvider>());
        await using LocalExtensibilityHostManager extensibilityHandler = new(dispatcher, uri => Task.FromResult(extensionMock.Object));
        await extensibilityHandler.InitializeExtensions(result.Compilation);

        var localDeployResult = await LocalDeployment.Deploy(extensibilityHandler, templateFile, parametersFile, TestContext.CancellationTokenSource.Token);

        localDeployResult.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Failed, because: "Extension returned a failure when attempting to create a resource.");
        localDeployResult.Deployment.Properties.Error.Should().NotBeNull();

        localDeployResult.Deployment.Properties.Error.Code.Should().Be("DeploymentFailed");
        localDeployResult.Deployment.Properties.Error.Details.Should().NotBeNullOrEmpty();
        localDeployResult.Deployment.Properties.Error.Details[0].Code.Should().Be("Code");
        localDeployResult.Deployment.Properties.Error.Details[0].Message.Should().Be("Error message");
    }
}

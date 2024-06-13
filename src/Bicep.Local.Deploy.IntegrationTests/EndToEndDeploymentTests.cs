// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Extensibility.Messages;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Local.Deploy;
using Bicep.Local.Deploy.Extensibility;
using Bicep.Local.Extension;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.Local.Deploy.IntegrationTests;

[TestClass]
public class EndToEndDeploymentTests : TestBase
{
    [TestMethod]
    public async Task End_to_end_deployment_basic()
    {
        var services = await ProviderTestHelper.GetServiceBuilderWithPublishedProvider(ThirdPartyTypeHelper.GetHttpProviderTypesTgz(), new(ExtensibilityEnabled: true, ProviderRegistry: true, LocalDeployEnabled: true));

        var result = await CompilationHelper.RestoreAndCompileParams(services,
            ("bicepconfig.json", """
{
  "providers": {
    "http": "br:example.azurecr.io/providers/foo:1.2.3"
  },
  "experimentalFeaturesEnabled": {
    "extensibility": true,
    "providerRegistry": true,
    "localDeploy": true
  }
}
"""),
            ("main.bicep", """
provider http

param coords {
  lattitude: string
  longitude: string
}

resource gridpointsReq 'request@v1' = {
  uri: 'https://api.weather.gov/points/${coords.lattitude},${coords.longitude}'
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
  lattitude: '47.6363726'
  longitude: '-122.1357068'
}
"""));

        result.Should().NotHaveAnyDiagnostics();

        var parametersFile = result.Compilation.Emitter.Parameters().Parameters!;
        var templateFile = result.Compilation.Emitter.Parameters().Template!.Template!;

        var providerMock = StrictMock.Of<LocalExtensibilityProvider>();
        providerMock.Setup(x => x.Save(It.Is<ExtensibilityOperationRequest>(req => req.Resource.Properties["uri"]!.ToString() == "https://api.weather.gov/points/47.6363726,-122.1357068"), It.IsAny<CancellationToken>()))
            .Returns<ExtensibilityOperationRequest, CancellationToken>((req, _) =>
            {
                req.Resource.Properties["body"] = """
{
  "properties": {
    "gridId": "SEW",
    "gridX": "131",
    "gridY": "68"
  }
}
""";

                return Task.FromResult<ExtensibilityOperationResponse>(new(req.Resource, null, null));
            });
        providerMock.Setup(x => x.Save(It.Is<ExtensibilityOperationRequest>(req => req.Resource.Properties["uri"]!.ToString() == "https://api.weather.gov/gridpoints/SEW/131,68/forecast"), It.IsAny<CancellationToken>()))
            .Returns<ExtensibilityOperationRequest, CancellationToken>((req, _) =>
            {
                req.Resource.Properties["body"] = """
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

                return Task.FromResult<ExtensibilityOperationResponse>(new(req.Resource, null, null));
            });

        await using LocalExtensibilityHandler extensibilityHandler = new(BicepTestConstants.ModuleDispatcher, uri => Task.FromResult(providerMock.Object));
        await extensibilityHandler.InitializeProviders(result.Compilation);

        var localDeployResult = await LocalDeployment.Deploy(extensibilityHandler, templateFile, parametersFile, TestContext.CancellationTokenSource.Token);

        localDeployResult.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Succeeded);
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
}

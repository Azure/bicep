// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDeploymentParametersHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private readonly ISerializer Serializer = StrictMock.Of<ISerializer>().Object;
        private readonly IDeploymentFileCompilationCache DeploymentFileCompilationCache = new DeploymentFileCompilationCache();

        [TestMethod]
        public async Task Handle_WithNoParamsInSourceFile_ShouldReturnEmptyListOfUpdatedDeploymentParameters()
        {
            var bicepFileContents = @"var test = 'abc'";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.20.27533"",
      ""templateHash"": ""6882627226792194393""
    }
  },
  ""variables"": {
    ""test"": ""abc""
  },
  ""resources"": []
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, string.Empty);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().BeEmpty();
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithUnusedParamInSourceFile_ShouldReturnUpdatedDeploymentParameters()
        {
            var bicepFileContents = @"param test string = 'abc'";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.18.56646"",
      ""templateHash"": ""2028391450931931217""
    }
  },
  ""parameters"": {
    ""test"": {
      ""type"": ""string"",
      ""defaultValue"": ""abc""
    }
  },
  ""resources"": []
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, bicepFileContents);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("test");
                    updatedParam.value.Should().Be("abc");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeFalse();
                });
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithOnlyDefaultValues_ShouldReturnUpdatedDeploymentParameters()
        {
            var bicepFileContents = @"param name string = 'test'
param location string = 'global
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: name
  location: location
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.18.56646"",
      ""templateHash"": ""3422964353444461889""
    }
  },
  ""parameters"": {
    ""name"": {
      ""type"": ""string"",
      ""defaultValue"": ""test""
    },
    ""location"": {
      ""type"": ""string"",
      ""defaultValue"": ""global""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Network/dnsZones"",
      ""apiVersion"": ""2018-05-01"",
      ""name"": ""[parameters('name')]"",
      ""location"": ""[parameters('location')]""
    }
  ]
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, bicepFileContents);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("name");
                    updatedParam.value.Should().Be("test");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeFalse();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("location");
                    updatedParam.value.Should().Be("global");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeFalse();
                });
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithDefaultValuesAndParametersFile_ShouldReturnUpdatedDeploymentParameters()
        {
            var bicepFileContents = @"param name string = 'test'
param location string
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: name
  location: location
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.18.56646"",
      ""templateHash"": ""3422964353444461889""
    }
  },
  ""parameters"": {
    ""name"": {
      ""type"": ""string"",
      ""defaultValue"": ""test""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Network/dnsZones"",
      ""apiVersion"": ""2018-05-01"",
      ""name"": ""[parameters('name')]"",
      ""location"": ""[parameters('location')]""
    }
  ]
}";
            var parametersFileContents = @"{
    ""location"": {
      ""value"": ""westus""
    }
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, bicepFileContents);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, parametersFilePath, template, CancellationToken.None);

            result.deploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("name");
                    updatedParam.value.Should().Be("test");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeFalse();
                });
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithMissingParametersFileAndDefaultValue_ShouldReturnUpdatedDeploymentParameters()
        {
            var bicepFileContents = @"param name string = 'test'
param location string
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: name
  location: location
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.18.56646"",
      ""templateHash"": ""3422964353444461889""
    }
  },
  ""parameters"": {
    ""name"": {
      ""type"": ""string"",
      ""defaultValue"": ""test""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Network/dnsZones"",
      ""apiVersion"": ""2018-05-01"",
      ""name"": ""[parameters('name')]"",
      ""location"": ""[parameters('location')]""
    }
  ]
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, bicepFileContents);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("name");
                    updatedParam.value.Should().Be("test");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeFalse();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("location");
                    updatedParam.value.Should().BeNull();
                    updatedParam.isMissingParam.Should().BeTrue();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeFalse();
                });
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_ParameterWithDefaultValueAndEntryInParametersFile_ShouldIgnoreParameter()
        {
            var bicepFileContents = @"param name string = 'test'
param location string = 'eastus'
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: name
  location: location
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.18.56646"",
      ""templateHash"": ""3422964353444461889""
    }
  },
  ""parameters"": {
    ""name"": {
      ""type"": ""string"",
      ""defaultValue"": ""test""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Network/dnsZones"",
      ""apiVersion"": ""2018-05-01"",
      ""name"": ""[parameters('name')]"",
      ""location"": ""[parameters('location')]""
    }
  ]
}";
            var parametersFileContents = @"{
    ""location"": {
      ""value"": ""westus""
    }
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, bicepFileContents);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, parametersFilePath, template, CancellationToken.None);

            result.deploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("name");
                    updatedParam.value.Should().Be("test");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeFalse();
                });
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithParameterOfTypeObjectAndDefaultValue_ShouldReturnEmptyListOfUpdatedDeploymentParameters()
        {
            var bicepFileContents = @"resource blueprintName_policyArtifact 'Microsoft.Blueprint/blueprints/artifacts@2018-11-01-preview' = {
  name: 'name/policyArtifact'
  kind: 'policyAssignment'
  properties: testProperties
}
param testProperties object = {
  displayName: 'Blocked Resource Types policy definition'
  description: 'Block certain resource types'
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.44.5715"",
      ""templateHash"": ""15862569082920623108""
    }
  },
  ""parameters"": {
    ""testProperties"": {
      ""type"": ""object"",
      ""defaultValue"": {
        ""displayName"": ""Blocked Resource Types policy definition"",
        ""description"": ""Block certain resource types""
      }
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Blueprint/blueprints/artifacts"",
      ""apiVersion"": ""2018-11-01-preview"",
      ""name"": ""name/policyArtifact"",
      ""kind"": ""policyAssignment"",
      ""properties"": ""[parameters('testProperties')]""
    }
  ]
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, string.Empty);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().BeEmpty();
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithParameterOfTypeArrayAndDefaultValue_ShouldReturnEmptyListOfUpdatedDeploymentParameters()
        {
            var bicepFileContents = @"resource blueprintName_policyArtifact 'Microsoft.Blueprint/blueprints/artifacts@2018-11-01-preview' = {
  name: 'name/policyArtifact'
  kind: 'policyAssignment'
  allowedOrigins: allowedOrigins
}
param allowedOrigins array = [
  'https://foo.com'
  'https://bar.com'
]";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.44.5715"",
      ""templateHash"": ""15862569082920623108""
    }
  },
  ""parameters"": {
    ""testProperties"": {
      ""type"": ""object"",
      ""defaultValue"": {
        ""displayName"": ""Blocked Resource Types policy definition"",
        ""description"": ""Block certain resource types""
      }
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Blueprint/blueprints/artifacts"",
      ""apiVersion"": ""2018-11-01-preview"",
      ""name"": ""name/policyArtifact"",
      ""kind"": ""policyAssignment"",
      ""properties"": ""[parameters('testProperties')]""
    }
  ]
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, string.Empty);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().BeEmpty();
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithParameterOfTypeObjectAndNoDefaultValue_ShouldReturnBicepDeploymentParametersResponseWithErrorMessage()
        {
            var bicepFileContents = @"resource blueprintName_policyArtifact 'Microsoft.Blueprint/blueprints/artifacts@2018-11-01-preview' = {
  name: 'name/policyArtifact'
  kind: 'policyAssignment'
  properties: testProperties
}
param testProperties object";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.44.5715"",
      ""templateHash"": ""15862569082920623108""
    }
  },
  ""parameters"": {
    ""testProperties"": {
      ""type"": ""object"",
      ""defaultValue"": {
        ""displayName"": ""Blocked Resource Types policy definition"",
        ""description"": ""Block certain resource types""
      }
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Blueprint/blueprints/artifacts"",
      ""apiVersion"": ""2018-11-01-preview"",
      ""name"": ""name/policyArtifact"",
      ""kind"": ""policyAssignment"",
      ""properties"": ""[parameters('testProperties')]""
    }
  ]
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, bicepFileContents);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().BeEmpty();
            result.errorMessage.Should().BeEquivalentToIgnoringNewlines(string.Format(LangServerResources.MissingParamValueForArrayOrObjectType, "testProperties"));
        }

        [TestMethod]
        public async Task Handle_ParameterWithDefaultValuesOfTypeExpression_ShouldReturnUpdatedDeploymentParametersWithIsExpressionSetToTrue()
        {
            var bicepFileContents = @"param location string = resourceGroup().location
param deploymentLocation string = 'deploy-${resourceGroup().location}'
param dataFactoryName string = 'datafactory${uniqueString(resourceGroup().id)}'
param policyDefinitionId string = resourceId('Microsoft.Network/virtualNetworks/subnets', 'virtualNetworkName_var', 'subnet1Name')
resource blueprintName_policyArtifact 'Microsoft.Blueprint/blueprints/artifacts@2018-11-01-preview' = {
  name: 'name/policyArtifact'
  kind: 'policyAssignment'
  location: location
  properties: {
    policyDefinitionId: policyDefinitionId
  }
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.46.5435"",
      ""templateHash"": ""8792531277105895125""
    }
  },
  ""parameters"": {
    ""location"": {
      ""type"": ""string"",
      ""defaultValue"": ""[resourceGroup().location]""
    },
    ""deploymentLocation"": {
      ""type"": ""string"",
      ""defaultValue"": ""[concat('deploy-',resourceGroup().location)]""
    },
    ""dataFactoryName"": {
        ""type"": ""string"",
        ""defaultValue"": ""[format('datafactory{0}', uniqueString(resourceGroup().id))]""
    },
    ""policyDefinitionId"": {
      ""type"": ""string"",
      ""defaultValue"": ""[resourceId('Microsoft.Network/virtualNetworks/subnets', 'virtualNetworkName_var', 'subnet1Name')]""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Blueprint/blueprints/artifacts"",
      ""apiVersion"": ""2018-11-01-preview"",
      ""name"": ""name/policyArtifact"",
      ""kind"": ""policyAssignment"",
      ""location"": ""[parameters('location')]"",
      ""properties"": {
        ""policyDefinitionId"": ""[parameters('policyDefinitionId')]""
      }
    }
  ]
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, bicepFileContents);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("location");
                    updatedParam.value.Should().Be("resourceGroup().location");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeTrue();
                    updatedParam.isSecure.Should().BeFalse();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("deploymentLocation");
                    updatedParam.value.Should().Be("concat('deploy-',resourceGroup().location)");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeTrue();
                    updatedParam.isSecure.Should().BeFalse();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("dataFactoryName");
                    updatedParam.value.Should().Be("format('datafactory{0}', uniqueString(resourceGroup().id))");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeTrue();
                    updatedParam.isSecure.Should().BeFalse();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("policyDefinitionId");
                    updatedParam.value.Should().Be("resourceId('Microsoft.Network/virtualNetworks/subnets', 'virtualNetworkName_var', 'subnet1Name')");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeTrue();
                    updatedParam.isSecure.Should().BeFalse();
                });
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithInvalidParametersFileContents_ShouldReturnBicepDeploymentParametersResponseWithErrorMessage()
        {
            var bicepFileContents = @"param name string = 'test'
param location string = 'eastus'
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: name
  location: location
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.18.56646"",
      ""templateHash"": ""3422964353444461889""
    }
  },
  ""parameters"": {
    ""name"": {
      ""type"": ""string"",
      ""defaultValue"": ""test""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Network/dnsZones"",
      ""apiVersion"": ""2018-05-01"",
      ""name"": ""[parameters('name')]"",
      ""location"": ""[parameters('location')]""
    }
  ]
}";
            var parametersFileContents = @"{
    ""location"": {
      ""value"": ""westus""
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, string.Empty);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, parametersFilePath, template, CancellationToken.None);

            result.errorMessage.Should().NotBeNull();
            result.errorMessage.Should().Be(string.Format(LangServerResources.InvalidParameterFile, parametersFilePath, "Unexpected end of content while loading JObject. Path 'location', line 4, position 1."));
        }

        [TestMethod]
        public async Task Handle_WithSecureParams_ShouldReturnUpdatedDeploymentParametersWithIsSecureSetToTrue()
        {
            var bicepFileContents = @"@secure()
param adminUsername string = 'abc'
@secure()
param location string
param zoneType string = 'Private'
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: adminUsername
  location: location
  properties:{
    zoneType: zoneType
  }
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.68.62354"",
      ""templateHash"": ""2266028446417982813""
    }
  },
  ""parameters"": {
    ""adminUsername"": {
      ""type"": ""securestring"",
      ""defaultValue"": ""abc""
    },
    ""location"": {
      ""type"": ""securestring""
    },
    ""zoneType"": {
      ""type"": ""string"",
      ""defaultValue"": ""Private""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Network/dnsZones"",
      ""apiVersion"": ""2018-05-01"",
      ""name"": ""[parameters('adminUsername')]"",
      ""location"": ""[parameters('location')]"",
      ""properties"": {
        ""zoneType"": ""[parameters('zoneType')]""
      }
    }
  ]
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, bicepFileContents);

            var result = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            result.deploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("adminUsername");
                    updatedParam.value.Should().Be("abc");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeTrue();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("location");
                    updatedParam.value.Should().BeNull();
                    updatedParam.isMissingParam.Should().BeTrue();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeTrue();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("zoneType");
                    updatedParam.value.Should().Be("Private");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.isExpression.Should().BeFalse();
                    updatedParam.isSecure.Should().BeFalse();
                });
            result.errorMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task Handle_WithValidInput_VerifyNoEntryInDeploymentFileCompilationCache()
        {
            var bicepFileContents = @"param name string = 'test'
param location string = 'global
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: name
  location: location
}";
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""metadata"": {
    ""_generator"": {
      ""name"": ""bicep"",
      ""version"": ""0.6.18.56646"",
      ""templateHash"": ""3422964353444461889""
    }
  },
  ""parameters"": {
    ""name"": {
      ""type"": ""string"",
      ""defaultValue"": ""test""
    },
    ""location"": {
      ""type"": ""string"",
      ""defaultValue"": ""global""
    }
  },
  ""resources"": [
    {
      ""type"": ""Microsoft.Network/dnsZones"",
      ""apiVersion"": ""2018-05-01"",
      ""name"": ""[parameters('name')]"",
      ""location"": ""[parameters('location')]""
    }
  ]
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            DeploymentFileCompilationCache.CacheCompilation(documentUri, compilation);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(DeploymentFileCompilationCache, Serializer);

            await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            Assert.IsNull(DeploymentFileCompilationCache.FindAndRemoveCompilation(documentUri));
        }

        [DataTestMethod]
        [DataRow("param test string = 'test'", ParameterType.String)]
        [DataRow("param test int = 1", ParameterType.Int)]
        [DataRow("param test bool = true", ParameterType.Bool)]
        [DataRow(@"param test array = [
  1
  2
]", ParameterType.Array)]
        [DataRow(@"param test object = {
  displayName: 'Blocked Resource Types policy definition'
  description: 'Block certain resource types'
}", ParameterType.Object)]
        [DataRow(@"type foo = bar
type bar = baz
type baz = quux
type quux = (1|2|3|4|5)[]
param test foo", ParameterType.Array)]
        [DataRow("param test ", null)]
        public async Task VerifyParameterType(string bicepFileContents, ParameterType? expected)
        {
            var outputPath = FileHelper.GetUniqueTestOutputPath(TestContext);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents, outputPath);
            var compiler = new ServiceBuilder().Build().GetCompiler();
            var compilation = await compiler.CreateCompilation(PathHelper.FilePathToFileUrl(bicepFilePath).ToIOUri());
            var parameterSymbol = compilation.GetEntrypointSemanticModel().Binder.FileSymbol.ParameterDeclarations.Single();

            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, string.Empty);

            var result = bicepDeploymentParametersHandler.GetParameterType(parameterSymbol);

            result.Should().Be(expected);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow("some_path")]
        public void GetParametersInfoFromProvidedFile_WithInvalidInput_ShouldReturnNull(string parametersFilePath)
        {
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, string.Empty);

            var result = bicepDeploymentParametersHandler.GetParametersInfoFromProvidedFile(parametersFilePath);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetParametersInfoFromProvidedFile_WithNonArmTemplateFormatParametersFile_ShouldReturnParametersInfo()
        {
            var parametersFileContents = @"{
    ""location"": {
      ""value"": ""westus""
    },
    ""sku"": {
      ""value"": 1
    }
}";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, string.Empty);

            var result = bicepDeploymentParametersHandler.GetParametersInfoFromProvidedFile(parametersFilePath);

            result.Should().NotBeNull();
            result!.Should().ContainKey("location");
            result!.Should().ContainKey("sku");

            var locationObject = result!["location"] as JObject;

            locationObject!.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": ""westus""
}");

            var skuObject = result!["sku"] as JObject;
            skuObject!.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": 1
}");
        }

        [TestMethod]
        public void GetParametersInfoFromProvidedFile_WithArmTemplateFormatParametersFile_ShouldReturnParametersInfo()
        {
            var parametersFileContents = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""exampleString"": {
      ""value"": ""test string""
    },
    ""exampleInt"": {
      ""value"": 4
    },
    ""exampleBool"": {
      ""value"": true
    },
    ""exampleArray"": {
      ""value"": [
        ""value 1"",
        ""value 2""
      ]
    },
    ""exampleObject"": {
      ""value"": {
        ""property1"": ""value1"",
        ""property2"": ""value2""
      }
    }
  }
}";
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", string.Empty);
            var bicepDeploymentParametersHandler = GetBicepDeploymentParametersHandler(bicepFilePath, string.Empty);

            var result = bicepDeploymentParametersHandler.GetParametersInfoFromProvidedFile(parametersFilePath);

            result.Should().NotBeNull();
            result!.Count().Should().Be(5);
            result!.Should().ContainKey("exampleString");
            result!.Should().ContainKey("exampleInt");
            result!.Should().ContainKey("exampleBool");
            result!.Should().ContainKey("exampleArray");
            result!.Should().ContainKey("exampleObject");

            var exampleStringObject = result!["exampleString"] as JObject;
            exampleStringObject!.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": ""test string""
}");

            var exampleIntObject = result!["exampleInt"] as JObject;
            exampleIntObject!.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": 4
}");
            var exampleBoolObject = result!["exampleBool"] as JObject;
            exampleBoolObject!.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": true
}");
            var exampleArrayObject = result!["exampleArray"] as JObject;
            exampleArrayObject!.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": [
    ""value 1"",
    ""value 2""
  ]
}");
            var exampleObject = result!["exampleObject"] as JObject;
            exampleObject!.ToString().Should().BeEquivalentToIgnoringNewlines(@"{
  ""value"": {
    ""property1"": ""value1"",
    ""property2"": ""value2""
  }
}");
        }

        private BicepDeploymentParametersHandler GetBicepDeploymentParametersHandler(string bicepFilePath, string bicepFileContents)
        {
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            DeploymentFileCompilationCache.CacheCompilation(documentUri, compilation);

            return new BicepDeploymentParametersHandler(DeploymentFileCompilationCache, Serializer);
        }
    }
}

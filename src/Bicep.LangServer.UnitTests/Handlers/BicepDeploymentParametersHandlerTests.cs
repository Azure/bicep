// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().BeEmpty();
            expected.errorMessage.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Handle_WithUnusedParamInSourceFile_ShouldReturnEmptyListOfUpdatedDeploymentParameters()
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().BeEmpty();
            expected.errorMessage.Should().BeEmpty();
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("name");
                    updatedParam.value.Should().Be("test");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.showDefaultValue.Should().BeTrue();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("location");
                    updatedParam.value.Should().Be("global");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.showDefaultValue.Should().BeTrue();
                });
            expected.errorMessage.Should().BeEmpty();
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, parametersFilePath, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("name");
                    updatedParam.value.Should().Be("test");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.showDefaultValue.Should().BeTrue();
                });
            expected.errorMessage.Should().BeEmpty();
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("name");
                    updatedParam.value.Should().Be("test");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.showDefaultValue.Should().BeTrue();
                },
                updatedParam =>
                {
                    updatedParam.name.Should().Be("location");
                    updatedParam.value.Should().BeNull();
                    updatedParam.isMissingParam.Should().BeTrue();
                    updatedParam.showDefaultValue.Should().BeFalse();
                });
            expected.errorMessage.Should().BeEmpty();
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", parametersFileContents);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, parametersFilePath, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().SatisfyRespectively(
                updatedParam =>
                {
                    updatedParam.name.Should().Be("name");
                    updatedParam.value.Should().Be("test");
                    updatedParam.isMissingParam.Should().BeFalse();
                    updatedParam.showDefaultValue.Should().BeTrue();
                });
            expected.errorMessage.Should().BeEmpty();
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().BeEmpty();
            expected.errorMessage.Should().BeEmpty();
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().BeEmpty();
            expected.errorMessage.Should().BeEmpty();
        }

        [TestMethod]
        public async Task Handle_WithParameterOfTypeObjectAndNoDefaultValue_ShouldReturnUpdatedDeploymentParameterWithShowDefaultSetToFalse()
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
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var bicepDeploymentParametersHandler = new BicepDeploymentParametersHandler(bicepCompilationManager, Serializer);

            var expected = await bicepDeploymentParametersHandler.Handle(bicepFilePath, string.Empty, template, CancellationToken.None);

            expected.bicepUpdatedDeploymentParameters.Should().BeEmpty();
            expected.errorMessage.Should().BeEquivalentToIgnoringNewlines("Following parameters of type object should either contain a default value in bicep file or must be specified in parameters.json file: testProperties");
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Deploy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.LangServer.UnitTests.Deploy
{
    [TestClass]

    public class DeploymentHelperTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("invalid_scope")]
        [DataTestMethod]
        public async Task CreateDeployment_WithInvalidScope_ReturnsDeploymentFailedMessage(string scope)
        {
            var armClient = CreateMockArmClient();
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), scope))
                .Throws(new Exception(string.Format(LangServerResources.UnsupportedTargetScopeMessage, scope)));
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                armClient,
                documentPath,
                string.Empty,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                scope,
                string.Empty);

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath,
                string.Format(LangServerResources.UnsupportedTargetScopeMessage, scope));

            result.Should().Be("Failed");
            outputMessage.Should().Be(expectedDeploymentOutputMessage);
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task CreateDeployment_WithSubscriptionScopeAndInvalidLocation_ReturnsDeploymentFailedMessage(string location)
        {
            var armClient = CreateMockArmClient();
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                CreateDeploymentCollectionProvider(),
                armClient,
                documentPath,
                string.Empty,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeSubscription,
                location);

            result.Should().Be("Failed");
            outputMessage.Should().Be(string.Format(LangServerResources.MissingLocationDeploymentFailedMessage, documentPath));
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task CreateDeployment_WithManagementGroupScopeAndInvalidLocation_ReturnsDeploymentFailedMessage(string location)
        {
            var armClient = CreateMockArmClient();
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                CreateDeploymentCollectionProvider(),
                armClient,
                documentPath,
                string.Empty,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeManagementGroup,
                location);

            result.Should().Be("Failed");
            outputMessage.Should().Be(string.Format(LangServerResources.MissingLocationDeploymentFailedMessage, documentPath));
        }

        [TestMethod]
        public async Task CreateDeployment_WithTenantScope_ReturnsDeploymentNotSupportedMessage()
        {
            var armClient = CreateMockArmClient();
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeTenant))
                .Throws(new Exception(string.Format(LangServerResources.UnsupportedTargetScopeMessage, LanguageConstants.TargetScopeTypeTenant)));
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                armClient,
                documentPath,
                string.Empty,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeTenant,
                string.Empty);

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath,
                string.Format(LangServerResources.UnsupportedTargetScopeMessage, LanguageConstants.TargetScopeTypeTenant));

            result.Should().Be("Failed");
            outputMessage.Should().Be(expectedDeploymentOutputMessage);
        }

        [DataRow(LanguageConstants.TargetScopeTypeManagementGroup, "eastus")]
        [DataRow(LanguageConstants.TargetScopeTypeResourceGroup, "")]
        [DataRow(LanguageConstants.TargetScopeTypeSubscription, "eastus")]
        [DataTestMethod]
        public async Task CreateDeployment_WithValidScopeAndInput_ReturnsDeploymentSucceededMessage(string scope, string location)
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-06-01"",
      ""name"": ""storageaccount"",
      ""location"": ""[resourceGroup().location]"",
      ""properties"": {}
    }
  ]
}";
            var deploymentCollection = CreateDeploymentCollection(scope);
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), scope))
                .Returns(deploymentCollection);
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                scope,
                location);

            result.Should().Be("Succeeded");
            outputMessage.Should().Be(string.Format(LangServerResources.DeploymentSucceededMessage, documentPath));
        }

        [TestMethod]
        public async Task CreateDeployment_WithInvalidValidParameterFilePath_ReturnsDeploymentFailedMessage()
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-06-01"",
      ""name"": ""storageaccount"",
      ""location"": ""[resourceGroup().location]"",
      ""properties"": {}
    }
  ]
}";
            var deploymentCollection = CreateDeploymentCollection(LanguageConstants.TargetScopeTypeSubscription);
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeSubscription))
                .Returns(deploymentCollection);
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                @"c:\parameter.json",
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeSubscription,
                "eastus");

            result.Should().Be("Failed");
            outputMessage.Should().Contain(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, documentPath, @"Could not find file"));
        }

        [TestMethod]
        public async Task CreateDeployment_WithInvalidValidParameterFileContents_ReturnsDeploymentFailesMessage()
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-06-01"",
      ""name"": ""storageaccount"",
      ""location"": ""[resourceGroup().location]"",
      ""properties"": {}
    }
  ]
}";
            var deploymentCollection = CreateDeploymentCollection(LanguageConstants.TargetScopeTypeSubscription);
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeSubscription))
                .Returns(deploymentCollection);
            string parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", "invalid_parameters_file");
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                parametersFilePath,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeSubscription,
                "eastus");

            result.Should().Be("Failed");
            outputMessage.Should().Be(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, documentPath, @"'i' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0."));
        }

        [TestMethod]
        public async Task CreateDeployment_WithNoDeploymentCollection_ReturnsDeploymentFailedMessage()
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-06-01"",
      ""name"": ""storageaccount"",
      ""location"": ""[resourceGroup().location]"",
      ""properties"": {}
    }
  ]
}";
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Returns<DeploymentCollection>(null);
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "");

            result.Should().Be("Failed");
            outputMessage.Should().Be(string.Format(LangServerResources.DeploymentFailedMessage, documentPath));
        }

        [TestMethod]
        public async Task CreateDeployment_WithExceptionWhileFetchingDeploymentCollection_ReturnsDeploymentFailedMessage()
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-06-01"",
      ""name"": ""storageaccount"",
      ""location"": ""[resourceGroup().location]"",
      ""properties"": {}
    }
  ]
}";
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            var errorMessage = "Encountered error while fetching deployments";
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Throws(new Exception(errorMessage));
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "");

            result.Should().Be("Failed");
            outputMessage.Should().Be(string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, errorMessage));
        }

        [TestMethod]
        public async Task CreateDeployment_WhenDeploymentCreateOrUpdateOperationHasNoValue_ReturnsDeploymentFailedMessage()
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-06-01"",
      ""name"": ""storageaccount"",
      ""location"": ""[resourceGroup().location]"",
      ""properties"": {}
    }
  ]
}";
            var deploymentCreateOrUpdateOperation = StrictMock.Of<DeploymentCreateOrUpdateOperation>();
            deploymentCreateOrUpdateOperation.Setup(m => m.HasValue).Returns(false);

            var deploymentCollection = StrictMock.Of<DeploymentCollection>();
            deploymentCollection
                .Setup(m => m.CreateOrUpdateAsync(
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<DeploymentInput>(),
                    It.IsAny<CancellationToken>())).Returns(Task.FromResult(deploymentCreateOrUpdateOperation.Object));
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Returns(deploymentCollection.Object);
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "");

            result.Should().Be("Failed");
            outputMessage.Should().Be(string.Format(LangServerResources.DeploymentFailedMessage, documentPath));
        }

        [TestMethod]
        public async Task CreateDeployment_WithStatusMessageOtherThan200Or201_ReturnsDeploymentFailedMessage()
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-06-01"",
      ""name"": ""storageaccount"",
      ""location"": ""[resourceGroup().location]"",
      ""properties"": {}
    }
  ]
}";
            var response = StrictMock.Of<Response>();
            response.Setup(m => m.Status).Returns(502);
            var responseMessage = "sample response";
            response.Setup(m => m.ToString()).Returns(responseMessage);

            var deploymentCreateOrUpdateOperation = StrictMock.Of<DeploymentCreateOrUpdateOperation>();
            deploymentCreateOrUpdateOperation.Setup(m => m.HasValue).Returns(true);
            deploymentCreateOrUpdateOperation.Setup(m => m.GetRawResponse()).Returns(response.Object);

            var deploymentCollection = StrictMock.Of<DeploymentCollection>();
            deploymentCollection
                .Setup(m => m.CreateOrUpdateAsync(
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<DeploymentInput>(),
                    It.IsAny<CancellationToken>())).Returns(Task.FromResult(deploymentCreateOrUpdateOperation.Object));

            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Returns(deploymentCollection.Object);
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "");

            result.Should().Be("Failed");
            outputMessage.Should().Be(string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, responseMessage));
        }

        [TestMethod]
        public async Task CreateDeployment_WithExceptionWhileCreatingDeployment_ReturnsDeploymentFailedMessage()
        {
            var template = @"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#"",
  ""resources"": [
    {
      ""type"": ""Microsoft.Storage/storageAccounts"",
      ""apiVersion"": ""2021-06-01"",
      ""name"": ""storageaccount"",
      ""location"": ""[resourceGroup().location]"",
      ""properties"": {}
    }
  ]
}";
            var deploymentCollection = StrictMock.Of<DeploymentCollection>();
            var errorMessage = "Encountered error while creating deployment";
            deploymentCollection
                .Setup(m => m.CreateOrUpdateAsync(
                    It.IsAny<bool>(),
                    It.IsAny<string>(),
                    It.IsAny<DeploymentInput>(),
                    It.IsAny<CancellationToken>()))
                .Throws(new Exception(errorMessage));
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Returns(deploymentCollection.Object);
            var documentPath = "some_path";

            (var result, var outputMessage) = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "");

            result.Should().Be("Failed");
            outputMessage.Should().Be(string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, errorMessage));
        }

        private static ArmClient CreateMockArmClient()
        {
            var clientMock = StrictMock.Of<ArmClient>();

            return clientMock.Object;
        }

        private static IDeploymentCollectionProvider CreateDeploymentCollectionProvider()
        {
            var deploymentCollectionProviderMock = StrictMock.Of<IDeploymentCollectionProvider>();

            return deploymentCollectionProviderMock.Object;
        }

        private static DeploymentCollection CreateDeploymentCollection(string scope)
        {
            var deploymentCollection = StrictMock.Of<DeploymentCollection>();

            if (scope == LanguageConstants.TargetScopeTypeManagementGroup ||
                scope == LanguageConstants.TargetScopeTypeResourceGroup ||
                scope == LanguageConstants.TargetScopeTypeSubscription)
            {
                var deploymentCreateOrUpdateOperation = StrictMock.Of<DeploymentCreateOrUpdateOperation>();
                var response = StrictMock.Of<Response>();
                response.Setup(m => m.Status).Returns(200);
                deploymentCreateOrUpdateOperation.Setup(m => m.HasValue).Returns(true);
                deploymentCreateOrUpdateOperation.Setup(m => m.GetRawResponse()).Returns(response.Object);

                deploymentCollection
                    .Setup(m => m.CreateOrUpdateAsync(
                        It.IsAny<bool>(),
                        It.IsAny<string>(),
                        It.IsAny<DeploymentInput>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(deploymentCreateOrUpdateOperation.Object));
            }
            return deploymentCollection.Object;
        }
    }
}

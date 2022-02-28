// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Azure;
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
            var result = await DeploymentHelper.CreateDeployment(
                CreateDeploymentCollectionProvider(),
                armClient,
                string.Empty,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                scope,
                string.Empty);

            result.Should().Be(LangServerResources.UnsupportedScopeDeploymentFailedMessage);
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task CreateDeployment_WithSubscriptionScopeAndInvalidLocation_ReturnsDeploymentFailedMessage(string location)
        {
            var armClient = CreateMockArmClient();
            var result = await DeploymentHelper.CreateDeployment(
                CreateDeploymentCollectionProvider(),
                armClient,
                string.Empty,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeSubscription,
                location);

            result.Should().Be(LangServerResources.MissingLocationDeploymentFailedMessage);
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task CreateDeployment_WithManagementGroupScopeAndInvalidLocation_ReturnsDeploymentFailedMessage(string location)
        {
            var armClient = CreateMockArmClient();
            var result = await DeploymentHelper.CreateDeployment(
                CreateDeploymentCollectionProvider(),
                armClient,
                string.Empty,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeManagementGroup,
                location);

            result.Should().Be(LangServerResources.MissingLocationDeploymentFailedMessage);
        }

        [TestMethod]
        public async Task CreateDeployment_WithTenantScope_ReturnsDeploymentNotSupportedMessage()
        {
            var armClient = CreateMockArmClient();
            var result = await DeploymentHelper.CreateDeployment(
                CreateDeploymentCollectionProvider(),
                armClient,
                string.Empty,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeTenant,
                string.Empty);

            result.Should().Be(LangServerResources.UnsupportedScopeDeploymentFailedMessage);
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
                .Setup(m => m.GetDeployments(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), scope))
                .Returns(deploymentCollection);

            var result = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                scope,
                location);

            result.Should().Be(LangServerResources.DeploymentSuccessfulMessage);
        }

        [TestMethod]
        public async Task CreateDeployment_WithInvalidValidParameterFilePath_ReturnsDeploymentFailesMessage()
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
                .Setup(m => m.GetDeployments(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeSubscription))
                .Returns(deploymentCollection);

            var result = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                template,
                @"c:\parameter.json",
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeSubscription,
                "eastus");

            result.Should().Be(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, @"Could not find file 'c:\parameter.json'."));
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
                .Setup(m => m.GetDeployments(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeSubscription))
                .Returns(deploymentCollection);
            string parametersFilePath = FileHelper.SaveResultFile(TestContext, "parameters.json", "invalid_parameters_file");

            var result = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                template,
                parametersFilePath,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeSubscription,
                "eastus");

            result.Should().Be(string.Format(LangServerResources.InvalidParameterFileDeploymentFailedMessage, @"'i' is an invalid start of a value. LineNumber: 0 | BytePositionInLine: 0."));
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
                .Setup(m => m.GetDeployments(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Returns<DeploymentCollection>(null);

            var result = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "");

            result.Should().Be(LangServerResources.DeploymentFailedMessage);
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
                .Setup(m => m.GetDeployments(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Throws(new Exception(errorMessage));

            var result = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "");

            result.Should().Be(string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, errorMessage));
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
                    It.IsAny<string>(),
                    It.IsAny<DeploymentInput>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .Throws(new Exception(errorMessage));
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeployments(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Returns(deploymentCollection.Object);

            var result = await DeploymentHelper.CreateDeployment(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                template,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "");

            result.Should().Be(string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, errorMessage));
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

        private static DeploymentCollection? CreateDeploymentCollection(string scope)
        {
            if (scope == LanguageConstants.TargetScopeTypeManagementGroup ||
                scope == LanguageConstants.TargetScopeTypeResourceGroup ||
                scope == LanguageConstants.TargetScopeTypeSubscription)
            {
                var deploymentCollection = StrictMock.Of<DeploymentCollection>();
                var deploymentCreateOrUpdateAtScopeOperation = StrictMock.Of<DeploymentCreateOrUpdateAtScopeOperation>();
                var response = StrictMock.Of<Response>();
                response.Setup(m => m.Status).Returns(200);
                deploymentCreateOrUpdateAtScopeOperation.Setup(m => m.HasValue).Returns(true);
                deploymentCreateOrUpdateAtScopeOperation.Setup(m => m.GetRawResponse()).Returns(response.Object);

                deploymentCollection
                    .Setup(m => m.CreateOrUpdateAsync(
                        It.IsAny<string>(),
                        It.IsAny<DeploymentInput>(),
                        It.IsAny<bool>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(deploymentCreateOrUpdateAtScopeOperation.Object));

                return deploymentCollection.Object;
            }
            return null;
        }
    }
}

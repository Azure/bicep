// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core;
using Bicep.Core.Json;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer;
using Bicep.LanguageServer.Deploy;
using Bicep.LanguageServer.Handlers;
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
        public async Task StartDeploymentAsync_WithInvalidScope_ReturnsDeploymentFailedMessage(string scope)
        {
            var armClient = CreateMockArmClient();
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), scope))
                .Throws(new Exception(string.Format(LangServerResources.UnsupportedTargetScopeMessage, scope)));
            var documentPath = "some_path";
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider.Object,
                armClient,
                documentPath,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                scope,
                string.Empty,
                string.Empty,
                "https://portal.azure.com",
                "bicep_deployment",
                JsonElementFactory.CreateElement("{}"),
                new DeploymentOperationsCache()
                );

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath,
                string.Format(LangServerResources.UnsupportedTargetScopeMessage, scope));

            bicepDeployStartResponse.isSuccess.Should().BeFalse();
            bicepDeployStartResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
            bicepDeployStartResponse.viewDeploymentInPortalMessage.Should().BeNull();
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task StartDeploymentAsync_WithSubscriptionScopeAndInvalidLocation_ReturnsDeploymentFailedMessage(string location)
        {
            var armClient = CreateMockArmClient();
            var documentPath = "some_path";
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                CreateDeploymentCollectionProvider(),
                armClient,
                documentPath,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeSubscription,
                location,
                string.Empty,
                "https://portal.azure.com",
                "bicep_deployment",
                JsonElementFactory.CreateElement("{}"),
                new DeploymentOperationsCache());

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.MissingLocationDeploymentFailedMessage, documentPath);

            bicepDeployStartResponse.isSuccess.Should().BeFalse();
            bicepDeployStartResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
            bicepDeployStartResponse.viewDeploymentInPortalMessage.Should().BeNull();
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataTestMethod]
        public async Task StartDeploymentAsync_WithManagementGroupScopeAndInvalidLocation_ReturnsDeploymentFailedMessage(string location)
        {
            var armClient = CreateMockArmClient();
            var documentPath = "some_path";
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                CreateDeploymentCollectionProvider(),
                armClient,
                documentPath,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeManagementGroup,
                location,
                string.Empty,
                "https://portal.azure.com",
                "bicep_deployment",
                JsonElementFactory.CreateElement("{}"),
                new DeploymentOperationsCache());

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.MissingLocationDeploymentFailedMessage, documentPath);

            bicepDeployStartResponse.isSuccess.Should().BeFalse();
            bicepDeployStartResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
            bicepDeployStartResponse.viewDeploymentInPortalMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task StartDeploymentAsync_WithTenantScope_ReturnsDeploymentNotSupportedMessage()
        {
            var armClient = CreateMockArmClient();
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeTenant))
                .Throws(new Exception(string.Format(LangServerResources.UnsupportedTargetScopeMessage, LanguageConstants.TargetScopeTypeTenant)));
            var documentPath = "some_path";
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider.Object,
                armClient,
                documentPath,
                string.Empty,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41",
                LanguageConstants.TargetScopeTypeTenant,
                string.Empty,
                string.Empty,
                "https://portal.azure.com",
                "bicep_deployment",
                JsonElementFactory.CreateElement("{}"),
                new DeploymentOperationsCache());

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath,
                string.Format(LangServerResources.UnsupportedTargetScopeMessage, LanguageConstants.TargetScopeTypeTenant));

            bicepDeployStartResponse.isSuccess.Should().BeFalse();
            bicepDeployStartResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
            bicepDeployStartResponse.viewDeploymentInPortalMessage.Should().BeNull();
        }

        [DataRow(LanguageConstants.TargetScopeTypeManagementGroup, "eastus")]
        [DataRow(LanguageConstants.TargetScopeTypeResourceGroup, "")]
        [DataRow(LanguageConstants.TargetScopeTypeSubscription, "eastus")]
        [DataTestMethod]
        public async Task StartDeploymentAsync_WithValidScopeAndInput_ReturnsDeploymentSucceededMessage(string scope, string location)
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
            var deployId = "bicep_deployment";
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                scope,
                location,
                string.Empty,
                "https://portal.azure.com",
                deployId,
                JsonElementFactory.CreateElement("{}"),
                new DeploymentOperationsCache());

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentStartedMessage, documentPath);
            var expectedDeploymentLink = @"https://portal.azure.com/#blade/HubsExtension/DeploymentDetailsBlade/overview/id/%2Fsubscriptions%2F07268dd7-4c50-434b-b1ff-67b8164edb41%2FresourceGroups%2Fbhavyatest%2Fproviders%2FMicrosoft.Resources%2Fdeployments%2Fbicep_deployment";
            var expectedViewDeploymentInPortalMessage = string.Format(LangServerResources.ViewDeploymentInPortalMessage, expectedDeploymentLink);

            bicepDeployStartResponse.isSuccess.Should().BeTrue();
            bicepDeployStartResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
            bicepDeployStartResponse.viewDeploymentInPortalMessage.Should().Be(expectedViewDeploymentInPortalMessage);
        }

        [TestMethod]
        public async Task StartDeploymentAsync_WithNoDeploymentCollection_ReturnsDeploymentFailedMessage()
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
                .Returns<ArmDeploymentCollection>(null);
            var documentPath = "some_path";
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "",
                string.Empty,
                "https://portal.azure.com",
                "bicep_deployment",
                JsonElementFactory.CreateElement("{}"),
                new DeploymentOperationsCache());

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedMessage, documentPath);

            bicepDeployStartResponse.isSuccess.Should().BeFalse();
            bicepDeployStartResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
            bicepDeployStartResponse.viewDeploymentInPortalMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task StartDeploymentAsync_WithExceptionWhileFetchingDeploymentCollection_ReturnsDeploymentFailedMessage()
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
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "",
                string.Empty,
                "https://portal.azure.com",
                "bicep_deployment",
                JsonElementFactory.CreateElement("{}"),
                new DeploymentOperationsCache());

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, errorMessage);

            bicepDeployStartResponse.isSuccess.Should().BeFalse();
            bicepDeployStartResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
            bicepDeployStartResponse.viewDeploymentInPortalMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task StartDeploymentAsync_WithExceptionWhileCreatingDeployment_ReturnsDeploymentFailedMessage()
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
            var deploymentCollection = StrictMock.Of<ArmDeploymentCollection>();
            var errorMessage = "Encountered error while creating deployment";
            deploymentCollection
                .Setup(m => m.CreateOrUpdateAsync(
                    It.IsAny<WaitUntil>(),
                    It.IsAny<string>(),
                    It.IsAny<ArmDeploymentContent>(),
                    It.IsAny<CancellationToken>()))
                .Throws(new Exception(errorMessage));
            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeResourceGroup))
                .Returns(deploymentCollection.Object);
            var documentPath = "some_path";
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                documentPath,
                template,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeResourceGroup,
                "",
                string.Empty,
                "https://portal.azure.com",
                "bicep_deployment",
                JsonElementFactory.CreateElement("{}"),
                new DeploymentOperationsCache());

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, errorMessage);

            bicepDeployStartResponse.isSuccess.Should().BeFalse();
            bicepDeployStartResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
            bicepDeployStartResponse.viewDeploymentInPortalMessage.Should().BeNull();
        }

        [TestMethod]
        public async Task StartDeploymentAsync_WithValidScopeAndInput_ShouldUpdateDeploymentOperationsCache()
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

            var armDeploymentResourceOperation = StrictMock.Of<ArmOperation<ArmDeploymentResource>>().Object;
            var deploymentCollection = StrictMock.Of<ArmDeploymentCollection>();

            deploymentCollection
                .Setup(m => m.CreateOrUpdateAsync(
                    It.IsAny<WaitUntil>(),
                    It.IsAny<string>(),
                    It.IsAny<ArmDeploymentContent>(),
                    It.IsAny<CancellationToken>())).Returns(Task.FromResult(armDeploymentResourceOperation));

            var deploymentCollectionProvider = StrictMock.Of<IDeploymentCollectionProvider>();
            deploymentCollectionProvider
                .Setup(m => m.GetDeploymentCollection(It.IsAny<ArmClient>(), It.IsAny<ResourceIdentifier>(), LanguageConstants.TargetScopeTypeSubscription))
                .Returns(deploymentCollection.Object);

            var deploymentOperationsCache = new DeploymentOperationsCache();
            var deployId = "bicep_deployment1";
            var deploymentHelper = new DeploymentHelper();

            var bicepDeployStartResponse = await deploymentHelper.StartDeploymentAsync(
                deploymentCollectionProvider.Object,
                CreateMockArmClient(),
                "some_path",
                template,
                "/subscriptions/07268dd7-4c50-434b-b1ff-67b8164edb41/resourceGroups/bhavyatest",
                LanguageConstants.TargetScopeTypeSubscription,
                "eastus",
                deployId,
                "https://portal.azure.com",
                "deployment_name",
                JsonElementFactory.CreateElement("{}"),
                deploymentOperationsCache);

            deploymentOperationsCache.FindAndRemoveDeploymentOperation(deployId).Should().NotBeNull();
        }

        [TestMethod]
        public async Task WaitForDeploymentCompletionAsync_WithStatusMessageOtherThan200Or201_ReturnsDeploymentFailedMessage()
        {
            var responseMessage = "sample response";

            var armDeploymentResourceResponse = StrictMock.Of<Response<ArmDeploymentResource>>();
            armDeploymentResourceResponse.Setup(m => m.GetRawResponse().Status).Returns(502);
            armDeploymentResourceResponse.Setup(m => m.ToString()).Returns(responseMessage);

            var armDeploymentResourceOperation = StrictMock.Of<ArmOperation<ArmDeploymentResource>>();
            armDeploymentResourceOperation.Setup(m => m.WaitForCompletionAsync(CancellationToken.None)).Returns(ValueTask.FromResult(armDeploymentResourceResponse.Object));
            armDeploymentResourceOperation.Setup(m => m.HasValue).Returns(true);

            var documentPath = "some_path";
            var deploymentId = "bicep_deployment";
            var deploymentOperationsCache = new DeploymentOperationsCache();
            deploymentOperationsCache.CacheDeploymentOperation(deploymentId, armDeploymentResourceOperation.Object);

            var bicepDeployWaitForCompletionResponse = await DeploymentHelper.WaitForDeploymentCompletionAsync(
                deploymentId,
                documentPath,
                deploymentOperationsCache);

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedWithExceptionMessage, documentPath, responseMessage);

            bicepDeployWaitForCompletionResponse.isSuccess.Should().BeFalse();
            bicepDeployWaitForCompletionResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
        }

        [DataRow(200)]
        [DataRow(201)]
        [DataTestMethod]
        public async Task WaitForDeploymentCompletionAsync_WithStatusMessage200Or201_ReturnsDeploymentSucceededMessage(int status)
        {
            var responseMessage = "sample response";

            var armDeploymentResourceResponse = StrictMock.Of<Response<ArmDeploymentResource>>();
            armDeploymentResourceResponse.Setup(m => m.GetRawResponse().Status).Returns(status);
            armDeploymentResourceResponse.Setup(m => m.ToString()).Returns(responseMessage);

            var armDeploymentResourceOperation = StrictMock.Of<ArmOperation<ArmDeploymentResource>>();
            armDeploymentResourceOperation.Setup(m => m.WaitForCompletionAsync(CancellationToken.None)).Returns(ValueTask.FromResult(armDeploymentResourceResponse.Object));
            armDeploymentResourceOperation.Setup(m => m.HasValue).Returns(true);

            var documentPath = "some_path";
            var deploymentId = "bicep_deployment";
            var deploymentOperationsCache = new DeploymentOperationsCache();
            deploymentOperationsCache.CacheDeploymentOperation(deploymentId, armDeploymentResourceOperation.Object);

            var bicepDeployWaitForCompletionResponse = await DeploymentHelper.WaitForDeploymentCompletionAsync(
                deploymentId,
                documentPath,
                deploymentOperationsCache);

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentSucceededMessage, documentPath);

            bicepDeployWaitForCompletionResponse.isSuccess.Should().BeTrue();
            bicepDeployWaitForCompletionResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
        }

        [TestMethod]
        public async Task WaitForDeploymentCompletionAsync_WithNoEntryFoundForDeployIdInDeploymentOperationsCache_ReturnsDeploymentFailedMessage()
        {
            var documentPath = "some_path";
            var bicepDeployWaitForCompletionResponse = await DeploymentHelper.WaitForDeploymentCompletionAsync(
                "bicep_deployment",
                documentPath,
                new DeploymentOperationsCache());

            var expectedDeploymentOutputMessage = string.Format(LangServerResources.DeploymentFailedMessage, documentPath);

            bicepDeployWaitForCompletionResponse.isSuccess.Should().BeFalse();
            bicepDeployWaitForCompletionResponse.outputMessage.Should().Be(expectedDeploymentOutputMessage);
        }

        [TestMethod]
        public async Task WaitForDeploymentCompletionAsync_WhenCalled_RemovesDeployIdFromDeploymentOperationsCache()
        {
            var responseMessage = "sample response";

            var armDeploymentResourceResponse = StrictMock.Of<Response<ArmDeploymentResource>>();
            armDeploymentResourceResponse.Setup(m => m.GetRawResponse().Status).Returns(It.IsAny<int>);
            armDeploymentResourceResponse.Setup(m => m.ToString()).Returns(responseMessage);

            var armDeploymentResourceOperation = StrictMock.Of<ArmOperation<ArmDeploymentResource>>();
            armDeploymentResourceOperation.Setup(m => m.WaitForCompletionAsync(CancellationToken.None)).Returns(ValueTask.FromResult(armDeploymentResourceResponse.Object));
            armDeploymentResourceOperation.Setup(m => m.HasValue).Returns(true);

            var documentPath = "some_path";
            var deploymentId1 = "bicep_deployment_1";
            var deploymentId2 = "bicep_deployment_2";
            var deploymentOperationsCache = new DeploymentOperationsCache();
            deploymentOperationsCache.CacheDeploymentOperation(deploymentId1, armDeploymentResourceOperation.Object);
            deploymentOperationsCache.CacheDeploymentOperation(deploymentId2, armDeploymentResourceOperation.Object);

            await DeploymentHelper.WaitForDeploymentCompletionAsync(
                deploymentId1,
                documentPath,
                deploymentOperationsCache);

            deploymentOperationsCache.FindAndRemoveDeploymentOperation(deploymentId1).Should().BeNull();
            deploymentOperationsCache.FindAndRemoveDeploymentOperation(deploymentId2).Should().NotBeNull();
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

        private static ArmDeploymentCollection CreateDeploymentCollection(string scope)
        {
            var deploymentCollection = StrictMock.Of<ArmDeploymentCollection>();

            if (scope == LanguageConstants.TargetScopeTypeManagementGroup ||
                scope == LanguageConstants.TargetScopeTypeResourceGroup ||
                scope == LanguageConstants.TargetScopeTypeSubscription)
            {
                var armDeploymentResourceOperation = StrictMock.Of<ArmOperation<ArmDeploymentResource>>();

                deploymentCollection
                    .Setup(m => m.CreateOrUpdateAsync(
                        It.IsAny<WaitUntil>(),
                        It.IsAny<string>(),
                        It.IsAny<ArmDeploymentContent>(),
                        It.IsAny<CancellationToken>())).Returns(Task.FromResult(armDeploymentResourceOperation.Object));
            }
            return deploymentCollection.Object;
        }
    }
}

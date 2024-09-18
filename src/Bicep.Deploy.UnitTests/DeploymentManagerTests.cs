// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Mocking;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Models;
using Bicep.Core.UnitTests.Mock;
using Bicep.Deploy.Exceptions;
using FluentAssertions;
using Moq;
using static FluentAssertions.FluentActions;

namespace Bicep.Deploy.UnitTests;

[TestClass]
public class DeploymentManagerTests
{
    private static readonly string DefaultSubscriptionId = "12345678-1234-1234-1234-123456789012";
    private static readonly ResourceIdentifier Identifier = new($"/subscriptions/{DefaultSubscriptionId}/resourceGroups/myRg/providers/TestRp/TestResourceType/TestResourceName");

    [TestMethod]
    public async Task CreateOrUpdateAsync_ShouldFetchDefaultSubscription_WhenSubscriptionIdIsEmpty()
    {
        var (clientMock, mockableArmClientMock, subscriptionResource, deploymentResource) = SetupMocks();

        var armDeploymentOperation = StrictMock.Of<ArmOperation<ArmDeploymentResource>>();
        armDeploymentOperation
            .Setup(x => x.Value)
            .Returns(deploymentResource.Object);

        deploymentResource
            .Setup(x => x.UpdateAsync(It.IsAny<WaitUntil>(), It.IsAny<ArmDeploymentContent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(armDeploymentOperation.Object));

        var deploymentManager = new DeploymentManager(clientMock.Object);
        await deploymentManager.CreateOrUpdateAsync(
            new ArmDeploymentDefinition(
                null, Guid.Empty.ToString(), "myRg", "myDeployment", new ArmDeploymentProperties(ArmDeploymentMode.Incremental)),
            CancellationToken.None);

        clientMock.Verify(x => x.GetDefaultSubscriptionAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockableArmClientMock.Verify(x => x.GetArmDeploymentResource(It.Is<ResourceIdentifier>(r => r.SubscriptionId == DefaultSubscriptionId)), Times.Once);
    }

    [TestMethod]
    public async Task CreateOrUpdateAsync_ShouldThrowDeploymentException_WhenRequestFailedExceptionIsThrown()
    {
        var deploymentDefinition = new ArmDeploymentDefinition(
                null, Guid.NewGuid().ToString(), "myRg", "myDeployment", new ArmDeploymentProperties(ArmDeploymentMode.Incremental));

        var (clientMock, mockableArmClientMock, _, deploymentResource) = SetupMocks();

        deploymentResource
            .Setup(resource => resource.UpdateAsync(WaitUntil.Completed, It.IsAny<ArmDeploymentContent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Request failed"));

        var deploymentManager = new DeploymentManager(clientMock.Object);
        await Invoking(async () => await deploymentManager.CreateOrUpdateAsync(deploymentDefinition, CancellationToken.None))
            .Should()
            .ThrowAsync<DeploymentException>()
            .WithMessage("Request failed");
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldFetchDefaultSubscription_WhenSubscriptionIdIsEmpty()
    {
        var (clientMock, mockableArmClientMock, subscriptionResource, deploymentResource) = SetupMocks();

        var armValidateOperation = StrictMock.Of<ArmOperation<ArmDeploymentValidateResult>>();
        var validateResult = StrictMock.Of<ArmDeploymentValidateResult>();
        armValidateOperation
            .Setup(x => x.Value)
            .Returns(validateResult.Object);

        deploymentResource
            .Setup(x => x.ValidateAsync(It.IsAny<WaitUntil>(), It.IsAny<ArmDeploymentContent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(armValidateOperation.Object));

        var deploymentManager = new DeploymentManager(clientMock.Object);
        await deploymentManager.ValidateAsync(
            new ArmDeploymentDefinition(
                null, Guid.Empty.ToString(), "myRg", "myDeployment", new ArmDeploymentProperties(ArmDeploymentMode.Incremental)),
            CancellationToken.None);

        clientMock.Verify(x => x.GetDefaultSubscriptionAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockableArmClientMock.Verify(x => x.GetArmDeploymentResource(It.Is<ResourceIdentifier>(r => r.SubscriptionId == DefaultSubscriptionId)), Times.Once);
    }

    [TestMethod]
    public async Task ValidateAsync_ShouldThrowValidationException_WhenRequestFailedExceptionIsThrown()
    {
        var deploymentDefinition = new ArmDeploymentDefinition(
                null, Guid.NewGuid().ToString(), "myRg", "myDeployment", new ArmDeploymentProperties(ArmDeploymentMode.Incremental));

        var (clientMock, mockableArmClientMock, _, deploymentResource) = SetupMocks();

        deploymentResource
            .Setup(resource => resource.ValidateAsync(WaitUntil.Completed, It.IsAny<ArmDeploymentContent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Request failed"));

        var deploymentManager = new DeploymentManager(clientMock.Object);
        await Invoking(async () => await deploymentManager.ValidateAsync(deploymentDefinition, CancellationToken.None))
            .Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("Request failed");
    }

    [TestMethod]
    public async Task WhatIfAsync_ShouldFetchDefaultSubscription_WhenSubscriptionIdIsEmpty()
    {
        var (clientMock, mockableArmClientMock, subscriptionResource, deploymentResource) = SetupMocks();

        var armWhatIfOperation = StrictMock.Of<ArmOperation<WhatIfOperationResult>>();
        var whatIfResult = StrictMock.Of<WhatIfOperationResult>();
        armWhatIfOperation
            .Setup(x => x.Value)
            .Returns(whatIfResult.Object);

        deploymentResource
            .Setup(x => x.WhatIfAsync(It.IsAny<WaitUntil>(), It.IsAny<ArmDeploymentWhatIfContent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(armWhatIfOperation.Object));

        var deploymentManager = new DeploymentManager(clientMock.Object);
        await deploymentManager.WhatIfAsync(
            new ArmDeploymentDefinition(
                null, Guid.Empty.ToString(), "myRg", "myDeployment", new ArmDeploymentWhatIfProperties(ArmDeploymentMode.Incremental)),
            CancellationToken.None);

        clientMock.Verify(x => x.GetDefaultSubscriptionAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockableArmClientMock.Verify(x => x.GetArmDeploymentResource(It.Is<ResourceIdentifier>(r => r.SubscriptionId == DefaultSubscriptionId)), Times.Once);
    }

    [TestMethod]
    public async Task WhatIfAsync_ShouldThrowWhatIfException_WhenRequestFailedExceptionIsThrown()
    {
        var deploymentDefinition = new ArmDeploymentDefinition(
                null, Guid.NewGuid().ToString(), "myRg", "myDeployment", new ArmDeploymentWhatIfProperties(ArmDeploymentMode.Incremental));

        var (clientMock, mockableArmClientMock, _, deploymentResource) = SetupMocks();

        deploymentResource
            .Setup(resource => resource.WhatIfAsync(WaitUntil.Completed, It.IsAny<ArmDeploymentWhatIfContent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Request failed"));

        var deploymentManager = new DeploymentManager(clientMock.Object);
        await Invoking(async () => await deploymentManager.WhatIfAsync(deploymentDefinition, CancellationToken.None))
            .Should()
            .ThrowAsync<WhatIfException>()
            .WithMessage("Request failed");
    }

    private (Mock<ArmClient> clientMock, Mock<MockableResourcesArmClient> mockableArmClientMock, Mock<SubscriptionResource> subscriptionResource, Mock<ArmDeploymentResource> deploymentResource) SetupMocks()
    {
        var clientMock = StrictMock.Of<ArmClient>();
        var subscriptionResource = StrictMock.Of<SubscriptionResource>();
        var deploymentResource = StrictMock.Of<ArmDeploymentResource>();

        subscriptionResource
            .Setup(x => x.Id)
            .Returns(Identifier);
        subscriptionResource
            .Setup(x => x.Data)
            .Returns(ResourceManagerModelFactory.SubscriptionData(id: Identifier, subscriptionId: DefaultSubscriptionId));

        var mockableArmClientMock = StrictMock.Of<MockableResourcesArmClient>();

        mockableArmClientMock
            .Setup(x => x.GetArmDeploymentResource(It.IsAny<ResourceIdentifier>()))
            .Returns(deploymentResource.Object);

        clientMock
            .Setup(x => x.GetCachedClient(It.IsAny<Func<ArmClient, MockableResourcesArmClient>>()))
            .Returns(mockableArmClientMock.Object);
        clientMock
            .Setup(x => x.GetDefaultSubscriptionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscriptionResource.Object);

        return (clientMock, mockableArmClientMock, subscriptionResource, deploymentResource);
    }
}

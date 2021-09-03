// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static FluentAssertions.FluentActions;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class TemplateSpecRepositoryTests
    {
        [TestMethod]
        public void FindTemplateSpecByIdAsync_TemplateSpecNotFound_ThrowsTemplateSpecException()
        {
            var client = CreateMockClient(resourcesOperationsMock => resourcesOperationsMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(404, "Not found.")));

            var repository = new TemplateSpecRepository(client);

            Invoking(async () => await repository.FindTemplateSpecByIdAsync("foo"))
                .Should()
                .Throw<TemplateSpecException>()
                .WithMessage("The referenced template spec does not exist. Not Found.");
        }

        [TestMethod]
        public void FindTemplateSpecByIdAsync_GotUnexpectedRequestFailedException_ConvertsToTemplateSpecException()
        {
            var client = CreateMockClient(resourcesOperationsMock => resourcesOperationsMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException("Unexpected error.")));

            var repository = new TemplateSpecRepository(client);

            Invoking(async () => await repository.FindTemplateSpecByIdAsync("foo"))
                .Should()
                .Throw<TemplateSpecException>()
                .WithMessage("Unexpected error.");
        }

        [TestMethod]
        public async Task FindTemplateSpecByIdAsync_TemlateSpecFound_ReturnsTemplateSpec()
        {
            var mockResponse = CreateMockResponse(new GenericResource
            {
                Properties = new Dictionary<string, string>
                {
                    ["mainTemplate"] = "{}"
                }
            });

            var client = CreateMockClient(resourcesOperationsMock => resourcesOperationsMock
                .Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse));

            var repository = new TemplateSpecRepository(client);

            var templateSpec = await repository.FindTemplateSpecByIdAsync("foo");

            templateSpec.MainTemplateContents.Should().Be("{}");
        }

        private static ResourcesManagementClient CreateMockClient(Action<Mock<ResourcesOperations>> setupResourcesOperationsMockAction)
        {
            var resourcesOperationsMock = StrictMock.Of<ResourcesOperations>();
            setupResourcesOperationsMockAction(resourcesOperationsMock);

            var clientMock = StrictMock.Of<ResourcesManagementClient>();
            clientMock.Setup(x => x.Resources).Returns(resourcesOperationsMock.Object);

            return clientMock.Object;
        }

        private static Response<T> CreateMockResponse<T>(T value)
        {
            var responseMock = StrictMock.Of<Response<T>>();
            responseMock.SetupGet(m => m.Value).Returns(value);
            responseMock.Setup(m => m.GetRawResponse()).Returns(StrictMock.Of<Response>().Object);

            return responseMock.Object;
        }
    }
}

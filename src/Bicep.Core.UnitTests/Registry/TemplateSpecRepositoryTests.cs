// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
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
        private static readonly string TestTemplateSpecId = "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/testRG/providers/Microsoft.Resources/templateSpecs/testSpec/versions/v1";

        [TestMethod]
        public async Task FindTemplateSpecByIdAsync_TemplateSpecNotFound_ThrowsTemplateSpecException()
        {
            var templateSpecVersionMock = CreateMockTemplateSpecVersion(templateSpecVersionMock => templateSpecVersionMock
            .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(404, "Not found.")));

            var clientMock = CreateMockClient();
            var templateSpecVersionProviderMock = CreateMockTemplateSpecVersionProvider(clientMock, templateSpecVersionMock);

            var repository = new TemplateSpecRepository(clientMock, templateSpecVersionProviderMock);

            await Invoking(async () => await repository.FindTemplateSpecByIdAsync(TestTemplateSpecId))
                .Should()
                .ThrowAsync<TemplateSpecException>()
                .WithMessage("The referenced template spec does not exist. Not Found.");
        }

        [TestMethod]
        public async Task FindTemplateSpecByIdAsync_GotUnexpectedRequestFailedException_ConvertsToTemplateSpecException()
        {
            var templateSpecVersionMock = CreateMockTemplateSpecVersion(templateSpecVersionMock => templateSpecVersionMock
            .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException("Unexpected error.")));

            var clientMock = CreateMockClient();
            var templateSpecVersionProviderMock = CreateMockTemplateSpecVersionProvider(clientMock, templateSpecVersionMock);

            var repository = new TemplateSpecRepository(clientMock, templateSpecVersionProviderMock);

            await Invoking(async () => await repository.FindTemplateSpecByIdAsync(TestTemplateSpecId))
                .Should()
                .ThrowAsync<TemplateSpecException>()
                .WithMessage("Unexpected error.");
        }

        [TestMethod]
        public async Task FindTemplateSpecByIdAsync_TemlateSpecFound_ReturnsTemplateSpec()
        {
            var data = new TemplateSpecVersionData("westus")
            {
                MainTemplate = "{}"
            };

            var templateSpecVersionMock = CreateMockTemplateSpecVersion(
                templateSpecVersionMock => templateSpecVersionMock
                    .SetupGet(x => x.Data)
                    .Returns(data),
                templateSpecVersionMock => templateSpecVersionMock
                    .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(CreateMockResponse(templateSpecVersionMock.Object)));

            var clientMock = CreateMockClient();
            var templateSpecVersionProviderMock = CreateMockTemplateSpecVersionProvider(clientMock, templateSpecVersionMock);

            var repository = new TemplateSpecRepository(clientMock, templateSpecVersionProviderMock);

            var templateSpec = await repository.FindTemplateSpecByIdAsync(TestTemplateSpecId);

            templateSpec.MainTemplate.ValueEquals("{}").Should().BeTrue();
        }

        private ITemplateSpecVersionProvider CreateMockTemplateSpecVersionProvider(
            ArmClient armClient,
            TemplateSpecVersion templateSpecVersion)
        {
            var templateSpecVersionProvider = StrictMock.Of<ITemplateSpecVersionProvider>();
            templateSpecVersionProvider
                .Setup(x => x.GetTemplateSpecVersion(armClient, It.IsAny<ResourceIdentifier>()))
                .Returns(templateSpecVersion);

            return templateSpecVersionProvider.Object;
        }

        private static TemplateSpecVersion CreateMockTemplateSpecVersion(params Action<Mock<TemplateSpecVersion>>[] setUpTemplateSpecVersionMockActions)
        {
            var templateSpecVersionMock = StrictMock.Of<TemplateSpecVersion>();

            foreach (var action in setUpTemplateSpecVersionMockActions)
            {
                action.Invoke(templateSpecVersionMock);
            }

            return templateSpecVersionMock.Object;
        }

        private static ArmClient CreateMockClient()
        {
            var clientMock = StrictMock.Of<ArmClient>();

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

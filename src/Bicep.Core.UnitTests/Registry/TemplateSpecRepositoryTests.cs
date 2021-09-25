// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
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
            var client = CreateMockClient(templateSpecVersionMock => templateSpecVersionMock
                .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException(404, "Not found.")));

            var repository = new TemplateSpecRepository(client);

            await Invoking(async () => await repository.FindTemplateSpecByIdAsync(TestTemplateSpecId))
                .Should()
                .ThrowAsync<TemplateSpecException>()
                .WithMessage("The referenced template spec does not exist. Not Found.");
        }

        [TestMethod]
        public async Task FindTemplateSpecByIdAsync_GotUnexpectedRequestFailedException_ConvertsToTemplateSpecException()
        {
            var client = CreateMockClient(templateSpecVersionMock => templateSpecVersionMock
                .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException("Unexpected error.")));

            var repository = new TemplateSpecRepository(client);

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

            var client = CreateMockClient(
                templateSpecVersionMock => templateSpecVersionMock
                    .SetupGet(x => x.Data)
                    .Returns(data),
                templateSpecVersionMock => templateSpecVersionMock
                    .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(CreateMockResponse(templateSpecVersionMock.Object)));

            var repository = new TemplateSpecRepository(client);

            var templateSpec = await repository.FindTemplateSpecByIdAsync(TestTemplateSpecId);

            templateSpec.MainTemplate.ValueEquals("{}").Should().BeTrue();
        }

        private static ArmClient CreateMockClient(params Action<Mock<TemplateSpecVersion>>[] setUpTemplateSpecVersionMockActions)
        {
            var templateSpecVersionMock = StrictMock.Of<TemplateSpecVersion>();

            foreach (var action in setUpTemplateSpecVersionMockActions)
            {
                action.Invoke(templateSpecVersionMock);
            }

            var clientMock = StrictMock.Of<ArmClient>();
            //clientMock.Setup(x => x.GetTemplateSpecVersion(It.IsAny<ResourceIdentifier>())).Returns(templateSpecVersionMock.Object);
            clientMock.Setup(x => x.UseClientContext(It.IsAny<Func<Uri, TokenCredential, ArmClientOptions, HttpPipeline, TemplateSpecVersion>>()))
                .Returns(templateSpecVersionMock.Object);

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

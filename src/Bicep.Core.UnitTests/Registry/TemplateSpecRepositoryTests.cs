// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
            var templateSpecVersionResourceMock = CreateMockTemplateSpecVersionResource(
                mock => mock
                    .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new RequestFailedException(404, "Not found.")));

            var clientMock = CreateMockClient(templateSpecVersionResourceMock);

            var repository = new TemplateSpecRepository(clientMock);

            await Invoking(async () => await repository.FindTemplateSpecByIdAsync(TestTemplateSpecId))
                .Should()
                .ThrowAsync<TemplateSpecException>()
                .WithMessage("The referenced template spec does not exist. Not Found.");
        }

        [TestMethod]
        public async Task FindTemplateSpecByIdAsync_GotUnexpectedRequestFailedException_ConvertsToTemplateSpecException()
        {
            var templateSpecVersionResourceMock = CreateMockTemplateSpecVersionResource(mock =>
                mock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new RequestFailedException("Unexpected error.")));

            var clientMock = CreateMockClient(templateSpecVersionResourceMock);

            var repository = new TemplateSpecRepository(clientMock);

            await Invoking(async () => await repository.FindTemplateSpecByIdAsync(TestTemplateSpecId))
                .Should()
                .ThrowAsync<TemplateSpecException>()
                .WithMessage("Unexpected error.");
        }

        [TestMethod]
        public async Task FindTemplateSpecByIdAsync_TemlateSpecFound_ReturnsTemplateSpec()
        {
            var data = new TemplateSpecVersionData("westus");
            var content = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            var templateSpecVersionResourceMock = CreateMockTemplateSpecVersionResource(
                mock => mock
                    .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(CreateMockResponse(content)));

            var clientMock = CreateMockClient(templateSpecVersionResourceMock);

            var repository = new TemplateSpecRepository(clientMock);

            var templateSpec = await repository.FindTemplateSpecByIdAsync(TestTemplateSpecId);

            templateSpec.Content.ReplaceLineEndings().Should().Be(@"{
  ""Location"": {
    ""Name"": ""westus"",
    ""DisplayName"": ""West US""
  },
  ""Tags"": {},
  ""Description"": null,
  ""LinkedTemplates"": [],
  ""Metadata"": null,
  ""MainTemplate"": null,
  ""UiFormDefinition"": null,
  ""Id"": null,
  ""Name"": null,
  ""ResourceType"": {
    ""Namespace"": null,
    ""Type"": null
  },
  ""SystemData"": null
}".ReplaceLineEndings());
        }

        private static TemplateSpecVersionResource CreateMockTemplateSpecVersionResource(params Action<Mock<TemplateSpecVersionResource>>[] setUpTemplateSpecVersionMockActions)
        {
            var templateSpecVersionMock = StrictMock.Of<TemplateSpecVersionResource>();

            foreach (var action in setUpTemplateSpecVersionMockActions)
            {
                action.Invoke(templateSpecVersionMock);
            }

            return templateSpecVersionMock.Object;
        }

        private static ArmClient CreateMockClient(TemplateSpecVersionResource resource)
        {
            var clientMock = StrictMock.Of<ArmClient>();

            clientMock.Setup(x => x.GetResourceClient(It.IsAny<Func<TemplateSpecVersionResource>>()))
                .Returns(resource);

            return clientMock.Object;
        }

        private static Response<TemplateSpecVersionResource> CreateMockResponse(string content)
        {
            var rawResponseMock = StrictMock.Of<Response>();
            rawResponseMock.SetupGet(x => x.Content).Returns(BinaryData.FromString(content));

            var responseMock = StrictMock.Of<Response<TemplateSpecVersionResource>>();
            responseMock.Setup(m => m.GetRawResponse()).Returns(rawResponseMock.Object);

            return responseMock.Object;
        }
    }
}

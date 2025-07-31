// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Host.Handlers;
using FluentAssertions;
using Moq;

namespace Bicep.Local.Extension.UnitTests.Host;

[TestClass]
public class ResourceHandlerCollectionTests
{
    private static IResourceHandler CreateMockHandler(string? type, string? apiVersion)
    {
        var mockHandler = StrictMock.Of<IResourceHandler>();
        mockHandler.SetupGet(x => x.Type).Returns(type);
        mockHandler.SetupGet(x => x.ApiVersion).Returns(apiVersion);

        return mockHandler.Object;
    }

    [TestMethod]
    public void TryGetHandler_returns_correct_handler_with_type_and_apiversion()
    {
        var mockHandler = CreateMockHandler("Microsoft.Test/Test", "2023-01-01");

        var sut = new ResourceHandlerCollection([mockHandler]);

        sut.TryGetHandler("Microsoft.Test/Test", "2023-01-01").Should().Be(mockHandler);
        sut.TryGetHandler("Microsoft.Test/Test2", "2023-01-01").Should().Be(null);
        sut.TryGetHandler("Microsoft.Test/Test", "2024-01-01").Should().Be(null);
        sut.TryGetHandler("Microsoft.Test/Test", null).Should().Be(null);
    }

    [TestMethod]
    public void TryGetHandler_returns_correct_handler_with_type_and_no_apiversion()
    {
        var mockHandler = CreateMockHandler("Microsoft.Test/Test", null);

        var sut = new ResourceHandlerCollection([mockHandler]);

        sut.TryGetHandler("Microsoft.Test/Test", "2023-01-01").Should().Be(null);
        sut.TryGetHandler("Microsoft.Test/Test", null).Should().Be(mockHandler);
        sut.TryGetHandler("Microsoft.Test/Tes2", null).Should().Be(null);
    }

    [TestMethod]
    public void TryGetHandler_uses_generic_handler_as_fallback()
    {
        var mockHandler = CreateMockHandler("Microsoft.Test/Test", null);
        var mockGenericHandler = CreateMockHandler(null, null);

        var sut = new ResourceHandlerCollection([mockHandler, mockGenericHandler]);

        sut.TryGetHandler("Microsoft.Test/Test", null).Should().Be(mockHandler);
        sut.TryGetHandler("Microsoft.Test/Tes2", null).Should().Be(mockGenericHandler);
    }

    [TestMethod]
    public void Constructor_prevents_duplicate_registrations()
    {
        var mockHandler = CreateMockHandler("Microsoft.Test/Test", "2023-01-01");
        var mockHandler2 = CreateMockHandler("Microsoft.Test/Test", "2023-01-01");

        FluentActions.Invoking(() => new ResourceHandlerCollection([mockHandler, mockHandler2]))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("A handler for type 'Microsoft.Test/Test' with API version '2023-01-01' is already registered.");
    }

    [TestMethod]
    public void Constructor_prevents_duplicate_registrations_no_api_versions()
    {
        var mockHandler = CreateMockHandler("Microsoft.Test/Test", null);
        var mockHandler2 = CreateMockHandler("Microsoft.Test/Test", null);

        FluentActions.Invoking(() => new ResourceHandlerCollection([mockHandler, mockHandler2]))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("A handler for type 'Microsoft.Test/Test' with API version '' is already registered.");
    }

    [TestMethod]
    public void Constructor_prevents_duplicate_generic_registrations()
    {
        var mockHandler = CreateMockHandler(null, null);
        var mockHandler2 = CreateMockHandler(null, null);

        FluentActions.Invoking(() => new ResourceHandlerCollection([mockHandler, mockHandler2]))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Only one generic resource handler can be registered.");
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Frozen;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Local.Extension.UnitTests.Host.Handlers;

[TestClass]
public class ResourceHandlerDispatcherTests
{
    // Test resource types
    private class TestResource { }
    private class AnotherResource { }

    // Generic resource handler (implements only IResourceHandler)
    private class GenericResourceHandler : IResourceHandler
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Preview(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
    }

    // Typed resource handler (implements IResourceHandler<T>)
    private class TestResourceHandler : IResourceHandler<TestResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<TestResource> request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Preview(HandlerRequest<TestResource> request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
    }

    // Another typed resource handler
    private class AnotherResourceHandler : IResourceHandler<AnotherResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<AnotherResource> request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Preview(HandlerRequest<AnotherResource> request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, [], [], request.ApiVersion));
    }

    #region Constructor Tests

    [TestMethod]

    public void Constructor_WithNullResourceHandlers_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<IResourceHandler>? handlers = null;

        // Act
        var action = () => new ResourceHandlerDispatcher(handlers!);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_WithEmptyResourceHandlers_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<IResourceHandler> handlers = [];

        // Act
        var action = () => new ResourceHandlerDispatcher(handlers);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_WithOnlyGenericResourceHandler_InitializesCorrectly()
    {
        // Arrange
        IResourceHandler[] handlers = [new GenericResourceHandler()];

        // Act
        var dispatcher = new ResourceHandlerDispatcher(handlers);

        // Assert
        dispatcher.GenericResourceHandler.Should().NotBeNull();
        dispatcher.TypedResourceHandlers.Should().NotBeNull();
        dispatcher.TypedResourceHandlers.Should().BeEmpty();
    }

    [TestMethod]
    public void Constructor_WithOnlyTypedResourceHandler_InitializesCorrectly()
    {
        // Arrange
        IResourceHandler[] handlers = [new TestResourceHandler()];

        // Act
        var dispatcher = new ResourceHandlerDispatcher(handlers);

        // Assert
        dispatcher.GenericResourceHandler.Should().BeNull();
        dispatcher.TypedResourceHandlers.Should().NotBeNull();
        dispatcher.TypedResourceHandlers.Should().HaveCount(1);
        dispatcher.TypedResourceHandlers.Should().ContainKey(nameof(TestResource));
        dispatcher.TypedResourceHandlers[nameof(TestResource)].Type.Should().Be(typeof(TestResource));
    }

    [TestMethod]
    public void Constructor_WithGenericAndTypedResourceHandlers_InitializesCorrectly()
    {
        // Arrange
        IResourceHandler[] handlers =
        [
            new GenericResourceHandler(),
            new TestResourceHandler(),
            new AnotherResourceHandler()
        ];

        // Act
        var dispatcher = new ResourceHandlerDispatcher(handlers);

        // Assert
        dispatcher.GenericResourceHandler.Should().NotBeNull();
        dispatcher.TypedResourceHandlers.Should().NotBeNull();
        dispatcher.TypedResourceHandlers.Should().HaveCount(2);
        dispatcher.TypedResourceHandlers.Should().ContainKey(nameof(TestResource));
        dispatcher.TypedResourceHandlers.Should().ContainKey(nameof(AnotherResource));

        dispatcher.TypedResourceHandlers[nameof(TestResource)].Type.Should().Be(typeof(TestResource));
        dispatcher.TypedResourceHandlers[nameof(TestResource)].Handler.Should().BeOfType<TestResourceHandler>();

        dispatcher.TypedResourceHandlers[nameof(AnotherResource)].Type.Should().Be(typeof(AnotherResource));
        dispatcher.TypedResourceHandlers[nameof(AnotherResource)].Handler.Should().BeOfType<AnotherResourceHandler>();
    }

    [TestMethod]
    public void Constructor_WithDuplicateTypedResourceHandlers_ThrowsArgumentException()
    {
        // Arrange
        IResourceHandler[] handlers =
        [
            new TestResourceHandler(),
            new TestResourceHandler() // Duplicate
        ];

        // Act
        var action = () => new ResourceHandlerDispatcher(handlers);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"A resource handler for {nameof(TestResource)} has already been registered.");
    }

    [TestMethod]
    public void Constructor_WithMultipleGenericResourceHandlers_ThrowsArgumentException()
    {
        // Arrange
        IResourceHandler[] handlers =
        [
            new GenericResourceHandler(),
            new GenericResourceHandler() // Duplicate
        ];

        // Act
        var action = () => new ResourceHandlerDispatcher(handlers);

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("A generic resource handler has already been registered.");
    }

    #endregion

    #region TryGetTypedResourceHandler(Type) Tests

    [TestMethod]
    public void TryGetTypedResourceHandler_ByType_WithNullType_ThrowsArgumentNullException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new GenericResourceHandler()]);
        Type? nullType = null;

        // Act
        var action = () => dispatcher.TryGetTypedResourceHandler(nullType!, out var resourceHandler);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("resourceType");
    }

    [TestMethod]
    public void TryGetTypedResourceHandler_ByType_WithTypedHandler_ReturnsTrueAndCorrectHandler()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler]);

        // Act
        var result = dispatcher.TryGetTypedResourceHandler(typeof(TestResource), out var typedResourceHandler);

        // Assert
        result.Should().BeTrue();
        typedResourceHandler.Should().NotBeNull();
        typedResourceHandler!.Handler.Should().Be(testHandler);
        typedResourceHandler.Type.Should().Be(typeof(TestResource));
    }

    [TestMethod]
    public void TryGetTypedResourceHandler_ByType_WithGenericHandler_ReturnsFalse()
    {
        // Arrange
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([genericHandler]);

        // Act
        var result = dispatcher.TryGetTypedResourceHandler(typeof(TestResource), out var typedResourceHandler);

        // Assert
        result.Should().BeFalse();
        typedResourceHandler.Should().BeNull();
    }

    [TestMethod]
    public void TryGetTypedResourceHandler_ByType_WithoutMatchingHandler_ReturnsFalse()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var result = dispatcher.TryGetTypedResourceHandler(typeof(AnotherResource), out var typedResourceHandler);

        // Assert
        result.Should().BeFalse();
        typedResourceHandler.Should().BeNull();
    }

    #endregion

    #region TryGetTypedResourceHandler(string) Tests

    [TestMethod]
    public void TryGetTypedResourceHandler_ByString_WithNullResourceType_ThrowsArgumentNullException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new GenericResourceHandler()]);
        string? nullType = null;

        // Act
        var action = () => dispatcher.TryGetTypedResourceHandler(nullType!, out var typedResourceHandler);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("resourceType");
    }

    [TestMethod]
    public void TryGetTypedResourceHandler_ByString_WithEmptyString_ThrowsArgumentException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var action = () => dispatcher.TryGetTypedResourceHandler(string.Empty, out var typedResourceHandler);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("resourceType");
    }

    [TestMethod]
    public void TryGetTypedResourceHandler_ByString_WithWhitespaceString_ThrowsArgumentException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var action = () => dispatcher.TryGetTypedResourceHandler("   ", out var typedResourceHandler);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("resourceType");
    }

    [TestMethod]
    public void TryGetTypedResourceHandler_ByString_WithTypedHandler_ReturnsTrueAndCorrectHandler()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler]);

        // Act
        var result = dispatcher.TryGetTypedResourceHandler(nameof(TestResource), out var typedResourceHandler);

        // Assert
        result.Should().BeTrue();
        typedResourceHandler.Should().NotBeNull();
        typedResourceHandler!.Handler.Should().Be(testHandler);
        typedResourceHandler.Type.Should().Be(typeof(TestResource));
    }

    [TestMethod]
    public void TryGetTypedResourceHandler_ByString_WithGenericHandler_ReturnsFalse()
    {
        // Arrange
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([genericHandler]);

        // Act
        var result = dispatcher.TryGetTypedResourceHandler("AnyResourceType", out var typedResourceHandler);

        // Assert
        result.Should().BeFalse();
        typedResourceHandler.Should().BeNull();
    }

    [TestMethod]
    public void TryGetTypedResourceHandler_ByString_WithoutMatchingHandler_ReturnsFalse()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var result = dispatcher.TryGetTypedResourceHandler("UnknownResourceType", out var typedResourceHandler);

        // Assert
        result.Should().BeFalse();
        typedResourceHandler.Should().BeNull();
    }

    #endregion

    #region Properties Tests

    [TestMethod]
    public void TypedResourceHandlers_ReturnsCorrectDictionary()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var anotherHandler = new AnotherResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler, anotherHandler]);

        // Act
        var typedHandlers = dispatcher.TypedResourceHandlers;

        // Assert
        typedHandlers.Should().NotBeNull();
        typedHandlers.Should().BeAssignableTo<FrozenDictionary<string, TypedResourceHandler>>();
        typedHandlers.Should().HaveCount(2);
        typedHandlers.Should().ContainKey(nameof(TestResource));
        typedHandlers.Should().ContainKey(nameof(AnotherResource));
        typedHandlers[nameof(TestResource)].Handler.Should().Be(testHandler);
        typedHandlers[nameof(AnotherResource)].Handler.Should().Be(anotherHandler);
    }

    [TestMethod]
    public void GenericResourceHandler_WithGenericHandler_ReturnsHandler()
    {
        // Arrange
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([genericHandler]);

        // Act
        var result = dispatcher.GenericResourceHandler;

        // Assert
        result.Should().Be(genericHandler);
    }

    [TestMethod]
    public void GenericResourceHandler_WithoutGenericHandler_ReturnsNull()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var result = dispatcher.GenericResourceHandler;

        // Assert
        result.Should().BeNull();
    }

    #endregion
}

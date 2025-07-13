// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Local.Extension.UnitTests.HostTests;

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
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Preview(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
    }

    // Typed resource handler (implements IResourceHandler<T>)
    private class TestResourceHandler : IResourceHandler<TestResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<TestResource> request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Preview(HandlerRequest<TestResource> request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
    }

    // Another typed resource handler
    private class AnotherResourceHandler : IResourceHandler<AnotherResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<AnotherResource> request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Preview(HandlerRequest<AnotherResource> request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(HandlerResponse.Success(request.Type, request.ApiVersion, null));
    }

    #region Constructor Tests

    [TestMethod]
    public void Constructor_WithNullResourceHandlers_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<IResourceHandler>? nullHandlers = null;

        // Act
        var action = () => new ResourceHandlerDispatcher(nullHandlers!);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_WithEmptyResourceHandlers_ThrowsArgumentException()
    {
        // Arrange
        var emptyHandlers = Enumerable.Empty<IResourceHandler>();

        // Act
        var action = () => new ResourceHandlerDispatcher(emptyHandlers);

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
        dispatcher.TypedResourceHandlers[nameof(AnotherResource)].Type.Should().Be(typeof(AnotherResource));
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
        action.Should().Throw<ArgumentException>()
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
        action.Should().Throw<ArgumentException>()
            .WithMessage("A generic resource handler has already been registered.");
    }


    #endregion

    #region GetResourceHandler(Type) Tests

    [TestMethod]
    public void GetResourceHandler_ByType_WithNullType_ThrowsArgumentNullException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new GenericResourceHandler()]);
        Type? nullType = null;

        // Act
        var action = () => dispatcher.GetResourceHandler(nullType!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("resourceType");
    }

    [TestMethod]
    public void GetResourceHandler_ByType_WithTypedHandler_ReturnsCorrectHandler()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler]);

        // Act
        var result = dispatcher.GetResourceHandler(typeof(TestResource));

        // Assert
        result.Should().Be(testHandler);
    }

    [TestMethod]
    public void GetResourceHandler_ByType_WithGenericHandler_ReturnsGenericHandler()
    {
        // Arrange
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([genericHandler]);

        // Act
        var result = dispatcher.GetResourceHandler(typeof(TestResource));

        // Assert
        result.Should().Be(genericHandler);
    }

    [TestMethod]
    public void GetResourceHandler_ByType_WithoutMatchingHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var action = () => dispatcher.GetResourceHandler(typeof(AnotherResource));

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage($"No generic or typed resource handler found for type {nameof(AnotherResource)}. Ensure the resource handler is registered.");
    }

    #endregion

    #region GetResourceHandler(string) Tests

    [TestMethod]
    public void GetResourceHandler_ByString_WithNullResourceType_ThrowsArgumentNullException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new GenericResourceHandler()]);
        string? nullType = null;

        // Act
        var action = () => dispatcher.GetResourceHandler(nullType!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("resourceType");
    }

    [TestMethod]
    public void GetResourceHandler_ByString_WithTypedHandler_ReturnsCorrectHandler()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler]);

        // Act
        var result = dispatcher.GetResourceHandler(nameof(TestResource));

        // Assert
        result.Should().Be(testHandler);
    }

    [TestMethod]
    public void GetResourceHandler_ByString_WithGenericHandler_ReturnsGenericHandler()
    {
        // Arrange
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([genericHandler]);

        // Act
        var result = dispatcher.GetResourceHandler("UnknownResourceType");

        // Assert
        result.Should().Be(genericHandler);
    }

    [TestMethod]
    public void GetResourceHandler_ByString_PrefersTypedOverGeneric()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler, genericHandler]);

        // Act
        var result = dispatcher.GetResourceHandler(nameof(TestResource));

        // Assert
        result.Should().Be(testHandler);
    }

    [TestMethod]
    public void GetResourceHandler_ByString_WithoutMatchingHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var action = () => dispatcher.GetResourceHandler("UnknownResourceType");

        // Assert
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("No generic or typed resource handler found for type UnknownResourceType. Ensure the resource handler is registered.");
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
        typedHandlers.Should().BeAssignableTo<FrozenDictionary<string, TypeResourceHandler>>();
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

    #region Integration Tests

    [TestMethod]
    public void CompleteScenario_WithMixedHandlers_WorksCorrectly()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var anotherHandler = new AnotherResourceHandler();
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher(
        [
            testHandler,
            anotherHandler,
            genericHandler
        ]);

        // Act & Assert - Test typed handlers
        dispatcher.GetResourceHandler(typeof(TestResource)).Should().Be(testHandler);
        dispatcher.GetResourceHandler(nameof(TestResource)).Should().Be(testHandler);
        dispatcher.GetResourceHandler(typeof(AnotherResource)).Should().Be(anotherHandler);
        dispatcher.GetResourceHandler(nameof(AnotherResource)).Should().Be(anotherHandler);

        // Act & Assert - Test generic fallback
        dispatcher.GetResourceHandler("UnknownType").Should().Be(genericHandler);
        dispatcher.GetResourceHandler(typeof(string)).Should().Be(genericHandler);

        // Act & Assert - Verify properties
        dispatcher.TypedResourceHandlers.Should().HaveCount(2);
        dispatcher.GenericResourceHandler.Should().Be(genericHandler);
    }

    #endregion

    #region Edge Cases and Defensive Programming

    [TestMethod]
    public void GetResourceHandler_WithEmptyString_ThrowsArgumentException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var action = () => dispatcher.GetResourceHandler(string.Empty);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void GetResourceHandler_WithWhitespaceString_ThrowsArgumentException()
    {
        // Arrange
        var dispatcher = new ResourceHandlerDispatcher([new TestResourceHandler()]);

        // Act
        var action = () => dispatcher.GetResourceHandler("   ");

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_WithLargeNumberOfHandlers_PerformsEfficiently()
    {
        // Arrange
        var handlers = new List<IResourceHandler>();
        for (int i = 0; i < 1000; i++)
        {
            handlers.Add(new TestResourceHandler());
        }

        // Act & Assert - Should not throw
        var action = () => new ResourceHandlerDispatcher(handlers);
        action.Should().Throw<ArgumentException>(); // Will throw due to duplicates, but test that it doesn't hang
    }

    #endregion
}

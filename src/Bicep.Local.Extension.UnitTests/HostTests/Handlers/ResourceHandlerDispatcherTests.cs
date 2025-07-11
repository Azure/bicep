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

namespace Bicep.Local.Extension.UnitTests.HostTests.Handlers;

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
        var result = dispatcher.TryGetTypedResourceHandler("UnknownResourceType", out var typedResourceHandler);

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

    [TestMethod]
    public void TryGetTypedResourceHandler_ByString_PrefersTypedOverGeneric()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler, genericHandler]);

        // Act
        var result = dispatcher.TryGetTypedResourceHandler(nameof(TestResource), out var typedResourceHandler);

        // Assert
        result.Should().BeTrue();
        typedResourceHandler.Should().NotBeNull();
        typedResourceHandler!.Handler.Should().Be(testHandler);
        typedResourceHandler.Type.Should().Be(typeof(TestResource));
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
        var result1 = dispatcher.TryGetTypedResourceHandler(typeof(TestResource), out var typedHandler1);
        result1.Should().BeTrue();
        typedHandler1!.Handler.Should().Be(testHandler);

        var result2 = dispatcher.TryGetTypedResourceHandler(nameof(TestResource), out var typedHandler2);
        result2.Should().BeTrue();
        typedHandler2!.Handler.Should().Be(testHandler);

        var result3 = dispatcher.TryGetTypedResourceHandler(typeof(AnotherResource), out var typedHandler3);
        result3.Should().BeTrue();
        typedHandler3!.Handler.Should().Be(anotherHandler);

        var result4 = dispatcher.TryGetTypedResourceHandler(nameof(AnotherResource), out var typedHandler4);
        result4.Should().BeTrue();
        typedHandler4!.Handler.Should().Be(anotherHandler);

        // Act & Assert - Test fallback to generic (should return false for unknown types)
        var result5 = dispatcher.TryGetTypedResourceHandler("UnknownType", out var typedHandler5);
        result5.Should().BeFalse();
        typedHandler5.Should().BeNull();

        var result6 = dispatcher.TryGetTypedResourceHandler(typeof(string), out var typedHandler6);
        result6.Should().BeFalse();
        typedHandler6.Should().BeNull();

        // Act & Assert - Verify properties
        dispatcher.TypedResourceHandlers.Should().HaveCount(2);
        dispatcher.GenericResourceHandler.Should().Be(genericHandler);
    }

    [TestMethod]
    public void CompleteScenario_GetHandlerLogic_WithTypedHandlerPreferred()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler, genericHandler]);

        // Act - Simulate the logic of getting a handler with fallback to generic
        IResourceHandler? actualHandler = null;
        if (dispatcher.TryGetTypedResourceHandler(nameof(TestResource), out var typedHandler))
        {
            actualHandler = typedHandler.Handler;
        }
        else
        {
            actualHandler = dispatcher.GenericResourceHandler;
        }

        // Assert - Should get the typed handler, not the generic one
        actualHandler.Should().Be(testHandler);
    }

    [TestMethod]
    public void CompleteScenario_GetHandlerLogic_WithGenericHandlerFallback()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var genericHandler = new GenericResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler, genericHandler]);

        // Act - Simulate the logic of getting a handler with fallback to generic for unknown type
        IResourceHandler? actualHandler = null;
        if (dispatcher.TryGetTypedResourceHandler("UnknownType", out var typedHandler))
        {
            actualHandler = typedHandler.Handler;
        }
        else
        {
            actualHandler = dispatcher.GenericResourceHandler;
        }

        // Assert - Should get the generic handler as fallback
        actualHandler.Should().Be(genericHandler);
    }

    [TestMethod]
    public void CompleteScenario_GetHandlerLogic_WithNoHandlerAvailable()
    {
        // Arrange
        var testHandler = new TestResourceHandler();
        var dispatcher = new ResourceHandlerDispatcher([testHandler]); // No generic handler

        // Act - Simulate the logic of getting a handler with no fallback available
        IResourceHandler? actualHandler = null;
        if (dispatcher.TryGetTypedResourceHandler("UnknownType", out var typedHandler))
        {
            actualHandler = typedHandler.Handler;
        }
        else
        {
            actualHandler = dispatcher.GenericResourceHandler;
        }

        // Assert - Should be null when no handler is available
        actualHandler.Should().BeNull();
    }

    #endregion

    #region Edge Cases and Defensive Programming

    [TestMethod]
    public void Constructor_WithLargeNumberOfHandlers_PerformsEfficiently()
    {
        // Arrange
        var handlers = new List<IResourceHandler>();
        for (int i = 0; i < 1000; i++)
        {
            handlers.Add(new TestResourceHandler());
        }

        // Act & Assert - Should throw due to duplicates, but test that it doesn't hang
        var action = () => new ResourceHandlerDispatcher(handlers);
        action.Should().Throw<ArgumentException>();
    }

    #endregion
}

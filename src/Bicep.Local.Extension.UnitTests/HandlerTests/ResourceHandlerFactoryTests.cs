// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Host.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Local.Extension.UnitTests.HandlerTests;

[TestClass]
public class ResourceHandlerFactoryTests
{
    // Minimal test resource types
    private record TestResourceA { }
    private record TestResourceB { }

    private class TestResourceHandler<T> : IResourceHandler<T> where T : class
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<T> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Preview(HandlerRequest<T> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        Task<HandlerResponse> IResourceHandler.CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        Task<HandlerResponse> IResourceHandler.Preview(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    // Minimal generic handler implementation
    private class GenericResourceHandler : IResourceHandler
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Preview(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    [TestMethod]
    public void Constructor_Throws_When_No_Handlers_Provided()
    {
        // Scenario 1: No handlers provided
        Action act = () => new ResourceHandlerFactory(Array.Empty<IResourceHandler>());
        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Constructor_Throws_When_Duplicate_Type_Handlers_Provided()
    {
        // Scenario 2: Two handlers for the same type
        var handler1 = new TestResourceHandler<TestResourceA>();
        var handler2 = new TestResourceHandler<TestResourceA>();
        Action act = () => new ResourceHandlerFactory(new IResourceHandler[] { handler1, handler2 });
        act.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Constructor_Succeeds_When_No_Generic_Handler()
    {
        // Scenario 3: Only typed handler(s), no generic
        var handler = new TestResourceHandler<TestResourceA>();
        var factory = new ResourceHandlerFactory(new IResourceHandler[] { handler });

        factory.TypedResourceHandlers.Should().NotBeNull();
        factory.GenericResourceHandler.Should().BeNull();
        factory.TypedResourceHandlers.Should().ContainKey(nameof(TestResourceA));
    }

    [TestMethod]
    public void Constructor_Succeeds_With_Only_Generic_Handler()
    {
        // Scenario 4: Only a generic handler
        var genericHandler = new GenericResourceHandler();
        var factory = new ResourceHandlerFactory(new IResourceHandler[] { genericHandler });

        factory.TypedResourceHandlers.Count.Should().Be(0);
        factory.GenericResourceHandler.Should().NotBeNull();
    }

    [TestMethod]
    public void GetResourceHandler_Returns_Typed_Handler_When_Exists()
    {
        var handler = new TestResourceHandler<TestResourceA>();
        var factory = new ResourceHandlerFactory(new IResourceHandler[] { handler });

        var result = factory.GetResourceHandler(typeof(TestResourceA));
        result.Should().NotBeNull();
        result.Type.Should().Be(typeof(TestResourceA));
    }

    [TestMethod]
    public void GetResourceHandler_Returns_Generic_Handler_When_Typed_Not_Found()
    {
        var genericHandler = new GenericResourceHandler();
        var factory = new ResourceHandlerFactory(new IResourceHandler[] { genericHandler });

        var result = factory.GetResourceHandler("NonExistentType");
        result.Should().NotBeNull();
        result.Type.Should().Be(typeof(EmptyGeneric));
    }

    [TestMethod]
    public void GetResourceHandler_Throws_When_No_Handler_Found()
    {
        var handler = new TestResourceHandler<TestResourceA>();
        var factory = new ResourceHandlerFactory(new IResourceHandler[] { handler });

        // No generic handler, and type not registered
        Action act = () => factory.GetResourceHandler("NonExistentType");
        act.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Constructor_Throws_When_Multiple_Generic_Handlers_Provided()
    {
        var genericHandler1 = new GenericResourceHandler();
        var genericHandler2 = new GenericResourceHandler();
        Action act = () => new ResourceHandlerFactory(new IResourceHandler[] { genericHandler1, genericHandler2 });
        act.Should().Throw<ArgumentException>();
    }
}

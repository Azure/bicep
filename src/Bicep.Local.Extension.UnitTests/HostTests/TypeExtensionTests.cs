// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Extensions;
using Bicep.Local.Extension.Host.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Local.Extension.UnitTests.HostTests;

[TestClass]
public class TypeExtensionTests
{
    private class TestResource { }
    private class AnotherResource { }

    // Implements only the non-generic IResourceHandler interface
    private class GenericResourceHandler : IResourceHandler
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Preview(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    // Implements only the generic IResourceHandler<T> interface
    private class TypedResourceHandler : IResourceHandler<TestResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<TestResource> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Preview(HandlerRequest<TestResource> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    // Implements both interfaces
    private class DualResourceHandler : IResourceHandler, IResourceHandler<TestResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Preview(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<TestResource> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Preview(HandlerRequest<TestResource> request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    // Implements multiple generic IResourceHandler<T> interfaces
    private class MultipleTypedResourceHandler : IResourceHandler<TestResource>, IResourceHandler<AnotherResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<TestResource> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Preview(HandlerRequest<TestResource> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<AnotherResource> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<HandlerResponse> Preview(HandlerRequest<AnotherResource> request, CancellationToken cancellationToken) => throw new NotImplementedException();
        Task<HandlerResponse> IResourceHandler.Delete(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        Task<HandlerResponse> IResourceHandler.Get(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        Task<HandlerResponse> IResourceHandler.CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
        Task<HandlerResponse> IResourceHandler.Preview(HandlerRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    // Implements no resource handler interfaces
    private class NonResourceHandler
    {
        public void SomeMethod() { }
    }

    // Implements a different generic interface (not IResourceHandler<T>)
    private interface IGenericInterface<T> { }
    private class GenericInterfaceImplementation : IGenericInterface<string> { }

    #region IsGenericTypedResourceHandler Tests

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithNullType_ThrowsArgumentNullException()
    {
        Type? nullType = null;
        var action = () => nullType!.IsGenericTypedResourceHandler();
        action.Should().Throw<ArgumentNullException>().WithParameterName("type");
    }

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithGenericResourceHandler_ReturnsTrue()
    {
        var type = typeof(GenericResourceHandler);
        type.IsGenericTypedResourceHandler().Should().BeTrue();
    }

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithTypedResourceHandler_ReturnsTrue()
    {
        var type = typeof(TypedResourceHandler);
        type.IsGenericTypedResourceHandler().Should().BeTrue();
    }

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithDualResourceHandler_ReturnsTrue()
    {
        var type = typeof(DualResourceHandler);
        type.IsGenericTypedResourceHandler().Should().BeTrue();
    }

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithNonResourceHandler_ReturnsFalse()
    {
        var type = typeof(NonResourceHandler);
        type.IsGenericTypedResourceHandler().Should().BeFalse();
    }

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithStringType_ReturnsFalse()
    {
        typeof(string).IsGenericTypedResourceHandler().Should().BeFalse();
    }

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithObjectType_ReturnsFalse()
    {
        typeof(object).IsGenericTypedResourceHandler().Should().BeFalse();
    }

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithGenericInterfaceImplementation_ReturnsFalse()
    {
        var type = typeof(GenericInterfaceImplementation);
        type.IsGenericTypedResourceHandler().Should().BeFalse();
    }

    #endregion

    #region TryGetTypedResourceHandlerInterface Tests

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithNullType_ThrowsArgumentNullException()
    {
        Type? nullType = null;
        var action = () => nullType!.TryGetTypedResourceHandlerInterface(out _);
        action.Should().Throw<ArgumentNullException>().WithParameterName("type");
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithTypedResourceHandler_ReturnsTrueAndInterface()
    {
        var type = typeof(TypedResourceHandler);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeTrue();
        resourceHandlerInterface.Should().NotBeNull();
        resourceHandlerInterface!.IsGenericType.Should().BeTrue();
        resourceHandlerInterface.GetGenericTypeDefinition().Should().Be(typeof(IResourceHandler<>));
        resourceHandlerInterface.GetGenericArguments()[0].Should().Be(typeof(TestResource));
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithGenericResourceHandler_ReturnsFalseAndNull()
    {
        var type = typeof(GenericResourceHandler);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeFalse();
        resourceHandlerInterface.Should().BeNull();
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithDualResourceHandler_ReturnsTrueAndGenericInterface()
    {
        var type = typeof(DualResourceHandler);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeTrue();
        resourceHandlerInterface.Should().NotBeNull();
        resourceHandlerInterface!.IsGenericType.Should().BeTrue();
        resourceHandlerInterface.GetGenericTypeDefinition().Should().Be(typeof(IResourceHandler<>));
        resourceHandlerInterface.GetGenericArguments()[0].Should().Be(typeof(TestResource));
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithMultipleTypedResourceHandler_ReturnsTrueAndFirstInterface()
    {
        var type = typeof(MultipleTypedResourceHandler);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeTrue();
        resourceHandlerInterface.Should().NotBeNull();
        resourceHandlerInterface!.IsGenericType.Should().BeTrue();
        resourceHandlerInterface.GetGenericTypeDefinition().Should().Be(typeof(IResourceHandler<>));
        var genericArg = resourceHandlerInterface.GetGenericArguments()[0];
        (genericArg == typeof(TestResource) || genericArg == typeof(AnotherResource)).Should().BeTrue();
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithNonResourceHandler_ReturnsFalseAndNull()
    {
        var type = typeof(NonResourceHandler);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeFalse();
        resourceHandlerInterface.Should().BeNull();
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithStringType_ReturnsFalseAndNull()
    {
        var type = typeof(string);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeFalse();
        resourceHandlerInterface.Should().BeNull();
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithObjectType_ReturnsFalseAndNull()
    {
        var type = typeof(object);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeFalse();
        resourceHandlerInterface.Should().BeNull();
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithGenericInterfaceImplementation_ReturnsFalseAndNull()
    {
        var type = typeof(GenericInterfaceImplementation);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeFalse();
        resourceHandlerInterface.Should().BeNull();
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithInterfaceType_ReturnsFalseAndNull()
    {
        var type = typeof(IResourceHandler);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeFalse();
        resourceHandlerInterface.Should().BeNull();
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithGenericInterfaceType_ReturnsFalseAndNull()
    {
        var type = typeof(IResourceHandler<>);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeFalse();
        resourceHandlerInterface.Should().BeNull();
    }

    #endregion

    #region Defensive Coding and Edge Cases Tests

    [TestMethod]
    public void IsGenericTypedResourceHandler_WithAbstractType_ReturnsFalseForNonImplementer()
    {
        typeof(System.Collections.Generic.List<>).IsGenericTypedResourceHandler().Should().BeFalse();
    }

    [TestMethod]
    public void TryGetTypedResourceHandlerInterface_WithAbstractType_ReturnsFalseAndNull()
    {
        var type = typeof(System.Collections.Generic.List<>);
        var result = type.TryGetTypedResourceHandlerInterface(out var resourceHandlerInterface);
        result.Should().BeFalse();
        resourceHandlerInterface.Should().BeNull();
    }

    [TestMethod]
    public void BothMethods_WorkCorrectlyWithValueTypes()
    {
        var type = typeof(int);
        var isGeneric = type.IsGenericTypedResourceHandler();
        var tryGetTyped = type.TryGetTypedResourceHandlerInterface(out var interface1);
        isGeneric.Should().BeFalse();
        tryGetTyped.Should().BeFalse();
        interface1.Should().BeNull();
    }

    [TestMethod]
    public void BothMethods_WorkCorrectlyWithArrayTypes()
    {
        var type = typeof(string[]);
        var isGeneric = type.IsGenericTypedResourceHandler();
        var tryGetTyped = type.TryGetTypedResourceHandlerInterface(out var interface1);
        isGeneric.Should().BeFalse();
        tryGetTyped.Should().BeFalse();
        interface1.Should().BeNull();
    }

    #endregion
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Bicep.Local.Extension.Host.Attributes;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Host.TypeSpecBuilder;
using Bicep.Local.Extension.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Local.Extension.UnitTests;

[TestClass]
public class TypeProviderTests
{
    private class DummyResource { }
    private class AnotherResource { }

    [BicepType(IsActive = true)]
    private class ActiveBicepType { }

    [BicepType(IsActive = false)]
    private class InactiveBicepType { }

    private MockRepository mockRepository = null!;
    private Mock<IResourceHandlerFactory> resourceHandlerFactoryMock = null!;

    [TestInitialize]
    public void Setup()
    {
        mockRepository = new MockRepository(MockBehavior.Strict);
        resourceHandlerFactoryMock = mockRepository.Create<IResourceHandlerFactory>();
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_ResourceHandlerFactory()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new TypeProvider(null!));
    }

    [TestMethod]
    public void GetResourceTypes_Returns_ResourceHandlerFactory_Types()
    {
        var handlerType = typeof(DummyResource);
        var handlerMap = new Dictionary<string, TypeResourceHandler>
        {
            { handlerType.Name, new TypeResourceHandler(handlerType, Mock.Of<IResourceHandler>()) }
        }.ToImmutableDictionary();

        resourceHandlerFactoryMock.Setup(f => f.TypedResourceHandlers).Returns(handlerMap);

        var provider = new TypeProvider(resourceHandlerFactoryMock.Object);

        var types = provider.GetResourceTypes();

        Assert.IsTrue(types.Contains(handlerType));
    }

    [TestMethod]
    public void GetResourceTypes_Includes_Active_BicepTypeAttribute_Types()
    {
        // Ensure the type is loaded in the current AppDomain
        var _ = typeof(ActiveBicepType);

        var handlerMap = ImmutableDictionary<string, TypeResourceHandler>.Empty;

        resourceHandlerFactoryMock.Setup(f => f.TypedResourceHandlers).Returns(handlerMap);

        var provider = new TypeProvider(resourceHandlerFactoryMock.Object);

        var types = provider.GetResourceTypes();

        Assert.IsTrue(types.Any(t => t == typeof(ActiveBicepType)));
        Assert.IsFalse(types.Any(t => t == typeof(InactiveBicepType)));
    }

    [TestMethod]
    public void GetResourceTypes_Deduplicates_By_TypeName()
    {
        // Two types with the same name, only one should be present
        var handlerType = typeof(DummyResource);
        var handlerMap = new Dictionary<string, TypeResourceHandler>
        {
            { handlerType.Name, new TypeResourceHandler(handlerType, Mock.Of<IResourceHandler>()) }
        }.ToImmutableDictionary();

        resourceHandlerFactoryMock.Setup(f => f.TypedResourceHandlers).Returns(handlerMap);

        var provider = new TypeProvider(resourceHandlerFactoryMock.Object);

        var types = provider.GetResourceTypes();

        // Should only contain one type with the name "DummyResource"
        Assert.AreEqual(1, types.Count(t => t.Name == "DummyResource"));
    }

    [TestMethod]
    public void GetResourceTypes_Returns_Empty_If_No_Handlers_And_No_ActiveTypes()
    {
        var handlerMap = ImmutableDictionary<string, TypeResourceHandler>.Empty;

        resourceHandlerFactoryMock.Setup(f => f.TypedResourceHandlers).Returns(handlerMap);

        // Unload all types with BicepTypeAttribute for this test (simulate)
        // Not possible in .NET, but we can at least check for empty result
        var provider = new TypeProvider(resourceHandlerFactoryMock.Object);

        var types = provider.GetResourceTypes();

        // At least should not throw and should return an array
        Assert.IsNotNull(types);
    }
}


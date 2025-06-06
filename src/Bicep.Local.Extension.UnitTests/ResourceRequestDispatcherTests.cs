// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Rpc;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Local.Extension.UnitTests;

[TestClass]
public class ResourceRequestDispatcherTests
{
    private MockRepository mockRepository = null!;
    private Mock<IResourceHandlerFactory> resourceHandlerFactoryMock = null!;
    private Mock<ILogger<ResourceRequestDispatcher>> loggerMock = null!;
    private ResourceRequestDispatcher dispatcher = null!;

    private Mock<IResourceHandler> handlerMock = null!;
    private TypeResourceHandler typeResourceHandler = null!;

    private static ServerCallContext DummyContext(string method = "TestMethod") =>
    TestServerCallContext.Create(
        method: method,
        host: "localhost",
        deadline: DateTime.UtcNow.AddMinutes(1),
        requestHeaders: new Metadata(),
        cancellationToken: CancellationToken.None,
        peer: "localhost",
        authContext: null,
        contextPropagationToken: null,
        writeHeadersFunc: _ => Task.CompletedTask,
        writeOptionsGetter: () => null,
        writeOptionsSetter: _ => { }
    );


    [TestInitialize]
    public void Setup()
    {
        mockRepository = new MockRepository(MockBehavior.Strict);
        resourceHandlerFactoryMock = mockRepository.Create<IResourceHandlerFactory>();
        loggerMock = mockRepository.Create<ILogger<ResourceRequestDispatcher>>();
        handlerMock = mockRepository.Create<IResourceHandler>();

        typeResourceHandler = new TypeResourceHandler(typeof(object), handlerMock.Object);

        dispatcher = new ResourceRequestDispatcher(resourceHandlerFactoryMock.Object, loggerMock.Object);
    }

    private static ResourceSpecification CreateResourceSpecification(string type = "MyType", string apiVersion = "2025-01-01", string? properties = null, string? config = null)
        => new()
        {
            Type = type,
            ApiVersion = apiVersion,
            Properties = properties ?? "{}",
            Config = config ?? "{}"
        };

    private static ResourceReference CreateResourceReference(string type = "MyType", string apiVersion = "2025-01-01", string? config = null)
        => new()
        {
            Type = type,
            ApiVersion = apiVersion,
            Config = config ?? "{}"
        };



    [TestMethod]
    public async Task CreateOrUpdate_Delegates_To_Handler_And_Returns_Response()
    {
        var spec = CreateResourceSpecification();
        var handlerResponse = new HandlerResponse("MyType", "2025-01-01", HandlerResponseStatus.Success, new JsonObject());

        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Returns(typeResourceHandler);
        handlerMock.Setup(h => h.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        var result = await dispatcher.CreateOrUpdate(spec, DummyContext("CreateOrUpdate"));

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Resource);
        Assert.AreEqual("MyType", result.Resource.Type);
        Assert.AreEqual("2025-01-01", result.Resource.ApiVersion);
        Assert.AreEqual("Success", result.Resource.Status);
    }

    [TestMethod]
    public async Task Preview_Delegates_To_Handler_And_Returns_Response()
    {
        var spec = CreateResourceSpecification();
        var handlerResponse = new HandlerResponse("MyType", "2025-01-01", HandlerResponseStatus.Success, new JsonObject());

        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Returns(typeResourceHandler);
        handlerMock.Setup(h => h.Preview(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        var result = await dispatcher.Preview(spec, DummyContext("Preview"));

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Resource);
        Assert.AreEqual("MyType", result.Resource.Type);
    }

    [TestMethod]
    public async Task Get_Delegates_To_Handler_And_Returns_Response()
    {
        var reference = CreateResourceReference();
        var handlerResponse = new HandlerResponse("MyType", "2025-01-01", HandlerResponseStatus.Success, new JsonObject());

        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Returns(typeResourceHandler);
        handlerMock.Setup(h => h.Get(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        var result = await dispatcher.Get(reference, DummyContext("Get"));

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Resource);
        Assert.AreEqual("MyType", result.Resource.Type);
    }

    [TestMethod]
    public async Task Delete_Delegates_To_Handler_And_Returns_Response()
    {
        var reference = CreateResourceReference();
        var handlerResponse = new HandlerResponse("MyType", "2025-01-01", HandlerResponseStatus.Success, new JsonObject());

        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Returns(typeResourceHandler);
        handlerMock.Setup(h => h.Delete(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        var result = await dispatcher.Delete(reference, DummyContext("Delete"));

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Resource);
        Assert.AreEqual("MyType", result.Resource.Type);
    }

    [TestMethod]
    public async Task Ping_Returns_Empty()
    {
        var result = await dispatcher.Ping(new Empty(), DummyContext("Ping"));
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(Empty));
    }

    [TestMethod]
    public async Task CreateOrUpdate_Returns_Error_On_Exception()
    {
        var spec = CreateResourceSpecification();

        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Throws(new InvalidOperationException("fail"));

        var result = await dispatcher.CreateOrUpdate(spec, DummyContext("CreateOrUpdate"));

        Assert.IsNotNull(result);
        Assert.IsNull(result.Resource);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("RpcException", result.ErrorData.Error.Code);
        StringAssert.Contains(result.ErrorData.Error.Message, "fail");
    }

    [TestMethod]
    public async Task Preview_Returns_Error_On_Exception()
    {
        var spec = CreateResourceSpecification();
        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Throws(new InvalidOperationException("fail"));

        var result = await dispatcher.Preview(spec, DummyContext("Preview"));

        Assert.IsNotNull(result);
        Assert.IsNull(result.Resource);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("RpcException", result.ErrorData.Error.Code);
        StringAssert.Contains(result.ErrorData.Error.Message, "fail");
    }

    [TestMethod]
    public async Task Get_Returns_Error_On_Exception()
    {
        var reference = CreateResourceReference();
        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Throws(new InvalidOperationException("fail"));

        var result = await dispatcher.Get(reference, DummyContext("Get"));

        Assert.IsNotNull(result);
        Assert.IsNull(result.Resource);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("RpcException", result.ErrorData.Error.Code);
        StringAssert.Contains(result.ErrorData.Error.Message, "fail");
    }

    [TestMethod]
    public async Task Delete_Returns_Error_On_Exception()
    {
        var reference = CreateResourceReference();
        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Throws(new InvalidOperationException("fail"));

        var result = await dispatcher.Delete(reference, DummyContext("Delete"));

        Assert.IsNotNull(result);
        Assert.IsNull(result.Resource);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("RpcException", result.ErrorData.Error.Code);
        StringAssert.Contains(result.ErrorData.Error.Message, "fail");
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_Arguments()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new ResourceRequestDispatcher(null!, loggerMock.Object));
        Assert.ThrowsException<ArgumentNullException>(() => new ResourceRequestDispatcher(resourceHandlerFactoryMock.Object, null!));
    }

    [TestMethod]
    public async Task CreateOrUpdate_Returns_Error_When_Handler_Returns_Null()
    {
        var spec = CreateResourceSpecification();
        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Returns(typeResourceHandler);
        handlerMock.Setup(h => h.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HandlerResponse?)null!);

        var result = await dispatcher.CreateOrUpdate(spec, DummyContext("CreateOrUpdate"));

        Assert.IsNotNull(result);
        Assert.IsNull(result.Resource);
        Assert.IsNotNull(result.ErrorData);
        Assert.AreEqual("RpcException", result.ErrorData.Error.Code);
    }


    [TestMethod]
    public async Task CreateOrUpdate_Returns_Error_When_HandlerResponse_Status_Is_Error_And_Error_Is_Null()
    {
        var spec = CreateResourceSpecification();
        var handlerResponse = new HandlerResponse("MyType", "2025-01-01", HandlerResponseStatus.Error, new JsonObject(), null);

        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Returns(typeResourceHandler);
        handlerMock.Setup(h => h.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        var result = await dispatcher.CreateOrUpdate(spec, DummyContext("CreateOrUpdate"));

        Assert.IsNotNull(result);
        Assert.IsNull(result.Resource);
        Assert.IsNotNull(result.ErrorData);
    }


}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Rpc;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Local.Extension.UnitTests;

[TestClass]
public class ResourceRequestDispatcherTests
{
    private Mock<IResourceHandlerFactory> resourceHandlerFactoryMock = StrictMock.Of<IResourceHandlerFactory>();
    private Mock<ILogger<ResourceRequestDispatcher>> loggerMock = StrictMock.Of<ILogger<ResourceRequestDispatcher>>();
    private Mock<IResourceHandler> handlerMock = StrictMock.Of<IResourceHandler>();

    private ResourceRequestDispatcher dispatcher = null!;    
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

        result.Should().NotBeNull();
        result.Resource.Should().NotBeNull();
        result.Resource.Type.Should().Be("MyType");
        result.Resource.ApiVersion.Should().Be("2025-01-01");
        result.Resource.Status.Should().Be("Success");
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

        result.Should().NotBeNull();
        result.Resource.Should().NotBeNull();
        result.Resource.Type.Should().Be("MyType");
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

        result.Should().NotBeNull();
        result.Resource.Should().NotBeNull();
        result.Resource.Type.Should().Be("MyType");
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

        result.Should().NotBeNull();
        result.Resource.Should().NotBeNull();
        result.Resource.Type.Should().Be("MyType");
    }

    [TestMethod]
    public async Task Ping_Returns_Empty()
    {
        var result = await dispatcher.Ping(new Empty(), DummyContext("Ping"));
        result.Should().NotBeNull();
        result.Should().BeOfType<Empty>();
    }

    [TestMethod]
    public async Task CreateOrUpdate_Returns_Error_On_Exception()
    {
        var spec = CreateResourceSpecification();

        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Throws(new InvalidOperationException("fail"));

        var result = await dispatcher.CreateOrUpdate(spec, DummyContext("CreateOrUpdate"));

        result.Should().NotBeNull();
        result.Resource.Should().BeNull();
        result.ErrorData.Should().NotBeNull();
        result.ErrorData.Error.Code.Should().Be("RpcException");
        result.ErrorData.Error.Message.Should().Contain("fail");
    }

    [TestMethod]
    public async Task Preview_Returns_Error_On_Exception()
    {
        var spec = CreateResourceSpecification();
        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Throws(new InvalidOperationException("fail"));

        var result = await dispatcher.Preview(spec, DummyContext("Preview"));

        result.Should().NotBeNull();
        result.Resource.Should().BeNull();
        result.ErrorData.Should().NotBeNull();
        result.ErrorData.Error.Code.Should().Be("RpcException");
        result.ErrorData.Error.Message.Should().Contain("fail");
    }

    [TestMethod]
    public async Task Get_Returns_Error_On_Exception()
    {
        var reference = CreateResourceReference();
        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Throws(new InvalidOperationException("fail"));

        var result = await dispatcher.Get(reference, DummyContext("Get"));

        result.Should().NotBeNull();
        result.Resource.Should().BeNull();
        result.ErrorData.Should().NotBeNull();
        result.ErrorData.Error.Code.Should().Be("RpcException");
        result.ErrorData.Error.Message.Should().Contain("fail");
    }

    [TestMethod]
    public async Task Delete_Returns_Error_On_Exception()
    {
        var reference = CreateResourceReference();
        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Throws(new InvalidOperationException("fail"));

        var result = await dispatcher.Delete(reference, DummyContext("Delete"));

        result.Should().NotBeNull();
        result.Resource.Should().BeNull();
        result.ErrorData.Should().NotBeNull();
        result.ErrorData.Error.Code.Should().Be("RpcException");
        result.ErrorData.Error.Message.Should().Contain("fail");
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_Arguments()
    {
        Action act1 = () => new ResourceRequestDispatcher(null!, loggerMock.Object);
        Action act2 = () => new ResourceRequestDispatcher(resourceHandlerFactoryMock.Object, null!);

        act1.Should().Throw<ArgumentNullException>();
        act2.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public async Task CreateOrUpdate_Returns_Error_When_Handler_Returns_Null()
    {
        var spec = CreateResourceSpecification();
        resourceHandlerFactoryMock.Setup(f => f.GetResourceHandler("MyType")).Returns(typeResourceHandler);
        handlerMock.Setup(h => h.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HandlerResponse?)null!);

        var result = await dispatcher.CreateOrUpdate(spec, DummyContext("CreateOrUpdate"));

        result.Should().NotBeNull();
        result.Resource.Should().BeNull();
        result.ErrorData.Should().NotBeNull();
        result.ErrorData.Error.Code.Should().Be("RpcException");
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

        result.Should().NotBeNull();
        result.Resource.Should().BeNull();
        result.ErrorData.Should().NotBeNull();
    }
}

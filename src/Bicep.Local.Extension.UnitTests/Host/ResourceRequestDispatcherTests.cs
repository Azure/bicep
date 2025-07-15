// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Moq;

namespace Bicep.Local.Extension.UnitTests.Host;

[TestClass]
public class ResourceRequestDispatcherTests
{
    private readonly Mock<ILogger<ResourceRequestDispatcher>> loggerMock;

    public ResourceRequestDispatcherTests()
    {
        loggerMock = StrictMock.Of<ILogger<ResourceRequestDispatcher>>();
        loggerMock.Setup(x => x.Log<It.IsAnyType>(
                            It.IsAny<LogLevel>(),
                            It.IsAny<EventId>(),
                            It.IsAny<It.IsAnyType>(),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                            ));
    }

    private ServerCallContext GetTestServerCallContext(string method)
    {
        return TestServerCallContext.Create(
            method: method,
            host: null,
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: null,
            cancellationToken: CancellationToken.None,
            peer: null,
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: null,
            writeOptionsGetter: null,
            writeOptionsSetter: null
        );
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_ResourceRequestHandler()
    {
        Action act = () => new ResourceRequestDispatcher(null!, loggerMock.Object);
        act.Should().Throw<ArgumentNullException>().WithParameterName("resourceHandlerDispatcher");
    }

    [TestMethod]
    public void Constructor_Throws_On_Null_Logger()
    {
        Action act = () => new ResourceRequestDispatcher(StrictMock.Of<IResourceHandlerDispatcher>().Object, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("\"hello\"")]
    public async Task Request_WithNullOrEmptyProperties_Return_RpcException(string properties)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);

        var (resourceSpec, _) = GetTestResourceSpecAndRef(withProperties: properties);
        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("AnyRequest"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");        
    }

    [TestMethod]
    public async Task CreateOrUpdate_NoRequestHandlers_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        requestHandlerDispatcher
            .Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);

        requestHandlerDispatcher
            .Setup(x => x.GenericResourceHandler)
            .Returns<IResourceHandler?>(null!);

        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        string resourceType = "Microsoft.Test/Test";
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType, "2023-01-01");

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    [TestMethod]
    public async Task Preview_NoRequestHandlers_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        requestHandlerDispatcher
            .Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);

        requestHandlerDispatcher
            .Setup(x => x.GenericResourceHandler)
            .Returns<IResourceHandler?>(null!);

        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        string resourceType = "Microsoft.Test/Test";
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType, "2023-01-01");

        var response = await dispatcher.Preview(resourceSpec, GetTestServerCallContext("Preview"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    [TestMethod]
    public async Task Delete_NoRequestHandlers_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        requestHandlerDispatcher
            .Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);

        requestHandlerDispatcher
            .Setup(x => x.GenericResourceHandler)
            .Returns<IResourceHandler?>(null!);

        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        string resourceType = "Microsoft.Test/Test";
        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType, "2023-01-01");

        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("Delete"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    [TestMethod]
    public async Task Get_NoRequestHandlers_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        requestHandlerDispatcher
            .Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);

        requestHandlerDispatcher
            .Setup(x => x.GenericResourceHandler)
            .Returns<IResourceHandler?>(null!);

        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        string resourceType = "Microsoft.Test/Test";
        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType, "2023-01-01");

        var response = await dispatcher.Get(resourceRef, GetTestServerCallContext("Get"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    private (ResourceSpecification ResourceSpec, ResourceReference ResourceRef) GetTestResourceSpecAndRef(
                string resourceType = "Microsoft.Test/Test"
              , string apiVersion = "2023-01-01"
              , string withProperties = ""
              , string withConfig = "")
    {
        var resourceSpec = new Rpc.ResourceSpecification
        {
            Type = resourceType,
            ApiVersion = apiVersion,
            Config = withConfig,
            Properties = withProperties
        };
        var resourceRef = new Rpc.ResourceReference
        {
            Type = resourceType,
            ApiVersion = apiVersion
        };
        return (resourceSpec, resourceRef);
    }
}

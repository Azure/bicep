// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    #region Fake Handlers

    public class FakeGenericResourceHandler : IResourceHandler
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, request.ApiVersion, HandlerResponseStatus.Succeeded, request.Identifiers));
        }
        public Task<HandlerResponse> Preview(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, request.ApiVersion, HandlerResponseStatus.Succeeded, request.Identifiers));
        }
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, request.ApiVersion, HandlerResponseStatus.Succeeded, request.Identifiers));
        }
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, request.ApiVersion, HandlerResponseStatus.Succeeded, request.Identifiers));
        }
    }

    public record class FakeResource(string Name, string Description);
    public class FakeTypedResourceHandler : IResourceHandler<FakeResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<FakeResource> request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, request.ApiVersion, HandlerResponseStatus.Succeeded, request.Identifiers));
        }
        public Task<HandlerResponse> Preview(HandlerRequest<FakeResource> request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, request.ApiVersion, HandlerResponseStatus.Succeeded, request.Identifiers));
        }

        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, request.ApiVersion, HandlerResponseStatus.Succeeded, request.Identifiers));
        }
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, request.ApiVersion, HandlerResponseStatus.Succeeded, request.Identifiers));
        }


    }

    #endregion Fake Handlers

    #region Constructor Tests
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

    #endregion Constructor Tests

    #region CreateOrUpdate Tests

    [TestMethod]
    public async Task When_CreateOrUpdate_ResourceSpecification_Is_Null_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var response = await dispatcher.CreateOrUpdate(null!, GetTestServerCallContext("CreateOrUpdate"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Resource specification cannot be null.");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_CreateOrUpdate_ResourceSpecification_ResourceType_IsEmptyOrWhiteSpace_ReturnsError(string resourceType)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType: resourceType);
        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_CreateOrUpdate_ResourceSpecification_Properties_IsEmptyOrWhiteSpace_ReturnsError(string properties)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: properties);
        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow("")]
    //[DataRow(" ")]
    //[DataRow(null)]
    public async Task When_CreateOrUpdate_ResourceSpecification_Config_IsEmptyOrWhiteSpace_Succeeds(string config)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        requestHandlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);

        requestHandlerDispatcher.Setup(x => x.GenericResourceHandler).Returns(new FakeGenericResourceHandler());
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var resourceType = "ConfigTest";
        var properties = "{ \"key\": \"value\" }"; // valid JSON properties
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType: resourceType, properties: properties, config: config);
        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.Resource.Type.Should().Be(resourceType);
        response.Resource.Properties.Should().Contain(properties);
        response.ErrorData.Should().BeNull();
    }

    #endregion CreateOrUpdate Tests

    #region Invalid RessourceRef Tests
    [TestMethod]
    public async Task When_ResourceReference_Is_Null_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var response = await dispatcher.Delete(null!, GetTestServerCallContext("Delete"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Resource reference cannot be null.");
    }



    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_ResourceReference_ResourceType_IsNullOrEmpty_ReturnsError(string resourceType)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType: resourceType);
        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException:");
    }

    #endregion Invalid RessourceRef Tests



    #region Handlers Tests
    private ResourceRequestDispatcher GetResourceRequestDispatcher_WithNoHandlers()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        requestHandlerDispatcher
            .Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        requestHandlerDispatcher
            .Setup(x => x.GenericResourceHandler)
            .Returns<IResourceHandler?>(null!);
        return new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
    }

    [TestMethod]
    public async Task When_NoRequestHandlers_CreateOrUpdate_ReturnsError()
    {
        var dispatcher = GetResourceRequestDispatcher_WithNoHandlers();
        string resourceType = "Microsoft.Test/Test";
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType, "2023-01-01");

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    [TestMethod]
    public async Task When_NoRequestHandlers_Preview_ReturnsError()
    {
        var dispatcher = GetResourceRequestDispatcher_WithNoHandlers();
        string resourceType = "Microsoft.Test/Test";
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType, "2023-01-01");

        var response = await dispatcher.Preview(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    [TestMethod]
    public async Task When_NoRequestHandlers_Get_ReturnsError()
    {
        var dispatcher = GetResourceRequestDispatcher_WithNoHandlers();
        string resourceType = "Microsoft.Test/Test";
        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType, "2023-01-01");

        var response = await dispatcher.Get(resourceRef, GetTestServerCallContext("CreateOrUpdate"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    [TestMethod]
    public async Task When_NoRequestHandlers_Dispatcher_ReturnsError()
    {
        var dispatcher = GetResourceRequestDispatcher_WithNoHandlers();
        string resourceType = "Microsoft.Test/Test";
        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType, "2023-01-01");

        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("CreateOrUpdate"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    #endregion Handlers Tests

    private (ResourceSpecification ResourceSpec, ResourceReference ResourceRef) GetTestResourceSpecAndRef(
                string? resourceType = ""
              , string? apiVersion = ""
              , string? properties = ""
              , string? config = "")
    {
        var resourceSpec = new Rpc.ResourceSpecification();
        resourceSpec.Type = resourceType;
        resourceSpec.ApiVersion = apiVersion;
        resourceSpec.Properties = properties;
        resourceSpec.Config = config;
        
        var resourceRef = new Rpc.ResourceReference()
        {
            Type = resourceType,
            ApiVersion = apiVersion,
            Config = config
        };

        return (resourceSpec, resourceRef);
    }
}

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
            return Task.FromResult(new HandlerResponse(request.Type, HandlerResponseStatus.Succeeded, request.Properties, request.Identifiers, request.ApiVersion));
        }
        public Task<HandlerResponse> Preview(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, HandlerResponseStatus.Succeeded, request.Properties, request.Identifiers, request.ApiVersion));
        }
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, HandlerResponseStatus.Succeeded, request.Properties, request.Identifiers, request.ApiVersion));
        }
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, HandlerResponseStatus.Succeeded, request.Properties, request.Identifiers, request.ApiVersion));
        }
    }

    public record class FakeResource(string Name, string Description);
    public class FakeTypedResourceHandler : IResourceHandler<FakeResource>
    {
        public Task<HandlerResponse> CreateOrUpdate(HandlerRequest<FakeResource> request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, HandlerResponseStatus.Succeeded, request.Properties, request.Identifiers, request.ApiVersion));
        }
        public Task<HandlerResponse> Preview(HandlerRequest<FakeResource> request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, HandlerResponseStatus.Succeeded, request.Properties, request.Identifiers, request.ApiVersion));
        }
        public Task<HandlerResponse> Delete(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, HandlerResponseStatus.Succeeded, request.Properties, request.Identifiers, request.ApiVersion));
        }
        public Task<HandlerResponse> Get(HandlerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HandlerResponse(request.Type, HandlerResponseStatus.Succeeded, request.Properties, request.Identifiers, request.ApiVersion));
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
        response.ErrorData.Error.Message.Should().Contain("ArgumentNullException");
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
    [DataRow(false)]
    [DataRow(true)]
    public async Task When_CreateOrUpdate_ResourceSpecification_IsValid_StatusIsSucceeded(bool useTypedHandler)
    {
        var handlerDispatcher = GetHandlerDispatcherWithFakeResourceHandler(useTypedHandler);
        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher, loggerMock.Object);
        var resourceType = "Microsoft.Test/Test";
        var properties = "{\"key\":\"value\"}"; // valid JSON properties
        var config = "{\"setting\":\"value\"}"; // valid JSON config
        var apiVersion = "2023-01-01";
        
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType: resourceType, apiVersion: apiVersion, properties: properties, config: config);

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.Resource.Type.Should().Be(resourceSpec.Type);
        response.Resource.Properties.Should().Contain(resourceSpec.Properties);
        response.Resource.HasApiVersion.Should().BeTrue();
        response.Resource.ApiVersion.Should().Be(resourceSpec.ApiVersion);
        response.Resource.HasStatus.Should().BeTrue();
        response.Resource.Status.Should().Be(HandlerResponseStatus.Succeeded.ToString());
        
        response.ErrorData.Should().BeNull();
    }

    #endregion CreateOrUpdate Tests


    #region Preview Tests

    [TestMethod]
    public async Task When_Preview_ResourceSpecification_Is_Null_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var response = await dispatcher.Preview(null!, GetTestServerCallContext("Preview"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("ArgumentNullException");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_Preview_ResourceSpecification_ResourceType_IsEmptyOrWhiteSpace_ReturnsError(string resourceType)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType: resourceType);
        var response = await dispatcher.Preview(resourceSpec, GetTestServerCallContext("Preview"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_Preview_ResourceSpecification_Properties_IsEmptyOrWhiteSpace_ReturnsError(string properties)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: properties);
        var response = await dispatcher.Preview(resourceSpec, GetTestServerCallContext("Preview"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task When_Preview_ResourceSpecification_IsValid_StatusIsSucceeded(bool useTypedHandler)
    {
        var handlerDispatcher = GetHandlerDispatcherWithFakeResourceHandler(useTypedHandler);
        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher, loggerMock.Object);

        var resourceType = "Microsoft.Test/Test";
        var properties = "{\"key\":\"value\"}"; // valid JSON properties
        var config = "{\"setting\":\"value\"}"; // valid JSON config
        var apiVersion = "2023-01-01";

        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType: resourceType, apiVersion: apiVersion, properties: properties, config: config);

        var response = await dispatcher.Preview(resourceSpec, GetTestServerCallContext("Preview"));

        response.Should().NotBeNull();
        response.Resource.Type.Should().Be(resourceSpec.Type);
        response.Resource.Identifiers.Should().Be("{}"); // empty json
        response.Resource.Properties.Should().Contain(resourceSpec.Properties);
        response.Resource.HasApiVersion.Should().BeTrue();
        response.Resource.ApiVersion.Should().Be(resourceSpec.ApiVersion);
        response.Resource.HasStatus.Should().BeTrue();
        response.Resource.Status.Should().Be(HandlerResponseStatus.Succeeded.ToString());

        response.ErrorData.Should().BeNull();
    }


    #endregion Preview Tests


    #region Get Tests

    [TestMethod]
    public async Task When_Get_ResourceSpecification_Is_Null_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var response = await dispatcher.Get(null!, GetTestServerCallContext("Get"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("ArgumentNullException");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_Get_ResourceSpecification_ResourceType_IsEmptyOrWhiteSpace_ReturnsError(string resourceType)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType: resourceType);
        var response = await dispatcher.Get(resourceRef, GetTestServerCallContext("Get"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_Get_ResourceSpecification_Properties_IsEmptyOrWhiteSpace_ReturnsError(string properties)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (_, resourceRef) = GetTestResourceSpecAndRef(properties: properties);
        var response = await dispatcher.Get(resourceRef, GetTestServerCallContext("Get"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task When_Get_ResourceSpecification_IsValid_StatusIsSucceeded(bool useTypedHandler)
    {
        var handlerDispatcher = GetHandlerDispatcherWithFakeResourceHandler(useTypedHandler);
        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher, loggerMock.Object);

        var resourceType = "Microsoft.Test/Test";
        var properties = "{\"key\":\"value\"}"; // valid JSON properties
        var config = "{\"setting\":\"value\"}"; // valid JSON config
        var identifiers = "{\"id\":\"12345\"}"; // valid JSON identifiers
        var apiVersion = "2023-01-01";

        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType: resourceType, apiVersion: apiVersion, properties: properties, config: config, identifiers: identifiers);

        var response = await dispatcher.Get(resourceRef, GetTestServerCallContext("Get"));

        response.Should().NotBeNull();
        response.Resource.Type.Should().Be(resourceRef.Type);
        response.Resource.Properties.Should().Be("{}"); // empty json
        response.Resource.Identifiers.Should().Be(resourceRef.Identifiers);
        response.Resource.HasApiVersion.Should().BeTrue();
        response.Resource.ApiVersion.Should().Be(resourceRef.ApiVersion);
        response.Resource.HasStatus.Should().BeTrue();
        response.Resource.Status.Should().Be(HandlerResponseStatus.Succeeded.ToString());

        response.ErrorData.Should().BeNull();
    }

    #endregion Get Tests

    #region Delete Tests

    [TestMethod]
    public async Task When_Delete_ResourceSpecification_Is_Null_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var response = await dispatcher.Delete(null!, GetTestServerCallContext("Delete"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("ArgumentNullException");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_Delete_ResourceSpecification_ResourceType_IsEmptyOrWhiteSpace_ReturnsError(string resourceType)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType: resourceType);
        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("Delete"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_Delete_ResourceSpecification_Properties_IsEmptyOrWhiteSpace_ReturnsError(string properties)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (_, resourceRef) = GetTestResourceSpecAndRef(properties: properties);
        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("Delete"));
        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task When_Delete_ResourceSpecification_IsValid_StatusIsSucceeded(bool useTypedHandler)
    {
        var handlerDispatcher = GetHandlerDispatcherWithFakeResourceHandler(useTypedHandler);
        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher, loggerMock.Object);

        var resourceType = "Microsoft.Test/Test";
        var properties = "{\"key\":\"value\"}"; // valid JSON properties
        var config = "{\"setting\":\"value\"}"; // valid JSON config
        var identifiers = "{\"id\":\"12345\"}"; // valid JSON identifiers
        var apiVersion = "2023-01-01";

        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType: resourceType, apiVersion: apiVersion, properties: properties, config: config, identifiers: identifiers);

        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("Delete"));

        response.Should().NotBeNull();
        response.Resource.Type.Should().Be(resourceRef.Type);
        response.Resource.Properties.Should().Be("{}"); // empty json
        response.Resource.Identifiers.Should().Be(resourceRef.Identifiers);
        response.Resource.HasApiVersion.Should().BeTrue();
        response.Resource.ApiVersion.Should().Be(resourceRef.ApiVersion);
        response.Resource.HasStatus.Should().BeTrue();
        response.Resource.Status.Should().Be(HandlerResponseStatus.Succeeded.ToString());

        response.ErrorData.Should().BeNull();
    }

    #endregion Delete Tests

    #region Handlers Tests

    [TestMethod]
    public async Task When_NoRequestHandlers_CreateOrUpdate_ReturnsError()
    {
        var dispatcher = GetResourceRequestDispatcher_WithNoHandlers();
        string resourceType = "Microsoft.Test/Test";
        var properties = "{\"key\":\"value\"}"; // valid JSON properties
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType: resourceType, properties: properties, apiVersion: "2023-01-01");

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
        var properties = "{\"key\":\"value\"}"; // valid JSON properties
        var (resourceSpec, _) = GetTestResourceSpecAndRef(resourceType: resourceType, properties: properties, apiVersion: "2023-01-01");

        var response = await dispatcher.Preview(resourceSpec, GetTestServerCallContext("Preview"));

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
        var properties = "{\"key\":\"value\"}"; // valid JSON properties
        var identifiers = "{\"id\":\"12345\"}"; // valid JSON identifiers
        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType: resourceType, properties: properties, apiVersion: "2023-01-01", identifiers: identifiers);

        var response = await dispatcher.Get(resourceRef, GetTestServerCallContext("Get"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    [TestMethod]
    public async Task When_NoRequestHandlers_Delete_ReturnsError()
    {
        var dispatcher = GetResourceRequestDispatcher_WithNoHandlers();
        string resourceType = "Microsoft.Test/Test";        
        var properties = "{\"key\":\"value\"}"; // valid JSON properties
        var identifiers = "{\"id\":\"12345\"}"; // valid JSON identifiers
        var (_ , resourceRef) = GetTestResourceSpecAndRef(resourceType: resourceType, properties: properties, apiVersion: "2023-01-01", identifiers: identifiers);

        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("Delete"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    #endregion Handlers Tests

    #region Helpers
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

    private (ResourceSpecification ResourceSpec, ResourceReference ResourceRef) GetTestResourceSpecAndRef(
                string? resourceType = ""
              , string? apiVersion = ""
              , string? properties = ""
              , string? config = ""
              , string? identifiers = "")
    {
        var resourceSpec = new Rpc.ResourceSpecification()
        {
            Type = resourceType,
            ApiVersion = apiVersion,
            Properties = properties,
            Config = config        
        };
        
        var resourceRef = new Rpc.ResourceReference()
        {
            Type = resourceType,
            ApiVersion = apiVersion,
            Config = config,
            Identifiers = identifiers
        };
        

        return (resourceSpec, resourceRef);
    }

    public IResourceHandlerDispatcher GetHandlerDispatcherWithFakeResourceHandler(bool useTypedHandler = false)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        if (useTypedHandler)
        {
            requestHandlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
                .Returns((string type, ref TypedResourceHandler handler) =>
                {
                    handler = new TypedResourceHandler(typeof(FakeResource), new FakeTypedResourceHandler());
                    return true;
                });
        }
        else
        {
            requestHandlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
                .Returns(false);
            requestHandlerDispatcher.Setup(x => x.GenericResourceHandler)
                .Returns(new FakeGenericResourceHandler());
        }


        return requestHandlerDispatcher.Object;
    }

    #endregion Helpers
}

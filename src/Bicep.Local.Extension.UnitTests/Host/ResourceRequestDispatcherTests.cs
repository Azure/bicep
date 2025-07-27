// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Rpc;
using FluentAssertions;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using Error = Bicep.Local.Extension.Host.Handlers.Error;

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
        var (_, resourceRef) = GetTestResourceSpecAndRef(resourceType: resourceType, properties: properties, apiVersion: "2023-01-01", identifiers: identifiers);

        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("Delete"));

        response.Resource.Should().BeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
    }

    #endregion Handlers Tests

    #region Ping Tests
    [TestMethod]
    public async Task Ping_ReturnsEmptyResponse()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);

        var response = await dispatcher.Ping(new Rpc.Empty(), GetTestServerCallContext("Ping"));

        response.Should().NotBeNull();
        response.Should().BeOfType<Rpc.Empty>();
    }
    #endregion Ping Tests

    #region JSON Parsing Tests
    [DataTestMethod]
    [DataRow("invalid json")]
    [DataRow("{")]
    [DataRow("null")]
    [DataRow("\"string\"")]
    [DataRow("123")]
    [DataRow("true")]
    public async Task When_CreateOrUpdate_InvalidJsonProperties_ReturnsError(string invalidJson)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: invalidJson);

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow("invalid json")]
    [DataRow("{")]
    [DataRow("null")]
    [DataRow("\"string\"")]
    [DataRow("123")]
    [DataRow("true")]
    public async Task When_CreateOrUpdate_InvalidJsonConfig_ReturnsError(string invalidJson)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(config: invalidJson);

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }

    [DataTestMethod]
    [DataRow("invalid json")]
    [DataRow("{")]
    [DataRow("null")]
    [DataRow("\"string\"")]
    [DataRow("123")]
    [DataRow("true")]
    public async Task When_Get_InvalidJsonIdentifiers_ReturnsError(string invalidJson)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (_, resourceRef) = GetTestResourceSpecAndRef(identifiers: invalidJson);

        var response = await dispatcher.Get(resourceRef, GetTestServerCallContext("Get"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.ArgumentException");
    }
    #endregion JSON Parsing Tests

    #region TypedResourceHandler Tests

    [TestMethod]
    public async Task When_TypedHandler_ActivatorCreateInstanceFails_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();

        // Create a TypedResourceHandler with a type that can't be properly instantiated
        var invalidType = typeof(string); // String doesn't have a constructor that matches HandlerRequest<T>
        var typedHandler = new TypedResourceHandler(invalidType, new FakeTypedResourceHandler());

        requestHandlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns((string type, ref TypedResourceHandler? handler) =>
            {
                handler = typedHandler;
                return true;
            });

        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: "{\"name\": \"test\", \"description\": \"test desc\"}");

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
    }
    #endregion TypedResourceHandler Tests

    #region HandlerResponse Processing Tests
    [TestMethod]
    public async Task When_HandlerResponse_IsNull_ReturnsError()
    {
        var handler = StrictMock.Of<IResourceHandler>();
        handler.Setup(x => x.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HandlerResponse)null!);

        var handlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        handlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        handlerDispatcher.Setup(x => x.GenericResourceHandler)
            .Returns(handler.Object);

        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: "{\"key\": \"value\"}");

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Failed to process handler response. No response was provided.");
    }

    [TestMethod]
    public async Task When_HandlerResponse_HasError_ReturnsErrorData()
    {
        var errorCode = "ErrorCode";
        var errorMessage = "ErrorMessage";
        var errorTarget = "ErrorTarget";
        var innerErrorMessage = "InnerErrorMessage";

        var error = new Error(Code: errorCode, Target: errorTarget, Message: errorMessage,
                            InnerError: new(Code: errorCode, Target: errorTarget, Message: innerErrorMessage));

        var handlerResponse = new HandlerResponse("Microsoft.Test/Test", HandlerResponseStatus.Failed,
            new JsonObject(), new JsonObject(), "2023-01-01", error);

        var handler = StrictMock.Of<IResourceHandler>();
        handler.Setup(x => x.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        var handlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        handlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        handlerDispatcher.Setup(x => x.GenericResourceHandler)
            .Returns(handler.Object);

        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: "{\"key\": \"value\"}");

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be(errorCode);
        response.ErrorData.Error.Message.Should().Be(errorMessage);
        response.ErrorData.Error.InnerError.Should().Be(innerErrorMessage);
        response.ErrorData.Error.Target.Should().Be(errorTarget);
    }

    [DataTestMethod]
    [DataRow(HandlerResponseStatus.Canceled)]
    [DataRow(HandlerResponseStatus.TimedOut)]
    public async Task When_HandlerResponse_HasNonFailedStatus_ReturnsResource(HandlerResponseStatus status)
    {
        var properties = new JsonObject { ["prop"] = "value" };
        var identifiers = new JsonObject { ["id"] = "123" };
        var handlerResponse = new HandlerResponse("Microsoft.Test/Test", status, properties, identifiers, "2023-01-01");

        var handler = StrictMock.Of<IResourceHandler>();
        handler.Setup(x => x.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        var handlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        handlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        handlerDispatcher.Setup(x => x.GenericResourceHandler)
            .Returns(handler.Object);

        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: "{\"key\": \"value\"}");

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.Resource.Should().NotBeNull();
        response.Resource.Status.Should().Be(status.ToString());
        response.Resource.Type.Should().Be("Microsoft.Test/Test");
        response.Resource.ApiVersion.Should().Be("2023-01-01");
        response.Resource.Properties.Should().Contain("prop");
        response.Resource.Identifiers.Should().Contain("id");
        response.ErrorData.Should().BeNull();
    }
    #endregion HandlerResponse Processing Tests

    #region Exception Handling Tests
    [TestMethod]
    public async Task When_Handler_ThrowsException_ReturnsWrappedError()
    {
        var handler = StrictMock.Of<IResourceHandler>();
        handler.Setup(x => x.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Handler threw exception"));

        var handlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        handlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        handlerDispatcher.Setup(x => x.GenericResourceHandler)
            .Returns(handler.Object);

        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: "{\"key\": \"value\"}");

        var response = await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("Rpc request failed: System.InvalidOperationException: Handler threw exception");
        response.Resource.Should().BeNull();

        // Verify logging was called
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<InvalidOperationException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
    #endregion Exception Handling Tests

    #region API Version and Config Tests
    [TestMethod]
    public async Task When_ResourceSpecification_HasNoApiVersion_HandlerReceivesNull()
    {
        HandlerRequest? capturedRequest = null;
        var handler = StrictMock.Of<IResourceHandler>();
        handler.Setup(x => x.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .Callback<HandlerRequest, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HandlerResponse("Microsoft.Test/Test", HandlerResponseStatus.Succeeded,
                new JsonObject(), new JsonObject(), null));

        var handlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        handlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        handlerDispatcher.Setup(x => x.GenericResourceHandler)
            .Returns(handler.Object);

        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher.Object, loggerMock.Object);
        var resourceSpec = new Rpc.ResourceSpecification
        {
            Type = "Microsoft.Test/Test",
            Properties = "{\"key\": \"value\"}"
            // No ApiVersion set
        };

        await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        capturedRequest.Should().NotBeNull();
        capturedRequest!.ApiVersion.Should().BeNull();
    }

    [TestMethod]
    public async Task When_ResourceSpecification_HasNoConfig_HandlerReceivesEmptyConfig()
    {
        HandlerRequest? capturedRequest = null;
        var handler = StrictMock.Of<IResourceHandler>();
        handler.Setup(x => x.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .Callback<HandlerRequest, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HandlerResponse("Microsoft.Test/Test", HandlerResponseStatus.Succeeded,
                new JsonObject(), new JsonObject(), "2023-01-01"));

        var handlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        handlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        handlerDispatcher.Setup(x => x.GenericResourceHandler)
            .Returns(handler.Object);

        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher.Object, loggerMock.Object);
        var resourceSpec = new Rpc.ResourceSpecification
        {
            Type = "Microsoft.Test/Test",
            Properties = "{\"key\": \"value\"}",
            ApiVersion = "2023-01-01"
            // No Config set
        };

        await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        capturedRequest.Should().NotBeNull();
        capturedRequest!.Config.Should().NotBeNull();
        capturedRequest.Config.Count.Should().Be(0);
    }

    [TestMethod]
    public async Task When_ResourceReference_HasNoIdentifiers_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);

        var resourceRef = new Rpc.ResourceReference
        {
            Type = "Microsoft.Test/Test",
            ApiVersion = "2023-01-01"
            // No Identifiers set
        };

        var response = await dispatcher.Get(resourceRef, GetTestServerCallContext("Get"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("ArgumentException");
    }
    #endregion API Version and Config Tests

    #region Cancellation Tests
    [TestMethod]
    public async Task When_CancellationRequested_PassesToHandler()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        CancellationToken capturedToken = default;
        var handlerDispatcher = GetHandlerDispatcherWithFakeResourceHandler(useTypedHandler: false);

        // Override the handler to capture the cancellation token
        var handler = StrictMock.Of<IResourceHandler>();
        handler.Setup(x => x.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .Callback<HandlerRequest, CancellationToken>((req, ct) => capturedToken = ct)
            .ReturnsAsync(new HandlerResponse("Microsoft.Test/Test", HandlerResponseStatus.Canceled,
                new JsonObject(), new JsonObject(), "2023-01-01"));

        var mockDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        mockDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        mockDispatcher.Setup(x => x.GenericResourceHandler)
            .Returns(handler.Object);

        var dispatcher = new ResourceRequestDispatcher(mockDispatcher.Object, loggerMock.Object);
        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: "{\"key\": \"value\"}");

        var context = TestServerCallContext.Create(
            method: "CreateOrUpdate",
            host: null,
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: null,
            cancellationToken: cts.Token,
            peer: null,
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: null,
            writeOptionsGetter: null,
            writeOptionsSetter: null
        );

        await dispatcher.CreateOrUpdate(resourceSpec, context);

        capturedToken.IsCancellationRequested.Should().BeTrue();
    }
    #endregion Cancellation Tests

    #region Complex JSON Scenarios Tests
    [TestMethod]
    public async Task When_ComplexJsonProperties_HandlerReceivesCorrectData()
    {
        HandlerRequest? capturedRequest = null;
        var handler = StrictMock.Of<IResourceHandler>();
        handler.Setup(x => x.CreateOrUpdate(It.IsAny<HandlerRequest>(), It.IsAny<CancellationToken>()))
            .Callback<HandlerRequest, CancellationToken>((req, ct) => capturedRequest = req)
            .ReturnsAsync(new HandlerResponse("Microsoft.Test/Test", HandlerResponseStatus.Succeeded,
                new JsonObject(), new JsonObject(), "2023-01-01"));

        var handlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        handlerDispatcher.Setup(x => x.TryGetTypedResourceHandler(It.IsAny<string>(), out It.Ref<TypedResourceHandler?>.IsAny))
            .Returns(false);
        handlerDispatcher.Setup(x => x.GenericResourceHandler)
            .Returns(handler.Object);

        var dispatcher = new ResourceRequestDispatcher(handlerDispatcher.Object, loggerMock.Object);

        var complexProperties = """
        {
            "name": "test-resource",
            "tags": {
                "environment": "test",
                "owner": "team"
            },
            "settings": [
                {"key": "setting1", "value": "value1"},
                {"key": "setting2", "value": "value2"}
            ],
            "enabled": true,
            "count": 42
        }
        """;

        var (resourceSpec, _) = GetTestResourceSpecAndRef(properties: complexProperties);

        await dispatcher.CreateOrUpdate(resourceSpec, GetTestServerCallContext("CreateOrUpdate"));

        capturedRequest.Should().NotBeNull();
        capturedRequest!.Properties.Should().NotBeNull();
        capturedRequest.Properties["name"]?.ToString().Should().Be("test-resource");
        capturedRequest.Properties["enabled"]?.GetValue<bool>().Should().BeTrue();
        capturedRequest.Properties["count"]?.GetValue<int>().Should().Be(42);
    }
    #endregion Complex JSON Scenarios Tests

    #region Resource Reference Validation Tests
    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task When_ResourceReference_Identifiers_IsEmptyOrWhiteSpace_ReturnsError(string identifiers)
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);
        var (_, resourceRef) = GetTestResourceSpecAndRef(identifiers: identifiers);

        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("Delete"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("ArgumentException");
    }

    [TestMethod]
    public async Task When_ResourceReference_Identifiers_IsNull_ReturnsError()
    {
        var requestHandlerDispatcher = StrictMock.Of<IResourceHandlerDispatcher>();
        var dispatcher = new ResourceRequestDispatcher(requestHandlerDispatcher.Object, loggerMock.Object);

        var resourceRef = new Rpc.ResourceReference
        {
            Type = "Microsoft.Test/Test",
            ApiVersion = "2023-01-01"
            // Identifiers is null by default
        };

        var response = await dispatcher.Delete(resourceRef, GetTestServerCallContext("Delete"));

        response.Should().NotBeNull();
        response.ErrorData.Should().NotBeNull();
        response.ErrorData.Error.Code.Should().Be("RpcException");
        response.ErrorData.Error.Message.Should().Contain("ArgumentException");
    }
    #endregion Resource Reference Validation Tests

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
                string? resourceType = "Microsoft.Test/Test"
              , string? apiVersion = "2023-01-01"
              , string? properties = "{\"key\":\"value\"}"
              , string? config = "{\"setting\":\"value\"}"
              , string? identifiers = "{\"id\":\"12345\"}")
    {
        var resourceSpec = new Rpc.ResourceSpecification();
        var resourceRef = new Rpc.ResourceReference();

        if (resourceType is not null)
        {
            resourceSpec.Type = resourceType;
            resourceRef.Type = resourceType;
        }

        if (apiVersion is not null)
        {
            resourceSpec.ApiVersion = apiVersion;
            resourceRef.ApiVersion = apiVersion;
        }

        if (properties is not null)
        {
            resourceSpec.Properties = properties;
        }

        if (config is not null)
        {
            resourceSpec.Config = config;
            resourceRef.Config = config;
        }

        if (identifiers is not null)
        {
            resourceRef.Identifiers = identifiers;
        }

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

    #endregion Helpers
}

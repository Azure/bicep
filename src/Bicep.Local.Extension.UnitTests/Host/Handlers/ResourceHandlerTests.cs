// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types.Attributes;
using Bicep.Local.Extension.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Local.Extension.UnitTests.Host.Handlers;

[TestClass]
public class ResourceHandlerTests
{
    private class TestGenericResourceHandler : GenericResourceHandler<TestGenericResourceHandler.ConfigType>
    {
        public class ConfigType
        {
            public string? Setting { get; set; }
        }

        public Func<ResourceRequest, CancellationToken, Task<ResourceResponse>>? OnCreateOrUpdate { get; set; }
        protected override Task<ResourceResponse> CreateOrUpdate(ResourceRequest request, CancellationToken cancellationToken)
            => OnCreateOrUpdate?.Invoke(request, cancellationToken) ?? base.CreateOrUpdate(request, cancellationToken);

        public Func<ResourceRequest, CancellationToken, Task<ResourceResponse>>? OnPreview { get; set; }
        protected override Task<ResourceResponse> Preview(ResourceRequest request, CancellationToken cancellationToken)
            => OnPreview?.Invoke(request, cancellationToken) ?? base.Preview(request, cancellationToken);

        public Func<ReferenceRequest, CancellationToken, Task<ResourceResponse>>? OnGet { get; set; }
        protected override Task<ResourceResponse> Get(ReferenceRequest reference, CancellationToken cancellationToken)
            => OnGet?.Invoke(reference, cancellationToken) ?? base.Get(reference, cancellationToken);

        public Func<ReferenceRequest, CancellationToken, Task<ResourceResponse>>? OnDelete { get; set; }
        protected override Task<ResourceResponse> Delete(ReferenceRequest request, CancellationToken cancellationToken)
            => OnDelete?.Invoke(request, cancellationToken) ?? base.Delete(request, cancellationToken);

        protected override JsonObject GetIdentifiers(JsonObject properties)
            => new()
            {
                ["id"] = "12345"
            };

        public ResourceResponse GetTestResponse(ResourceRequest request) => GetResponse(request);

        public ResourceResponse GetTestResponse(ReferenceRequest request, JsonObject properties) => GetResponse(request, properties);
    }

    private class TestTypedResourceHandler : TypedResourceHandler<TestTypedResourceHandler.PropertiesType, TestTypedResourceHandler.IdentifiersType, TestGenericResourceHandler.ConfigType>
    {
        [ResourceType("Microsoft.Test/Test", "2023-01-01")]
        public class PropertiesType
        {
            public string? Key { get; set; }
        }

        public class IdentifiersType
        {
            public string? Id { get; set; }
        }

        public class ConfigType
        {
            public string? Setting { get; set; }
        }

        public Func<ResourceRequest, CancellationToken, Task<ResourceResponse>>? OnCreateOrUpdate { get; set; }
        protected override Task<ResourceResponse> CreateOrUpdate(ResourceRequest request, CancellationToken cancellationToken)
            => OnCreateOrUpdate?.Invoke(request, cancellationToken) ?? base.CreateOrUpdate(request, cancellationToken);

        public Func<ResourceRequest, CancellationToken, Task<ResourceResponse>>? OnPreview { get; set; }
        protected override Task<ResourceResponse> Preview(ResourceRequest request, CancellationToken cancellationToken)
            => OnPreview?.Invoke(request, cancellationToken) ?? base.Preview(request, cancellationToken);

        public Func<ReferenceRequest, CancellationToken, Task<ResourceResponse>>? OnGet { get; set; }
        protected override Task<ResourceResponse> Get(ReferenceRequest reference, CancellationToken cancellationToken)
            => OnGet?.Invoke(reference, cancellationToken) ?? base.Get(reference, cancellationToken);

        public Func<ReferenceRequest, CancellationToken, Task<ResourceResponse>>? OnDelete { get; set; }
        protected override Task<ResourceResponse> Delete(ReferenceRequest request, CancellationToken cancellationToken)
            => OnDelete?.Invoke(request, cancellationToken) ?? base.Delete(request, cancellationToken);

        protected override IdentifiersType GetIdentifiers(PropertiesType properties)
            => new()
            {
                Id = "12345"
            };

        public ResourceResponse GetTestResponse(ResourceRequest request) => GetResponse(request);

        public ResourceResponse GetTestResponse(ReferenceRequest request, PropertiesType properties) => GetResponse(request, properties);
    }

    [TestMethod]
    [DataRow("CreateOrUpdate")]
    [DataRow("Preview")]
    [DataRow("Get")]
    [DataRow("Delete")]
    public async Task GenericResourceHandler_methods_should_correctly_deserialize_data(string methodName)
    {
        var (resourceSpec, resourceRef) = BicepExtensionTests.GetTestResourceSpecAndRef();
        var properties = (JsonNode.Parse(resourceSpec.Properties) as JsonObject)!;
        var identifiers = (JsonNode.Parse(resourceRef.Identifiers) as JsonObject)!;

        var handler = new TestGenericResourceHandler();
        handler.OnCreateOrUpdate = (request, cancellationToken) => Task.FromResult(handler.GetTestResponse(request));
        handler.OnPreview = (request, cancellationToken) => Task.FromResult(handler.GetTestResponse(request));
        handler.OnGet = (request, cancellationToken) => Task.FromResult(handler.GetTestResponse(request, properties));
        handler.OnDelete = (request, cancellationToken) => Task.FromResult(handler.GetTestResponse(request, properties));

        var response = methodName switch
        {
            "CreateOrUpdate" => await handler.CreateOrUpdate(resourceSpec, CancellationToken.None),
            "Preview" => await handler.Preview(resourceSpec, CancellationToken.None),
            "Get" => await handler.Get(resourceRef, CancellationToken.None),
            "Delete" => await handler.Delete(resourceRef, CancellationToken.None),
            _ => throw new UnreachableException(),
        };

        response.ErrorData.Should().BeNull();
        response.Resource.Should().DeepEqualJson("""
        {
          "type": "Microsoft.Test/Test",
          "apiVersion": "2023-01-01",
          "identifiers": "{\"id\":\"12345\"}",
          "properties": "{\"key\":\"value\"}"
        }
        """);
    }

    [TestMethod]
    public void GenericResourceHandler_should_return_null_type_and_apiversion()
    {
        var handler = new TestGenericResourceHandler();

        handler.Type.Should().BeNull();
        handler.ApiVersion.Should().BeNull();
    }

    [TestMethod]
    [DataRow("CreateOrUpdate")]
    [DataRow("Preview")]
    [DataRow("Get")]
    [DataRow("Delete")]
    public async Task TypedResourceHandler_methods_should_correctly_deserialize_data(string methodName)
    {
        var (resourceSpec, resourceRef) = BicepExtensionTests.GetTestResourceSpecAndRef();
        var properties = JsonSerializer.Deserialize<TestTypedResourceHandler.PropertiesType>(resourceSpec.Properties, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        var identifiers = JsonSerializer.Deserialize<TestTypedResourceHandler.IdentifiersType>(resourceRef.Identifiers, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;

        var handler = new TestTypedResourceHandler();
        handler.OnCreateOrUpdate = (request, cancellationToken) => Task.FromResult(handler.GetTestResponse(request));
        handler.OnPreview = (request, cancellationToken) => Task.FromResult(handler.GetTestResponse(request));
        handler.OnGet = (request, cancellationToken) => Task.FromResult(handler.GetTestResponse(request, properties));
        handler.OnDelete = (request, cancellationToken) => Task.FromResult(handler.GetTestResponse(request, properties));

        var response = methodName switch
        {
            "CreateOrUpdate" => await handler.CreateOrUpdate(resourceSpec, CancellationToken.None),
            "Preview" => await handler.Preview(resourceSpec, CancellationToken.None),
            "Get" => await handler.Get(resourceRef, CancellationToken.None),
            "Delete" => await handler.Delete(resourceRef, CancellationToken.None),
            _ => throw new UnreachableException(),
        };

        response.ErrorData.Should().BeNull();
        response.Resource.Should().DeepEqualJson("""
        {
          "type": "Microsoft.Test/Test",
          "apiVersion": "2023-01-01",
          "identifiers": "{\"id\":\"12345\"}",
          "properties": "{\"key\":\"value\"}"
        }
        """);
    }

    [TestMethod]
    [DataRow("CreateOrUpdate")]
    [DataRow("Preview")]
    [DataRow("Get")]
    [DataRow("Delete")]
    public async Task TypedResourceHandler_methods_should_catch_and_return_info_from_unhandled_exceptions(string methodName)
    {
        var (resourceSpec, resourceRef) = BicepExtensionTests.GetTestResourceSpecAndRef();
        var properties = JsonSerializer.Deserialize<TestTypedResourceHandler.PropertiesType>(resourceSpec.Properties, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        var identifiers = JsonSerializer.Deserialize<TestTypedResourceHandler.IdentifiersType>(resourceRef.Identifiers, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;

        var handler = new TestTypedResourceHandler();
        handler.OnCreateOrUpdate = (request, cancellationToken) => throw new InvalidOperationException("This is an unhandled exception");
        handler.OnPreview = (request, cancellationToken) => throw new InvalidOperationException("This is an unhandled exception");
        handler.OnGet = (request, cancellationToken) => throw new InvalidOperationException("This is an unhandled exception");
        handler.OnDelete = (request, cancellationToken) => throw new InvalidOperationException("This is an unhandled exception");

        var response = methodName switch
        {
            "CreateOrUpdate" => await handler.CreateOrUpdate(resourceSpec, CancellationToken.None),
            "Preview" => await handler.Preview(resourceSpec, CancellationToken.None),
            "Get" => await handler.Get(resourceRef, CancellationToken.None),
            "Delete" => await handler.Delete(resourceRef, CancellationToken.None),
            _ => throw new UnreachableException(),
        };

        response.Resource.Should().BeNull();
        response.ErrorData.Should().DeepEqualJson("""
        {
          "error": {
            "code": "RpcException",
            "message": "Rpc request failed: This is an unhandled exception"
          }
        }
        """);
    }

    [TestMethod]
    [DataRow("CreateOrUpdate")]
    [DataRow("Preview")]
    [DataRow("Get")]
    [DataRow("Delete")]
    public async Task TypedResourceHandler_methods_should_catch_and_return_info_from_exceptions(string methodName)
    {
        var (resourceSpec, resourceRef) = BicepExtensionTests.GetTestResourceSpecAndRef();
        var properties = JsonSerializer.Deserialize<TestTypedResourceHandler.PropertiesType>(resourceSpec.Properties, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        var identifiers = JsonSerializer.Deserialize<TestTypedResourceHandler.IdentifiersType>(resourceRef.Identifiers, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;

        var handler = new TestTypedResourceHandler();
        TestTypedResourceHandler.ErrorDetail errorDetail = new()
        {
            Code = "NestedCode",
            Target = "NestedTarget",
            Message = "NestedMessage"
        };
        handler.OnCreateOrUpdate = (request, cancellationToken) => throw new TestTypedResourceHandler.ResourceErrorException("TestCode", "TestMessage", "TestTarget", [errorDetail]);
        handler.OnPreview = (request, cancellationToken) => throw new TestTypedResourceHandler.ResourceErrorException("TestCode", "TestMessage", "TestTarget", [errorDetail]);
        handler.OnGet = (request, cancellationToken) => throw new TestTypedResourceHandler.ResourceErrorException("TestCode", "TestMessage", "TestTarget", [errorDetail]);
        handler.OnDelete = (request, cancellationToken) => throw new TestTypedResourceHandler.ResourceErrorException("TestCode", "TestMessage", "TestTarget", [errorDetail]);

        var response = methodName switch
        {
            "CreateOrUpdate" => await handler.CreateOrUpdate(resourceSpec, CancellationToken.None),
            "Preview" => await handler.Preview(resourceSpec, CancellationToken.None),
            "Get" => await handler.Get(resourceRef, CancellationToken.None),
            "Delete" => await handler.Delete(resourceRef, CancellationToken.None),
            _ => throw new UnreachableException(),
        };

        response.Resource.Should().BeNull();
        response.ErrorData.Should().DeepEqualJson("""
        {
          "error": {
            "code": "TestCode",
            "target": "TestTarget",
            "message": "TestMessage",
            "details": [
              {
                "code": "NestedCode",
                "target": "NestedTarget",
                "message": "NestedMessage"
              }
            ]
          }
        }
        """);
    }

    [TestMethod]
    [DataRow("CreateOrUpdate")]
    [DataRow("Preview")]
    [DataRow("Get")]
    [DataRow("Delete")]
    public async Task TypedResourceHandler_methods_should_return_errors_if_unimplemented(string methodName)
    {
        var (resourceSpec, resourceRef) = BicepExtensionTests.GetTestResourceSpecAndRef();
        var handler = new TestTypedResourceHandler();

        var response = methodName switch
        {
            "CreateOrUpdate" => await handler.CreateOrUpdate(resourceSpec, CancellationToken.None),
            "Preview" => await handler.Preview(resourceSpec, CancellationToken.None),
            "Get" => await handler.Get(resourceRef, CancellationToken.None),
            "Delete" => await handler.Delete(resourceRef, CancellationToken.None),
            _ => throw new UnreachableException(),
        };

        response.Resource.Should().BeNull();
        response.ErrorData.Should().DeepEqualJson($$"""
        {
          "error": {
            "code": "NotImplemented",
            "message": "Operation '{{typeof(TestTypedResourceHandler).FullName}}.{{methodName}}' has not been implemented."
          }
        }
        """);
    }

    [TestMethod]
    public void TypedResourceHandler_should_return_type_and_apiversion_from_resource()
    {
        var handler = new TestTypedResourceHandler();

        handler.Type.Should().Be("Microsoft.Test/Test");
        handler.ApiVersion.Should().Be("2023-01-01");
    }
}

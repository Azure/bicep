// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Local.Extension.Host;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Types;
using Bicep.Local.Extension.UnitTests.Assertions;
using Bicep.Local.Rpc;
using FluentAssertions;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Moq;
using Newtonsoft.Json.Linq;
using BicepExtension = Bicep.Local.Extension.Host.BicepExtension;

namespace Bicep.Local.Extension.UnitTests.Host;

[TestClass]
public class BicepExtensionTests
{
    [TestMethod]
    public async Task Ping_returns_empty_response()
    {
        var handlerCollectionMock = StrictMock.Of<IResourceHandlerCollection>();
        var typeDefinitionBuilderMock = StrictMock.Of<ITypeDefinitionBuilder>();

        var test = new BicepExtension(handlerCollectionMock.Object, typeDefinitionBuilderMock.Object);

        var response = await test.Ping(new Rpc.Empty(), GetTestServerCallContext());
        response.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetTypeFiles_returns_content_from_types()
    {
        var handlerCollectionMock = StrictMock.Of<IResourceHandlerCollection>();
        var typeDefinitionBuilderMock = StrictMock.Of<ITypeDefinitionBuilder>();

        TypeDefinition dummyTypes = new(
            IndexFileContent: "index content",
            TypeFileContents: new Dictionary<string, string>
            {
                { "type1.json", "content1" },
                { "type2.json", "content2" }
            }.ToImmutableDictionary()
        );
        typeDefinitionBuilderMock.Setup(x => x.GenerateTypeDefinition()).Returns(dummyTypes);

        var extension = new BicepExtension(handlerCollectionMock.Object, typeDefinitionBuilderMock.Object);
        var response = await extension.GetTypeFiles(new(), GetTestServerCallContext());

        response.IndexFile.Should().Be(dummyTypes.IndexFileContent);
        response.TypeFiles["type1.json"].Should().Be(dummyTypes.TypeFileContents["type1.json"]);
        response.TypeFiles["type2.json"].Should().Be(dummyTypes.TypeFileContents["type2.json"]);
    }

    [TestMethod]
    [DataRow("CreateOrUpdate")]
    [DataRow("Preview")]
    [DataRow("Delete")]
    [DataRow("Get")]
    public async Task Methods_return_error_if_no_handler_found(string methodName)
    {
        var handlerCollectionMock = StrictMock.Of<IResourceHandlerCollection>();
        var typeDefinitionBuilderMock = StrictMock.Of<ITypeDefinitionBuilder>();

        handlerCollectionMock.Setup(x => x.TryGetHandler(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string type, string apiVersion) => null);

        var extension = new BicepExtension(handlerCollectionMock.Object, typeDefinitionBuilderMock.Object);
        var (resourceSpec, resourceRef) = GetTestResourceSpecAndRef();

        var response = methodName switch
        {
            "CreateOrUpdate" => await extension.CreateOrUpdate(resourceSpec, GetTestServerCallContext()),
            "Preview" => await extension.Preview(resourceSpec, GetTestServerCallContext()),
            "Delete" => await extension.Delete(resourceRef, GetTestServerCallContext()),
            "Get" => await extension.Get(resourceRef, GetTestServerCallContext()),
            _ => throw new UnreachableException(),
        };

        response.Resource.Should().BeNull();
        response.ErrorData.Should().DeepEqualJson("""
        {
          "error": {
            "code": "HandlerNotRegistered",
            "message": "No handler registered for type 'Microsoft.Test/Test@2023-01-01'."
          }
        }
        """);
    }

    [TestMethod]
    [DataRow("CreateOrUpdate")]
    [DataRow("Preview")]
    [DataRow("Delete")]
    [DataRow("Get")]
    public async Task Methods_invoke_correct_handler(string methodName)
    {
        var handlerCollectionMock = StrictMock.Of<IResourceHandlerCollection>();
        var typeDefinitionBuilderMock = StrictMock.Of<ITypeDefinitionBuilder>();
        var handlerMock = StrictMock.Of<IResourceHandler>();

        var (resourceSpec, resourceRef) = GetTestResourceSpecAndRef();
        LocalExtensibilityOperationResponse expectedResponse = new()
        {
            Resource = new Resource
            {
                Type = resourceSpec.Type,
                Identifiers = resourceRef.Identifiers,
                Properties = resourceSpec.Properties,
                ApiVersion = resourceSpec.ApiVersion,
                Status = "Succeeded",
            }
        };
        handlerCollectionMock.Setup(x => x.TryGetHandler("Microsoft.Test/Test", "2023-01-01")).Returns(handlerMock.Object);
        handlerMock.Setup(x => x.CreateOrUpdate(resourceSpec, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        handlerMock.Setup(x => x.Preview(resourceSpec, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        handlerMock.Setup(x => x.Get(resourceRef, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        handlerMock.Setup(x => x.Delete(resourceRef, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        var extension = new BicepExtension(handlerCollectionMock.Object, typeDefinitionBuilderMock.Object);

        var response = methodName switch
        {
            "CreateOrUpdate" => await extension.CreateOrUpdate(resourceSpec, GetTestServerCallContext()),
            "Preview" => await extension.Preview(resourceSpec, GetTestServerCallContext()),
            "Delete" => await extension.Delete(resourceRef, GetTestServerCallContext()),
            "Get" => await extension.Get(resourceRef, GetTestServerCallContext()),
            _ => throw new UnreachableException(),
        };

        response.Should().BeSameAs(expectedResponse);
    }

    public static (ResourceSpecification ResourceSpec, ResourceReference ResourceRef) GetTestResourceSpecAndRef(
        string? resourceType = "Microsoft.Test/Test",
        string? apiVersion = "2023-01-01",
        string? properties = "{\"key\":\"value\"}",
        string? config = "{\"setting\":\"value\"}",
        string? identifiers = "{\"id\":\"12345\"}")
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

    private ServerCallContext GetTestServerCallContext()
    {
        return TestServerCallContext.Create(
            method: "dummy",
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
}

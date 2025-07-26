// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Types;
using Bicep.Local.Rpc;
using Grpc.Core;

namespace Bicep.Local.Extension.Host;

public class BicepExtension(
    IResourceHandlerCollection handlerCollection,
    ITypeDefinitionBuilder typeDefinitionBuilder) : Rpc.BicepExtension.BicepExtensionBase
{
    public override async Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ResourceSpecification request, ServerCallContext context)
    {
        var apiVersion = request.HasApiVersion ? request.ApiVersion : null;
        if (handlerCollection.TryGetHandler(request.Type, apiVersion) is { } handler)
        {
            return await handler.CreateOrUpdate(request, context.CancellationToken);
        }

        return CreateHandlerNotRegisteredResponse(request.Type, apiVersion);
    }

    public override async Task<LocalExtensibilityOperationResponse> Preview(ResourceSpecification request, ServerCallContext context)
    {
        var apiVersion = request.HasApiVersion ? request.ApiVersion : null;
        if (handlerCollection.TryGetHandler(request.Type, apiVersion) is { } handler)
        {
            return await handler.Preview(request, context.CancellationToken);
        }

        return CreateHandlerNotRegisteredResponse(request.Type, apiVersion);
    }

    public override async Task<LocalExtensibilityOperationResponse> Delete(ResourceReference request, ServerCallContext context)
    {
        var apiVersion = request.HasApiVersion ? request.ApiVersion : null;
        if (handlerCollection.TryGetHandler(request.Type, apiVersion) is { } handler)
        {
            return await handler.Delete(request, context.CancellationToken);
        }

        return CreateHandlerNotRegisteredResponse(request.Type, apiVersion);
    }

    public override async Task<LocalExtensibilityOperationResponse> Get(ResourceReference request, ServerCallContext context)
    {
        var apiVersion = request.HasApiVersion ? request.ApiVersion : null;
        if (handlerCollection.TryGetHandler(request.Type, apiVersion) is { } handler)
        {
            return await handler.Get(request, context.CancellationToken);
        }

        return CreateHandlerNotRegisteredResponse(request.Type, apiVersion);
    }

    public override Task<Rpc.Empty> Ping(Rpc.Empty request, ServerCallContext context)
        => Task.FromResult(new Rpc.Empty());

    public override Task<TypeFilesResponse> GetTypeFiles(Empty request, ServerCallContext context)
    {
        var types = typeDefinitionBuilder.GenerateTypeDefinition();

        TypeFilesResponse response = new()
        {
            IndexFile = types.IndexFileContent,
        };

        foreach (var kvp in types.TypeFileContents)
        {
            response.TypeFiles.Add(kvp.Key, kvp.Value);
        }

        return Task.FromResult(response);
    }

    private LocalExtensibilityOperationResponse CreateHandlerNotRegisteredResponse(string type, string? apiVersion)
    {
        var fullType = apiVersion is null ? type : $"{type}@{apiVersion}";

        return new()
        {
            ErrorData = new()
            {
                Error = new()
                {
                    Code = "HandlerNotRegistered",
                    Message = $"No handler registered for type '{fullType}'."
                },
            },
        };
    }
}

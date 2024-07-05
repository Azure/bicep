// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Text.Json.Nodes;
using Bicep.Local.Extension.Protocol;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Bicep.Local.Extension.Rpc;

public class BicepExtensionImpl : BicepExtension.BicepExtensionBase
{
    private readonly ILogger<BicepExtensionImpl> logger;
    private readonly ResourceDispatcher dispatcher;

    public BicepExtensionImpl(ILogger<BicepExtensionImpl> logger, ResourceDispatcher dispatcher)
    {
        this.logger = logger;
        this.dispatcher = dispatcher;
    }

    public override Task<ExtensibilityOperationResponse> Save(ExtensibilityOperationRequest request, ServerCallContext context)
        => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Resource.Type).Save(Convert(request), context.CancellationToken)));

    public override Task<ExtensibilityOperationResponse> PreviewSave(ExtensibilityOperationRequest request, ServerCallContext context)
        => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Resource.Type).PreviewSave(Convert(request), context.CancellationToken)));

    public override Task<ExtensibilityOperationResponse> Get(ExtensibilityOperationRequest request, ServerCallContext context)
        => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Resource.Type).Get(Convert(request), context.CancellationToken)));

    public override Task<ExtensibilityOperationResponse> Delete(ExtensibilityOperationRequest request, ServerCallContext context)
        => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Resource.Type).Delete(Convert(request), context.CancellationToken)));

    public override Task<Empty> Ping(Empty request, ServerCallContext context)
        => Task.FromResult(new Empty());

    private static Protocol.ExtensibilityOperationRequest Convert(ExtensibilityOperationRequest request)
    {
        return new(
            new(request.Import.Provider, request.Import.Version, request.Import.Config is { } ? JsonNode.Parse(request.Import.Config) as JsonObject : null),
            new(request.Resource.Type, request.Resource.Properties is { } ? JsonNode.Parse(request.Resource.Properties) as JsonObject : null));
    }

    private static ExtensibilityOperationResponse Convert(Protocol.ExtensibilityOperationResponse response)
    {
        var output = new ExtensibilityOperationResponse();
        if (response.Resource is { })
        {
            output.Resource = new ExtensibleResourceData
            {
                Type = response.Resource.Type,
                Properties = response.Resource.Properties?.ToString()
            };
        }

        if (response.ResourceMetadata is { } metadata)
        {
            output.ResourceMetadata.ReadOnlyProperties.AddRange(metadata.ReadOnlyProperties);
            output.ResourceMetadata.ImmutableProperties.AddRange(metadata.ImmutableProperties);
            output.ResourceMetadata.DynamicProperties.AddRange(metadata.DynamicProperties);
        }

        if (response.Errors is { } errors)
        {
            output.Errors.AddRange(errors.Select(error => new ExtensibilityError
            {
                Code = error.Code,
                Message = error.Message,
                Target = error.Target
            }));
        }

        return output;
    }

    private static async Task<ExtensibilityOperationResponse> WrapExceptions(Func<Task<ExtensibilityOperationResponse>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            var response = new ExtensibilityOperationResponse();
            response.Errors.Add(new ExtensibilityError
            {
                Code = "RpcException",
                Message = $"Rpc request failed: {ex}",
                Target = ""
            });

            return response;
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Nodes;
using Bicep.Local.Extension.Protocol;
using Google.Protobuf.Collections;
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

    public override Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ResourceSpecification request, ServerCallContext context)
        => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Type).CreateOrUpdate(Convert(request), context.CancellationToken)));

    public override Task<LocalExtensibilityOperationResponse> Preview(ResourceSpecification request, ServerCallContext context)
        => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Type).Preview(Convert(request), context.CancellationToken)));

    public override Task<LocalExtensibilityOperationResponse> Get(ResourceReference request, ServerCallContext context)
        => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Type).Get(Convert(request), context.CancellationToken)));

    public override Task<LocalExtensibilityOperationResponse> Delete(ResourceReference request, ServerCallContext context)
        => WrapExceptions(async () => Convert(await dispatcher.GetHandler(request.Type).Delete(Convert(request), context.CancellationToken)));

    public override Task<Empty> Ping(Empty request, ServerCallContext context)
        => Task.FromResult(new Empty());

    private Protocol.ResourceSpecification Convert(ResourceSpecification request)
    {
        JsonObject? config = GetExtensionConfig(request.Config);
        var properties = ToJsonObject(request.Properties, "Parsing resource properties failed. Please ensure is non-null or empty and is a valid JSON object.");

        return new(request.Type, request.ApiVersion, properties, config);
    }

    private Protocol.ResourceReference Convert(ResourceReference request)
    {
        JsonObject identifiers = ToJsonObject(request.Identifiers, "Parsing resource identifiers failed. Please ensure is non-null or empty and is a valid JSON object.");
        JsonObject? config = GetExtensionConfig(request.Config);

        return new(request.Type, request.ApiVersion, identifiers, config);
    }

    private JsonObject? GetExtensionConfig(string extensionConfig)
    {
        JsonObject? config = null;
        if (!string.IsNullOrEmpty(extensionConfig))
        {
            config = ToJsonObject(extensionConfig, "Parsing extension config failed. Please ensure is a valid JSON object.");
        }
        return config;
    }

    private JsonObject ToJsonObject(string json, string errorMessage)
        => JsonNode.Parse(json)?.AsObject() ?? throw new ArgumentNullException(errorMessage);

    private Resource? Convert(Protocol.Resource? response)
        => response is null ? null :
            new()
            {
                Identifiers = response.Identifiers.ToJsonString(),
                Properties = response.Properties.ToJsonString(),
                Status = response.Status,
                Type = response.Type,
                ApiVersion = response.ApiVersion,
            };

    private ErrorData? Convert(Protocol.ErrorData? response)
    {
        if (response is null)
        {
            return null;
        }

        var errorData = new ErrorData()
        {
            Error = new Error()
            {
                Code = response.Error.Code,
                Message = response.Error.Message,
                Target = response.Error.Target,
            }
        };

        if (response.Error.InnerError?.ToJsonString() is { } innerError)
        {
            errorData.Error.InnerError = innerError;
        }

        if (Convert(response.Error.Details) is { } errorDetails)
        {
            errorData.Error.Details.AddRange(errorDetails);
        }
        return errorData;
    }

    private RepeatedField<ErrorDetail>? Convert(Protocol.ErrorDetail[]? response)
    {
        if (response is null)
        {
            return null;
        }

        var list = new RepeatedField<ErrorDetail>();
        foreach (var item in response)
        {
            list.Add(Convert(item));
        }
        return list;
    }

    private ErrorDetail Convert(Protocol.ErrorDetail response)
        => new()
        {
            Code = response.Code,
            Message = response.Message,
            Target = response.Target,
        };

    private LocalExtensibilityOperationResponse Convert(Protocol.LocalExtensionOperationResponse response)
        => new()
        {
            ErrorData = Convert(response.ErrorData),
            Resource = Convert(response.Resource)
        };

    private static async Task<LocalExtensibilityOperationResponse> WrapExceptions(Func<Task<LocalExtensibilityOperationResponse>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            var response = new LocalExtensibilityOperationResponse
            {
                Resource = null,
                ErrorData = new ErrorData
                {
                    Error = new Error
                    {
                        Message = $"Rpc request failed: {ex}",
                        Code = "RpcException",
                        Target = ""
                    }
                }
            };

            return response;
        }
    }
}

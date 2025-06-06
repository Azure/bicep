// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Bicep.Local.Extension.Host.Handlers;
using Bicep.Local.Extension.Rpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Error = Bicep.Local.Extension.Rpc.Error;

namespace Bicep.Local.Extension.Host;

public class ResourceRequestDispatcher
    : BicepExtension.BicepExtensionBase
{
    private readonly ILogger<ResourceRequestDispatcher> logger;
    private readonly IResourceHandlerFactory resourceHandlerFactory;

    public ResourceRequestDispatcher(IResourceHandlerFactory resourceHandlerFactory, ILogger<ResourceRequestDispatcher> logger)
    {
        this.resourceHandlerFactory = resourceHandlerFactory ?? throw new ArgumentNullException(nameof(resourceHandlerFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<LocalExtensibilityOperationResponse> CreateOrUpdate(ResourceSpecification request, ServerCallContext context)
        => WrapExceptions(async () =>
           {
               var requestAndHandlers = GetHandlerRequestAndHandler(request);
               return ToLocalOperationResponse(await requestAndHandlers.Handler.CreateOrUpdate(requestAndHandlers.Request, context.CancellationToken));
           });


    public override Task<LocalExtensibilityOperationResponse> Preview(ResourceSpecification request, ServerCallContext context)
        => WrapExceptions(async () =>
           {
               var requestAndHandlers = GetHandlerRequestAndHandler(request);
               return ToLocalOperationResponse(await requestAndHandlers.Handler.Preview(requestAndHandlers.Request, context.CancellationToken));
           });

    public override Task<LocalExtensibilityOperationResponse> Get(ResourceReference request, ServerCallContext context)
        => WrapExceptions(async () => ToLocalOperationResponse(await resourceHandlerFactory.GetResourceHandler(request.Type).Handler.Get(ToHandlerRequest(request), context.CancellationToken)));

    public override Task<LocalExtensibilityOperationResponse> Delete(ResourceReference request, ServerCallContext context)
        => WrapExceptions(async () => ToLocalOperationResponse(await resourceHandlerFactory.GetResourceHandler(request.Type).Handler.Delete(ToHandlerRequest(request), context.CancellationToken)));

    public override Task<Empty> Ping(Empty request, ServerCallContext context)
        => Task.FromResult(new Empty());

    private (HandlerRequest Request, IResourceHandler Handler) GetHandlerRequestAndHandler(ResourceSpecification request)
    {
        var handlerMap = resourceHandlerFactory.GetResourceHandler(request.Type);

        if (handlerMap is null)
        {
            throw new InvalidOperationException("No typed or generic handler available for processing");
        }

        var resourceJson = ToJsonObject(request.Properties, "Parsing requested resource properties failed.");
        var extensionSettings = GetExtensionConfig(request.Config);

        if (handlerMap.Type == typeof(EmptyGeneric))
        {
            return (new HandlerRequest(request.Type, request.ApiVersion, extensionSettings, resourceJson),
                   handlerMap.Handler);
        }

        var resource = DeserializeJson(request.Type, resourceJson, handlerMap);
        var resourceType = typeof(HandlerRequest<>).MakeGenericType(handlerMap.Type);

        if (resourceType is null)
        {
            throw new InvalidOperationException($"Failed to generate request for {request.Type}");
        }

        var handlerRequest = Activator.CreateInstance(resourceType, resource, request.ApiVersion, extensionSettings, resourceJson) as HandlerRequest;

        return (handlerRequest ?? throw new InvalidOperationException($"Failed to process strongly typed request for {request.Type}")
              , handlerMap.Handler);
    }

    protected virtual HandlerRequest ToHandlerRequest(ResourceReference resourceReference)
    {
        var extensionSettings = GetExtensionConfig(resourceReference.Config);

        return new HandlerRequest(resourceReference.Type, resourceReference.HasApiVersion ? resourceReference.ApiVersion : "0.0.0");
    }

    protected virtual LocalExtensibilityOperationResponse ToLocalOperationResponse(HandlerResponse? handlerResponse)
        => handlerResponse is not null ?
        new LocalExtensibilityOperationResponse()
        {
            ErrorData = handlerResponse.Status == HandlerResponseStatus.Error ?
                            new ErrorData
                            {
                                Error = new Error()
                                {
                                    Code = handlerResponse.Error?.Code ?? $"HandlerException",
                                    Message = $"Handler request failed: {handlerResponse.Message}",
                                    InnerError = handlerResponse.Error?.Message ?? string.Empty,
                                    Target = handlerResponse.Error?.Target ?? string.Empty,
                                }
                            } : null,
            Resource = handlerResponse.Status != HandlerResponseStatus.Error ?
                            new Resource()
                            {
                                Status = handlerResponse.Status.ToString(),
                                Type = handlerResponse.Type,
                                ApiVersion = handlerResponse.ApiVersion ?? string.Empty,
                                Properties = handlerResponse.Properties?.ToJsonString() ?? string.Empty,
                                Identifiers = string.Empty
                            } : null
        }
        : throw new ArgumentNullException("Failed to process handler response. No response was provided.");

    protected virtual JsonObject? GetExtensionConfig(string extensionConfig)
    {
        JsonObject? config = null;
        if (!string.IsNullOrEmpty(extensionConfig))
        {
            config = ToJsonObject(extensionConfig, "Parsing extension config failed. Please ensure is a valid JSON object.");
        }
        return config;
    }

    protected virtual JsonObject ToJsonObject(string json, string errorMessage)
        => JsonNode.Parse(json)?.AsObject() ?? throw new ArgumentNullException(errorMessage);

    protected virtual object? DeserializeJson(string bicepType, JsonObject? resourceJson, TypeResourceHandler handlerMap)
    {
        if (resourceJson is null)
        {
            throw new ArgumentNullException($"No type mapping exists for resource `{resourceJson}`");
        }

        var jsonSerializerSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
        };

        var resource = JsonSerializer.Deserialize(resourceJson.ToJsonString(), handlerMap.Type, options: jsonSerializerSettings)
            ?? throw new ArgumentNullException($"No type mapping exists for resource `{bicepType}`");

        return resource;
    }

    protected virtual async Task<LocalExtensibilityOperationResponse> WrapExceptions(Func<Task<LocalExtensibilityOperationResponse>> func)
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
                        Target = string.Empty
                    }
                }
            };

            return response;
        }
    }
}

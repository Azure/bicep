// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Handlers;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Bicep.Local.Extension.Host;



public class ResourceRequestDispatcher
    : Rpc.BicepExtension.BicepExtensionBase
{
    private readonly ILogger<ResourceRequestDispatcher> logger;
    private readonly IResourceHandlerDispatcher resourceHandlerDispatcher;

    public ResourceRequestDispatcher(IResourceHandlerDispatcher resourceHandlerDispatcher, ILogger<ResourceRequestDispatcher> logger)
    {
        this.logger = logger;
        this.resourceHandlerDispatcher = resourceHandlerDispatcher ?? throw new ArgumentNullException(nameof(resourceHandlerDispatcher));
    }

    public override Task<Rpc.LocalExtensibilityOperationResponse> CreateOrUpdate(Rpc.ResourceSpecification request, ServerCallContext context)
        => WrapExceptions(async () =>
        {
            var handlerRequest = GetHandlerAndHandlerRequest(request);
            return ToLocalOperationResponse(await handlerRequest.Handler.CreateOrUpdate(handlerRequest.Request, context.CancellationToken));
        });


    public override Task<Rpc.LocalExtensibilityOperationResponse> Preview(Rpc.ResourceSpecification request, ServerCallContext context)
        => WrapExceptions(async () =>
        {
            var handlerRequest = GetHandlerAndHandlerRequest(request);
            return ToLocalOperationResponse(await handlerRequest.Handler.Preview(handlerRequest.Request, context.CancellationToken));
        });

    public override Task<Rpc.LocalExtensibilityOperationResponse> Get(Rpc.ResourceReference request, ServerCallContext context)
        => WrapExceptions(async () =>
        {
            var handlerRequest = GetHandlerAndHandlerRequest(request);
            return ToLocalOperationResponse(await handlerRequest.Handler.Get(handlerRequest.Request, context.CancellationToken));
        });

    public override Task<Rpc.LocalExtensibilityOperationResponse> Delete(Rpc.ResourceReference request, ServerCallContext context)
        => WrapExceptions(async () =>
        {
            var handlerRequest = GetHandlerAndHandlerRequest(request);
            return ToLocalOperationResponse(await handlerRequest.Handler.Delete(handlerRequest.Request, context.CancellationToken));
        });

    public override Task<Rpc.Empty> Ping(Rpc.Empty request, ServerCallContext context)
        => Task.FromResult(new Rpc.Empty());

    protected virtual (IResourceHandler Handler, HandlerRequest Request) GetHandlerAndHandlerRequest(Rpc.ResourceSpecification resourceSpecification)
    {
        ArgumentNullException.ThrowIfNull(resourceSpecification, nameof(resourceSpecification));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceSpecification.Type, nameof(resourceSpecification.Type));

        var resourceJson = ToJsonObject(resourceSpecification.Properties, "Parsing requested resource properties failed.");
        var config = resourceSpecification.HasConfig ? GetExtensionConfig(resourceSpecification.Config) : [];
        var apiVersion = resourceSpecification.HasApiVersion ? resourceSpecification.ApiVersion : null;

        return InternalGetHandlerAndHandlerRequest(resourceSpecification.Type, apiVersion, config, resourceJson, [], []);
    }

    protected virtual (IResourceHandler Handler, HandlerRequest Request) GetHandlerAndHandlerRequest(Rpc.ResourceReference resourceReference)
    {
        ArgumentNullException.ThrowIfNull(resourceReference, nameof(resourceReference));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceReference.Type, nameof(resourceReference.Type));
        
        var config = resourceReference.HasConfig ? GetExtensionConfig(resourceReference.Config) : [];
        var apiVersion = resourceReference.HasApiVersion ? resourceReference.ApiVersion : null;

        return InternalGetHandlerAndHandlerRequest(resourceReference.Type
                                                 , apiVersion
                                                 , config
                                                 , []
                                                 , []
                                                 , []);
    }

    protected virtual HandlerRequest ToHandlerRequest(Rpc.ResourceReference resourceReference)
    {
        var extensionSettings = GetExtensionConfig(resourceReference.Config);

        return new HandlerRequest(resourceReference.Type, resourceReference.HasApiVersion ? resourceReference.ApiVersion : "0.0.0");
    }

    private (IResourceHandler Handler, HandlerRequest Request) InternalGetHandlerAndHandlerRequest(string resourceType, string? apiVersion, JsonObject? config, JsonObject resourceJson, JsonObject identifiers, JsonObject properties)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceType, nameof(resourceType));

        if (!resourceHandlerDispatcher.TryGetTypedResourceHandler(resourceType, out TypedResourceHandler? typedResourceHandler))
        {
            // If no specific handler is found, validate a generic handler exists.
            // If a generic handler is available create a generic HandlerRequest.
            var genericHandler = resourceHandlerDispatcher.GenericResourceHandler;
            if (genericHandler is null)
            {
                throw new InvalidOperationException($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
            }

            return (genericHandler, new HandlerRequest(resourceType, apiVersion, config, resourceJson, [], []));
        }

        var resource = DeserializeJson(resourceType, resourceJson, typedResourceHandler);
        var type = typeof(HandlerRequest<>).MakeGenericType(typedResourceHandler.Type);

        if (type is null)
        {
            throw new InvalidOperationException($"Failed to generate request for {type}");
        }

        var handlerRequest = Activator.CreateInstance(type, resource, apiVersion, config, resourceJson, identifiers, properties) as HandlerRequest;

        ArgumentNullException.ThrowIfNull(handlerRequest, $"Failed to create handler request for resource type `{resourceType}`. Ensure the resource type is registered with a handler that supports it.");

        return (typedResourceHandler.Handler, handlerRequest);
    }

    protected virtual Rpc.LocalExtensibilityOperationResponse ToLocalOperationResponse(HandlerResponse? handlerResponse)
        => handlerResponse is not null ? new Rpc.LocalExtensibilityOperationResponse()
        {
            ErrorData = handlerResponse.Status == HandlerResponseStatus.Failed && handlerResponse.Error is not null ?
                            new Rpc.ErrorData
                            {
                                Error = new Rpc.Error()
                                {
                                    Code = handlerResponse.Error.Code,
                                    Message = handlerResponse.Message,
                                    InnerError = handlerResponse.Error.Message,
                                    Target = handlerResponse.Error.Target,
                                }
                            } : null,
            Resource = handlerResponse.Status != HandlerResponseStatus.Failed ?
                            new Rpc.Resource()
                            {
                                Status = handlerResponse.Status.ToString(),
                                Type = handlerResponse.Type,
                                ApiVersion = handlerResponse.ApiVersion,
                                Properties = handlerResponse.Properties?.ToJsonString(),
                                Identifiers = string.Empty
                            } : null
        } : throw new ArgumentNullException("Failed to process handler response. No response was provided.");

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

    protected virtual object? DeserializeJson(string bicepType, JsonObject resourceJson, TypedResourceHandler handler)
    {
        ArgumentNullException.ThrowIfNull(resourceJson, nameof(resourceJson));
        ArgumentNullException.ThrowIfNull(handler, nameof(handler));

        var jsonSerializerSettings = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
        };

        var resource = JsonSerializer.Deserialize(resourceJson.ToJsonString(), handler.Type, options: jsonSerializerSettings);

        if (resource is null)
        {
            throw new ArgumentNullException($"No type mapping exists for resource `{bicepType}`");
        }

        return resource;
    }

    protected virtual async Task<Rpc.LocalExtensibilityOperationResponse> WrapExceptions(Func<Task<Rpc.LocalExtensibilityOperationResponse>> func)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            var response = new Rpc.LocalExtensibilityOperationResponse
            {
                Resource = null,
                ErrorData = new Rpc.ErrorData
                {
                    Error = new Rpc.Error
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

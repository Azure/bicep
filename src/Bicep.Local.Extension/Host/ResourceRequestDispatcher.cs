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
using Bicep.Local.Extension.Types;
using Bicep.Local.Rpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Bicep.Local.Extension.Host;

public class ResourceRequestDispatcher : Rpc.BicepExtension.BicepExtensionBase
{
    private readonly ITypeDefinitionBuilder typeDefinitionBuilder;
    private readonly ILogger<ResourceRequestDispatcher> logger;

    public ResourceRequestDispatcher(IResourceHandlerDispatcher resourceHandlerDispatcher, ITypeDefinitionBuilder typeDefinitionBuilder, ILogger<ResourceRequestDispatcher> logger)
    {
        this.ResourceHandlerDispatcher = resourceHandlerDispatcher;
        this.typeDefinitionBuilder = typeDefinitionBuilder;
        this.logger = logger;
    }

    public IResourceHandlerDispatcher ResourceHandlerDispatcher { get; }

    public override async Task<Rpc.LocalExtensibilityOperationResponse> CreateOrUpdate(Rpc.ResourceSpecification request, ServerCallContext context)
        => await WrapExceptionsAsync(async () =>
        {
            var handlerRequest = GetHandlerAndHandlerRequest(request);
            return ToLocalOperationResponse(await handlerRequest.Handler.CreateOrUpdate(handlerRequest.Request, context.CancellationToken));
        });

    public override async Task<Rpc.LocalExtensibilityOperationResponse> Preview(Rpc.ResourceSpecification request, ServerCallContext context)
        => await WrapExceptionsAsync(async () =>
        {
            var handlerRequest = GetHandlerAndHandlerRequest(request);
            return ToLocalOperationResponse(await handlerRequest.Handler.Preview(handlerRequest.Request, context.CancellationToken));
        });

    public override async Task<Rpc.LocalExtensibilityOperationResponse> Get(Rpc.ResourceReference request, ServerCallContext context)
        => await WrapExceptionsAsync(async () =>
        {
            var handlerRequest = GetHandlerAndHandlerRequest(request);
            return ToLocalOperationResponse(await handlerRequest.Handler.Get(handlerRequest.Request, context.CancellationToken));
        });

    public override async Task<Rpc.LocalExtensibilityOperationResponse> Delete(Rpc.ResourceReference request, ServerCallContext context)
        => await WrapExceptionsAsync(async () =>
        {
            var handlerRequest = GetHandlerAndHandlerRequest(request);
            return ToLocalOperationResponse(await handlerRequest.Handler.Delete(handlerRequest.Request, context.CancellationToken));
        });

    public override Task<Rpc.Empty> Ping(Rpc.Empty request, ServerCallContext context)
        => Task.FromResult(new Rpc.Empty());

    public override async Task<TypeFilesResponse> GetTypeFiles(Empty request, ServerCallContext context)
    {
        await Task.CompletedTask;
        var types = typeDefinitionBuilder.GenerateTypeDefinition();

        TypeFilesResponse response = new()
        {
            IndexFile = types.IndexFileContent,
        };

        foreach (var kvp in types.TypeFileContents)
        {
            response.TypeFiles.Add(kvp.Key, kvp.Value);
        }

        return response;
    }

    protected virtual (IResourceHandler Handler, HandlerRequest Request) GetHandlerAndHandlerRequest(Rpc.ResourceSpecification resourceSpecification)
    {
        ArgumentNullException.ThrowIfNull(resourceSpecification, nameof(resourceSpecification));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceSpecification.Type, nameof(resourceSpecification.Type));

        var properties = ToJsonObject(resourceSpecification.Properties, "Parsing requested resource properties failed.");
        var config = resourceSpecification.HasConfig ? GetExtensionConfig(resourceSpecification.Config) : [];
        var apiVersion = resourceSpecification.HasApiVersion ? resourceSpecification.ApiVersion : null;

        return InternalGetHandlerAndHandlerRequest(resourceType: resourceSpecification.Type
                                                 , apiVersion: apiVersion
                                                 , config: config
                                                 , properties: properties
                                                 , identifiers: []);
    }

    protected virtual (IResourceHandler Handler, HandlerRequest Request) GetHandlerAndHandlerRequest(Rpc.ResourceReference resourceReference)
    {
        ArgumentNullException.ThrowIfNull(resourceReference, nameof(resourceReference));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceReference.Type, nameof(resourceReference.Type));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceReference.Identifiers, nameof(resourceReference.Identifiers));

        var config = resourceReference.HasConfig ? GetExtensionConfig(resourceReference.Config) : [];
        var apiVersion = resourceReference.HasApiVersion ? resourceReference.ApiVersion : null;
        var identifiers = ToJsonObject(resourceReference.Identifiers, "Parsing resource identifiers failed.");

        return InternalGetHandlerAndHandlerRequest(resourceType: resourceReference.Type
                                                 , apiVersion: apiVersion
                                                 , config: config ?? []
                                                 , properties: []
                                                 , identifiers: identifiers);
    }

    private (IResourceHandler Handler, HandlerRequest Request) InternalGetHandlerAndHandlerRequest(string resourceType, string? apiVersion, JsonObject config, JsonObject properties, JsonObject identifiers)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(resourceType, nameof(resourceType));

        if (!ResourceHandlerDispatcher.TryGetTypedResourceHandler(resourceType, out TypedResourceHandler? typedResourceHandler))
        {
            // If no specific handler is found, validate a generic handler exists.
            // If a generic handler is available create a generic HandlerRequest.
            var genericHandler = ResourceHandlerDispatcher.GenericResourceHandler;
            if (genericHandler is null)
            {
                throw new InvalidOperationException($"No handler found for resource type `{resourceType}`. Ensure a handler is registered for this type or provide a generic handler.");
            }

            return (genericHandler, new HandlerRequest(type: resourceType
                                                     , properties: properties
                                                     , config: config
                                                     , identifiers: identifiers
                                                     , apiVersion: apiVersion));
        }

        var resource = DeserializeJson(resourceType, properties, typedResourceHandler);
        var typedRequest = typeof(HandlerRequest<>).MakeGenericType(typedResourceHandler.Type);

        if (typedRequest is null)
        {
            throw new InvalidOperationException($"Failed to generate request for {resourceType}");
        }

        var handlerRequest = Activator.CreateInstance(typedRequest, resource, resourceType, properties, config, identifiers, apiVersion) as HandlerRequest;

        ArgumentNullException.ThrowIfNull(handlerRequest, $"Failed to create handler request for resource type `{resourceType}`. Ensure the resource type is registered with a handler that supports it.");

        return (typedResourceHandler.Handler, handlerRequest);
    }

    protected virtual Rpc.LocalExtensibilityOperationResponse ToLocalOperationResponse(HandlerResponse? handlerResponse)
    {
        if (handlerResponse is null)
        {
            throw new ArgumentNullException("Failed to process handler response. No response was provided.");
        }

        Rpc.Resource resource = new()
        {
            Status = handlerResponse.Status.ToString(),
            Type = handlerResponse.Type,
        };

        // This pattern with null checks is necessary, because the gRPC generated code throws on null value assignment, even if the field is optional.
        if (handlerResponse.ApiVersion is { } apiVersion)
        {
            resource.ApiVersion = apiVersion;
        }

        if (handlerResponse.Properties is { } properties)
        {
            resource.Properties = properties.ToJsonString();
        }

        if (handlerResponse.Identifiers is { } identifiers)
        {
            resource.Identifiers = identifiers.ToJsonString();
        }

        Rpc.Error? error = null;
        if (handlerResponse.Error is { } handlerError)
        {
            error = new()
            {
                Code = handlerError.Code,
                Message = handlerError.Message,
                Target = handlerError.Target,
            };

            foreach (var detail in handlerError.Details ?? [])
            {
                error.Details.Add(new Rpc.ErrorDetail
                {
                    Code = detail.Code,
                    Message = detail.Message,
                    Target = detail.Target,
                });
            }
        }

        return new()
        {
            ErrorData = error is { } ? new() { Error = error } : null,
            Resource = resource,
        };
    }

    protected virtual JsonObject GetExtensionConfig(string extensionConfig)
    {
        JsonObject? config = null;
        if (!string.IsNullOrWhiteSpace(extensionConfig))
        {
            config = ToJsonObject(extensionConfig, "Parsing extension config failed. Please ensure is a valid JSON object.");
        }
        return config ?? [];
    }

    protected virtual JsonObject ToJsonObject(string json, string errorMessage)
    {
        try
        {
            return JsonNode.Parse(json)?.AsObject() ?? throw new ArgumentNullException(errorMessage);
        }
        catch (Exception ex)
        {
            throw new ArgumentException(errorMessage, ex); // New stack trace here
        }
    }

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

    protected virtual async Task<Rpc.LocalExtensibilityOperationResponse> WrapExceptionsAsync(Func<Task<Rpc.LocalExtensibilityOperationResponse>> func)
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

            logger.LogError(ex, "An error occurred while processing the RPC request.");

            return response;
        }
    }

}

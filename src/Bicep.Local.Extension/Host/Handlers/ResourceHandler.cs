// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bicep.Local.Extension.Types.Attributes;

namespace Bicep.Local.Extension.Host.Handlers;

public abstract class ResourceHandler<TProperties, TIdentifiers, TConfig> : IResourceHandler
    where TProperties : class
    where TIdentifiers : class
    where TConfig : class
{
    public class ResourceBase
    {
        public required string Type { get; set; }
        public string? ApiVersion { get; set; }
        public required TConfig Config { get; set; }
    }

    public class ResourceRequest : ResourceBase
    {
        public required TProperties Properties { get; set; }
    }

    public class ReferenceRequest : ResourceBase
    {
        public required TIdentifiers Identifiers { get; set; }
    }

    public class ResourceResponse
    {
        public required string Type { get; set; }
        public string? ApiVersion { get; set; }
        public TProperties? Properties { get; set; }
        public required TIdentifiers Identifiers { get; set; }
    }

    public class ErrorDetail
    {
        public required string Code { get; set; }
        public string? Target { get; set; }
        public required string Message { get; set; }
    }

    public class Error
    {
        public required string Code { get; set; }
        public string? Target { get; set; }
        public required string Message { get; set; }
        public ImmutableArray<ErrorDetail>? Details { get; set; }
    }

    public class ResourceErrorException : Exception
    {
        public ResourceErrorException(string code, string message, string? target = null, ImmutableArray<ErrorDetail>? details = null)
            : this(new Error
            {
                Code = code,
                Message = message,
                Target = target,
                Details = details
            })
        {
        }

        public ResourceErrorException(Error error)
            : base(error.Message)
        {
            Error = error;
        }

        public Error Error { get; }
    }

    public Task<Rpc.LocalExtensibilityOperationResponse> CreateOrUpdate(Rpc.ResourceSpecification rpcRequest, CancellationToken cancellationToken)
        => WrapExceptionsAsync(async () =>
        {
            var request = GetResourceRequest(rpcRequest);
            var response = await CreateOrUpdate(request, cancellationToken);

            return GetRpcResponse(response);
        });

    public Task<Rpc.LocalExtensibilityOperationResponse> Preview(Rpc.ResourceSpecification rpcRequest, CancellationToken cancellationToken)
        => WrapExceptionsAsync(async () =>
        {
            var request = GetResourceRequest(rpcRequest);
            var response = await Preview(request, cancellationToken);

            return GetRpcResponse(response);
        });

    public Task<Rpc.LocalExtensibilityOperationResponse> Get(Rpc.ResourceReference rpcRequest, CancellationToken cancellationToken)
        => WrapExceptionsAsync(async () =>
        {
            var request = GetReferenceRequest(rpcRequest);
            var response = await Get(request, cancellationToken);

            return GetRpcResponse(response);
        });

    public Task<Rpc.LocalExtensibilityOperationResponse> Delete(Rpc.ResourceReference rpcRequest, CancellationToken cancellationToken)
        => WrapExceptionsAsync(async () =>
        {
            var request = GetReferenceRequest(rpcRequest);
            var response = await Delete(request, cancellationToken);

            return GetRpcResponse(response);
        });

    public abstract string? Type { get; }

    public abstract string? ApiVersion { get; }

    protected virtual Task<ResourceResponse> CreateOrUpdate(ResourceRequest request, CancellationToken cancellationToken)
        => throw new ResourceErrorException("NotImplemented", $"Operation '{this.GetType().FullName}.{nameof(CreateOrUpdate)}' has not been implemented.");

    protected virtual Task<ResourceResponse> Preview(ResourceRequest request, CancellationToken cancellationToken)
        => throw new ResourceErrorException("NotImplemented", $"Operation '{this.GetType().FullName}.{nameof(Preview)}' has not been implemented.");

    protected virtual Task<ResourceResponse> Get(ReferenceRequest request, CancellationToken cancellationToken)
        => throw new ResourceErrorException("NotImplemented", $"Operation '{this.GetType().FullName}.{nameof(Get)}' has not been implemented.");

    protected virtual Task<ResourceResponse> Delete(ReferenceRequest request, CancellationToken cancellationToken)
        => throw new ResourceErrorException("NotImplemented", $"Operation '{this.GetType().FullName}.{nameof(Delete)}' has not been implemented.");

    protected abstract TIdentifiers GetIdentifiers(TProperties properties);

    protected virtual JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private Rpc.LocalExtensibilityOperationResponse GetRpcResponse(ResourceResponse response)
    {
        return new()
        {
            Resource = new()
            {
                Type = response.Type,
                ApiVersion = response.ApiVersion,
                Properties = JsonSerializer.Serialize(response.Properties, SerializerOptions),
                Identifiers = JsonSerializer.Serialize(response.Identifiers, SerializerOptions),
            },
        };
    }

    private ResourceRequest GetResourceRequest(Rpc.ResourceSpecification request)
    {
        var config = GetConfig(request.Config);
        var properties = JsonSerializer.Deserialize<TProperties>(request.Properties, options: SerializerOptions)
            ?? throw new ArgumentException("Properties cannot be null.", nameof(request.Properties));

        return new()
        {
            Type = request.Type,
            ApiVersion = request.ApiVersion,
            Config = config,
            Properties = properties,
        };
    }

    protected ResourceResponse GetResponse(ResourceRequest request)
        => new()
        {
            Type = request.Type,
            ApiVersion = request.ApiVersion,
            Properties = request.Properties,
            Identifiers = GetIdentifiers(request.Properties),
        };

    public static ResourceResponse GetResponse(ReferenceRequest request, TProperties? properties)
        => new()
        {
            Type = request.Type,
            ApiVersion = request.ApiVersion,
            Properties = properties,
            Identifiers = request.Identifiers,
        };

    private ReferenceRequest GetReferenceRequest(Rpc.ResourceReference request)
    {
        var config = GetConfig(request.Config);
        var identifiers = JsonSerializer.Deserialize<TIdentifiers>(request.Identifiers, options: SerializerOptions)
            ?? throw new ArgumentException("Identifiers cannot be null.", nameof(request.Identifiers));

        return new()
        {
            Type = request.Type,
            ApiVersion = request.ApiVersion,
            Config = config,
            Identifiers = identifiers,
        };
    }

    protected virtual TConfig GetConfig(string configJson)
        => JsonSerializer.Deserialize<TConfig>(configJson, options: SerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize configuration.");

    protected virtual async Task<Rpc.LocalExtensibilityOperationResponse> WrapExceptionsAsync(Func<Task<Rpc.LocalExtensibilityOperationResponse>> func)
    {
        try
        {
            return await func();
        }
        catch (ResourceErrorException ex)
        {
            Rpc.Error error = new()
            {
                Message = ex.Error.Message,
                Code = ex.Error.Code,
            };

            if (ex.Error.Target is { } target)
            {
                error.Target = target;
            }

            foreach (var detail in ex.Error.Details ?? [])
            {
                Rpc.ErrorDetail errorDetail = new()
                {
                    Message = detail.Message,
                    Code = detail.Code,
                };

                if (detail.Target is { } detailTarget)
                {
                    errorDetail.Target = detailTarget;
                }

                error.Details.Add(errorDetail);
            }

            return new Rpc.LocalExtensibilityOperationResponse
            {
                ErrorData = new Rpc.ErrorData
                {
                    Error = error
                }
            };
        }
        catch (Exception ex)
        {
            return new Rpc.LocalExtensibilityOperationResponse
            {
                ErrorData = new Rpc.ErrorData
                {
                    Error = new Rpc.Error
                    {
                        Message = $"Rpc request failed: {ex.Message}",
                        Code = "RpcException",
                    }
                }
            };
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.ExtensionHost.Extensions;
using Bicep.Local.Rpc;

namespace Bicep.ExtensionHost.Api;

/// <summary>
/// Request model for extension operations.
/// </summary>
public record ExtensionRequest
{
    /// <summary>
    /// The operation to perform: "CreateOrUpdate", "Preview", "Get", or "Delete".
    /// </summary>
    public required string Operation { get; init; }

    /// <summary>
    /// Resource specification for CreateOrUpdate/Preview operations.
    /// </summary>
    public ResourceSpecificationDto? Specification { get; init; }

    /// <summary>
    /// Resource reference for Get/Delete operations.
    /// </summary>
    public ResourceReferenceDto? Reference { get; init; }
}

/// <summary>
/// Resource specification DTO matching the gRPC ResourceSpecification message.
/// </summary>
public record ResourceSpecificationDto
{
    public string? Config { get; init; }
    public required string Type { get; init; }
    public string? ApiVersion { get; init; }
    public required JsonElement Properties { get; init; }
}

/// <summary>
/// Resource reference DTO matching the gRPC ResourceReference message.
/// </summary>
public record ResourceReferenceDto
{
    public required JsonElement Identifiers { get; init; }
    public string? Config { get; init; }
    public required string Type { get; init; }
    public string? ApiVersion { get; init; }
}

/// <summary>
/// Response model for extension operations.
/// </summary>
public record ExtensionResponse
{
    public ResourceDto? Resource { get; init; }
    public ErrorDataDto? ErrorData { get; init; }
}

/// <summary>
/// Resource DTO matching the gRPC Resource message.
/// </summary>
public record ResourceDto
{
    public required string Type { get; init; }
    public string? ApiVersion { get; init; }
    public JsonElement? Identifiers { get; init; }
    public JsonElement? Properties { get; init; }
    public string? Status { get; init; }
}

/// <summary>
/// Error data DTO matching the gRPC ErrorData message.
/// </summary>
public record ErrorDataDto
{
    public required ErrorDto Error { get; init; }
}

/// <summary>
/// Error DTO matching the gRPC Error message.
/// </summary>
public record ErrorDto
{
    public required string Code { get; init; }
    public string? Target { get; init; }
    public required string Message { get; init; }
    public List<ErrorDetailDto>? Details { get; init; }
    public string? InnerError { get; init; }
}

/// <summary>
/// Error detail DTO matching the gRPC ErrorDetail message.
/// </summary>
public record ErrorDetailDto
{
    public required string Code { get; init; }
    public string? Target { get; init; }
    public required string Message { get; init; }
}

/// <summary>
/// Handles extension API requests by proxying them to the appropriate gRPC server.
/// </summary>
public class ExtensionApiHandler
{
    private readonly ExtensionManager _extensionManager;
    private readonly ILogger<ExtensionApiHandler> _logger;

    public ExtensionApiHandler(ExtensionManager extensionManager, ILogger<ExtensionApiHandler> logger)
    {
        _extensionManager = extensionManager;
        _logger = logger;
    }

    /// <summary>
    /// Processes an extension request and returns the response.
    /// </summary>
    public async Task<ExtensionResponse> HandleRequestAsync(
        string extensionName,
        ExtensionRequest request,
        CancellationToken cancellationToken)
    {
        var extension = _extensionManager.GetExtension(extensionName);
        if (extension is null)
        {
            return new ExtensionResponse
            {
                ErrorData = new ErrorDataDto
                {
                    Error = new ErrorDto
                    {
                        Code = "ExtensionNotFound",
                        Message = $"Extension '{extensionName}' not found. Available extensions: {string.Join(", ", _extensionManager.GetExtensionNames())}"
                    }
                }
            };
        }

        if (!extension.IsRunning)
        {
            return new ExtensionResponse
            {
                ErrorData = new ErrorDataDto
                {
                    Error = new ErrorDto
                    {
                        Code = "ExtensionNotRunning",
                        Message = $"Extension '{extensionName}' is not running"
                    }
                }
            };
        }

        try
        {
            var grpcResponse = request.Operation.ToUpperInvariant() switch
            {
                "CREATEORUPDATE" => await HandleCreateOrUpdateAsync(extension, request, cancellationToken),
                "PREVIEW" => await HandlePreviewAsync(extension, request, cancellationToken),
                "GET" => await HandleGetAsync(extension, request, cancellationToken),
                "DELETE" => await HandleDeleteAsync(extension, request, cancellationToken),
                _ => throw new ArgumentException($"Unknown operation: {request.Operation}")
            };

            return ConvertResponse(grpcResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling request for extension '{Name}'", extensionName);
            return new ExtensionResponse
            {
                ErrorData = new ErrorDataDto
                {
                    Error = new ErrorDto
                    {
                        Code = "InternalError",
                        Message = ex.Message
                    }
                }
            };
        }
    }

    private async Task<LocalExtensibilityOperationResponse> HandleCreateOrUpdateAsync(
        ExtensionInstance extension,
        ExtensionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Specification is null)
        {
            throw new ArgumentException("Specification is required for CreateOrUpdate operation");
        }

        var grpcRequest = ConvertToResourceSpecification(request.Specification);
        return await extension.Client.CreateOrUpdateAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    private async Task<LocalExtensibilityOperationResponse> HandlePreviewAsync(
        ExtensionInstance extension,
        ExtensionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Specification is null)
        {
            throw new ArgumentException("Specification is required for Preview operation");
        }

        var grpcRequest = ConvertToResourceSpecification(request.Specification);
        return await extension.Client.PreviewAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    private async Task<LocalExtensibilityOperationResponse> HandleGetAsync(
        ExtensionInstance extension,
        ExtensionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Reference is null)
        {
            throw new ArgumentException("Reference is required for Get operation");
        }

        var grpcRequest = ConvertToResourceReference(request.Reference);
        return await extension.Client.GetAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    private async Task<LocalExtensibilityOperationResponse> HandleDeleteAsync(
        ExtensionInstance extension,
        ExtensionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Reference is null)
        {
            throw new ArgumentException("Reference is required for Delete operation");
        }

        var grpcRequest = ConvertToResourceReference(request.Reference);
        return await extension.Client.DeleteAsync(grpcRequest, cancellationToken: cancellationToken);
    }

    private static ResourceSpecification ConvertToResourceSpecification(ResourceSpecificationDto dto)
    {
        var spec = new ResourceSpecification
        {
            Type = dto.Type,
            Properties = dto.Properties.GetRawText()
        };

        if (dto.Config is not null)
        {
            spec.Config = dto.Config;
        }

        if (dto.ApiVersion is not null)
        {
            spec.ApiVersion = dto.ApiVersion;
        }

        return spec;
    }

    private static ResourceReference ConvertToResourceReference(ResourceReferenceDto dto)
    {
        var reference = new ResourceReference
        {
            Type = dto.Type,
            Identifiers = dto.Identifiers.GetRawText()
        };

        if (dto.Config is not null)
        {
            reference.Config = dto.Config;
        }

        if (dto.ApiVersion is not null)
        {
            reference.ApiVersion = dto.ApiVersion;
        }

        return reference;
    }

    private static ExtensionResponse ConvertResponse(LocalExtensibilityOperationResponse grpcResponse)
    {
        var response = new ExtensionResponse();

        if (grpcResponse.Resource is { } resource)
        {
            response = response with
            {
                Resource = new ResourceDto
                {
                    Type = resource.Type,
                    ApiVersion = string.IsNullOrEmpty(resource.ApiVersion) ? null : resource.ApiVersion,
                    Identifiers = ParseJsonElement(resource.Identifiers),
                    Properties = ParseJsonElement(resource.Properties),
                    Status = string.IsNullOrEmpty(resource.Status) ? null : resource.Status
                }
            };
        }

        if (grpcResponse.ErrorData is { } errorData)
        {
            response = response with
            {
                ErrorData = new ErrorDataDto
                {
                    Error = new ErrorDto
                    {
                        Code = errorData.Error.Code,
                        Message = errorData.Error.Message,
                        Target = string.IsNullOrEmpty(errorData.Error.Target) ? null : errorData.Error.Target,
                        InnerError = string.IsNullOrEmpty(errorData.Error.InnerError) ? null : errorData.Error.InnerError,
                        Details = errorData.Error.Details.Count > 0
                            ? errorData.Error.Details.Select(d => new ErrorDetailDto
                            {
                                Code = d.Code,
                                Message = d.Message,
                                Target = string.IsNullOrEmpty(d.Target) ? null : d.Target
                            }).ToList()
                            : null
                    }
                }
            };
        }

        return response;
    }

    private static JsonElement? ParseJsonElement(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonDocument.Parse(json).RootElement.Clone();
    }
}

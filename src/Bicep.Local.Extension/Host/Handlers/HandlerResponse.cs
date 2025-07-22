// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Represents the status of a resource handler operation.
/// </summary>
public enum HandlerResponseStatus
{
    /// <summary>
    /// Indicates the operation completed successfully.
    /// </summary>
    Succeeded,

    /// <summary>
    /// Indicates the operation failed due to an error.
    /// </summary>
    Failed,

    /// <summary>
    /// Indicates the operation was canceled before completion.
    /// </summary>
    Canceled,

    /// <summary>
    /// Indicates the operation exceeded its allowed execution time.
    /// </summary>
    TimedOut
}

public record ErrorDetail(string Code, string Target, string Message);

/// <summary>
/// Represents an error that occurred during a resource handler operation.
/// </summary>
/// <param name="Code">The error code that identifies the error type.</param>
/// <param name="Target">The target of the error (e.g., resource name, property name).</param>
/// <param name="Message">A human-readable description of the error.</param>
/// <param name="Details">Optional inner errors that provide additional details about the root cause.</param>
public record Error(string Code, string Target, string Message, ImmutableArray<ErrorDetail>? Details = null);

/// <summary>
/// Represents the response from a resource handler operation.
/// Contains the result status, resource data, and any error information.
/// </summary>
public class HandlerResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerResponse"/> class with the specified parameters.
    /// </summary>
    /// <param name="type">The resource type identifier (e.g., "Microsoft.Storage/storageAccounts"). Cannot be null or whitespace.</param>
    /// <param name="status">The status of the operation.</param>
    /// <param name="properties">The resource properties as a JSON object. Cannot be null.</param>
    /// <param name="identifiers">The resource identifiers as a JSON object. Cannot be null.</param>
    /// <param name="apiVersion">The API version of the resource. Can be null if no specific version is applicable.</param>
    /// <param name="error">The error that occurred during the operation, if any. Should be provided when status is Failed or TimedOut.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is null or whitespace.</exception>
    public HandlerResponse(string type, HandlerResponseStatus status, JsonObject properties, JsonObject identifiers, string? apiVersion, Error? error = null)
    {
        Type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentException(nameof(type)) : type;
        ApiVersion = apiVersion;
        Status = status;
        Properties = properties;
        Identifiers = identifiers;
        Error = error;
    }

    /// <summary>
    /// Gets the resource type identifier (e.g., "Microsoft.Storage/storageAccounts").
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the API version of the resource. Can be null if no specific version is applicable.
    /// </summary>
    public string? ApiVersion { get; }

    /// <summary>
    /// Gets the status of the operation.
    /// </summary>
    public HandlerResponseStatus Status { get; }

    /// <summary>
    /// Gets the resource properties as a JSON object.
    /// Contains the actual resource configuration and state information.
    /// </summary>
    public JsonObject Properties { get; }

    /// <summary>
    /// Gets the resource identifiers as a JSON object.
    /// Contains key-value pairs that uniquely identify the resource instance.
    /// </summary>
    public JsonObject Identifiers { get; }

    /// <summary>
    /// Gets the error that occurred during the operation, if any.
    /// Typically populated when Status is Failed or TimedOut.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="resourceType">The resource type identifier.</param>
    /// <param name="properties">The resource properties as a JSON object.</param>
    /// <param name="identifiers">The resource identifiers as a JSON object.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <returns>A new <see cref="HandlerResponse"/> with a <see cref="HandlerResponseStatus.Succeeded"/> status.</returns>
    public static HandlerResponse Success(string resourceType, JsonObject properties, JsonObject identifiers, string? apiVersion)
        => new(resourceType, HandlerResponseStatus.Succeeded, properties: properties, identifiers: identifiers, apiVersion: apiVersion);

    /// <summary>
    /// Creates a failed response indicating an error.
    /// </summary>
    /// <param name="resourceType">The resource type identifier.</param>
    /// <param name="properties">The resource properties as a JSON object.</param>
    /// <param name="identifiers">The resource identifiers as a JSON object.</param>
    /// <param name="error">The error that occurred during the operation.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <returns>A new <see cref="HandlerResponse"/> with a <see cref="HandlerResponseStatus.Failed"/> status.</returns>
    public static HandlerResponse Failed(string resourceType, JsonObject properties, JsonObject identifiers, Error error, string? apiVersion)
        => new(resourceType, HandlerResponseStatus.Failed, properties: properties, identifiers: identifiers, apiVersion: apiVersion, error: error);

    /// <summary>
    /// Creates a response indicating the operation was canceled.
    /// </summary>
    /// <param name="resourceType">The resource type identifier.</param>
    /// <param name="properties">The resource properties as a JSON object.</param>
    /// <param name="identifiers">The resource identifiers as a JSON object.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <returns>A new <see cref="HandlerResponse"/> with a <see cref="HandlerResponseStatus.Canceled"/> status.</returns>
    public static HandlerResponse Canceled(string resourceType, JsonObject properties, JsonObject identifiers, string? apiVersion)
        => new(resourceType, HandlerResponseStatus.Canceled, properties: properties, identifiers: identifiers, apiVersion: apiVersion);

    /// <summary>
    /// Creates a response indicating the operation timed out.
    /// </summary>
    /// <param name="resourceType">The resource type identifier.</param>
    /// <param name="properties">The resource properties as a JSON object.</param>
    /// <param name="identifiers">The resource identifiers as a JSON object.</param>
    /// <param name="error">The error that describes the timeout condition.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <returns>A new <see cref="HandlerResponse"/> with a <see cref="HandlerResponseStatus.TimedOut"/> status.</returns>
    public static HandlerResponse TimedOut(string resourceType, JsonObject properties, JsonObject identifiers, Error error, string? apiVersion)
        => new(resourceType, HandlerResponseStatus.TimedOut, properties: properties, identifiers: identifiers, apiVersion: apiVersion, error: error);
}

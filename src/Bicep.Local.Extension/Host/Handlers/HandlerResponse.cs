// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    Success,

    /// <summary>
    /// Indicates the operation failed due to an error.
    /// </summary>
    Error,

    /// <summary>
    /// Indicates the operation was canceled before completion.
    /// </summary>
    Canceled,

    /// <summary>
    /// Indicates the operation exceeded its allowed execution time.
    /// </summary>
    TimedOut
}

/// <summary>
/// Represents an error that occurred during a resource handler operation.
/// </summary>
/// <param name="Code">The error code that identifies the error type.</param>
/// <param name="Target">The target of the error (e.g., resource name, property name).</param>
/// <param name="Message">A human-readable description of the error.</param>
public record Error(string Code, string Target, string Message);

/// <summary>
/// Represents the response from a resource handler operation.
/// </summary>
public class HandlerResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerResponse"/> class with the specified parameters.
    /// </summary>
    /// <param name="type">The resource type identifier.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <param name="status">The status of the operation.</param>
    /// <param name="properties">The properties of the response.</param>
    public HandlerResponse(string type, string? apiVersion, HandlerResponseStatus status, JsonObject? properties)
        : this(type, apiVersion, status, properties, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerResponse"/> class with the specified parameters.
    /// </summary>
    /// <param name="type">The resource type identifier. Cannot be null or whitespace.</param>
    /// <param name="apiVersion">The API version of the resource. Cannot be null or whitespace.</param>
    /// <param name="status">The status of the operation.</param>
    /// <param name="properties">The properties of the response. If null, an empty object is used.</param>
    /// <param name="error">The error that occurred during the operation, if any.</param>
    /// <param name="message">An optional message providing additional information about the response.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> or <paramref name="apiVersion"/> is null or whitespace.</exception>
    public HandlerResponse(string type, string? apiVersion, HandlerResponseStatus status, JsonObject? properties, Error? error, string? message = null)
    {
        Type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentException(nameof(type)) : type;
        ApiVersion = apiVersion;
        Status = status;
        Properties = properties ?? [];
        Error = error;
        Message = message;
    }

    /// <summary>
    /// Gets the resource type identifier.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the API version of the resource.
    /// </summary>
    public string? ApiVersion { get; }

    /// <summary>
    /// Gets the status of the operation.
    /// </summary>
    public HandlerResponseStatus Status { get; }

    /// <summary>
    /// Gets the properties of the response.
    /// Never null; defaults to an empty object if not provided.
    /// </summary>
    public JsonObject Properties { get; }

    /// <summary>
    /// Gets additional extension settings included in the response.
    /// </summary>
    public JsonObject? ExtensionSettings { get; }

    /// <summary>
    /// Gets the error that occurred during the operation, if any.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Gets an optional message providing additional information about the response.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="resourceType">The resource type identifier.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <param name="properties">The properties of the response.</param>
    /// <param name="message">An optional message providing additional information about the response.</param>
    /// <returns>A new <see cref="HandlerResponse"/> with a <see cref="HandlerResponseStatus.Success"/> status.</returns>
    public static HandlerResponse Success(string resourceType, string? apiVersion, JsonObject? properties, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Success, properties, null, message: message);

    /// <summary>
    /// Creates a failed response indicating an error.
    /// </summary>
    /// <param name="resourceType">The resource type identifier.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <param name="properties">The properties of the response.</param>
    /// <param name="error">The error that occurred during the operation.</param>
    /// <param name="message">An optional message providing additional information about the error.</param>
    /// <returns>A new <see cref="HandlerResponse"/> with a <see cref="HandlerResponseStatus.Error"/> status.</returns>
    public static HandlerResponse Failed(string resourceType, string? apiVersion, JsonObject? properties, Error? error = null, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Error, properties, error, message: message);

    /// <summary>
    /// Creates a response indicating the operation was canceled.
    /// </summary>
    /// <param name="resourceType">The resource type identifier.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <param name="properties">The properties of the response.</param>
    /// <param name="message">An optional message providing additional information about the cancellation.</param>
    /// <returns>A new <see cref="HandlerResponse"/> with a <see cref="HandlerResponseStatus.Canceled"/> status.</returns>
    public static HandlerResponse Canceled(string resourceType, string? apiVersion, JsonObject? properties, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Canceled, properties, null, message: message);

    /// <summary>
    /// Creates a response indicating the operation timed out.
    /// </summary>
    /// <param name="resourceType">The resource type identifier.</param>
    /// <param name="apiVersion">The API version of the resource.</param>
    /// <param name="properties">The properties of the response.</param>
    /// <param name="message">An optional message providing additional information about the timeout.</param>
    /// <returns>A new <see cref="HandlerResponse"/> with a <see cref="HandlerResponseStatus.TimedOut"/> status.</returns>
    public static HandlerResponse TimedOut(string resourceType, string? apiVersion, JsonObject? properties, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.TimedOut, properties, null, message: message);
}

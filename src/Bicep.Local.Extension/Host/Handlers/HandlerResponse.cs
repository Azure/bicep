// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Nodes;

namespace Bicep.Local.Extension.Host.Handlers;
public enum HandlerResponseStatus
{
    Success,
    Error,
    Canceled,
    TimedOut
}

public record Error(string Code, string Target, string Message);

public class HandlerResponse
{
    public HandlerResponse(string type, string? apiVersion, HandlerResponseStatus status, JsonObject? properties)
        : this(type, apiVersion, status, properties, null)
    { }

    public HandlerResponse(string type, string? apiVersion, HandlerResponseStatus status, JsonObject? properties, Error? error, string? message = null)
    {
        Type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentNullException(nameof(type)) : type;
        ApiVersion = apiVersion;
        Status = status;
        Properties = properties ?? [];
        Error = error;
        Message = message;
    }

    public string Type { get; }
    public string? ApiVersion { get; }
    public HandlerResponseStatus Status { get; }
    public JsonObject Properties { get; }
    public JsonObject? ExtensionSettings { get; }
    public Error? Error { get; }
    public string? Message { get; }

    public static HandlerResponse Success(string resourceType, string apiVersion, JsonObject? properties, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Success, properties, null, message: message);

    public static HandlerResponse Failed(string resourceType, string apiVersion, JsonObject? properties, Error? errors = null, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Error, properties, errors, message: message);

    public static HandlerResponse Canceled(string resourceType, string apiVersion, JsonObject? properties, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Canceled, properties, null, message: message);

    public static HandlerResponse TimedOut(string resourceType, string apiVersion, JsonObject? properties, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.TimedOut, properties, null, message: message);
}

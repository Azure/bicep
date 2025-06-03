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
    public HandlerResponse(string type, string version, HandlerResponseStatus status, JsonObject? properties)
        : this(type, version, status, properties, null)
    { }

    public HandlerResponse(string type, string version, HandlerResponseStatus status, JsonObject? properties, Error? error, string? message = null)
    {
        Type = type;
        Version = version;
        Status = status;
        Properties = properties ?? new();
        Error = error;
        Message = message;
    }

    public string Type { get; }
    public string? Version { get; }
    public HandlerResponseStatus Status { get; }
    public JsonObject Properties { get; }
    public JsonObject? ExtensionSettings { get; }
    public Error? Error { get; }
    public string? Message { get; }

    public static HandlerResponse Success(string resourceType, string apiVersion, JsonObject? properties = null, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Success, properties, null, message: message);

    public static HandlerResponse Failed(string resourceType, string apiVersion, JsonObject? properties = null, Error? errors = null, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Error, properties, errors, message: message);

    public static HandlerResponse Canceled(string resourceType, string apiVersion, JsonObject? properties = null, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.Canceled, properties, null, message: message);

    public static HandlerResponse TimedOut(string resourceType, string apiVersion, JsonObject? properties = null, string? message = null)
        => new(resourceType, apiVersion, HandlerResponseStatus.TimedOut, properties, null, message: message);
}


// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Nodes;

namespace Bicep.Local.Extension.Host.Handlers;
public class HandlerRequest
{
    public HandlerRequest(string type, string apiVersion)
        : this(type, apiVersion, new(), new())
    { }

    public HandlerRequest(string type, string? apiVersion, JsonObject? extensionSettings, JsonObject? resourceJson)
    {
        Type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentNullException(nameof(type)) : type;
        ApiVersion = apiVersion;
        ExtensionSettings = extensionSettings ?? [];
        ResourceJson = resourceJson ?? [];
    }

    public string Type { get; }
    public string? ApiVersion { get; }

    public JsonObject? ExtensionSettings { get; }
    public JsonObject? ResourceJson { get; }
}

public class HandlerRequest<TResource>
    : HandlerRequest
    where TResource : class
{
    public HandlerRequest(TResource resource, string? apiVersion, JsonObject? extensionSettings, JsonObject resourceJson)
        : base(resource?.GetType().Name ?? throw new ArgumentNullException(nameof(resource))
            , apiVersion
            , extensionSettings
            , resourceJson)
    {
        Resource = resource;
    }

    public TResource Resource { get; }
}

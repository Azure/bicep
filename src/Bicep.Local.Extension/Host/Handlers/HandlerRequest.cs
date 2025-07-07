// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Nodes;

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Represents a request to perform an operation on a resource through a resource handler.
/// </summary>
public class HandlerRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerRequest"/> class with the specified resource type and API version.
    /// </summary>
    /// <param name="type">The resource type identifier.</param>
    /// <param name="apiVersion">The API version to use for the request.</param>
    public HandlerRequest(string type, string apiVersion)
        : this(type, apiVersion, [], [])
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerRequest"/> class with the specified parameters.
    /// </summary>
    /// <param name="type">The resource type identifier. Cannot be null or whitespace.</param>
    /// <param name="apiVersion">The API version to use for the request.</param>
    /// <param name="extensionSettings">Additional settings for the extension processing the request. If null, an empty object is used.</param>
    /// <param name="resourceJson">The JSON representation of the resource. If null, an empty object is used.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null or whitespace.</exception>
    public HandlerRequest(string type, string apiVersion, JsonObject? extensionSettings, JsonObject? resourceJson)
    {
        Type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentNullException(nameof(type)) : type;
        ApiVersion = apiVersion;
        ExtensionSettings = extensionSettings ?? [];
        ResourceJson = resourceJson ?? [];
    }

    /// <summary>
    /// Gets the resource type identifier.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the API version to use for the request.
    /// </summary>
    public string ApiVersion { get; }

    /// <summary>
    /// Gets additional settings for the extension processing the request.
    /// </summary>
    public JsonObject? ExtensionSettings { get; }

    /// <summary>
    /// Gets the JSON representation of the resource.
    /// </summary>
    public JsonObject? ResourceJson { get; }
}

/// <summary>
/// Represents a strongly-typed request to perform an operation on a resource through a resource handler.
/// </summary>
/// <typeparam name="TResource">The type of the resource.</typeparam>
public class HandlerRequest<TResource>
    : HandlerRequest
    where TResource : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerRequest{TResource}"/> class with the specified parameters.
    /// </summary>
    /// <param name="resource">The strongly-typed resource instance. Cannot be null.</param>
    /// <param name="apiVersion">The API version to use for the request.</param>
    /// <param name="extensionSettings">Additional settings for the extension processing the request. If null, an empty object is used.</param>
    /// <param name="resourceJson">The JSON representation of the resource. If null, an empty object is used.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resource"/> is null.</exception>
    public HandlerRequest(TResource resource, string apiVersion, JsonObject? extensionSettings, JsonObject resourceJson)
        : base(resource?.GetType().Name ?? throw new ArgumentNullException(nameof(resource))
            , apiVersion
            , extensionSettings
            , resourceJson)
    {
        Resource = resource;
    }

    /// <summary>
    /// Gets the strongly-typed resource instance.
    /// </summary>
    public TResource Resource { get; }
}

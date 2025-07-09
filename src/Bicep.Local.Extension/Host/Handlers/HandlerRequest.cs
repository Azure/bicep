// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

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
    public HandlerRequest(string type, string? apiVersion)
        : this(type, apiVersion, [], [], [], [])
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerRequest"/> class with the specified parameters.
    /// </summary>
    /// <param name="type">The resource type identifier. Cannot be null or whitespace.</param>
    /// <param name="apiVersion">The API version to use for the request. Can be null.</param>
    /// <param name="config">Additional settings for the extension processing the request. If null, an empty object is used.</param>
    /// <param name="resourceJson">The JSON representation of the resource. If null, an empty object is used.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is null or whitespace.</exception>
    public HandlerRequest(string type, string? apiVersion, JsonObject? config, JsonObject? resourceJson, JsonObject identifiers, JsonObject properties)
    {
        Type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentException(nameof(type)) : type;
        ApiVersion = apiVersion;
        Config = config ?? [];
        ResourceJson = resourceJson ?? [];
        Identifiers = identifiers;
        Properties = properties;
    }

    /// <summary>
    /// Gets the resource type identifier.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the API version to use for the request. Can be null if no specific API version is required.
    /// </summary>
    public string? ApiVersion { get; }

    /// <summary>
    /// Gets additional settings for the extension processing the request.
    /// Never null; defaults to an empty object if not provided.
    /// </summary>
    public JsonObject Config { get; }

    /// <summary>
    /// Gets the JSON representation of the resource.
    /// Never null; defaults to an empty object if not provided.
    /// </summary>
    public JsonObject ResourceJson { get; protected set; }

    public JsonObject Identifiers { get; protected set; }

    public JsonObject Properties { get; protected set; }
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
    /// <param name="apiVersion">The API version to use for the request. Can be null.</param>
    /// <param name="config">Additional settings for the extension processing the request. If null, an empty object is used.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resource"/> is null.</exception>
    /// <remarks>
    /// The resource type identifier is derived from the runtime type name of the provided resource instance.
    /// </remarks>
    public HandlerRequest(TResource resource, string? apiVersion, JsonObject? config, JsonObject? resourceJson, JsonObject identifiers, JsonObject properties)
        : base(resource?.GetType().Name ?? throw new ArgumentNullException(nameof(resource))
            , apiVersion
            , config
            , resourceJson
            , identifiers
            , properties)
    {
        Resource = resource;
    }



    /// <summary>
    /// Gets the strongly-typed resource instance.
    /// </summary>
    public TResource Resource { get; }
}

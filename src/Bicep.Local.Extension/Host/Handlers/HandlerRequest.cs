// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Represents a request to perform an operation on a resource through a resource handler.
/// Contains the resource type, properties, configuration, and metadata needed for resource operations.
/// </summary>
public class HandlerRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerRequest"/> class with the specified parameters.
    /// </summary>
    /// <param name="type">The resource type identifier (e.g., "Microsoft.Storage/storageAccounts"). Cannot be null or whitespace.</param>
    /// <param name="properties">The resource properties as a JSON object. Cannot be null.</param>
    /// <param name="config">Extension configuration settings as a JSON object. Cannot be null.</param>
    /// <param name="identifiers">Resource identifiers as a JSON object. Cannot be null.</param>
    /// <param name="apiVersion">The API version to use for the request. Can be null if no specific API version is required.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is null or whitespace.</exception>
    public HandlerRequest(string type, JsonObject properties, JsonObject config, JsonObject identifiers, string? apiVersion)
    {
        Type = string.IsNullOrWhiteSpace(type) ? throw new ArgumentException(nameof(type)) : type;
        ApiVersion = apiVersion;
        Config = config;
        Properties = properties;
        Identifiers = identifiers;
    }

    /// <summary>
    /// Gets the resource type identifier (e.g., "Microsoft.Storage/storageAccounts").
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the API version to use for the request. Can be null if no specific API version is required.
    /// </summary>
    public string? ApiVersion { get; }

    /// <summary>
    /// Gets the extension configuration settings as a JSON object.
    /// Contains configuration values that control how the extension processes the request.
    /// </summary>
    public JsonObject Config { get; }

    /// <summary>
    /// Gets or sets the resource identifiers as a JSON object.
    /// Contains key-value pairs that uniquely identify the resource instance.
    /// </summary>
    public JsonObject Identifiers { get; protected set; }

    /// <summary>
    /// Gets or sets the resource properties as a JSON object.
    /// Contains the actual resource configuration and state information.
    /// </summary>
    public JsonObject Properties { get; protected set; }
}

/// <summary>
/// Represents a strongly-typed request to perform an operation on a resource through a resource handler.
/// Provides both the raw JSON representation and a strongly-typed resource instance.
/// </summary>
/// <typeparam name="TResource">The type of the resource that constrains the operations to a specific resource model.</typeparam>
public class HandlerRequest<TResource> : HandlerRequest
    where TResource : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerRequest{TResource}"/> class with the specified parameters.
    /// </summary>
    /// <param name="resource">The strongly-typed resource instance. Cannot be null.</param>
    /// <param name="resourceType">The resource type identifier (e.g., "Microsoft.Storage/storageAccounts"). Cannot be null or whitespace.</param>
    /// <param name="properties">The resource properties as a JSON object. Cannot be null.</param>
    /// <param name="config">Extension configuration settings as a JSON object. Cannot be null.</param>
    /// <param name="identifiers">Resource identifiers as a JSON object. Cannot be null.</param>
    /// <param name="apiVersion">The API version to use for the request. Can be null if no specific API version is required.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="resource"/> is null.</exception>
    /// <remarks>
    /// This constructor provides both strongly-typed access through the <see cref="Resource"/> property
    /// and raw JSON access through the inherited properties for maximum flexibility.
    /// </remarks>
    public HandlerRequest(TResource resource, string resourceType, JsonObject properties, JsonObject config, JsonObject identifiers, string? apiVersion)
        : base(resourceType, properties, config, identifiers, apiVersion)
    {
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
    }

    /// <summary>
    /// Gets the strongly-typed resource instance.
    /// Provides type-safe access to the resource properties and methods.
    /// </summary>
    public TResource Resource { get; }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Represents a pairing of a resource Type with its corresponding resource Handler.
/// This record is used to map specific resource types to their appropriate handlers.
/// </summary>
/// <param name="Type">The CLR Type that represents the resource model (e.g., typeof(StorageAccount)).</param>
/// <param name="Handler">The resource handler that processes operations for the specified Type.</param>
public record TypedResourceHandler(Type Type, IResourceHandler Handler);

/// <summary>
/// Defines a dispatcher that routes resource operations to the appropriate resource handlers.
/// The dispatcher maintains mappings between resource types and their corresponding handlers,
/// supporting both strongly-typed and generic resource handlers.
/// </summary>
/// <remarks>
/// The dispatcher uses a two-tier approach:
/// 1. First, it attempts to find a strongly-typed handler for the specific resource type
/// 2. If no typed handler is found, it falls back to a generic handler that can process any resource type
/// This allows for both type-safe operations and flexible fallback handling.
/// </remarks>
public interface IResourceHandlerDispatcher
{
    /// <summary>
    /// Gets a frozen dictionary of type-specific resource handlers, where the key is the resource type name
    /// and the value is the associated TypedResourceHandler.
    /// </summary>
    /// <remarks>
    /// This dictionary contains handlers for strongly-typed resources that implement <see cref="IResourceHandler{TResource}"/>.
    /// The dictionary is frozen for performance and immutability after initialization.
    /// Can be null if no typed handlers are registered.
    /// </remarks>
    FrozenDictionary<string, TypedResourceHandler>? TypedResourceHandlers { get; }

    /// <summary>
    /// Gets the generic resource handler that can process any resource type if available.
    /// </summary>
    /// <remarks>
    /// This is a fallback handler used when no type-specific handler is found for a resource type.
    /// It implements the non-generic <see cref="IResourceHandler"/> interface and works with JSON objects directly.
    /// Can be null if no generic handler is registered.
    /// </remarks>
    IResourceHandler? GenericResourceHandler { get; }

    /// <summary>
    /// Attempts to retrieve a typed resource handler for the specified CLR resource type.
    /// </summary>
    /// <param name="resourceType">The CLR Type of the resource to get a handler for (e.g., typeof(StorageAccount)).</param>
    /// <param name="typedResourceHandler">When this method returns, contains the TypedResourceHandler if found; otherwise, null.</param>
    /// <returns>True if a typed handler was found for the specified CLR type; otherwise, false.</returns>
    /// <remarks>
    /// This overload is used when you have the actual CLR Type and want to find its corresponding handler.
    /// It searches the TypedResourceHandlers dictionary using the type information.
    /// </remarks>
    bool TryGetTypedResourceHandler(Type resourceType, [NotNullWhen(true)] out TypedResourceHandler? typedResourceHandler);

    /// <summary>
    /// Attempts to retrieve a typed resource handler for the specified resource type name.
    /// </summary>
    /// <param name="resourceType">The name/identifier of the resource type (e.g., "Microsoft.Storage/storageAccounts").</param>
    /// <param name="typedResourceHandler">When this method returns, contains the TypedResourceHandler if found; otherwise, null.</param>
    /// <returns>True if a typed handler was found for the specified resource type name; otherwise, false.</returns>
    /// <remarks>
    /// This overload is used when you have the resource type name string and want to find its corresponding handler.
    /// It searches the TypedResourceHandlers dictionary using the string key.
    /// This is typically used during request processing when the resource type comes from external sources.
    /// </remarks>
    bool TryGetTypedResourceHandler(string resourceType, [NotNullWhen(true)] out TypedResourceHandler? typedResourceHandler);
}

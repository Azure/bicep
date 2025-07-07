// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Represents a pairing of a resource Type with its corresponding resource Handler.
/// This record is used to map specific resource types to their appropriate handlers.
/// </summary>
/// <param name="Type">The CLR Type that represents the resource model.</param>
/// <param name="Handler">The resource handler that processes operations for the specified Type.</param>
public record TypeResourceHandler(Type Type, IResourceHandler Handler);

/// <summary>
/// Defines a dispatcher that routes resource operations to the appropriate resource handlers.
/// The dispatcher maintains mappings between resource types and their corresponding handlers,
/// supporting both typed and generic resource handlers.
/// </summary>
public interface IResourceHandlerDispatcher
{
    /// <summary>
    /// Gets a dictionary of type-specific resource handlers, where the key is the resource type name
    /// and the value is the associated TypeResourceHandler.
    /// </summary>
    /// <remarks>
    /// This dictionary contains handlers for strongly-typed resources that implement <see cref="IResourceHandler{TResource}"/>.
    /// </remarks>
    FrozenDictionary<string, TypeResourceHandler>? TypedResourceHandlers { get; }
    
    /// <summary>
    /// Gets the generic resource handler that can process any resource type if available.
    /// </summary>
    /// <remarks>
    /// This is a fallback handler used when no type-specific handler is found for a resource type.
    /// It implements <see cref="IResourceHandler"/> without the generic type parameter.
    /// </remarks>
    IResourceHandler? GenericResourceHandler { get; }

    /// <summary>
    /// Retrieves the appropriate resource handler for the specified resource type name.
    /// </summary>
    /// <param name="resourceType">The name of the resource type to get a handler for.</param>
    /// <returns>A TypeResourceHandler that can process operations for the specified resource type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is found for the specified resource type.</exception>
    /// <remarks>
    /// This method first checks for a type-specific handler in TypedResourceHandlers and falls back to
    /// the GenericResourceHandler if no specific handler is found.
    /// </remarks>
    IResourceHandler GetResourceHandler(string resourceType);

    /// <summary>
    /// Retrieves the appropriate resource handler for the specified resource type.
    /// </summary>
    /// <param name="resourceType">The CLR Type of the resource to get a handler for.</param>
    /// <returns>A TypeResourceHandler that can process operations for the specified resource type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when resourceType is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no handler is found for the specified resource type.</exception>
    /// <remarks>
    /// This method internally calls <see cref="GetResourceHandler(string)"/> with the name of the provided type.
    /// </remarks>
    IResourceHandler GetResourceHandler(Type resourceType);
}

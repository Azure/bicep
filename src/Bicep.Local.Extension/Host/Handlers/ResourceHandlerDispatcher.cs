// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Bicep.Local.Extension.Host.Extensions;

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Represents a generic resource type used as a placeholder for the generic resource handler when no specific type is required.
/// This is used internally to represent untyped handlers in the type mapping system.
/// </summary>
internal record GenericResource();

/// <summary>
/// Implements the <see cref="IResourceHandlerDispatcher"/> interface to route resource operations to appropriate handlers.
/// This dispatcher maintains a registry of resource handlers and maps resource types to their corresponding handlers,
/// supporting both strongly-typed and generic resource handlers.
/// </summary>
/// <remarks>
/// The dispatcher uses a two-tier approach:
/// 1. Strongly-typed handlers that implement <see cref="IResourceHandler{TResource}"/> for specific resource types
/// 2. A generic handler that implements <see cref="IResourceHandler"/> as a fallback for any resource type
/// During initialization, it analyzes all provided handlers and builds appropriate type mappings.
/// </remarks>
public class ResourceHandlerDispatcher : IResourceHandlerDispatcher
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceHandlerDispatcher"/> class.
    /// </summary>
    /// <param name="resourceHandlers">A collection of resource handlers to be registered with the dispatcher.</param>
    /// <exception cref="ArgumentException">Thrown when no resource handlers are provided or the collection is null.</exception>
    /// <remarks>
    /// During initialization, the dispatcher analyzes each handler to determine if it's a strongly-typed
    /// handler (implements <see cref="IResourceHandler{TResource}"/>) or a generic handler (implements only <see cref="IResourceHandler"/>).
    /// Only one generic handler is allowed, but multiple strongly-typed handlers can be registered for different resource types.
    /// </remarks>
    public ResourceHandlerDispatcher(IEnumerable<IResourceHandler> resourceHandlers)
    {
        if (resourceHandlers is null || resourceHandlers.Count() == 0)
        {
            throw new ArgumentException("No resource handlers were provided.", nameof(resourceHandlers));
        }

        var resourceHandlerMaps = BuildResourceHandlerTypeMap(resourceHandlers);

        TypedResourceHandlers = resourceHandlerMaps.Typed;
        GenericResourceHandler = resourceHandlerMaps.Generic?.Handler;
    }

    /// <summary>
    /// Gets a frozen dictionary of type-specific resource handlers, where the key is the resource type name
    /// and the value is the associated TypedResourceHandler.
    /// </summary>
    /// <remarks>
    /// This dictionary contains handlers for strongly-typed resources that implement <see cref="IResourceHandler{TResource}"/>.
    /// The dictionary is frozen for performance and immutability after initialization.
    /// Keys are the CLR type names (e.g., "StorageAccount") of the resource types.
    /// </remarks>
    public FrozenDictionary<string, TypedResourceHandler> TypedResourceHandlers { get; }

    /// <summary>
    /// Gets the generic resource handler that can process any resource type if available.
    /// </summary>
    /// <remarks>
    /// This is a fallback handler used when no type-specific handler is found for a resource type.
    /// It implements the non-generic <see cref="IResourceHandler"/> interface and works with JSON objects directly.
    /// Can be null if no generic handler was registered during initialization.
    /// </remarks>
    public IResourceHandler? GenericResourceHandler { get; }

    /// <summary>
    /// Attempts to retrieve a typed resource handler for the specified CLR resource type.
    /// </summary>
    /// <param name="resourceType">The CLR Type of the resource to get a handler for (e.g., typeof(StorageAccount)).</param>
    /// <param name="typedResourceHandler">When this method returns, contains the TypedResourceHandler if found; otherwise, null.</param>
    /// <returns>True if a typed handler was found for the specified CLR type; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when resourceType is null.</exception>
    /// <remarks>
    /// This overload delegates to the string-based overload using the type's Name property.
    /// It does not fall back to the generic handler - use the generic handler separately if needed.
    /// </remarks>
    public bool TryGetTypedResourceHandler(Type resourceType, [NotNullWhen(true)] out TypedResourceHandler? typedResourceHandler)
        => TryGetTypedResourceHandler(resourceType?.Name ?? throw new ArgumentNullException(nameof(resourceType))
                                    , out typedResourceHandler);

    /// <summary>
    /// Attempts to retrieve a typed resource handler for the specified resource type name.
    /// </summary>
    /// <param name="resourceType">The name of the resource type to get a handler for (typically the CLR type name).</param>
    /// <param name="typedResourceHandler">When this method returns, contains the TypedResourceHandler if found; otherwise, null.</param>
    /// <returns>True if a typed handler was found for the specified resource type name; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when resourceType is null or whitespace.</exception>
    /// <remarks>
    /// This method only searches the TypedResourceHandlers dictionary and does not fall back to the GenericResourceHandler.
    /// If no typed handler is found, callers should check the GenericResourceHandler property separately.
    /// </remarks>
    public bool TryGetTypedResourceHandler(string resourceType, [NotNullWhen(true)] out TypedResourceHandler? typedResourceHandler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceType, nameof(resourceType));

        return TypedResourceHandlers.TryGetValue(resourceType, out typedResourceHandler);        
    }

    private static (TypedResourceHandler? Generic, FrozenDictionary<string, TypedResourceHandler> Typed) BuildResourceHandlerTypeMap(IEnumerable<IResourceHandler> resourceHandlers)
    {
        var handlerDictionary = new Dictionary<string, TypedResourceHandler>();
        TypedResourceHandler? genericHandler = null;

        foreach (var resourceHandler in resourceHandlers)
        {
            var resourceHandlerType = resourceHandler.GetType();

            if (resourceHandlerType.TryGetTypedResourceHandlerInterface(out Type? baseInterface))
            {
                if (baseInterface is null)
                {
                    throw new InvalidOperationException($"Unable to find a resource handler interface for {resourceHandlerType.FullName}");
                }

                Type resourceType = baseInterface.GetGenericArguments()[0];

                if (!handlerDictionary.TryAdd(resourceType.Name, new(resourceType, resourceHandler)))
                {
                    throw new InvalidOperationException($"A resource handler for {resourceType.Name} has already been registered.");
                }
            }
            else if (resourceHandlerType.IsGenericTypedResourceHandler())
            {
                if (genericHandler is not null)
                {
                    throw new InvalidOperationException($"A generic resource handler has already been registered.");
                }

                genericHandler = new TypedResourceHandler(typeof(GenericResource), resourceHandler);
            }
            else
            {
                throw new InvalidOperationException($"{resourceHandlerType.FullName} does not implement a valid resource handler interface.");
            }
        }

        return (genericHandler, handlerDictionary.ToFrozenDictionary());
    }
}

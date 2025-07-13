// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Bicep.Local.Extension.Host.Extensions;

namespace Bicep.Local.Extension.Host.Handlers;

/// <summary>
/// Represents a generic resource type used as a placeholder for the untyped resource handler when no specific type is required.
/// </summary>
internal record GenericResource();

/// <summary>
/// Implements the <see cref="IResourceHandlerDispatcher"/> interface to route resource operations to appropriate handlers.
/// This dispatcher maintains a registry of resource handlers and maps resource types to their corresponding handlers.
/// </summary>
public class ResourceHandlerDispatcher
    : IResourceHandlerDispatcher
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceHandlerDispatcher"/> class.
    /// </summary>
    /// <param name="resourceHandlers">A collection of resource handlers to be registered with the dispatcher.</param>
    /// <exception cref="ArgumentException">Thrown when no resource handlers are provided.</exception>
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
    /// Gets a dictionary of type-specific resource handlers, where the key is the resource type name
    /// and the value is the associated TypeResourceHandler.
    /// </summary>
    /// <remarks>
    /// This dictionary contains handlers for strongly-typed resources that implement <see cref="IResourceHandler{TResource}"/>.
    /// </remarks>
    public FrozenDictionary<string, TypedResourceHandler> TypedResourceHandlers { get; }

    /// <summary>
    /// Gets the untyped resource handler that can process any resource type if available.
    /// </summary>
    /// <remarks>
    /// This is a fallback handler used when no type-specific handler is found for a resource type.
    /// It implements the non-generic <see cref="IResourceHandler"/> interface.
    /// </remarks>
    public IResourceHandler? GenericResourceHandler { get; }


    /// <summary>
    /// Retrieves the appropriate resource handler for the specified resource type.
    /// </summary>
    /// <param name="resourceType">The CLR Type of the resource to get a handler for.</param>
    /// <returns>An IResourceHandler that can process operations for the specified resource type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when resourceType is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when no handler is found for the specified resource type.</exception>
    /// <remarks>    
    /// </remarks>
    public bool TryGetTypedResourceHandler(Type resourceType, [NotNullWhen(true)] out TypedResourceHandler? typedResourceHandler)
        => TryGetTypedResourceHandler(resourceType?.Name ?? throw new ArgumentNullException(nameof(resourceType))
                                    , out typedResourceHandler);

    /// <summary>
    /// Retrieves the appropriate resource handler for the specified resource type name.
    /// </summary>
    /// <param name="resourceType">The name of the resource type to get a handler for.</param>
    /// <returns>An IResourceHandler that can process operations for the specified resource type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is found for the specified resource type.</exception>
    /// <remarks>
    /// This method first checks for a type-specific handler in TypedResourceHandlers and falls back to
    /// the GenericResourceHandler if no specific handler is found.
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

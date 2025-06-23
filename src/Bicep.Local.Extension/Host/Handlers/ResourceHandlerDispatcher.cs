// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.Local.Extension.Host.Extensions;

namespace Bicep.Local.Extension.Host.Handlers;

public record EmptyGeneric();

public class ResourceHandlerDispatcher
    : IResourceHandlerDispatcher
{
    public FrozenDictionary<string, TypeResourceHandler> TypedResourceHandlers { get; }
    public TypeResourceHandler? GenericResourceHandler { get; }

    public ResourceHandlerDispatcher(IEnumerable<IResourceHandler> resourceHandlers)
    {
        if (resourceHandlers is null || resourceHandlers.Count() == 0)
        {
            throw new InvalidOperationException("No resource handlers were provided.");
        }

        var resourceHandlerMaps = BuildResourceHandlerTypeMap(resourceHandlers);

        TypedResourceHandlers = resourceHandlerMaps.Typed;
        GenericResourceHandler = resourceHandlerMaps.Generic;
    }


    public TypeResourceHandler GetResourceHandler(Type resourceType)
        => GetResourceHandler(resourceType?.Name ?? throw new ArgumentNullException(nameof(resourceType)));

    public TypeResourceHandler GetResourceHandler(string resourceType)
    {
        if (TypedResourceHandlers.TryGetValue(resourceType, out var handlerMap))
        {
            return handlerMap;
        }
        else if (GenericResourceHandler is not null)
        {
            return GenericResourceHandler;
        }

        throw new InvalidOperationException($"No generic or typed resource handler found for type {resourceType}. Ensure the resource handler is registered.");

    }

    private static (TypeResourceHandler? Generic, FrozenDictionary<string, TypeResourceHandler> Typed) BuildResourceHandlerTypeMap(IEnumerable<IResourceHandler> resourceHandlers)
    {
        var handlerDictionary = new Dictionary<string, TypeResourceHandler>();
        TypeResourceHandler? genericHandler = null;

        foreach (var resourceHandler in resourceHandlers)
        {
            var resourceHandlerType = resourceHandler.GetType();

            if (resourceHandlerType.TryGetTypedResourceHandlerInterface(out Type? baseInterface))
            {
                if (baseInterface is null)
                {
                    throw new ArgumentException($"Unable to find a resource handler interface for {resourceHandlerType.FullName}");
                }

                Type resourceType = baseInterface.GetGenericArguments()[0];

                if (!handlerDictionary.TryAdd(resourceType.Name, new(resourceType, resourceHandler)))
                {
                    throw new ArgumentException($"A resource handler for {resourceType.Name} has already been registered.");
                }
            }
            else if (resourceHandlerType.IsGenericTypedResourceHandler())
            {
                if (genericHandler is not null)
                {
                    throw new ArgumentException($"A generic resource handler has already been registered.");
                }

                genericHandler = new TypeResourceHandler(typeof(EmptyGeneric), resourceHandler);
            }
            else
            {
                throw new ArgumentException($"Unable to register handler {resourceHandlerType.FullName}");
            }
        }

        return (genericHandler, handlerDictionary.ToFrozenDictionary());
    }
}

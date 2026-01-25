// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;
using Bicep.Local.Extension.Host.Handlers;

namespace Bicep.Local.Extension.Host;

public class ResourceHandlerCollection : IResourceHandlerCollection
{
    private record HandlerKey(string Type, string? ApiVersion);

    private readonly FrozenDictionary<HandlerKey, IResourceHandler> typedHandlers;
    private readonly IResourceHandler? genericHandler;

    public ResourceHandlerCollection(IEnumerable<IResourceHandler> handlers)
    {


        IResourceHandler? genericHandler = null;
        var typedHandlers = new Dictionary<HandlerKey, IResourceHandler>();
        foreach (var handler in handlers)
        {
            if (handler.Type is null)
            {
                if (genericHandler is not null)
                {
                    throw new InvalidOperationException("Only one generic resource handler can be registered.");
                }
                genericHandler = handler;
            }
            else
            {
                HandlerKey key = new(handler.Type, handler.ApiVersion);
                if (!typedHandlers.TryAdd(key, handler))
                {
                    throw new InvalidOperationException($"A handler for type '{key.Type}' with API version '{key.ApiVersion}' is already registered.");
                }
            }
        }

        this.genericHandler = genericHandler;
        this.typedHandlers = typedHandlers.ToFrozenDictionary();
    }

    public IResourceHandler? TryGetHandler(string type, string? apiVersion)
    {
        if (typedHandlers.TryGetValue(new(type, apiVersion), out var handler))
        {
            return handler;
        }

        if (genericHandler is not null)
        {
            return genericHandler;
        }

        return null;
    }
}

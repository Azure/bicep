// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Local.Extension.Protocol;

public class ResourceDispatcherBuilder
{
    private IGenericResourceHandler? genericResourceHandler;
    private readonly Dictionary<string, IResourceHandler> resourceHandlers = new(StringComparer.OrdinalIgnoreCase);

    public ResourceDispatcherBuilder AddHandler(IResourceHandler handler)
    {
        if (!this.resourceHandlers.TryAdd(handler.ResourceType, handler))
        {
            throw new ArgumentException($"Resource type '{handler.ResourceType}' has already been registered.");
        }

        this.resourceHandlers[handler.ResourceType] = handler;
        return this;
    }

    public ResourceDispatcherBuilder AddGenericHandler(IGenericResourceHandler handler)
    {
        if (this.genericResourceHandler is not null)
        {
            throw new ArgumentException($"Generic resource handler has already been registered.");
        }

        this.genericResourceHandler = handler;
        return this;
    }

    public ResourceDispatcher Build()
    {
        return new(
            this.genericResourceHandler,
            this.resourceHandlers.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase));
    }
}

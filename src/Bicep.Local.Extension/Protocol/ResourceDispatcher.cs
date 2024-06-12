// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Local.Extension.Protocol;

public class ResourceDispatcher
{
    private readonly IGenericResourceHandler? genericResourceHandler;
    private readonly ImmutableDictionary<string, IResourceHandler> resourceHandlers;

    public ResourceDispatcher(
        IGenericResourceHandler? genericResourceHandler,
        ImmutableDictionary<string, IResourceHandler> resourceHandlers)
    {
        this.genericResourceHandler = genericResourceHandler;
        this.resourceHandlers = resourceHandlers;
    }

    public IGenericResourceHandler GetHandler(string resourceType)
    {
        if (this.resourceHandlers.TryGetValue(resourceType, out var handler))
        {
            return handler;
        }

        if (this.genericResourceHandler is { })
        {
            return this.genericResourceHandler;
        }

        throw new ArgumentException($"Resource type '{resourceType}' is not supported.");
    }
}

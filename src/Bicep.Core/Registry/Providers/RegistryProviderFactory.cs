// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Registry.Sessions;

namespace Bicep.Core.Registry.Providers;

/// <summary>
/// Resolves registry providers for a given OCI reference.
/// </summary>
public class RegistryProviderFactory
{
    private readonly ImmutableArray<IRegistryProvider> providers;

    public RegistryProviderFactory(IEnumerable<IRegistryProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);
        this.providers = providers.OrderByDescending(provider => provider.Priority).ToImmutableArray();
    }

    public IRegistryProvider Resolve(string registry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registry);

        foreach (var provider in providers)
        {
            if (provider.CanHandle(registry))
            {
                return provider;
            }
        }

        var fallback = providers.FirstOrDefault(provider =>
            string.Equals(provider.Name, WellKnownRegistryProviders.Generic, StringComparison.OrdinalIgnoreCase));

        if (fallback is null)
        {
            throw new InvalidOperationException("Failed to locate a registry provider capable of handling the reference.");
        }

        return fallback;
    }

    public IRegistryProvider Resolve(OciArtifactReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return Resolve(reference.Registry);
    }

    public IRegistrySession CreateSession(RegistryRef reference, RegistryProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(context);

        var provider = Resolve(reference.Host);
        return provider.CreateSession(reference, context);
    }
}

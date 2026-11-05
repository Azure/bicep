// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Azure;
using Bicep.Core.Registry.Oci.Oras;
using Bicep.Core.Registry.Sessions;

namespace Bicep.Core.Registry.Oci;

public class OciRegistryTransportFactory : IOciRegistryTransportFactory
{
    private static readonly ImmutableArray<string> AcrHostSuffixes =
    [
        ".azurecr.io",
        ".azurecr.cn",
        ".azurecr.us",
        ".azurecr.de",
        ".azurecr.gov"
    ];

    private readonly OrasOciRegistryTransport transport;
    private readonly AzureContainerRegistryManager azureManager;

    public OciRegistryTransportFactory(OrasOciRegistryTransport transport, AzureContainerRegistryManager azureManager)
    {
        this.transport = transport;
        this.azureManager = azureManager;
    }

    public IOciRegistryTransport GetTransport(OciArtifactReference reference) =>
        GetTransport(reference.Registry, reference.FeatureProvider.OciEnabled);

    public IOciRegistryTransport GetTransport(string registry, bool useOrasSdk)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registry);
        return useOrasSdk ? transport : azureManager;
    }

    public IRegistrySession CreateSession(OciArtifactReference reference, IBicepCloudConfiguration cloud)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(cloud);

        return reference.FeatureProvider.OciEnabled
            ? transport.CreateSession(cloud, reference.Registry, reference.Repository)
            : new AcrRegistrySession(azureManager, cloud);
    }

    /// <summary>
    /// Returns true for hosts that use Azure credentials rather than Docker credentials.
    /// </summary>
    internal static bool IsAzureRegistry(string registry)
    {
        if (!TryNormalizeHost(registry, out var host))
        {
            return false;
        }

        foreach (var suffix in AcrHostSuffixes)
        {
            if (host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryNormalizeHost(string registry, out string host)
    {
        host = registry;

        if (Uri.TryCreate($"https://{registry}", UriKind.Absolute, out var uri))
        {
            host = uri.Host;
            return true;
        }

        if (Uri.TryCreate(registry, UriKind.Absolute, out uri))
        {
            host = uri.Host;
            return true;
        }

        return !string.IsNullOrWhiteSpace(registry);
    }
}

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

    // Hosts that, while not ACRs, are served by Microsoft and supported by the Azure SDK auth path.
    private static readonly ImmutableHashSet<string> MicrosoftManagedHosts =
        ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, "mcr.microsoft.com");

    private readonly AzureContainerRegistryManager azureManager;
    private readonly DockerCredentialProvider dockerCredentialProvider;

    public OciRegistryTransportFactory(
        AzureContainerRegistryManager azureManager,
        DockerCredentialProvider dockerCredentialProvider)
    {
        this.azureManager = azureManager;
        this.dockerCredentialProvider = dockerCredentialProvider;
    }

    public IOciRegistryTransport GetTransport(OciArtifactReference reference) =>
        GetTransport(reference.Registry);

    // Catalog enumeration currently always goes through the Azure SDK transport.
    public IOciRegistryTransport GetTransport(string registry)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registry);
        return azureManager;
    }

    public IRegistrySession CreateSession(OciArtifactReference reference, CloudConfiguration cloud)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(cloud);

        // ACR hosts (and Microsoft-managed hosts like mcr.microsoft.com) always go through the Azure SDK.
        if (IsAzureSdkHost(reference.Registry))
        {
            return new AcrRegistrySession(azureManager, cloud);
        }

        // Any other host: when the experimental OCI feature is enabled, use ORAS with docker-config credentials.
        // Otherwise, fall back to the Azure SDK session (preserves pre-experimental-flag behavior).
        if (reference.ReferencingFile.Features.OciEnabled)
        {
            return new OrasRegistrySession(reference.Registry, reference.Repository, dockerCredentialProvider);
        }

        return new AcrRegistrySession(azureManager, cloud);
    }

    /// <summary>
    /// Returns true for hosts that should be routed through the Azure SDK transport: ACR registries
    /// and other Microsoft-managed hosts (e.g. mcr.microsoft.com) that the Azure SDK can authenticate to.
    /// </summary>
    internal static bool IsAzureSdkHost(string registry)
    {
        if (!TryNormalizeHost(registry, out var host))
        {
            return false;
        }

        if (MicrosoftManagedHosts.Contains(host))
        {
            return true;
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

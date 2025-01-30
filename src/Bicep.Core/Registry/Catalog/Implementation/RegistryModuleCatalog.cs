// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.Catalog.Implementation;

/// <summary>
/// Provides a list of modules from a registry (public or private)
/// </summary>
public class RegistryModuleCatalog : IRegistryModuleCatalog
{
    private readonly IPrivateAcrModuleMetadataProviderFactory providerFactory;
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;
    private readonly IConfigurationManager configurationManager;
    private readonly object lockObject = new();

    private readonly Dictionary<string, IRegistryModuleMetadataProvider> registryProviders = new();

    public RegistryModuleCatalog(
        IPublicModuleMetadataProvider publicModuleMetadataProvider,
        IPrivateAcrModuleMetadataProviderFactory privateProviderFactory,
        IContainerRegistryClientFactory containerRegistryClientFactory,
        IConfigurationManager configurationManager
    )
    {
        providerFactory = privateProviderFactory;
        this.containerRegistryClientFactory = containerRegistryClientFactory;
        this.configurationManager = configurationManager;

        registryProviders["mcr.microsoft.com"] = publicModuleMetadataProvider;
    }

    public IRegistryModuleMetadataProvider GetProviderForRegistry(CloudConfiguration cloud, string registry)
    {
        lock (lockObject)
        {
            if (registryProviders.TryGetValue(registry, out var provider))
            {
                return provider;
            }

            provider = providerFactory.Create(cloud, registry, containerRegistryClientFactory);
            registryProviders[registry] = provider;

            return provider;
        }
    }

    public IRegistryModuleMetadataProvider? TryGetCachedRegistry(string registry)
    {
        if (registryProviders.TryGetValue(registry, out var provider))
        {
            return provider;
        }

        return null;
    }
}

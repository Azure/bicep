// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.Indexing;

public class RegistryIndexer : IRegistryIndexer
{
    private readonly IPrivateAcrModuleMetadataProviderFactory providerFactory;
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;
    private readonly IConfigurationManager configurationManager;

    private readonly Dictionary<string, IRegistryModuleMetadataProvider> registryProviders = new();

    public RegistryIndexer(
        IPublicModuleMetadataProvider publicModuleMetadataProvider,
        IPrivateAcrModuleMetadataProviderFactory privateProviderFactory,
        IContainerRegistryClientFactory containerRegistryClientFactory,
        IConfigurationManager configurationManager
        )
    {
        this.providerFactory = privateProviderFactory;
        this.containerRegistryClientFactory = containerRegistryClientFactory;
        this.configurationManager = configurationManager;

        registryProviders["mcr.microsoft.com"] = publicModuleMetadataProvider;
    }

    public IRegistryModuleMetadataProvider GetRegistry(CloudConfiguration cloud, string registry)
    {
        if (registryProviders.TryGetValue(registry, out var provider))
        {
            return provider;
        }

        provider = providerFactory.Create(cloud, registry, containerRegistryClientFactory);
        registryProviders[registry] = provider; //asdfg threading

        //asdfg remove from cache, esp if error
        return provider;
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

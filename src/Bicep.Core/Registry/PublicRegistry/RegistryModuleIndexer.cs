// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.Core.Registry.PublicRegistry;

public interface IRegistryModuleIndexer
{
    IRegistryModuleMetadataProvider GetRegistry(string registry);

    void StartUpdateCache(bool forceUpdate = false); //asdfg?      asdfg rename StartUpCache()?
}

public class RegistryModuleIndexer : IRegistryModuleIndexer
{
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;
    private readonly IConfigurationManager configurationManager;

    private readonly Dictionary<string, IRegistryModuleMetadataProvider> registryProviders = new();

    public RegistryModuleIndexer(
        IPublicRegistryModuleMetadataProvider publicRegistryModuleMetadataProvider/*asdfgasdfgasdfg asdfg2 create it ourselves? */,
        IContainerRegistryClientFactory containerRegistryClientFactory,
        IConfigurationManager configurationManager
        )
    {
        this.containerRegistryClientFactory = containerRegistryClientFactory;
        this.configurationManager = configurationManager;

        registryProviders["mcr.microsoft.com"] = publicRegistryModuleMetadataProvider;
    }

    public IRegistryModuleMetadataProvider GetRegistry(string registry)
    {
        if (registryProviders.TryGetValue(registry, out var provider))
        {
            return provider;
        }

        //asdfg cache
        provider = new PrivateAcrRegistryModuleMetadataProvider(registry, containerRegistryClientFactory, configurationManager); // asdfg use factory pattern?
        registryProviders[registry] = provider; //asdfg threading

        //asdfg remove from cache, esp if error

        //throw new InvalidOperationException($"No provider found for registry '{registry}'"); //asdfg
        return provider;
    }

    public void StartUpdateCache(bool forceUpdate = false) //asdfg?
    {
        foreach (var provider in registryProviders.Values)
        {
            provider.StartUpdateCache(forceUpdate);
        }
    }
}

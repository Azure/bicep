// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;

public class RegistryIndexer : IRegistryIndexer
{
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;
    private readonly IConfigurationManager configurationManager;

    private readonly Dictionary<string, IRegistryModuleMetadataProvider> registryProviders = new();

    public RegistryIndexer(
        IPublicRegistryModuleMetadataProvider publicRegistryModuleMetadataProvider/*asdfgasdfgasdfg asdfg2 create it ourselves? */,
        IContainerRegistryClientFactory containerRegistryClientFactory,
        IConfigurationManager configurationManager
        )
    {
        this.containerRegistryClientFactory = containerRegistryClientFactory;
        this.configurationManager = configurationManager;

        registryProviders["mcr.microsoft.com"] = publicRegistryModuleMetadataProvider;
    }

    public IRegistryModuleMetadataProvider GetRegistry(string registry, CloudConfiguration cloud)
    {
        if (registryProviders.TryGetValue(registry, out var provider))
        {
            return provider;
        }

        provider = new PrivateAcrRegistryModuleMetadataProvider(cloud, registry, containerRegistryClientFactory);
        registryProviders[registry] = provider; //asdfg threading

        //asdfg remove from cache, esp if error

        //throw new InvalidOperationException($"No provider found for registry '{registry}'"); //asdfg
        return provider;
    }
}

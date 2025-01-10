// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.Indexing;

public class RegistryIndexer : IRegistryIndexer
{
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;
    private readonly IConfigurationManager configurationManager;

    private readonly Dictionary<string, IRegistryModuleMetadataProvider> registryProviders = new();

    public RegistryIndexer(
        IPublicModuleMetadataProvider publicModuleMetadataProvider/*asdfgasdfgasdfg asdfg2 create it ourselves? */,
        IContainerRegistryClientFactory containerRegistryClientFactory,
        IConfigurationManager configurationManager
        )
    {
        this.containerRegistryClientFactory = containerRegistryClientFactory;
        this.configurationManager = configurationManager;

        registryProviders["mcr.microsoft.com"] = publicModuleMetadataProvider;
    }

    public IRegistryModuleMetadataProvider GetRegistry(string registry, CloudConfiguration cloud)
    {
        if (registryProviders.TryGetValue(registry, out var provider))
        {
            return provider;
        }

        provider = new PrivateAcrModuleMetadataProvider(cloud, registry, containerRegistryClientFactory);
        registryProviders[registry] = provider; //asdfg threading

        //asdfg remove from cache, esp if error
        return provider;
    }
}

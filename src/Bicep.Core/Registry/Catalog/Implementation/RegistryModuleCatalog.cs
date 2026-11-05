// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;
using Bicep.Core.Registry.Oci;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.Catalog.Implementation;

/// <summary>
/// Provides a list of modules from a registry (public or private)
/// </summary>
public class RegistryModuleCatalog : IRegistryModuleCatalog
{
    private readonly IPrivateAcrModuleMetadataProviderFactory providerFactory;
    private readonly IOciRegistryTransportFactory transportFactory;
    private readonly IBicepConfigurationManager configurationManager;
    private readonly object lockObject = new();

    private readonly Dictionary<(string Registry, bool UseOrasSdk), IRegistryModuleMetadataProvider> registryProviders = new();

    public RegistryModuleCatalog(
        IPublicModuleMetadataProvider publicModuleMetadataProvider,
        IPrivateAcrModuleMetadataProviderFactory privateProviderFactory,
        IOciRegistryTransportFactory transportFactory,
        IBicepConfigurationManager configurationManager
    )
    {
        providerFactory = privateProviderFactory;
        this.transportFactory = transportFactory;
        this.configurationManager = configurationManager;

        registryProviders[("mcr.microsoft.com", false)] = publicModuleMetadataProvider;
        registryProviders[("mcr.microsoft.com", true)] = publicModuleMetadataProvider;
    }

    public IRegistryModuleMetadataProvider GetProviderForRegistry(IBicepCloudConfiguration cloud, string registry, bool useOrasSdk)
    {
        lock (lockObject)
        {
            if (registryProviders.TryGetValue((registry, useOrasSdk), out var provider))
            {
                return provider;
            }

            provider = providerFactory.Create(cloud, registry, transportFactory, useOrasSdk);
            registryProviders[(registry, useOrasSdk)] = provider;

            return provider;
        }
    }

    public IRegistryModuleMetadataProvider? TryGetCachedRegistry(string registry, bool useOrasSdk)
    {
        lock (lockObject)
        {
            return registryProviders.TryGetValue((registry, useOrasSdk), out var provider) ? provider : null;
        }
    }
}

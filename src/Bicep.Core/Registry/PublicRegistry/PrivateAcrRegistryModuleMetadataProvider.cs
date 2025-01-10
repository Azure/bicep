// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Packaging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;//asdfg rename folder/namespace PublicRegistry?

/// <summary>
/// Provider to get modules metadata from a private ACR registry
/// </summary>
public class PrivateAcrRegistryModuleMetadataProvider : RegistryModuleMetadataProviderBase, IRegistryModuleMetadataProvider
{
    private readonly CloudConfiguration cloud;
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;

    public PrivateAcrRegistryModuleMetadataProvider(
        CloudConfiguration cloud,
        string registry,
        IContainerRegistryClientFactory containerRegistryClientFactory)
        : base(registry)
    {
        this.cloud = cloud;
        this.containerRegistryClientFactory = containerRegistryClientFactory;
    }

    protected override async Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
    {
        var registry = Registry;
        var repository = modulePath;

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var tags = await acrManager.GetRepositoryTags(cloud, registry, repository);
        return [.. tags.Select(t =>
            new RegistryModuleVersionMetadata(
                t,
                null,
                null
            )
        )];
    }

    protected override async Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync()
    {
        var filter = new Regex(""); //asdfg

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var catalog = await acrManager.GetCatalogAsync(cloud, Registry);
        var modules = catalog
            .Where(m => filter.IsMatch(m))
            .Select(m =>
            new CachedModule(
                new RegistryModuleMetadata(Registry, m, "asdfg description", "asdfg documentation uri"),
                null
            )
        ).ToImmutableArray();

        return modules;
    }
}

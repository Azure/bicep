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

namespace Bicep.Core.Registry.Indexing;

/// <summary>
/// Provider to get modules metadata from a private ACR registry
/// </summary>
public class PrivateAcrModuleMetadataProvider : BaseModuleMetadataProvider, IRegistryModuleMetadataProvider
{
    private readonly CloudConfiguration cloud;
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;

    // TODO: Allow configuration (note that the default allows bicep anywhere in the module path) //asdfg test filter
    private string filterExpression = "bicep"; 

    public PrivateAcrModuleMetadataProvider(
        CloudConfiguration cloud,
        string registry,
        IContainerRegistryClientFactory containerRegistryClientFactory
    ) : base(registry)
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

    //asdfg bug: br:sawbicep.azurecr.io/de| => br:sawbicep.azurecr.io/dedemo

    protected override async Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync()
    {
        var filterRegex = new Regex(filterExpression, RegexOptions.IgnoreCase, matchTimeout: TimeSpan.FromMilliseconds(1));

        Trace.WriteLine($"Retrieving catalog for registry {Registry}...");

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var catalog = await acrManager.GetCatalogAsync(cloud, Registry);
        var filteredCatalog = catalog.Where(m => filterRegex.IsMatch(m)).ToImmutableArray();

        Trace.WriteLine($"Found {catalog.Length} repositories, of which {filteredCatalog.Length} matched the filter (\"{filterExpression}\")");

        var modules = filteredCatalog
            .Select(m =>
            new CachedModule(
                new RegistryModuleMetadata(Registry, m, "asdfg description", "asdfg documentation uri"),
                null
            )
        ).ToImmutableArray();

        return modules;
    }
}

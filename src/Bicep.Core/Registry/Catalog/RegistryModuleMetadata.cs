// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry.Catalog.Implementation;

namespace Bicep.Core.Registry.Catalog;

public class RegistryModuleMetadata : IRegistryModuleMetadata
{
    public string Registry { get; init; }
    public string ModuleName { get; init; }

    public record ComputedData(
        RegistryMetadataDetails Details,
        ImmutableArray<RegistryModuleVersionMetadata> Versions
    );

    private readonly MightBeLazyAsync<ComputedData> data;

    public RegistryModuleMetadata(
        string registry,
        string moduleName,
        ComputedData computedData)
    {
        this.Registry = registry;
        this.ModuleName = moduleName;

        this.data = new(computedData);
    }

    public RegistryModuleMetadata(
            string registry,
            string moduleName,
            Func<Task<ComputedData>> getDataAsyncFunc)
    {
        this.Registry = registry;
        this.ModuleName = moduleName;

        this.data = new(getDataAsyncFunc);
    }

    public async Task<RegistryMetadataDetails> TryGetDetailsAsync()
    {
        try
        {
            return (await data.GetValueAsync()).Details;
        }
        catch
        {
            return new RegistryMetadataDetails(null, null);
        }
    }

    public async Task<ImmutableArray<RegistryModuleVersionMetadata>> TryGetVersionsAsync()
    {
        try
        {
            return (await data.GetValueAsync()).Versions;
        }
        catch
        {
            return [];
        }
    }

    public ImmutableArray<RegistryModuleVersionMetadata> GetCachedVersions()
    {
        if (this.data.TryGetValue(out var data))
        {
            return data.Versions;
        }
        else
        {
            return [];
        }
    }
}

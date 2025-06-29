// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using Bicep.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using static Bicep.Core.Registry.Catalog.RegistryModuleMetadata;

namespace Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;

/// <summary>
/// Provider to get modules metadata that we store at the public mcr.microsoft.com/bicep registry.
/// </summary>
public class PublicModuleMetadataProvider : BaseModuleMetadataProvider, IPublicModuleMetadataProvider
{
    private readonly IPublicModuleIndexHttpClient client;

    public PublicModuleMetadataProvider(IPublicModuleIndexHttpClient publicModuleIndexClient)
        : base(LanguageConstants.BicepPublicMcrRegistry)
    {
        client = publicModuleIndexClient;
    }

    protected override async Task<ImmutableArray<IRegistryModuleMetadata>> GetLiveDataCoreAsync()
    {
        var modules = await client.GetModuleIndexAsync();

        return [.. modules.Select(m =>
        {
            var moduleDetails = new RegistryMetadataDetails(m.GetDescription(), m.GetDocumentationUri());
            var versions = ImmutableArray.Create([.. m.Versions.Select(
                        t => new RegistryModuleVersionMetadata(
                            t,
                            IsBicepModule: true,
                            m.PropertiesByTag.TryGetValue(t, out var properties)
                                ? new RegistryMetadataDetails(properties.Description, properties.DocumentationUri)
                                : new RegistryMetadataDetails(null, null))
                    )]);
            return new RegistryModuleMetadata(
                Registry,
                $"{LanguageConstants.BicepPublicMcrPathPrefix}{m.ModulePath}",
                new ComputedData(moduleDetails, versions));
        })];
    }

    protected override Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
    {
        throw new UnreachableException("This method should never get called because versions are pre-filled with a resolved task");
    }
}

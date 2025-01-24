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
using Bicep.Core.Registry.Catalog.HttpClients;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.Catalog;

/// <summary>
/// Provider to get modules metadata that we store at the public mcr.microsoft.com/bicep registry.
/// </summary>
public class PublicModuleMetadataProvider : BaseModuleMetadataProvider, IPublicModuleMetadataProvider
{
    private readonly IPublicModuleIndexHttpClient client;

    public PublicModuleMetadataProvider(IPublicModuleIndexHttpClient publicModuleIndexClient)
        : base(LanguageConstants.BicepPublicMcrRegistry)
    {
        this.client = publicModuleIndexClient; //asdfg lifetime
    }

    protected override async Task<ImmutableArray<IRegistryModuleMetadata>> GetLiveDataCoreAsync()
    {
        var modules = await client.GetModuleIndexAsync();

        return [.. modules.Select(m =>
            new DefaultRegistryModuleMetadata(
                Registry,
                $"{LanguageConstants.BicepPublicMcrPathPrefix}{m.ModulePath}", //asdfg remove prefix and get tests to fail
                new RegistryMetadataDetails(m.GetDescription(), m.GetDocumentationUri()),
                ImmutableArray.Create<RegistryModuleVersionMetadata>([.. m.Versions.Select(
                    t => new RegistryModuleVersionMetadata(
                        t,
                        new RegistryMetadataDetails(
                            m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].Description:null,
                            m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].DocumentationUri:null)
                    )
                )])
            )
        )];
    }

    protected override Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
    {
        throw new NotImplementedException("This method should never get called because versions are pre-filled with a resolved task"); throw new NotImplementedException();
    }

    //asdfg
    //protected override Task<RegistryModuleVersionMetadata?> GetLiveModuleVersionMetadataAsync(string modulePath, string version)
    //{
    //    throw new NotImplementedException("This method should never get called because versions are pre-filled with a resolved task");
    //}

    //asdfg
    //private override async Task<RegistryModuleMetadata> TryGetModuleMetadataFromAsdfgAsync(CachableModuleMetadata metadata)
    //{
    //    return [.. modules.Select(m =>
    //        new CachableModule(
    //            new RegistryModuleMetadata(LanguageConstants.BicepPublicMcrRegistry, $"{LanguageConstants.BicepPublicMcrPathPrefix}{m.ModulePath}", m.GetDescription(), m.GetDocumentationUri()),
    //            [.. m.Versions.Select(
    //                t => new RegistryModuleVersionMetadata(
    //                    t,
    //                    m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].Description:null,
    //                    m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].DocumentationUri:null
    //                )
    //            )]
    //        )
    //    )];
    //}
}

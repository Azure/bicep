// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bicep.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

//asdfg these shouldn't be in core
namespace Bicep.Core.Registry.PublicRegistry;

public interface IPublicRegistryModuleMetadataProvider : IRegistryModuleMetadataProvider { }

/// <summary>
/// Provider to get modules metadata that we store at a public endpoint.
/// </summary>
public class PublicRegistryModuleMetadataProvider : RegistryModuleMetadataProviderBase, IPublicRegistryModuleMetadataProvider
{
    private readonly IPublicRegistryModuleIndexHttpClient client;

    public PublicRegistryModuleMetadataProvider(IPublicRegistryModuleIndexHttpClient publicRegistryModuleIndexClient)
        : base(LanguageConstants.BicepPublicMcrRegistry)
    {
        this.client = publicRegistryModuleIndexClient; //asdfg lifetime
    }

    protected override async Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync()
    {
        var modules = await client.GetModuleIndexAsync();

        return [.. modules.Select(m =>
            new CachedModule(
                new RegistryModuleMetadata(LanguageConstants.BicepPublicMcrRegistry, $"{LanguageConstants.BicepPublicMcrPathPrefix}{m.ModulePath}", m.GetDescription(), m.GetDocumentationUri()),
                [.. m.Versions.Select(
                    t => new RegistryModuleVersionMetadata(
                        t,
                        m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].Description:null,
                        m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].DocumentationUri:null
                    )
                )]
            )
        )];
    }

    protected override Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
    {
        throw new NotImplementedException("This method should never get called because versions are pre-filled with a resolved task");
    }
}

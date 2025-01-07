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

/// <summary>
/// Provider to get modules metadata that we store at a public endpoint.
/// </summary>
public class PublicRegistryModuleMetadataProvider : RegistryModuleMetadataProviderBase, IRegistryModuleMetadataProvider
{
    private readonly PublicRegistryModuleIndexHttpClient client;

    //asdfg
    //private readonly object queryingLiveSyncObject = new();
    //private Task? queryLiveDataTask;
    //private DateTime? lastSuccessfulQuery;
    //private int consecutiveFailures = 0;

    public PublicRegistryModuleMetadataProvider(PublicRegistryModuleIndexHttpClient publicRegistryModuleIndexClient)
        : base(LanguageConstants.BicepPublicMcrRegistry)
    {
        this.client = publicRegistryModuleIndexClient;
    }

    public static RegistryModuleMetadata GetPublicRegistryModuleMetadata(string modulePath, string? description, string? documentationUri)
        => new(LanguageConstants.BicepPublicMcrRegistry, $"{LanguageConstants.BicepPublicMcrPathPrefix}{modulePath}", description, documentationUri);

    protected override async Task<ImmutableArray<CachedModule>> GetLiveDataCoreAsync()
    {
        var modules = await client.GetModuleIndexAsync();

        return [.. modules.Select(m =>
            new CachedModule(
                GetPublicRegistryModuleMetadata(m.ModulePath, m.GetDescription(), m.GetDocumentationUri()),
                Task.FromResult<ImmutableArray<RegistryModuleVersionMetadata>>(
                    [.. m.Versions.Select(
                        t => new RegistryModuleVersionMetadata(
                            t,
                            m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].Description:null,
                            m.PropertiesByTag.ContainsKey(t) ? m.PropertiesByTag[t].DocumentationUri:null
                        )
                    )])
            )
        )];
    }

    protected override Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
    {
        throw new NotImplementedException("This method should never get called because versions are pre-filled with a resolved task");
    }
}

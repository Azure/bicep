// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bicep.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.PublicRegistry;

/// <summary>
/// Typed http client to get modules metadata that we store at a public endpoint (currently https://github.com/Azure/bicep-registry-modules)
/// </summary>
public class PublicRegistryModuleMetadataHttpClient(HttpClient httpClient) : PublicRegistryModuleIndexHttpClient
{
    private const string LiveDataEndpoint = "https://aka.ms/br-module-index-data";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Relying on references to required properties of the generic type elsewhere in the codebase.")]
    public async Task<ImmutableArray<PublicRegistryModuleIndexEntry>> GetModuleIndexAsync()
    {
        Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataHttpClient)}: Retrieving list of public registry modules...");
   //var asdfg = await TryGetCatalog("sawbiceppublic.azurecr.io");

        try
        {
            var metadata = await httpClient.GetFromJsonAsync<PublicRegistryModuleIndexEntry[]>(LiveDataEndpoint, JsonSerializerOptions);

            if (metadata is not null)
            {
                Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataHttpClient)}: Retrieved info on {metadata.Length} public registry modules.");
                return [.. metadata];
            }
            else
            {
                throw new Exception($"Error: List of MCR modules at {LiveDataEndpoint} was empty");
            }
        }
        catch (Exception ex)
        {
            throw new Exception(string.Format($"Error retrieving MCR modules metadata: {0}", ex.Message), ex);
        }
    }
}

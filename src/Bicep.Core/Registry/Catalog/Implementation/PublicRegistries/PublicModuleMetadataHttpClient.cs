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

namespace Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;

/// <summary>
/// Typed http client to get modules metadata that we store at a public endpoint (currently https://github.com/Azure/bicep-registry-modules)
/// </summary>
public class PublicModuleMetadataHttpClient(HttpClient httpClient) : IPublicModuleIndexHttpClient
{
    private const string LiveDataEndpoint = "https://aka.ms/br-module-index-data";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Relying on references to required properties of the generic type elsewhere in the codebase.")]
    public async Task<ImmutableArray<PublicModuleIndexEntry>> GetModuleIndexAsync()
    {
        Trace.WriteLine($"{nameof(PublicModuleMetadataHttpClient)}: Retrieving list of public registry modules...");

        try
        {
            var metadata = await httpClient.GetFromJsonAsync<PublicModuleIndexEntry[]>(LiveDataEndpoint, JsonSerializerOptions);

            if (metadata is not null)
            {
                Trace.WriteLine($"{nameof(PublicModuleMetadataHttpClient)}: Retrieved info on {metadata.Length} public registry modules.");
                return [.. metadata];
            }
            else
            {
                throw new Exception($"Error: List of MCR modules at {LiveDataEndpoint} was empty");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving MCR modules metadata: {ex.Message}", ex);
        }
    }
}

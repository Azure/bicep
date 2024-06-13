// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using Bicep.Core.Extensions;

namespace Bicep.LanguageServer.Providers;

/// <summary>
/// Typed http client to get modules metadata that we store at a public endpoint (currently https://github.com/Azure/bicep-registry-modules)
/// </summary>
public class PublicRegistryModuleMetadataClient(HttpClient httpClient) : IPublicRegistryModuleMetadataClient
{
    private const string LiveDataEndpoint = "https://aka.ms/br-module-index-data";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Relying on references to required properties of the generic type elsewhere in the codebase.")]
    public async Task<ImmutableArray<ModuleMetadata>> GetModuleMetadata()
    {
        Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataClient)}: Retrieving list of public registry modules...");

        try
        {
            var metadata = await httpClient.GetFromJsonAsync<ModuleMetadata[]>(LiveDataEndpoint, JsonSerializerOptions);

            if (metadata is not null)
            {
                Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataProvider)}: Retrieved info on {metadata.Length} public registry modules.");
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

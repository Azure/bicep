// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Bicep.Core.Extensions;

namespace Bicep.Core.Registry.PublicRegistry;

/// <summary>
/// Provider to get modules metadata that we store at a public endpoint.
/// This endpoint caches module names and versions of modules available in this github repository - https://github.com/Azure/bicep-registry-modules
/// </summary>
public class PublicRegistryModuleMetadataClient(HttpClient httpClient) : IPublicRegistryModuleMetadataClient
{
    private const string LiveDataEndpoint = "https://aka.ms/br-module-index-data";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<ImmutableArray<ModuleMetadata>> GetModuleMetadata()
    {
        Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataClient)}: Retrieving list of public registry modules...");

        try
        {
            //asdfg why wasn't pragma necessary in previous project?
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            var metadata = await httpClient.GetFromJsonAsync<ModuleMetadata[]>(LiveDataEndpoint, JsonSerializerOptions);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

            if (metadata is not null)
            {
                Trace.WriteLine($"{nameof(PublicRegistryModuleMetadataProvider)}: Retrieved info on {metadata.Length} public registry modules.");
                return metadata.ToImmutableArray();
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

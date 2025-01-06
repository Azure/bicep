// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//using System.Collections.Immutable;
//using System.Diagnostics;
//using System.Net.Http.Json;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using Bicep.Core.Extensions;
//using Bicep.Core.Registry.Oci;
//using Microsoft.Extensions.DependencyInjection;
//using Semver;
//using Semver.Comparers;

//namespace Bicep.Core.Registry.PublicRegistry;

//public interface IAcrRegistryModuleCatalogClient
//{
//    Task<ImmutableArray<string>?> TryGetCatalog(string loginServer);
//}

//public class AcrRegistryModuleCatalogClient(HttpClient httpClient) : IAcrRegistryModuleCatalogClient
//{
//    public async Task<ImmutableArray<string>?> TryGetCatalog(string loginServer)  //asdfg move
//    {
//        Trace.WriteLine($"asdfg Retrieving list of public registry modules...");

//        try
//        {
//            //ASDFG multiple pages
//            var catalogEndpoint = $"https://{loginServer}/v2/_catalog";
//#pragma warning disable IL2026 // asdfg Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
//            var metadata = await httpClient.GetFromJsonAsync<string[]>(catalogEndpoint);

//            if (metadata is not null)
//            {
//                return metadata.ToImmutableArray();
//            }
//            else
//            {
//                throw new Exception($"asdfg List of MCR modules at {{asdfg LiveDataEndpoint}} was empty");
//            }
//        }
//        catch (Exception e)
//        {
//            Trace.TraceError(string.Format("asdfgError retrieving MCR modules metadata: {0}", e.Message)); //asdfg add server name
//            return null;
//        }
//    }

//}

////public record AcrRegistryModuleCatalog( //asdfg rename
////    [property: JsonPropertyName("moduleName")] string ModulePath, // e.g. "avm/app/dapr-containerapp"
////    ImmutableArray<string> Tags, // e.g. "1.0.0" (not guaranteed to be in that format, although it currently is for public modules)
////    [property: JsonPropertyName("properties")] ImmutableDictionary<string, PublicRegistryModuleProperties/*asdfg rename*/> PropertiesByTag // Module properties per tag
////)
////{
////    private static readonly SemVersion DefaultVersion = new(0);

////    // Sort tags by version numbers in descending order.
////    public ImmutableArray<string> Versions
////    {
////        get
////        {
////            {
////                var parsedVersions = Tags.Select(x =>
////                    (@string: x, version: SemVersion.TryParse(x, SemVersionStyles.AllowV, out var version) ? version : DefaultVersion))
////                    .ToArray();
////                return [.. parsedVersions.OrderByDescending(x => x.version, SemVersion.SortOrderComparer).Select(x => x.@string)];
////            }
////        }
////    }

////    public string? GetDescription(string? version = null) => this.GetProperty(version, x => x.Description);

////    public string? GetDocumentationUri(string? version = null) => this.GetProperty(version, x => x.DocumentationUri);

////    private string? GetProperty(string? version, Func<PublicRegistryModuleProperties, string> propertySelector)
////    {
////        if (version is null)
////        {
////            // Get description for most recent version with a description
////            foreach (var tag in this.Versions)
////            {
////                if (this.PropertiesByTag.TryGetValue(tag, out var propertiesEntry))
////                {
////                    return propertySelector(propertiesEntry);
////                }
////            }

////            return null;
////        }
////        else
////        {
////            return this.PropertiesByTag.TryGetValue(version, out var propertiesEntry) ? propertySelector(propertiesEntry) : null;
////        }
////    }
////}

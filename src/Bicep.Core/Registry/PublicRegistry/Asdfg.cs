// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Diagnostics;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace Bicep.Core.Registry.PublicRegistry
//{
////    class Asdfg(HttpClient httpClient)
////    {
////        private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

////        private async Task<ImmutableArray<string>?> TryGetCatalog(string loginServer)  //asdfg move
////        {
////            Trace.WriteLine($"asdfg Retrieving list of public registry modules...");

////            try
////            {
////                //ASDFG multiple pages
////                var catalogEndpoint = $"https://{loginServer}/v2/_catalog";
////#pragma warning disable IL2026 // asdfg Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
////                var metadata = await httpClient.GetFromJsonAsync<string[]>(catalogEndpoint, JsonSerializerOptions);

////                if (metadata is not null)
////                {
////                    return metadata.ToImmutableArray();
////                }
////                else
////                {
////                    throw new Exception($"asdfg List of MCR modules at {LiveDataEndpoint} was empty");
////                }
////            }
////            catch (Exception e)
////            {
////                Trace.TraceError(string.Format("asdfgError retrieving MCR modules metadata: {0}", e.Message));
////                return null;
////            }
////        }

////    }
//}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer.Providers
{
    public record ModuleMetadata(string moduleName, List<string> tags);

    /// <summary>
    /// Provider to get modules metadata from this endpoint - https://live-data.bicep.azure.com/modulesMetadata
    /// The above endpoint helps fetch module names and versions of modules available in this github repository - https://github.com/Azure/bicep-registry-modules
    /// </summary>
    public class PublicRegistryModuleMetadataProvider : IPublicRegistryModuleMetadataProvider
    {
        private const string LiveDataEndpoint = "https://live-data.bicep.azure.com/modulesMetadata";
        private List<ModuleMetadata> moduleMetadataCache = new List<ModuleMetadata>();

        public async Task Initialize()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var moduleMetadata = await httpClient.GetStringAsync(LiveDataEndpoint);
                var metadata = JsonConvert.DeserializeObject<List<ModuleMetadata>>(moduleMetadata);

                if (metadata is not null)
                {
                    moduleMetadataCache = metadata;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Encountered following exception while intializing MCR modules metadata: {0}", e.Message));
            }

            Trace.WriteLine("Unable to initialize MCR modules metadata.");
        }

        public async Task<IEnumerable<string>> GetModuleNames()
        {
            List<string> moduleNames = new List<string>();

            if (!moduleMetadataCache.Any())
            {
                await Initialize();
            }

            foreach (var moduleMetadataCacheEntry in moduleMetadataCache)
            {
                moduleNames.Add(moduleMetadataCacheEntry.moduleName);
            }

            return moduleNames;
        }

        public async Task<IEnumerable<string>> GetVersions(string moduleName)
        {
            List<string> versions = new List<string>();

            if (!moduleMetadataCache.Any())
            {
                await Initialize();
            }

            ModuleMetadata? metadata = moduleMetadataCache.FirstOrDefault(x => x.moduleName.Equals(moduleName, StringComparison.Ordinal));

            if (metadata is not null)
            {
                return metadata.tags.OrderDescending();
            }

            return Enumerable.Empty<string>();
        }
    }
}

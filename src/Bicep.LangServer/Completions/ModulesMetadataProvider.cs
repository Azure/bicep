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
    public class ModulesMetadataProvider : IModulesMetadataProvider
    {
        private const string LiveDataEndpoint = "https://live-data.bicep.azure.com/modulesMetadata";
        private List<ModuleMetadata> moduleMetadataCache = new List<ModuleMetadata>();

        public async Task<string> Initialize()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var moduleMetadata = await httpClient.GetStringAsync(LiveDataEndpoint);
                var metadata = JsonConvert.DeserializeObject<List<ModuleMetadata>>(moduleMetadata);

                if (metadata is not null)
                {
                    moduleMetadataCache = metadata;
                    return "Initialized MCR modules metadata";
                }
            }
            catch (Exception e)
            {
                return string.Format("Encountered following exception while intializing MCR modules metadata: {0}", e.Message);
            }

            return "Unable to initialize MCR modules metadata.";
        }

        public IEnumerable<string> GetModuleNames()
        {
            List<string> moduleNames = new List<string>();

            foreach (var moduleMetadataCacheEntry in moduleMetadataCache)
            {
                moduleNames.Add(moduleMetadataCacheEntry.moduleName);
            }

            return moduleNames;
        }

        public IEnumerable<string> GetVersions(string moduleName)
        {
            List<string> versions = new List<string>();
            ModuleMetadata? metadata = moduleMetadataCache.FirstOrDefault(x => x.moduleName.Equals(moduleName, StringComparison.Ordinal));

            if (metadata is not null)
            {
                return metadata.tags;
            }

            return Enumerable.Empty<string>();
        }
    }
}

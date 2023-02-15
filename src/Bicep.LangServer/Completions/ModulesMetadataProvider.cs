// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bicep.LanguageServer.Providers
{
    public record ModuleMetadata(string moduleName, List<string> tags);

    public class ModulesMetadataProvider : IModulesMetadataProvider
    {
        private List<ModuleMetadata> moduleMetadataCache = new List<ModuleMetadata>();

        public async Task Initialize()
        {
            HttpClient client = new HttpClient();

            var moduleMetadata = await client.GetStringAsync("https://live-data.bicep.azure.com/modulesMetadata");

            var metadata = JsonConvert.DeserializeObject<List<ModuleMetadata>>(moduleMetadata);

            if (metadata is not null)
            {
                moduleMetadataCache = metadata;
            }
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

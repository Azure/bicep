// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bicep.Core.Registry.Oci;
using Bicep.LanguageServer.Completions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public record ModuleMetadata(string moduleName, List<string> tags, string readmeLink);

    public class ModulesMetadataProvider : IModulesMetadataProvider
    {
        private List<ModuleMetadata> moduleMetadataCache = new List<ModuleMetadata>();

        public async Task Initialize()
        {
            HttpClient client = new HttpClient();

            var moduleMetadata = await client.GetStringAsync("https://modulesmetadata.bicep-df.azure.com/moduleNamesWithTags");

            var metadata = JsonConvert.DeserializeObject<List<ModuleMetadata>>(moduleMetadata);

            if (metadata is not null)
            {
                moduleMetadataCache = metadata;
            }
        }

        public List<CompletionItem> GetModuleNames()
        {
            List<CompletionItem> completionItems = new List<CompletionItem>();

            foreach (var moduleName in moduleMetadataCache)
            {
                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, moduleName.moduleName).Build();
                completionItems.Add(completionItem);
            }

            return completionItems;
        }

        public List<CompletionItem> GetTags(string moduleName)
        {
            List<CompletionItem> completionItems = new List<CompletionItem>();
            ModuleMetadata? metadata = moduleMetadataCache.FirstOrDefault(x => x.moduleName.Equals(moduleName, StringComparison.Ordinal));

            if (metadata is not null)
            {
                foreach (var tag in metadata.tags)
                {
                    var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, tag).Build();
                    completionItems.Add(completionItem);
                }
            }

            return completionItems;
        }
    }
}

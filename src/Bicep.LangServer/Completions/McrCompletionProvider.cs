// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bicep.LanguageServer.Completions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public class McrCompletionProvider
    {
        private Dictionary<string, List<string>> moduleNamesWithTags = new();

        private McrCompletionProvider()
        {
        }

        public static async Task<McrCompletionProvider> Create()
        {
            var mcrCompletionProvider = new McrCompletionProvider();
            await mcrCompletionProvider.InitializeCacheAsync();
            return mcrCompletionProvider;
        }

        private async Task InitializeCacheAsync()
        {
            HttpClient client = new HttpClient();

            var moduleMetadata = await client.GetStringAsync("https://teststgaccbhsubra.blob.core.windows.net/bicep-cdn-bhsubra-container/bicepModuleRegistryReferenceCompletionMetadata/package.json");

            var metadata = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(moduleMetadata);

            if (metadata is not null)
            {
                moduleNamesWithTags = metadata;
            }
        }

        public List<CompletionItem> GetModuleNames()
        {
            List<CompletionItem> completionItems = new List<CompletionItem>();

            foreach (var moduleName in moduleNamesWithTags.Keys.ToList())
            {
                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, moduleName).Build();
                completionItems.Add(completionItem);
            }

            return completionItems;
        }

        public List<CompletionItem> GetTags(string moduleName)
        {
            List<CompletionItem> completionItems = new List<CompletionItem>();

            foreach (var tag in moduleNamesWithTags[moduleName])
            {
                var completionItem = CompletionItemBuilder.Create(CompletionItemKind.Reference, tag).Build();
                completionItems.Add(completionItem);
            }

            return completionItems;
        }
    }
}

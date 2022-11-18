// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using Bicep.LanguageServer.Completions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public class McrCompletionProvider : IMcrCompletionProvider
    {
        private Dictionary<string, List<string>> moduleNamesWithTags = new();

        public McrCompletionProvider()
        {
            InitializeCache();
        }

        private void InitializeCache()
        {
            string moduleNamesWithTagsFromCache = File.ReadAllText(@"C:\Users\bhsubra\Downloads\moduleNamesWithTags.json");

            var metadata = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(moduleNamesWithTagsFromCache);

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

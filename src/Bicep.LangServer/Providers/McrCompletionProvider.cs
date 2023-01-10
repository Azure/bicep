// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.LanguageServer.Completions;
using Microsoft.Extensions.FileSystemGlobbing;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Bicep.LanguageServer.Providers
{
    public record ModuleMetadata(string moduleName, List<string> tags, string readmeLink);

    public class McrCompletionProvider : IMcrCompletionProvider
    {
        private readonly Regex moduleRegistryAliasWithPath = new Regex(@"br/public:(?<name>(.*?)):(?<version>(.*?))", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private readonly Regex moduleRegistryWithoutAliasWithPath = new Regex(@"br:mcr.microsoft.com/bicep/(?<name>(.*?)):(?<version>(.*?))", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

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

        public List<CompletionItem> GetVersions(string moduleName)
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

        public string? GetReadmeLink(string modulePath)
        {
            if (string.IsNullOrWhiteSpace(modulePath))
            {
                return null;
            }

            string? moduleName = null;
            if (moduleRegistryAliasWithPath.IsMatch(modulePath))
            {
                MatchCollection matches = moduleRegistryAliasWithPath.Matches(modulePath);
                moduleName = matches[0].Groups["name"].Value;
            }
            else if (moduleRegistryWithoutAliasWithPath.IsMatch(modulePath))
            {
                MatchCollection matches = moduleRegistryWithoutAliasWithPath.Matches(modulePath);
                moduleName = matches[0].Groups["name"].Value;
            }

            if (moduleName is not null)
            {
                ModuleMetadata? metadata = moduleMetadataCache.FirstOrDefault(x => x.moduleName.Equals(moduleName, StringComparison.Ordinal));

                if (metadata is not null)
                {
                    return metadata.readmeLink;
                }
            }

            return null;
        }
    }
}

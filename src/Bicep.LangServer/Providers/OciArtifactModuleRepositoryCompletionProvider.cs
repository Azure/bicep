// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.ResourceManager.ResourceGraph;
using Bicep.LanguageServer.Completions;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Providers
{
    public class OciArtifactModuleRepositoryCompletionProvider : IOciArtifactModuleRepositoryCompletionProvider
    {
        private readonly IServiceClientCredentialsProvider serviceClientCredentialsProvider;

        public OciArtifactModuleRepositoryCompletionProvider(IServiceClientCredentialsProvider serviceClientCredentialsProvider)
        {
            this.serviceClientCredentialsProvider = serviceClientCredentialsProvider;
        }

        public async Task<CompletionList> GetOciArtifactModuleRepositoryPathCompletionsAsync(Uri templateUri)
        {
            ClientCredentials clientCredentials = await serviceClientCredentialsProvider.GetServiceClientCredentials(templateUri);;

            ResourceGraphClient resourceGraphClient = new ResourceGraphClient(clientCredentials);
            QueryRequest queryRequest = new QueryRequest(@"Resources
| where type == ""microsoft.containerregistry/registries""
| project properties[""loginServer""]
");
            QueryResponse queryResponse = resourceGraphClient.Resources(queryRequest);
            JArray jArray = JArray.FromObject(queryResponse.Data);
            List<CompletionItem> repositories = new List<CompletionItem>();

            foreach (JObject item in jArray)
            {
                if (item is not null &&
                    item.GetValue("properties_loginServer") is JToken jToken &&
                    jToken is not null &&
                    jToken.Value<string>() is string loginServer)
                {
                    repositories.Add(CompletionItemBuilder.Create(CompletionItemKind.File, loginServer).WithFilterText(loginServer).Build());
                }
            }

            return new CompletionList(repositories, isIncomplete: false);
        }
    }
}

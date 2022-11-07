// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Containers.ContainerRegistry;
using Azure;
using Azure.Core;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.LanguageServer.Completions;
using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using MicrosoftRestTokenCredentials = Microsoft.Rest.TokenCredentials;

namespace Bicep.LanguageServer.Providers
{
    public class OciArtifactModuleRepositoryCompletionProvider : IOciArtifactModuleRepositoryCompletionProvider
    {
        private readonly ITokenCredentialFactory tokenCredentialFactory;
        private readonly IConfigurationManager configurationManager;
        private readonly IAccessTokenProvider accessTokenProvider;

        public OciArtifactModuleRepositoryCompletionProvider(IAccessTokenProvider accessTokenProvider, IConfigurationManager configurationManager, ITokenCredentialFactory tokenCredentialFactory)
        {
            this.accessTokenProvider = accessTokenProvider;
            this.configurationManager = configurationManager;
            this.tokenCredentialFactory = tokenCredentialFactory;
        }

        public async Task<CompletionList> GetOciArtifactModuleRepositoryPathCompletionsAsync(Uri templateUri)
        {
            var completions = await GetRepositories(templateUri);
            var accessToken = await accessTokenProvider.GetAccessTokenAsync(templateUri);

            ResourceGraphClient resourceGraphClient = new ResourceGraphClient(new MicrosoftRestTokenCredentials(accessToken.Token));
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

        private async Task<CompletionList> GetRepositories(Uri templateUri)
        {
            var configuration = configurationManager.GetConfiguration(templateUri);
            // Create a new ContainerRegistryClient
            var containerRegistryClientOptions = new ContainerRegistryClientOptions()
            {
                Audience = new ContainerRegistryAudience(configuration.Cloud.ResourceManagerAudience)
            };

            TokenCredential credential = tokenCredentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);
            ContainerRegistryClient client = new ContainerRegistryClient(new Uri("https://bhsubrarg.azurecr.io/"), credential, containerRegistryClientOptions);
            List<CompletionItem> completions = new List<CompletionItem>();
            AsyncPageable<string> repositories = client.GetRepositoryNamesAsync();
            await foreach (var repository in repositories)
            {
                completions.Add(CompletionItemBuilder.Create(CompletionItemKind.File, repository).WithFilterText(repository).Build());
            }

            return new CompletionList(completions, isIncomplete: false);
        }
    }
}

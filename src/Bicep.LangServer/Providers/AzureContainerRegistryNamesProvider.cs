// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Providers
{
    public class AzureContainerRegistryNamesProvider: IAzureContainerRegistryNamesProvider
    {
        private readonly IServiceClientCredentialsProvider serviceClientCredentialsProvider;

        public AzureContainerRegistryNamesProvider(IServiceClientCredentialsProvider serviceClientCredentialsProvider)
        {
            this.serviceClientCredentialsProvider = serviceClientCredentialsProvider;
        }

        public async Task<List<string>> GetRegistryNames(Uri templateUri)
        {
            ClientCredentials clientCredentials = await serviceClientCredentialsProvider.GetServiceClientCredentials(templateUri); ;

            ResourceGraphClient resourceGraphClient = new ResourceGraphClient(clientCredentials);
            QueryRequest queryRequest = new QueryRequest(@"Resources
| where type == ""microsoft.containerregistry/registries""
| project properties[""loginServer""]
");
            QueryResponse queryResponse = resourceGraphClient.Resources(queryRequest);
            JArray jArray = JArray.FromObject(queryResponse.Data);
            List<string> registryNames = new();

            foreach (JObject item in jArray)
            {
                if (item is not null &&
                    item.GetValue("properties_loginServer") is JToken jToken &&
                    jToken is not null &&
                    jToken.Value<string>() is string loginServer)
                {
                    registryNames.Add(loginServer);
                }
            }

            return registryNames;
        }
    }
}

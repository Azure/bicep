// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Azure.Management.ResourceGraph;
using Microsoft.Azure.Management.ResourceGraph.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// This provider helps fetch all the Azure Container Registries(ACR) names that the user has access to.
    /// 
    /// </summary>
    public class AzureContainerRegistryNamesProvider: IAzureContainerRegistryNamesProvider
    {
        private readonly IServiceClientCredentialsProvider serviceClientCredentialsProvider;

        public AzureContainerRegistryNamesProvider(IServiceClientCredentialsProvider serviceClientCredentialsProvider)
        {
            this.serviceClientCredentialsProvider = serviceClientCredentialsProvider;
        }

        public async Task<IEnumerable<string>> GetRegistryNames(Uri templateUri)
        {
            ClientCredentials clientCredentials = await serviceClientCredentialsProvider.GetServiceClientCredentials(templateUri);

            ResourceGraphClient resourceGraphClient = new ResourceGraphClient(clientCredentials);
            QueryRequest queryRequest = new QueryRequest(@"Resources
| where type == ""microsoft.containerregistry/registries""
| project properties[""loginServer""]
");
            QueryResponse queryResponse = resourceGraphClient.Resources(queryRequest);

            if (queryResponse is null || queryResponse.Data is null)
            {
                return Enumerable.Empty<string>();
            }

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

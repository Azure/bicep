// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.CompilerServices;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.ResourceGraph;
using Azure.ResourceManager.ResourceGraph.Models;
using Azure.ResourceManager.Resources;
using Bicep.Core.Configuration;
using Bicep.Core.AzureApi;
using Bicep.Core.Tracing;
using Newtonsoft.Json.Linq;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// This provider fetches all the Azure Container Registries (ACR) names that the user has access to via Azure
    /// </summary>
    public class AzureContainerRegistriesProvider : IAzureContainerRegistriesProvider
    {
        private readonly ITokenCredentialFactory tokenCredentialFactory;

        private const string queryToGetRegistryNames = @"Resources
| where type == ""microsoft.containerregistry/registries""
| project properties[""loginServer""]";

        public AzureContainerRegistriesProvider(ITokenCredentialFactory tokenCredentialFactory)
        {
            this.tokenCredentialFactory = tokenCredentialFactory;
        }

        // Used for completions after typing "'br:"
        public async IAsyncEnumerable<string> GetContainerRegistriesAccessibleFromAzure(CloudConfiguration cloud, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var armClient = GetArmClient(cloud);
            TenantCollection tenants = armClient.GetTenants();

            await foreach (TenantResource tenant in tenants)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var queryContent = new ResourceQueryContent(queryToGetRegistryNames);
                Response<ResourceQueryResult> queryResponse = await tenant.GetResourcesAsync(queryContent);

                if (queryResponse.Value is ResourceQueryResult resourceQueryResult &&
                    resourceQueryResult.TotalRecords > 0 &&
                    resourceQueryResult.Data is BinaryData data)
                {
                    JArray jArray = JArray.Parse(data.ToString());

                    foreach (JObject item in jArray)
                    {
                        if (item is not null &&
                            item.GetValue("properties_loginServer") is JToken jToken &&
                            jToken is not null &&
                            jToken.Value<string>() is string loginServer)
                        {
                            yield return loginServer;
                        }
                    }
                }
            }
        }

        private ArmClient GetArmClient(CloudConfiguration cloud)
        {
            var credential = tokenCredentialFactory.CreateChain(cloud.CredentialPrecedence, cloud.CredentialOptions, cloud.ActiveDirectoryAuthorityUri);

            var options = new ArmClientOptions();
            options.Diagnostics.ApplySharedResourceManagerSettings();
            options.Environment = new ArmEnvironment(cloud.ResourceManagerEndpointUri, cloud.AuthenticationScope);

            return new ArmClient(credential);
        }
    }
}

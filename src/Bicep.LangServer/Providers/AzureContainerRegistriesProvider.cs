// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.ResourceManager;
using Azure.ResourceManager.ResourceGraph;
using Azure.ResourceManager.ResourceGraph.Models;
using Azure.ResourceManager.Resources;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Tracing;
using Newtonsoft.Json.Linq;

namespace Bicep.LanguageServer.Providers
{
    /// <summary>
    /// This provider fetches all the Azure Container Registries (ACR) names that the user has access to via Azure. It does
    /// so by downloading a list 
    /// </summary>
    public class AzureContainerRegistriesProvider : IAzureContainerRegistriesProvider
    {
        private readonly IConfigurationManager configurationManager;
        private readonly ITokenCredentialFactory tokenCredentialFactory;

        private const string queryToGetRegistryNames = @"Resources
| where type == ""microsoft.containerregistry/registries""
| project properties[""loginServer""]";

        public AzureContainerRegistriesProvider(IConfigurationManager configurationManager, ITokenCredentialFactory tokenCredentialFactory)
        {
            this.configurationManager = configurationManager;
            this.tokenCredentialFactory = tokenCredentialFactory;
        }

        // Used for completions after typing "'br:"
        public async IAsyncEnumerable<string> GetRegistryUris(Uri templateUri, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var armClient = GetArmClient(templateUri);
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

        private ArmClient GetArmClient(Uri templateUri)
        {
            var rootConfiguration = configurationManager.GetConfiguration(templateUri);
            var credential = tokenCredentialFactory.CreateChain(rootConfiguration.Cloud.CredentialPrecedence, rootConfiguration.Cloud.ActiveDirectoryAuthorityUri);

            var options = new ArmClientOptions();
            options.Diagnostics.ApplySharedResourceManagerSettings();
            options.Environment = new ArmEnvironment(rootConfiguration.Cloud.ResourceManagerEndpointUri, rootConfiguration.Cloud.AuthenticationScope);

            return new ArmClient(credential);
        }
    }
}

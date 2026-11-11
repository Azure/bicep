// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Tracing;

namespace Bicep.Core.RegistryClients.TemplateSpecs
{
    public class TemplateSpecClientFactory(IArmClientProvider armClientProvider) : ITemplateSpecClientFactory
    {
        public ITemplateSpecClient CreateClient(IBicepCloudConfiguration cloud, string subscriptionId)
        {
            var armClient = armClientProvider.CreateArmClient(cloud, subscriptionId);

            return new TemplateSpecClient(armClient);
        }
    }
}

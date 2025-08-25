// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.ResourceManager;
using Bicep.Core.AzureApi;
using Bicep.Core.Configuration;
using Bicep.Core.Tracing;

namespace Bicep.Core.Registry
{
    public class TemplateSpecRepositoryFactory(IArmClientProvider armClientProvider) : ITemplateSpecRepositoryFactory
    {
        public ITemplateSpecRepository CreateRepository(RootConfiguration configuration, string subscriptionId)
        {
            var armClient = armClientProvider.CreateArmClient(configuration, subscriptionId);

            return new TemplateSpecRepository(armClient);
        }
    }
}

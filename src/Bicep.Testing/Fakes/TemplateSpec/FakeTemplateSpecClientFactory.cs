// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.RegistryClients.TemplateSpecs;

namespace Bicep.Testing.Fakes.TemplateSpec
{
    public class FakeTemplateSpecClientFactory : ITemplateSpecClientFactory
    {
        private readonly FakeTemplateSpecClient client;

        public FakeTemplateSpecClientFactory(FakeTemplateSpecClient client)
        {
            this.client = client;
        }

        public ITemplateSpecClient CreateClient(IBicepCloudConfiguration cloud, string subscriptionId) => this.client;
    }
}

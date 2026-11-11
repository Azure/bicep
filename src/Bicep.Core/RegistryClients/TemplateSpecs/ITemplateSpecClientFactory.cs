// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;

namespace Bicep.Core.RegistryClients.TemplateSpecs
{
    public interface ITemplateSpecClientFactory
    {
        ITemplateSpecClient CreateClient(IBicepCloudConfiguration cloud, string subscriptionId);
    }
}

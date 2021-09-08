// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;

namespace Bicep.Core.Registry
{
    public class TemplateSpecRepositoryFactory : ITemplateSpecRepositoryFactory
    {
        public ITemplateSpecRepository CreateRepository(Uri? endpointUri, string subscriptionId, TokenCredential? tokenCredential = null)
        {
            tokenCredential ??= new DefaultAzureCredential();

            var options = new ArmClientOptions();
            options.Diagnostics.ApplicationId = $"{LanguageConstants.LanguageId}/{ThisAssembly.AssemblyFileVersion}";
            options.ApiVersions.SetApiVersion("templateSpecs", "2021-05-01");

            var armClient = new ArmClient(subscriptionId, endpointUri, tokenCredential, options);

            return new TemplateSpecRepository(armClient);
        }
    }
}

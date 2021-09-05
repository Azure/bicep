// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Resources;

namespace Bicep.Core.Registry
{
    public class TemplateSpecRepositoryFactory : ITemplateSpecRepositoryFactory
    {
        public ITemplateSpecRepository CreateRepository(Uri? endpoint, string subscriptionId, TokenCredential? tokenCredential = null)
        {
            tokenCredential ??= new DefaultAzureCredential();

            var options = new ResourcesManagementClientOptions();
            options.Diagnostics.ApplicationId = $"{LanguageConstants.LanguageId}/{ThisAssembly.AssemblyFileVersion}";

            return new TemplateSpecRepository(endpoint, subscriptionId, tokenCredential, options);
        }
    }
}

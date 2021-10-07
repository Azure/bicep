// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Azure.ResourceManager;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Tracing;

namespace Bicep.Core.Registry
{
    public class TemplateSpecRepositoryFactory : ITemplateSpecRepositoryFactory
    {
        private readonly ITokenCredentialFactory credentialFactory;

        public TemplateSpecRepositoryFactory(ITokenCredentialFactory credentialFactory)
        {
            this.credentialFactory = credentialFactory;
        }

        public ITemplateSpecRepository CreateRepository(RootConfiguration configuration, Uri? endpointUri, string subscriptionId)
        {
            var options = new ArmClientOptions();
            options.Diagnostics.ApplySharedResourceManagerSettings();
            options.ApiVersions.SetApiVersion("templateSpecs", "2021-05-01");

            var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence);
            var armClient = new ArmClient(subscriptionId, endpointUri, credential, options);

            return new TemplateSpecRepository(armClient);
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Tracing;
using System;

namespace Bicep.Core.Registry
{
    public class ContainerRegistryClientFactory : IContainerRegistryClientFactory
    {
        private readonly ITokenCredentialFactory credentialFactory;

        public ContainerRegistryClientFactory(ITokenCredentialFactory credentialFactory)
        {
            this.credentialFactory = credentialFactory;
        }

        public IOciRegistryContentClient CreateAuthenticatedBlobClient(RootConfiguration configuration, Uri registryUri, string repository)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(configuration.Cloud.ResourceManagerAudience);

            var credential = this.credentialFactory.CreateChain(configuration.Cloud.CredentialPrecedence, configuration.Cloud.ActiveDirectoryAuthorityUri);

            return new OciRegistryContentClient(registryUri, repository, credential, options);
        }

        public IOciRegistryContentClient CreateAnonymousBlobClient(RootConfiguration configuration, Uri registryUri, string repository)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(configuration.Cloud.ResourceManagerAudience);

            return new OciRegistryContentClient(registryUri, repository, options);
        }
    }
}

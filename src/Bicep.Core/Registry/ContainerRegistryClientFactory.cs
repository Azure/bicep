// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using Bicep.Core.Tracing;

namespace Bicep.Core.Registry
{
    public class ContainerRegistryClientFactory : IContainerRegistryClientFactory
    {
        private readonly ITokenCredentialFactory credentialFactory;

        public ContainerRegistryClientFactory(ITokenCredentialFactory credentialFactory)
        {
            this.credentialFactory = credentialFactory;
        }

        public ContainerRegistryContentClient CreateAuthenticatedBlobClient(CloudConfiguration cloud, Uri registryUri, string repository)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(cloud.ResourceManagerAudience);

            var credential = this.credentialFactory.CreateChain(cloud.CredentialPrecedence, cloud.CredentialOptions, cloud.ActiveDirectoryAuthorityUri);

            return new(registryUri, repository, credential, options);
        }

        public ContainerRegistryContentClient CreateAnonymousBlobClient(CloudConfiguration cloud, Uri registryUri, string repository)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(cloud.ResourceManagerAudience);

            return new(registryUri, repository, options);
        }

        public ContainerRegistryClient CreateAuthenticatedContainerClient(CloudConfiguration cloud, Uri registryUri)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(cloud.ResourceManagerAudience);

            var credential = this.credentialFactory.CreateChain(cloud.CredentialPrecedence, cloud.CredentialOptions, cloud.ActiveDirectoryAuthorityUri);

            return new(registryUri, credential, options);
        }

        public ContainerRegistryClient CreateAnonymousContainerClient(CloudConfiguration cloud, Uri registryUri)
        {
            var options = new ContainerRegistryClientOptions();
            options.Diagnostics.ApplySharedContainerRegistrySettings();
            options.Audience = new ContainerRegistryAudience(cloud.ResourceManagerAudience);

            return new(registryUri, options);
        }
    }
}

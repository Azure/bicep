// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;

namespace Bicep.Testing.Fakes.ContainerRegistry
{
    public class FakeContainerRegistryClientFactory : IContainerRegistryClientFactory
    {
        private readonly ConcurrentDictionary<Uri, FakeContainerRegistry> registriesByUri = new();

        public ContainerRegistryContentClient CreateAnonymousBlobClient(IBicepCloudConfiguration cloud, Uri registryUri, string repositoryName)
        {
            var registry = this.registriesByUri.GetOrAdd(registryUri, uri => new FakeContainerRegistry(uri.Host));
            var repository = registry.GetRepository(repositoryName);

            return new FakeContainerRegistryContentClient(repository);
        }

        public ContainerRegistryClient CreateAnonymousContainerClient(IBicepCloudConfiguration cloud, Uri registryUri)
        {
            var registry = this.registriesByUri.GetOrAdd(registryUri, uri => new FakeContainerRegistry(uri.Host));

            return new FakeContainerRegistryClient(registry);
        }

        /*
         * For simplicity, we don't implement authenticated clients in the fake factory.
         * The anonymous clients should be sufficient for testing purposes.
         */

        public ContainerRegistryContentClient CreateAuthenticatedBlobClient(IBicepCloudConfiguration cloud, Uri registryUri, string repository) => CreateAnonymousBlobClient(cloud, registryUri, repository);

        public ContainerRegistryClient CreateAuthenticatedContainerClient(IBicepCloudConfiguration cloud, Uri registryUri) => CreateAnonymousContainerClient(cloud, registryUri);
    }
}

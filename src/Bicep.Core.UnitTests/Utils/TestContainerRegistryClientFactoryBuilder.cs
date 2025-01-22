// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using FluentAssertions;
using Moq;

namespace Bicep.Core.UnitTests.Utils
{
    public class TestContainerRegistryClientFactoryBuilder
    {
        private readonly ImmutableDictionary<(Uri registryUri, string repository), MockRegistryBlobClient>.Builder blobClientsBuilder = ImmutableDictionary.CreateBuilder<(Uri registryUri, string repository), MockRegistryBlobClient>();
        private FakeContainerRegistryClient containerClient = new();

        public TestContainerRegistryClientFactoryBuilder WithRepository(string registryHost, string repository, string[] tags)
        {
            blobClientsBuilder.TryAdd((new Uri($"https://{registryHost}"), repository), new MockRegistryBlobClient());

            if (!containerClient.FakeRepositories.ContainsKey(repository))
            {
                containerClient.FakeRepositories.Add(repository, new(registryHost, repository, [.. tags]));
            }

            return this;
        }

        public TestContainerRegistryClientFactoryBuilder WithRepository(string registryHost, string repository, string tag)
        {
            blobClientsBuilder.TryAdd((new Uri($"https://{registryHost}"), repository), new MockRegistryBlobClient());

            if (containerClient.FakeRepositories.ContainsKey(repository))
            {
                containerClient.FakeRepositories[repository].Tags.Add(tag);
            }
            else
            {
                containerClient.FakeRepositories.Add(repository, new(registryHost, repository, [tag]));
            }

            return this;
        }

        public TestContainerRegistryClientFactoryBuilder WithRepository(string registryHost, string repository, string[] tags, MockRegistryBlobClient client)
        {
            blobClientsBuilder.TryAdd((new Uri($"https://{registryHost}"), repository), client);

            if (!containerClient.FakeRepositories.ContainsKey(repository))
            {
                containerClient.FakeRepositories.Add(repository, new(registryHost, repository, [.. tags]));
            }

            return this;
        }

        public TestContainerRegistryClientFactoryBuilder WithFakeContainerRegistryClient(FakeContainerRegistryClient containerRegistryClient)
        {
            this.containerClient.FakeRepositories.Should().BeEmpty("Must set up ContainerRegistryClient before adding repos");
            this.containerClient = containerRegistryClient;
            return this;
        }

        public (IContainerRegistryClientFactory clientFactory, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks, FakeContainerRegistryClient containerRegistryClient) Build()
        {
            var blobClientsByRepository = blobClientsBuilder.ToImmutable();

            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();

            clientFactory
                .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<CloudConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns<CloudConfiguration, Uri, string>((_, registryUri, repository) => GetBlobClient(registryUri, repository));
            clientFactory
                .Setup(m => m.CreateAnonymousBlobClient(It.IsAny<CloudConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns<CloudConfiguration, Uri, string>((_, registryUri, repository) => GetBlobClient(registryUri, repository));

            clientFactory
                .Setup(m => m.CreateAuthenticatedContainerClient(It.IsAny<CloudConfiguration>(), It.IsAny<Uri>()))
                .Returns<CloudConfiguration, Uri>((_, registryUri) => containerClient);
            clientFactory
                .Setup(m => m.CreateAuthenticatedContainerClient(It.IsAny<CloudConfiguration>(), It.IsAny<Uri>()))
                .Returns<CloudConfiguration, Uri>((_, registryUri) => containerClient);

            return (clientFactory.Object, blobClientsByRepository, containerClient);

            MockRegistryBlobClient GetBlobClient(Uri registryUri, string repository)
            {
                if (blobClientsByRepository.TryGetValue((registryUri, repository), out var client))
                {
                    return client;
                }

                throw new InvalidOperationException($"No mock blob client was registered for Uri '{registryUri}' and repository '{repository}'.");
            }
        }
    }
}



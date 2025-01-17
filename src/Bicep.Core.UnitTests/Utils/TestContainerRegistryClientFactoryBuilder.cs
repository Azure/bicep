// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
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
        private readonly FakeContainerRegistryClient containerClient = new();

        public TestContainerRegistryClientFactoryBuilder WithRepository(string registryHost, string repository)
        {
            blobClientsBuilder.TryAdd((new Uri($"https://{registryHost}"), repository), new MockRegistryBlobClient());

            if (!containerClient.FakeRepositoryNames.ContainsKey(repository))
            {
                containerClient.FakeRepositoryNames.Add(repository, repository);
            }

            return this;
        }

        public void WithRepository(string registryHost, string repository, MockRegistryBlobClient client)
        {
            blobClientsBuilder.TryAdd((new Uri($"https://{registryHost}"), repository), client);

            if (!containerClient.FakeRepositoryNames.ContainsKey(repository))
            {
                containerClient.FakeRepositoryNames.Add(repository, repository);
            }
        }

        public (IContainerRegistryClientFactory clientFactory, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks) Build()
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

            return (clientFactory.Object, blobClientsByRepository);

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



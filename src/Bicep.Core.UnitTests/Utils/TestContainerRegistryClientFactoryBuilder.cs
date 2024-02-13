// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Registry;
using Moq;

namespace Bicep.Core.UnitTests.Utils
{
    public class TestContainerRegistryClientFactoryBuilder
    {
        private readonly ImmutableDictionary<(Uri registryUri, string repository), MockRegistryBlobClient>.Builder clientsBuilder = ImmutableDictionary.CreateBuilder<(Uri registryUri, string repository), MockRegistryBlobClient>();

        public TestContainerRegistryClientFactoryBuilder RegisterMockRepositoryBlobClient(string registryHost, string repository)
        {
            clientsBuilder.TryAdd((new Uri($"https://{registryHost}"), repository), new MockRegistryBlobClient());

            return this;
        }

        public void RegisterMockRepositoryBlobClient(string registryHost, string repository, MockRegistryBlobClient client)
        {
            clientsBuilder.TryAdd((new Uri($"https://{registryHost}"), repository), client);
        }

        public (IContainerRegistryClientFactory clientFactory, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks) Build()
        {
            var repoToClient = clientsBuilder.ToImmutable();
            var clientFactory = StrictMock.Of<IContainerRegistryClientFactory>();

            clientFactory
            .Setup(m => m.CreateAuthenticatedBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
            .Returns<RootConfiguration, Uri, string>((_, registryUri, repository) =>
            {
                if (repoToClient.TryGetValue((registryUri, repository), out var client))
                {
                    return client;
                }

                throw new InvalidOperationException($"No mock authenticated client was registered for Uri '{registryUri}' and repository '{repository}'.");
            });

            clientFactory
                .Setup(m => m.CreateAnonymousBlobClient(It.IsAny<RootConfiguration>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns<RootConfiguration, Uri, string>((_, registryUri, repository) =>
                {
                    if (repoToClient.TryGetValue((registryUri, repository), out var client))
                    {
                        return client;
                    }

                    throw new InvalidOperationException($"No mock anonymous client was registered for Uri '{registryUri}' and repository '{repository}'.");
                });

            return (clientFactory.Object, repoToClient);
        }
    }
}



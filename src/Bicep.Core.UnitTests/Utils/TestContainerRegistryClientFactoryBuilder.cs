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
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.UnitTests.Utils
{
    public class TestContainerRegistryClientFactoryBuilder
    {
        private readonly ImmutableDictionary<(Uri registryUri, string repository), FakeRegistryBlobClient>.Builder blobClientsBuilder = ImmutableDictionary.CreateBuilder<(Uri registryUri, string repository), FakeRegistryBlobClient>();
        private FakeContainerRegistryClient containerClient = new();

        public TestContainerRegistryClientFactoryBuilder WithRepository(RepoDescriptor repo, FakeRegistryBlobClient? client = null)
        {
            client ??= new FakeRegistryBlobClient();
            blobClientsBuilder.TryAdd((new Uri($"https://{repo.Registry}"), repo.Repository), client);

            if (!containerClient.FakeRepositories.ContainsKey(repo.Repository))
            {
                containerClient.FakeRepositories.Add(repo.Repository, new(repo.Registry, repo.Repository, [.. repo.Tags.Select(t => t.Tag)]));
            }

            return this;
        }

        public TestContainerRegistryClientFactoryBuilder WithFakeContainerRegistryClient(FakeContainerRegistryClient containerRegistryClient)
        {
            this.containerClient.FakeRepositories.Should().BeEmpty("Must set up ContainerRegistryClient before adding repos");
            this.containerClient = containerRegistryClient;
            return this;
        }

        public IContainerRegistryClientFactory Build()
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

            return clientFactory.Object;

            FakeRegistryBlobClient GetBlobClient(Uri registryUri, string repository)
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



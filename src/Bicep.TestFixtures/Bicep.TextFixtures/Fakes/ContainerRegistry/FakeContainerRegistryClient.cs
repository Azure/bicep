// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Containers.ContainerRegistry;

namespace Bicep.TextFixtures.Fakes.ContainerRegistry
{
    public class FakeContainerRegistryClient : ContainerRegistryClient
    {
        public record FakeRepository(string Registry, string Repository, List<string> Tags);

        private readonly FakeContainerRegistry registry;

        public FakeContainerRegistryClient(FakeContainerRegistry registry)
            : base()
        {
            // ensure we call the base parameterless constructor to prevent outgoing calls
            this.registry = registry;
        }

        public SortedList<string, FakeRepository> FakeRepositories { get; } = new();

        public override AsyncPageable<string> GetRepositoryNamesAsync(CancellationToken cancellationToken = default)
        {
            var repositoryNames = this.registry.EnumerateRepositoryNames().OrderBy(x => x).ToArray();
            var page = Page<string>.FromValues(repositoryNames, continuationToken: null, DummyResponse.Instance);

            return AsyncPageable<string>.FromPages([page]);
        }

        public override ContainerRepository GetRepository(string repositoryName) => this.registry.GetRepository(repositoryName);

    }
}

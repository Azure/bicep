// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using Azure;
using Azure.Containers.ContainerRegistry;
using Bicep.Core.Modules;
using Bicep.Core.Registry.Oci;
using Bicep.Core.UnitTests.Mock;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Legacy.Table;
using Moq;

namespace Bicep.Core.UnitTests.Registry
{
    /// <summary>
    /// Mock OCI registry blob client. This client is intended to represent a single repository within a specific registry Uri.
    /// </summary>
    public class FakeContainerRegistryClient : ContainerRegistryClient
    {
        public record FakeRepository(string Registry, string Repository, List<string> Tags);

        public FakeContainerRegistryClient() : base()
        {
            // ensure we call the base parameterless constructor to prevent outgoing calls
        }

        public int CallsToGetRepositoryNamesAsync { get; private set; }
        public int CallsToGetAllManifestPropertiesAsync { get; private set; }

        public SortedList<string, FakeRepository> FakeRepositories { get; } = new();

        public override AsyncPageable<string> GetRepositoryNamesAsync(CancellationToken cancellationToken = default)
        {
            CallsToGetRepositoryNamesAsync++;

            var page = Page<string>.FromValues(FakeRepositories.Keys.ToArray(), continuationToken: null, StrictMock.Of<Response>().Object);
            return AsyncPageable<string>.FromPages([page]);
        }

        public override ContainerRepository GetRepository(string repositoryName)
        {
            var repository = StrictMock.Of<ContainerRepository>();
            repository.Setup(x => x.GetAllManifestPropertiesAsync(It.IsAny<ArtifactManifestOrder>(), It.IsAny<CancellationToken>()))
                .Returns((ArtifactManifestOrder order, CancellationToken token) =>
                    {
                        CallsToGetAllManifestPropertiesAsync++;

                        var constructor = typeof(ArtifactManifestProperties).GetConstructor(
                            BindingFlags.NonPublic | BindingFlags.Instance,
                            null,
                            [typeof(string), typeof(DateTimeOffset), typeof(DateTimeOffset)],
                            null);

                        var properties = (ArtifactManifestProperties)constructor!.Invoke(["digest", DateTimeOffset.Now, DateTimeOffset.Now]);

                        var tagsField = typeof(ArtifactManifestProperties).GetField("<Tags>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                        tagsField!.SetValue(properties, FakeRepositories[repositoryName].Tags.ToImmutableArray());

                        return AsyncPageable<ArtifactManifestProperties>.FromPages(
                            new[] { Page<ArtifactManifestProperties>.FromValues(
                            new[] { properties }, null, StrictMock.Of<Response>().Object) }
                        );
                    }
                );
            return repository.Object;
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Registry;

namespace Bicep.Core.UnitTests.Utils;

public class ContainerRegistryClientFactoryExtensions
{
    public static (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks, FakeContainerRegistryClient containerRegistryClient) CreateMockRegistryClients(params (string, string)[] clients)
    {
        var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

        foreach (var (registryHost, repository) in clients)
        {
            containerRegistryFactoryBuilder.WithRepository(registryHost, repository);

        }

        return containerRegistryFactoryBuilder.Build();
    }
}

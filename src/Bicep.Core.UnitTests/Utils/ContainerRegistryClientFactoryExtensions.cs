// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Registry;

namespace Bicep.Core.UnitTests.Utils;

public class ContainerRegistryClientFactoryExtensions
{
    public static (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks) CreateMockRegistryClients(params (string, string)[] clients)
    {
        var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

        foreach (var (registryHost, repository) in clients)
        {
            containerRegistryFactoryBuilder.RegisterMockRepositoryBlobClient(registryHost, repository);

        }

        return containerRegistryFactoryBuilder.Build();
    }
}

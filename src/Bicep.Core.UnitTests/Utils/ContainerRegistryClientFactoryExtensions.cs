// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Registry;
using static Bicep.Core.UnitTests.Utils.RegistryHelper;

namespace Bicep.Core.UnitTests.Utils;

public class ContainerRegistryClientFactoryExtensions
{
    public static (IContainerRegistryClientFactory factoryMock, ImmutableDictionary<(Uri, string), MockRegistryBlobClient> blobClientMocks, FakeContainerRegistryClient containerRegistryClient)
        CreateMockRegistryClients(params RepoDescriptor[] repos)
    {
        var containerRegistryFactoryBuilder = new TestContainerRegistryClientFactoryBuilder();

        foreach (var repo in repos)
        {
            containerRegistryFactoryBuilder.WithRepository(repo);
        }

        return containerRegistryFactoryBuilder.Build();
    }
}

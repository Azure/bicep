// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Registry.PublicRegistry;
using Bicep.Core.UnitTests.Mock;
using Moq;

namespace Bicep.Core.UnitTests.Mock;

public static class PublicRegistryModuleIndexClientMock
{
    // CONSIDER: Mock HttpClient rather than the typed client

    public static Mock<IPublicRegistryModuleIndexClient> Create(IEnumerable<PublicRegistryModuleIndexEntry> metadata)
    {
        var mock = StrictMock.Of<IPublicRegistryModuleIndexClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ReturnsAsync(() => metadata.ToImmutableArray());
        return mock;
    }

    public static Mock<IPublicRegistryModuleIndexClient> CreateToThrow(Exception exception)
    {
        var mock = StrictMock.Of<IPublicRegistryModuleIndexClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ThrowsAsync(exception);
        return mock;
    }
}

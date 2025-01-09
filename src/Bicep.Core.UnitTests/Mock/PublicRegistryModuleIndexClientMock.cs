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

    public static Mock<IPublicRegistryModuleIndexHttpClient> Create(IEnumerable<PublicRegistryModuleIndexEntry> metadata)
    {
        var mock = StrictMock.Of<IPublicRegistryModuleIndexHttpClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ReturnsAsync(() => [.. metadata]);
        return mock;
    }

    public static Mock<IPublicRegistryModuleIndexHttpClient> CreateToThrow(Exception exception)
    {
        var mock = StrictMock.Of<IPublicRegistryModuleIndexHttpClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ThrowsAsync(exception);
        return mock;
    }
}

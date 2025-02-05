// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Registry.PublicRegistry;
using Moq;

namespace Bicep.Core.UnitTests.Mock.Registry;

public static class PublicModuleIndexHttpClientMocks
{
    // CONSIDER: Mock HttpClient rather than the typed client

    public static Mock<IPublicModuleIndexClient> Create(IEnumerable<PublicModuleIndexEntry> metadata)
    {
        var mock = StrictMock.Of<IPublicModuleIndexClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ReturnsAsync(() => metadata.ToImmutableArray());
        return mock;
    }

    public static Mock<IPublicModuleIndexClient> CreateToThrow(Exception exception)
    {
        var mock = StrictMock.Of<IPublicModuleIndexClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ThrowsAsync(exception);
        return mock;
    }
}

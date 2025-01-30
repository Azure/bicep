// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Moq;

namespace Bicep.Core.UnitTests.Mock.Registry.Catalog;

public static class PublicModuleIndexHttpClientMocks
{
    public static Mock<IPublicModuleIndexHttpClient> Create(IEnumerable<PublicModuleIndexEntry> metadata)
    {
        var mock = StrictMock.Of<IPublicModuleIndexHttpClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ReturnsAsync(() => [.. metadata]);
        return mock;
    }

    public static Mock<IPublicModuleIndexHttpClient> CreateToThrow(Exception exception)
    {
        var mock = StrictMock.Of<IPublicModuleIndexHttpClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ThrowsAsync(exception);
        return mock;
    }
}

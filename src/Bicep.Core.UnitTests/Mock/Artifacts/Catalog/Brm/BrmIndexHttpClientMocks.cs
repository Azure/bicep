// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Artifacts.Catalog.Brm;
using Moq;

namespace Bicep.Core.UnitTests.Mock.Artifacts.Catalog.Brm;

public static class BrmIndexHttpClientMocks
{
    // CONSIDER: Mock HttpClient rather than the typed client

    public static Mock<IBrmIndexHttpClient> Create(IEnumerable<BrmIndexEntry> metadata)
    {
        var mock = StrictMock.Of<IBrmIndexHttpClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ReturnsAsync(() => metadata.ToImmutableArray());
        return mock;
    }

    public static Mock<IBrmIndexHttpClient> CreateToThrow(Exception exception)
    {
        var mock = StrictMock.Of<IBrmIndexHttpClient>();
        mock
            .Setup(client => client.GetModuleIndexAsync())
            .ThrowsAsync(exception);
        return mock;
    }
}

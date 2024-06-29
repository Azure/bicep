// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Immutable;
using Bicep.Core.Registry.PublicRegistry;
using Bicep.Core.UnitTests.Mock;
using Moq;

namespace Bicep.Core.UnitTests.Mock;

public static class PublicRegistryModuleMetadataClientMock
{
    // CONSIDER: Mock HttpClient rather than the typed client

    public static Mock<IPublicRegistryModuleMetadataClient> Create(IEnumerable<BicepModuleMetadata> metadata)
    {
        var mock = StrictMock.Of<IPublicRegistryModuleMetadataClient>();
        mock
            .Setup(client => client.GetModuleMetadata())
            .ReturnsAsync(() => metadata.ToImmutableArray());
        return mock;
    }

    public static Mock<IPublicRegistryModuleMetadataClient> CreateToThrow(Exception exception)
    {
        var mock = StrictMock.Of<IPublicRegistryModuleMetadataClient>();
        mock
            .Setup(client => client.GetModuleMetadata())
            .ThrowsAsync(exception);
        return mock;
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Moq;

namespace Bicep.Core.UnitTests.Mock;

public class MockPublicModuleIndexHttpClient : IPublicModuleIndexHttpClient
{
    public MockPublicModuleIndexHttpClient(HttpClient _)
    {
    }

    public Task<ImmutableArray<PublicModuleIndexEntry>> GetModuleIndexAsync() => Task.FromResult(ImmutableArray<PublicModuleIndexEntry>.Empty);
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.Catalog.Brm;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;
using Moq;

namespace Bicep.Core.UnitTests.Mock.Artifacts.Catalog.Brm;

public class MockBrmIndexHttpClient : IBrmIndexHttpClient
{
    public MockBrmIndexHttpClient(HttpClient _)
    {
    }

    public Task<ImmutableArray<BrmIndexEntry>> GetModuleIndexAsync() => Task.FromResult(ImmutableArray<BrmIndexEntry>.Empty);
}

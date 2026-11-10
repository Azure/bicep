// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Artifacts.Catalog.Brm;

public interface IBrmIndexHttpClient
{
    Task<ImmutableArray<BrmIndexEntry>> GetModuleIndexAsync();
}

public readonly record struct BrmIndexProperties(string Description, string DocumentationUri);


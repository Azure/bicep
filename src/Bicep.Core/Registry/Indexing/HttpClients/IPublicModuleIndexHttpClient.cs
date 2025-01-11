// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry.Indexing.HttpClients;

public interface IPublicModuleIndexHttpClient
{
    Task<ImmutableArray<PublicModuleIndexEntry>> GetModuleIndexAsync();
}

public readonly record struct PublicModuleIndexProperties(string Description, string DocumentationUri);


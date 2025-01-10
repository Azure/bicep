// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Registry.PublicRegistry.HttpClients; //asdfg rename to Registry.Acr?  Oci?  Modules?   Public?

public interface IPublicModuleIndexHttpClient
{
    Task<ImmutableArray<PublicModuleIndexEntry>> GetModuleIndexAsync();
}

public readonly record struct PublicModuleIndexProperties(string Description, string DocumentationUri);


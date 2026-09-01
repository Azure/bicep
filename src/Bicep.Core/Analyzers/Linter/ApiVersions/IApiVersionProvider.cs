// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.TypeSystem;
using System.Collections.Immutable;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public interface IApiVersionProvider
{
    public ImmutableSortedSet<AzureResourceApiVersion> GetApiVersions(ResourceScope scope, string fullyQualifiedResourceType);
}

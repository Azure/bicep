// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public interface IApiVersionProvider
{
    public ImmutableSortedSet<AzureResourceApiVersion> GetApiVersions(ResourceScope scope, string fullyQualifiedResourceType);
}

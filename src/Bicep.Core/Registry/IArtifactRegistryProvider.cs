// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Represents the configured artifact registries for the current instance of Bicep.
    /// </summary>
    public interface IArtifactRegistryProvider
    {
        ImmutableArray<string> SupportedSchemes { get; }

        public IArtifactRegistry? TryGetRegistry(string scheme);
    }
}

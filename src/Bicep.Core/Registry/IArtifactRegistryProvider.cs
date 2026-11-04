// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Registry
{
    /// <summary>
    /// Represents the configured registries of remote artifacts for the current instance of Bicep.
    /// </summary>
    public interface IArtifactRegistryProvider
    {
        /// <summary>
        /// Gets the schemes of remote artifact references that can be resolved, e.g. <c>br</c> and <c>ts</c>.
        /// </summary>
        ImmutableArray<string> SupportedSchemes { get; }

        public IArtifactRegistry? TryGetRegistry(string scheme);

        /// <summary>
        /// Gets the registry that handles the type of <paramref name="reference"/>.
        /// </summary>
        public IArtifactRegistry? TryGetRegistry(ExternalArtifactReference reference);
    }
}

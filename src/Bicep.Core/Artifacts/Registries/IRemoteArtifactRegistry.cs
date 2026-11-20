// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Artifacts.Registries
{
    /// <summary>
    /// An implementation of a Bicep artifact registry.
    /// </summary>
    public interface IRemoteArtifactRegistry
    {
        /// <summary>
        /// Gets the schemes that name this registry's artifacts in artifact references.
        /// </summary>
        ImmutableArray<string> Schemes { get; }

        /// <summary>
        /// Returns true if this registry handles the specified reference.
        /// </summary>
        bool CanHandle(RemoteArtifactReference reference);

        /// <summary>
        /// Opens the artifact from the local cache, or returns <c>null</c> if the artifact has not been restored (or
        /// cannot be, for example because its registry is not trusted).
        /// </summary>
        Artifact? TryOpen(RemoteArtifactReference reference);

        /// <summary>
        /// Downloads the specified artifacts from the registry and caches them locally. A forced restore deletes the
        /// cached copies first. Returns the errors for the artifacts that could not be deleted or downloaded.
        /// </summary>
        Task<IDictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreAsync(IEnumerable<RemoteArtifactReference> references, RestoreMode mode);
    }
}

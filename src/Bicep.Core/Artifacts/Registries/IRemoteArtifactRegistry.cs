// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Restore;
using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Registries
{
    /// <summary>
    /// An implementation of a Bicep artifact registry.
    /// </summary>
    public interface IRemoteArtifactRegistry
    {
        /// <summary>
        /// Gets the scheme used by this registry in artifact references.
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Returns true if this registry handles the specified reference.
        /// </summary>
        bool CanHandle(RemoteArtifactReference reference);

        /// <summary>
        /// Gets the file in the local cache that the compiler loads for the artifact, or <c>null</c> if the artifact
        /// has not been restored (or cannot be, for example because its registry is not trusted).
        /// </summary>
        IFileHandle? TryGetEntryPoint(RemoteArtifactReference reference);

        /// <summary>
        /// Downloads the specified artifacts from the registry and caches them locally. A forced restore deletes the
        /// cached copies first. Returns the errors for the artifacts that could not be deleted or downloaded.
        /// </summary>
        Task<IDictionary<RemoteArtifactReference, DiagnosticBuilder.DiagnosticBuilderDelegate>> RestoreAsync(IEnumerable<RemoteArtifactReference> references, RestoreMode mode);
    }
}

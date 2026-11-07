// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using Bicep.Core.Artifacts.References;

namespace Bicep.Core.Artifacts.Restore;

/// <summary>
/// Restores remote artifacts into the local artifact cache and tracks recent restore failures.
/// Local artifacts are never restored.
/// </summary>
public interface IArtifactRestorer
{
    ArtifactRestoreState GetRestoreState(ExternalArtifactReference reference);

    /// <returns><c>true</c> if a restore was attempted and the caller should recompile.</returns>
    Task<bool> RestoreAsync(IEnumerable<ExternalArtifactReference> references, RestoreMode mode);
}

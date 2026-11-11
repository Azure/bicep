// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using Bicep.Core.Artifacts.References;

namespace Bicep.Core.Artifacts.Restore;

/// <summary>
/// Restores remote artifacts into the local artifact cache and tracks recent restore failures.
/// Local artifacts are never restored.
/// </summary>
public interface IRemoteArtifactRestorer
{
    RemoteArtifactRestoreState GetRestoreState(RemoteArtifactReference reference);

    /// <returns><c>true</c> if a restore was attempted and the caller should recompile.</returns>
    Task<bool> RestoreAsync(IEnumerable<RemoteArtifactReference> references, RestoreMode mode);
}

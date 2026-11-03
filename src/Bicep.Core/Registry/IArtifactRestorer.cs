// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry;

/// <summary>
/// Restores artifacts into the local artifact cache and tracks their restore status.
/// </summary>
public interface IArtifactRestorer
{
    ArtifactRestoreStatus GetRestoreStatus(ArtifactReference reference, out DiagnosticBuilder.DiagnosticBuilderDelegate? errorDetailBuilder);

    ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPoint(ArtifactReference reference);

    Task<bool> RestoreArtifacts(IEnumerable<ArtifactReference> references, bool forceRestore);

    void PruneRestoreStatuses();
}

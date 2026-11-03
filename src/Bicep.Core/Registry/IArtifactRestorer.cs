// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry;

/// <summary>
/// Restores artifacts into the local artifact cache and tracks recent restore failures.
/// </summary>
public interface IArtifactRestorer
{
    ArtifactRestoreState GetRestoreState(ArtifactReference reference);

    ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPoint(ArtifactReference reference);

    /// <returns><c>true</c> if a restore was attempted and the caller should recompile.</returns>
    Task<bool> RestoreAsync(IEnumerable<ArtifactReference> references, RestoreMode mode);
}

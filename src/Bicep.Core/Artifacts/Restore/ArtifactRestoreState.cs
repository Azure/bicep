// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Restore;

/// <summary>
/// The restore state of an artifact.
/// </summary>
public abstract record ArtifactRestoreState
{
    private ArtifactRestoreState() { }

    /// <summary>
    /// The artifact is available in the local cache.
    /// </summary>
    /// <param name="EntryPoint">The file in the local cache that the compiler loads for the artifact.</param>
    public sealed record Restored(IFileHandle EntryPoint) : ArtifactRestoreState;

    /// <summary>
    /// The artifact is not in the local cache and no recent restore has failed.
    /// </summary>
    public sealed record NotRestored : ArtifactRestoreState;

    /// <summary>
    /// A recent restore of the artifact failed. It is not retried until the failure expires or a forced restore is requested.
    /// </summary>
    public sealed record RestoreFailed(DiagnosticBuilder.DiagnosticBuilderDelegate Error) : ArtifactRestoreState;
}

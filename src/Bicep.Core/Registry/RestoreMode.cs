// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry;

public enum RestoreMode
{
    /// <summary>
    /// Restore artifacts that are not in the local cache. Artifacts whose restore failed recently are not retried.
    /// </summary>
    IfMissing,

    /// <summary>
    /// Clear the local cache entries and recent failures for the artifacts, then restore them again.
    /// </summary>
    Force,
}

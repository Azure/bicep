// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;

namespace Bicep.Core.Utils.Snapshots;

/// <summary>
/// A predicted resource paired with its origin. <see cref="Resource"/> is exactly the customer snapshot JSON.
/// Equality of this record compares <see cref="Resource"/> by reference.
/// </summary>
public sealed record SnapshotAnalysisResource(
    JsonElement Resource,
    SnapshotResourceOrigin Origin,
    bool IsExtensible);

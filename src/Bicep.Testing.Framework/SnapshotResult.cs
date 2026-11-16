// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Testing.Framework;

/// <summary>
/// The flattened resources, evaluation diagnostics, and resolved outputs predicted by the CLI.
/// Diagnostics must be checked before relying on a potentially incomplete prediction.
/// </summary>
public sealed record SnapshotResult
{
    [JsonPropertyName("predictedResources")]
    public required ImmutableArray<SnapshotResource> PredictedResources { get; init; }

    [JsonPropertyName("diagnostics")]
    public required ImmutableArray<string> Diagnostics { get; init; }

    [JsonPropertyName("outputs")]
    public required ImmutableDictionary<string, JsonElement> Outputs { get; init; }
}

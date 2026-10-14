// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Utils.Snapshots;

[JsonSerializable(typeof(Snapshot))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class SnapshotSerializationContext : JsonSerializerContext
{
    public static SnapshotSerializationContext FileSerializer { get; } = new SnapshotSerializationContext(new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    });
}

public record Snapshot(
    ImmutableArray<JsonElement> PredictedResources,
    ImmutableArray<string> Diagnostics,
    ImmutableDictionary<string, JsonElement> Outputs);

internal sealed record SnapshotAnalysisResult(
    Snapshot Snapshot,
    ImmutableArray<JsonElement> ExtensibleResources,
    ImmutableArray<SnapshotResourceProvenance> ResourceProvenance);

internal record SnapshotResourceProvenance(
    string? SymbolicName,
    string? CopyLoopSymbolicName,
    string? CopyLoopName,
    int? CopyIndex,
    string? Path,
    int? LineNumber,
    int? LinePosition,
    bool IsExtensible = false)
{
    public static readonly SnapshotResourceProvenance Unknown = new(null, null, null, null, null, null, null);
}

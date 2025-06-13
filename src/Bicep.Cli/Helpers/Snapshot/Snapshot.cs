// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Cli.Helpers.Snapshot;

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
    ImmutableArray<string> Diagnostics);

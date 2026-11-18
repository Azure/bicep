// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Testing.Framework;

[JsonSerializable(typeof(Snapshot))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    RespectNullableAnnotations = true)]
internal partial class SnapshotSerializationContext : JsonSerializerContext
{
    public static SnapshotSerializationContext FileSerializer { get; } = new SnapshotSerializationContext(new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        RespectNullableAnnotations = true,
    });
}

internal sealed record Snapshot
{
    public required ImmutableArray<JsonElement> PredictedResources { get; init; }

    public required ImmutableArray<string> Diagnostics { get; init; }

    public required ImmutableDictionary<string, JsonElement> Outputs { get; init; }
}
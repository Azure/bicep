// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Cli.Helpers.Snapshot;

[JsonSerializable(typeof(Snapshot))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class SnapshotSerializationContext : JsonSerializerContext { }

public record Snapshot(
    ImmutableArray<JsonElement> PredictedResources,
    ImmutableArray<string> Diagnostics);

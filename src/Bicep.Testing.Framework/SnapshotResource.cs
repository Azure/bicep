// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Testing.Framework;

/// <summary>
/// An evaluated resource. Azure-specific fields may be absent for extensible resources.
/// </summary>
public sealed record SnapshotResource
{
    [JsonPropertyName("id")]
    public SnapshotValue<SnapshotResourceIdentifier> Id { get; init; } = SnapshotValue<SnapshotResourceIdentifier>.Missing;

    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("name")]
    public SnapshotValue<string>? Name { get; init; }

    [JsonPropertyName("apiVersion")]
    public string? ApiVersion { get; init; }

    [JsonPropertyName("location")]
    public SnapshotValue<string>? Location { get; init; }

    [JsonPropertyName("kind")]
    public SnapshotValue<string>? Kind { get; init; }

    [JsonPropertyName("properties")]
    public JsonElement Properties { get; init; }

    [JsonPropertyName("sku")]
    public JsonElement? Sku { get; init; }

    [JsonPropertyName("tags")]
    public JsonElement? Tags { get; init; }

    [JsonPropertyName("identity")]
    public JsonElement? Identity { get; init; }

    [JsonPropertyName("zones")]
    public JsonElement? Zones { get; init; }

    [JsonPropertyName("plan")]
    public JsonElement? Plan { get; init; }

    [JsonPropertyName("managedBy")]
    public SnapshotValue<string>? ManagedBy { get; init; }

    [JsonPropertyName("extendedLocation")]
    public JsonElement? ExtendedLocation { get; init; }

    /// <summary>
    /// Other resource fields not modeled by this class, such as <c>identifiers</c>.
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; init; }
}

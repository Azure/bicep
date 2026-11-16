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
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("apiVersion")]
    public string? ApiVersion { get; init; }

    [JsonPropertyName("location")]
    public string? Location { get; init; }

    [JsonPropertyName("properties")]
    public JsonElement Properties { get; init; }

    /// <summary>
    /// Other resource fields, such as <c>sku</c>, <c>tags</c>, <c>identity</c>, or <c>identifiers</c>.
    /// </summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement>? AdditionalProperties { get; init; }
}

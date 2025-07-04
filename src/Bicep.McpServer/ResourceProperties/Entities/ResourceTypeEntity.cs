// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Bicep.McpServer.ResourceProperties.Entities;

public class ResourceTypeEntity : ComplexType
{
    [JsonPropertyName("bodyType")]
    public required ComplexType BodyType { get; init; }

    [JsonPropertyName("flags")]
    public string Flags { get; init; } = string.Empty;

    [JsonPropertyName("scopeType")]
    public string ScopeType { get; init; } = "Unknown";

    [JsonPropertyName("readOnlyScopes")]
    public string? ReadOnlyScopes { get; init; }

    public override bool Equals(object? obj) =>
        obj is ResourceTypeEntity other &&
        Name == other.Name &&
        BodyType.Equals(other.BodyType) &&
        Flags == other.Flags &&
        ScopeType == other.ScopeType &&
        ReadOnlyScopes == other.ReadOnlyScopes;

    public override int GetHashCode() =>
        HashCode.Combine(Name, BodyType, Flags, ScopeType, ReadOnlyScopes);
}

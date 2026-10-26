// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.McpServer.Core.ResourceProperties.Entities;

public class ResourceTypeEntity : ComplexType
{
    public required ComplexType BodyType { get; init; }

    public string Flags { get; init; } = string.Empty;

    public required string ReadableScopes { get; init; }

    public required string WritableScopes { get; init; }

    public override bool Equals(object? obj) =>
        obj is ResourceTypeEntity other &&
        Name == other.Name &&
        BodyType.Equals(other.BodyType) &&
        Flags == other.Flags &&
        ReadableScopes == other.ReadableScopes &&
        WritableScopes == other.WritableScopes;

    public override int GetHashCode() =>
        HashCode.Combine(Name, BodyType, Flags, ReadableScopes, WritableScopes);
}

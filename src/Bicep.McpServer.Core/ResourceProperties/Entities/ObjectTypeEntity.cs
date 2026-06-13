// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.McpServer.Core.ResourceProperties.Entities;

public class ObjectTypeEntity : ComplexType
{
    public List<PropertyInfo> Properties { get; init; } = [];

    public string? AdditionalPropertiesType { get; init; }

    public bool? Sensitive { get; init; }

    public override bool Equals(object? obj) =>
        obj is ObjectTypeEntity other &&
        Name == other.Name &&
        Properties.SequenceEqual(other.Properties) &&
        AdditionalPropertiesType == other.AdditionalPropertiesType &&
        Sensitive == other.Sensitive;

    public override int GetHashCode() =>
        HashCode.Combine(
            Name,
            Properties.Aggregate(0, (hash, prop) => HashCode.Combine(hash, prop)),
            AdditionalPropertiesType,
            Sensitive
        );
}

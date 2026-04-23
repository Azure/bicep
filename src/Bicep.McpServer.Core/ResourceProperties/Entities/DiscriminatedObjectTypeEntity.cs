// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.McpServer.Core.ResourceProperties.Entities;

public class DiscriminatedObjectTypeEntity : ComplexType
{
    public List<PropertyInfo> BaseProperties { get; init; } = [];

    public List<ComplexType> Elements { get; init; } = [];

    public override bool Equals(object? obj) =>
        obj is DiscriminatedObjectTypeEntity other &&
        Name == other.Name &&
        BaseProperties.SequenceEqual(other.BaseProperties) &&
        Elements.SequenceEqual(other.Elements);

    public override int GetHashCode() =>
        HashCode.Combine(
            Name,
            BaseProperties.Aggregate(0, (hash, prop) => HashCode.Combine(hash, prop)),
            Elements.Aggregate(0, (hash, elem) => HashCode.Combine(hash, elem))
        );
}

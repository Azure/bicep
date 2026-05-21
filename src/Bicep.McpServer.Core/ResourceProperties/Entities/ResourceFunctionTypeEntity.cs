// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.McpServer.Core.ResourceProperties.Entities;

public class ResourceFunctionTypeEntity : ComplexType
{
    public required string ResourceType { get; init; }

    public required string ApiVersion { get; init; }

    public string? InputType { get; init; }

    public required string OutputType { get; init; }

    public override bool Equals(object? obj) =>
        obj is ResourceFunctionTypeEntity other &&
        Name == other.Name &&
        ResourceType == other.ResourceType &&
        ApiVersion == other.ApiVersion &&
        InputType == other.InputType &&
        OutputType == other.OutputType;

    public override int GetHashCode() =>
        HashCode.Combine(Name, ResourceType, ApiVersion, InputType, OutputType);
}

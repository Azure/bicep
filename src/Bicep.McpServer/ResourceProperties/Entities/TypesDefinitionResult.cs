// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.McpServer.ResourceProperties.Entities;

public record TypesDefinitionResult
{
    public required string ResourceProvider { get; init; }

    public required string ApiVersion { get; init; }

    public List<ResourceTypeEntity> ResourceTypeEntities { get; init; } = [];

    public List<ResourceFunctionTypeEntity> ResourceFunctionTypeEntities { get; init; } = [];

    public List<ComplexType> OtherComplexTypeEntities { get; set; } = [];
}

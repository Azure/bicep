// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Bicep.McpServer.ResourceProperties.Helpers;

namespace Bicep.McpServer.ResourceProperties.Entities;

public record ResourcePropertyEntity
{
    public ResourcePropertyEntity(Guid resourceTypeId, string fullyQualifiedName, string description, string type, string flags, string valueConstraints, string? additionalPropertiesType)
    {
        ResourceTypeId = resourceTypeId;
        FullyQualifiedName = fullyQualifiedName;
        Type = type;
        Description = description;
        Flags = flags;
        ValueConstraints = valueConstraints;
        AdditionalPropertiesType = additionalPropertiesType;
    }

    [JsonPropertyName("id")]
    public Guid Id
    {
        get => GuidHelper.GenerateDeterministicGuid(ResourceTypeId.ToString().ToLower(), FullyQualifiedName.ToLower());
    }

    [JsonPropertyName("resourceTypeId")]
    public Guid ResourceTypeId { get; init; }

    [JsonPropertyName("fullyQualifiedName")]
    public string FullyQualifiedName { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("flags")]
    public string Flags { get; init; }

    [JsonPropertyName("valueConstraints")]
    public string ValueConstraints { get; init; }

    [JsonPropertyName("additionalPropertiesType")]
    public string? AdditionalPropertiesType { get; init; }
}

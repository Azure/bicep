// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Bicep.McpServer.ResourceProperties.Entities;

public record PropertyInfo
{
    public PropertyInfo(string name, string type, string description, string flags, string modifiers)
    {
        Name = name;
        Type = type;
        Description = description;
        Flags = flags;
        Modifiers = modifiers;
    }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("flags")]
    public string Flags { get; init; }

    [JsonPropertyName("modifiers")]
    public string Modifiers { get; init; }

}

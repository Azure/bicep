// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.McpServer.Core.ResourceProperties.Entities;

public record PropertyInfo
{
    public PropertyInfo(string name, string type, string description, string flags, string? modifiers)
    {
        Name = name;
        Type = type;
        Description = description;
        Flags = flags;
        Modifiers = modifiers;
    }

    public string Name { get; init; }

    public string Type { get; init; }

    public string Description { get; init; }

    public string Flags { get; init; }

    public string? Modifiers { get; init; }
}

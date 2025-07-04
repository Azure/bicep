// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.McpServer.ResourceProperties.Helpers;
using System.Text.Json.Serialization;

namespace Bicep.McpServer.ResourceProperties.Entities;

[JsonConverter(typeof(ComplexTypeConverter))]
public abstract class ComplexType
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

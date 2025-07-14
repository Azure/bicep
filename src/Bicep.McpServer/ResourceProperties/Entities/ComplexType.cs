// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Bicep.McpServer.ResourceProperties.Helpers;

namespace Bicep.McpServer.ResourceProperties.Entities;

[JsonConverter(typeof(ComplexTypeConverter))]
public abstract class ComplexType
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

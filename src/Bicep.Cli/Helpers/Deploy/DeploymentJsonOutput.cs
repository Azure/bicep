// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Bicep.Cli.Helpers.Deploy;

public record DeploymentJsonOutput(
    ImmutableDictionary<string, JsonNode>? Outputs,
    string? Error)
{
    public string ToFormattedString()
        => JsonSerializer.Serialize(this, DeploymentJsonOutputSerializationContext.FileSerializer.DeploymentJsonOutput);
}

[JsonSerializable(typeof(DeploymentJsonOutput))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class DeploymentJsonOutputSerializationContext : JsonSerializerContext
{
    public static DeploymentJsonOutputSerializationContext FileSerializer { get; } = new DeploymentJsonOutputSerializationContext(new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    });
}
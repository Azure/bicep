// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.McpServer.ResourceProperties.Helpers;
using System.Text.Json.Serialization;

namespace Bicep.McpServer.ResourceProperties.Entities;

public record ResourceTypeRecord
{

    public ResourceTypeRecord(string provider, string apiVersion, string resourceType, string flags, string scopeType)
    {
        Provider = provider;
        ApiVersion = apiVersion;
        ResourceType = resourceType;
        Flags = flags;
        ScopeType = scopeType;
    }

    [JsonPropertyName("id")]
    public Guid Id
    {
        get => GuidHelper.GenerateDeterministicGuid(Provider.ToLower(), ApiVersion.ToLower(), ResourceType.ToLower());
    }

    [JsonPropertyName("provider")]
    public string Provider { get; init; }

    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; init; }

    [JsonPropertyName("resourceType")]
    public string ResourceType { get; init; }

    [JsonPropertyName("flags")]
    public string Flags { get; init; }

    [JsonPropertyName("scopeType")]
    public string ScopeType { get; init; }
}

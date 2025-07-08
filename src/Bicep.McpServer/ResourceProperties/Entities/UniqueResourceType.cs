﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Azure.Deployments.Core.Comparers;
using Bicep.McpServer.ResourceProperties.Helpers;

namespace Bicep.McpServer.ResourceProperties.Entities;

public record UniqueResourceType
{
    public UniqueResourceType(string resourceType)
    {
        ResourceType = resourceType;
        ApiVersions = new SortedSet<string>(ApiVersionComparer.Instance);
    }

    [JsonPropertyName("resourceType")]
    public string ResourceType { get; init; }

    [JsonPropertyName("apiVersions")]
    public SortedSet<string> ApiVersions { get; init; }
}

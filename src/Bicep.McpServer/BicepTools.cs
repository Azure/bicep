// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Text.Json;
using Azure.Bicep.Types.Az;
using Bicep.McpServer.ResourceProperties;
using Bicep.McpServer.ResourceProperties.Entities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepTools
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    private readonly ILoggerFactory loggerFactory;
    private readonly ResourceVisitor _resourceVisitor;

    public BicepTools()
    {
        loggerFactory = NullLoggerFactory.Instance;
        _resourceVisitor = new ResourceVisitor(loggerFactory.CreateLogger<ResourceVisitor>());
    }

    [McpServerTool, Description("Lists all available Azure resource types for a specific provider.")]
    public static string ListAzResourceTypesForProvider(
        [Description("The resource provider (or namespace) of the Azure resource; e.g. Microsoft.KeyVault")] string provider)
    {
        return ListAzResourceTypesFromAzTypeLoader(provider);
    }

    [McpServerTool, Description("Gets the schema for a specific Azure resource type.")]
    public string GetAzResourceTypeSchema(
        [Description("The resource type of the Azure resource; e.g. Microsoft.KeyVault/vaults")] string azResourceType,
        [Description("The API version of the resource type; e.g. 2024-11-01 or 2024-12-01-preview")] string apiVersion)
    {
        TypesDefinitionResult typesDefinition = _resourceVisitor.LoadSingleResourceType(azResourceType, apiVersion);

        var allComplexTypes = new List<ComplexType>();
        allComplexTypes.AddRange(typesDefinition.ResourceTypeEntities);
        allComplexTypes.AddRange(typesDefinition.ResourceFunctionTypeEntities);
        allComplexTypes.AddRange(typesDefinition.OtherComplexTypeEntities);

        return JsonSerializer.Serialize(allComplexTypes, JsonSerializerOptions);
    }

    private static string ListAzResourceTypesFromAzTypeLoader(string? provider = null)
    {
        AzTypeLoader azTypeLoader = new();
        var azResourceTypes = azTypeLoader.LoadTypeIndex().Resources.Keys;

        if (!string.IsNullOrEmpty(provider))
        {
            azResourceTypes = azResourceTypes.Where(type => type.StartsWith(provider, StringComparison.OrdinalIgnoreCase));
        }

        return azResourceTypes.Aggregate(string.Empty, (current, resourceType) =>
            current + $"{resourceType}\n");
    }
}

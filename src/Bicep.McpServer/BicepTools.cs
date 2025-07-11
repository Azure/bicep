// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.McpServer.ResourceProperties;
using Bicep.McpServer.ResourceProperties.Entities;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepTools(
    AzResourceTypeLoader azResourceTypeLoader,
    ResourceVisitor resourceVisitor)
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [McpServerTool, Description(@"Lists all available Azure resource types for a specific provider. The return value is a newline-separated list
    of resource types including their API version, e.g. Microsoft.KeyVault/vaults@2024-11-01. Such information is the most accurate and up-to-date
    as it is sourced from the Azure Resource Provider APIs.")]
    public string ListAzResourceTypesForProvider(
        [Description("The resource provider (or namespace) of the Azure resource; e.g. Microsoft.KeyVault")] string providerNamespace)
    {
        var azResourceTypes = azResourceTypeLoader.GetAvailableTypes();

        if (providerNamespace is { })
        {
            azResourceTypes = azResourceTypes.Where(type => type.TypeSegments[0].Equals(providerNamespace, StringComparison.OrdinalIgnoreCase));
            return string.Join("\n", azResourceTypes.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase));
        }
        else
        {
            return string.Empty;
        }
    }

    [McpServerTool, Description(@"Gets the schema for a specific Azure resource type and API version. Such information is the most accurate and up-to-date
    as it is sourced from the Azure Resource Provider APIs.")]
    public string GetAzResourceTypeSchema(
        [Description("The resource type of the Azure resource; e.g. Microsoft.KeyVault/vaults")] string azResourceType,
        [Description("The API version of the resource type; e.g. 2024-11-01 or 2024-12-01-preview")] string apiVersion)
    {
        TypesDefinitionResult typesDefinition = resourceVisitor.LoadSingleResourceType(azResourceType, apiVersion);

        var allComplexTypes = new List<ComplexType>();
        allComplexTypes.AddRange(typesDefinition.ResourceTypeEntities);
        allComplexTypes.AddRange(typesDefinition.ResourceFunctionTypeEntities);
        allComplexTypes.AddRange(typesDefinition.OtherComplexTypeEntities);

        return JsonSerializer.Serialize(allComplexTypes, JsonSerializerOptions);
    }
}

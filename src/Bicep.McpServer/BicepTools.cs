// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.McpServer.ResourceProperties;
using Bicep.McpServer.ResourceProperties.Entities;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepTools(
    AzResourceTypeLoader azResourceTypeLoader,
    IPublicModuleIndexHttpClient publicModuleIndexHttpClient,
    ResourceVisitor resourceVisitor)
{
    private static Lazy<BinaryData> BestPracticesMarkdownLazy { get; } = new(() =>
        BinaryData.FromStream(
            typeof(BicepTools).Assembly.GetManifestResourceStream("Files/bestpractices.md") ??
            throw new InvalidOperationException("Could not find embedded resource 'Files/bestpractices.md'")));

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [McpServerTool(Title = "List available Azure resource types", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true)]
    [Description("""
    Lists all available Azure resource types and their API versions for a specific Azure resource provider namespace.
    
    Use this tool to:
    - Discover what resource types are available in a provider (e.g., what can be created under Microsoft.Storage)
    - Find the latest API versions for Azure resources
    - Explore the complete resource type catalog for a given provider
    
    Returns a newline-separated list of fully-qualified resource types with API versions (e.g., Microsoft.KeyVault/vaults@2024-11-01).
    Data is sourced directly from Azure Resource Provider APIs, ensuring accuracy and currency.
    
    Example provider namespaces: Microsoft.Compute, Microsoft.Storage, Microsoft.Network, Microsoft.Web, Microsoft.KeyVault
    """)]
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

    [McpServerTool(Title = "Get Azure resource type schema", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true)]
    [Description("""
    Retrieves the complete JSON schema definition for a specific Azure resource type and API version, including all properties, nested types, and constraints.
    
    Use this tool to:
    - Understand what properties are available on an Azure resource
    - Learn about required vs optional properties, their types, and allowed values
    - Discover nested resource types and their schemas
    - Find available resource functions and their signatures
    - Generate accurate Bicep code with proper property names and types
    
    Returns a JSON object containing:
    - Resource type definitions with all properties and their types
    - Nested complex type definitions
    - Resource function signatures (like list* operations)
    - Property constraints (min/max values, allowed values, regex patterns)
    
    Data is sourced directly from Azure Resource Provider APIs, ensuring the most accurate and up-to-date schema information.
    Specify the resource type (e.g., Microsoft.KeyVault/vaults) and API version (e.g., 2024-11-01 or 2024-12-01-preview).
    """)]
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

    [McpServerTool(Title = "Get Bicep best-practices", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true)]
    [Description("""
    Retrieves comprehensive, up-to-date best practices and coding standards for authoring Bicep templates.
    
    Use this tool when:
    - Generating new Bicep code to ensure it follows current best practices
    - Reviewing existing Bicep code for quality improvements
    - Learning recommended patterns for common scenarios
    - Understanding security, maintainability, and reliability guidelines
    
    Returns a detailed markdown document covering:
    - Naming conventions and code organization
    - Parameter and variable usage patterns
    - Resource declaration best practices
    - Module composition strategies
    - Security recommendations (secrets management, least privilege, etc.)
    - Performance optimization tips
    - Testing and validation approaches
    
    The practices are maintained by the Bicep team and reflect current recommended approaches.
    """)]
    public string GetBicepBestPractices() => BestPracticesMarkdownLazy.Value.ToString();

    [McpServerTool(Title = "List Azure Verified Modules (AVM)", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true)]
    [Description("""
    Lists metadata for all Azure Verified Modules (AVM) - Microsoft's official, pre-built, tested, and maintained Bicep modules for common Azure resource patterns.
    
    Use this tool to:
    - Discover reusable, production-ready Bicep modules for common scenarios
    - Find officially supported modules instead of writing resources from scratch
    - Check available versions and documentation for AVM modules
    - Accelerate Bicep development by leveraging tested, best-practice implementations
    
    Azure Verified Modules provide:
    - Pre-configured resource deployments following Microsoft best practices
    - Built-in security, reliability, and compliance features
    - Regular updates and maintenance by Microsoft
    - Comprehensive documentation and examples
    
    Returns a newline-separated list where each entry includes:
    - Module path (e.g., avm/res/storage/storage-account)
    - Human-readable description
    - Available versions
    - Documentation URI for detailed usage instructions
    
    Use these modules in your Bicep files to reduce code and improve quality.
    """)]
    public async Task<string> ListAvmMetadata()
    {
        var metadata = await publicModuleIndexHttpClient.GetModuleIndexAsync();

        StringBuilder sb = new();
        foreach (var entry in metadata)
        {
            var description = (entry.GetDescription() ?? "No description available").ReplaceLineEndings(" ");
            var docUri = entry.GetDocumentationUri() ?? "No documentation URI available";
            var allVersions = string.Join(", ", entry.Versions) ?? "No versions available";
            sb.AppendLine($"Module: {entry.ModulePath}; Description: {description}; Versions: [{allVersions}]; Doc URI: {docUri}");
        }

        return sb.ToString();
    }
}

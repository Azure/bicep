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
    public record ResourceTypeListResult(
        [Description("Array of Azure resource types with API versions (e.g., Microsoft.KeyVault/vaults@2024-11-01)")]
        ImmutableArray<string> ResourceTypes);

    public record ResourceTypeSchemaResult(
        [Description("JSON schema definition for the Azure resource type including all properties, nested types, and constraints")]
        string Schema);

    public record BestPracticesResult(
        [Description("Markdown document containing comprehensive Bicep best practices and coding standards")]
        string Content);

    public record AvmModuleMetadata(
        [Description("The module path (e.g., avm/res/storage/storage-account)")]
        string ModulePath,
        [Description("Human-readable description of the module")]
        string? Description,
        [Description("Available versions of the module")]
        ImmutableArray<string> Versions,
        [Description("Documentation URI for detailed usage instructions")]
        string? DocumentationUri);

    public record AvmMetadataResult(
        [Description("List of Azure Verified Module metadata entries")]
        ImmutableArray<AvmModuleMetadata> Modules);

    private static Lazy<BinaryData> BestPracticesMarkdownLazy { get; } = new(() =>
        BinaryData.FromStream(
            typeof(BicepTools).Assembly.GetManifestResourceStream("Files/bestpractices.md") ??
            throw new InvalidOperationException("Could not find embedded resource 'Files/bestpractices.md'")));

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [McpServerTool(Title = "List available Azure resource types", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Lists all available Azure resource types and their API versions for a specific Azure resource provider namespace.
    
    Use this tool to:
    - Discover what resource types are available in a provider (e.g., what can be created under Microsoft.Storage)
    - Find the latest API versions for Azure resources
    - Explore the complete resource type catalog for a given provider
    
    Data is sourced directly from Azure Resource Provider APIs, ensuring accuracy and currency.
    
    Example provider namespaces: Microsoft.Compute, Microsoft.Storage, Microsoft.Network, Microsoft.Web, Microsoft.KeyVault
    """)]
    public ResourceTypeListResult ListAzResourceTypesForProvider(
        [Description("The resource provider (or namespace) of the Azure resource; e.g. Microsoft.KeyVault")] string providerNamespace)
    {
        var azResourceTypes = azResourceTypeLoader
            .GetAvailableTypes()
            .Where(type => type.TypeSegments[0].Equals(providerNamespace, StringComparison.OrdinalIgnoreCase));

        return new([.. azResourceTypes.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase)]);
    }

    [McpServerTool(Title = "Get Azure resource type schema", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Retrieves the complete JSON schema definition for a specific Azure resource type and API version, including all properties, nested types, and constraints.
    
    Use this tool to:
    - Understand what properties are available on an Azure resource
    - Learn about required vs optional properties, their types, and allowed values
    - Discover nested resource types and their schemas
    - Find available resource functions and their signatures
    - Generate accurate Bicep code with proper property names and types
    
    The returned JSON schema includes resource type definitions, nested complex types, resource function signatures (like list* operations), and property constraints.
    Data is sourced directly from Azure Resource Provider APIs, ensuring the most accurate and up-to-date schema information.
    Specify the resource type (e.g., Microsoft.KeyVault/vaults) and API version (e.g., 2024-11-01 or 2024-12-01-preview).
    """)]
    public ResourceTypeSchemaResult GetAzResourceTypeSchema(
        [Description("The resource type of the Azure resource; e.g. Microsoft.KeyVault/vaults")] string azResourceType,
        [Description("The API version of the resource type; e.g. 2024-11-01 or 2024-12-01-preview")] string apiVersion)
    {
        TypesDefinitionResult typesDefinition = resourceVisitor.LoadSingleResourceType(azResourceType, apiVersion);

        var allComplexTypes = new List<ComplexType>();
        allComplexTypes.AddRange(typesDefinition.ResourceTypeEntities);
        allComplexTypes.AddRange(typesDefinition.ResourceFunctionTypeEntities);
        allComplexTypes.AddRange(typesDefinition.OtherComplexTypeEntities);

        return new ResourceTypeSchemaResult(JsonSerializer.Serialize(allComplexTypes, JsonSerializerOptions));
    }

    [McpServerTool(Title = "Get Bicep best-practices", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Retrieves comprehensive, up-to-date best practices and coding standards for authoring Bicep templates.
    
    Use this tool when:
    - Generating new Bicep code to ensure it follows current best practices
    - Reviewing existing Bicep code for quality improvements
    - Learning recommended patterns for common scenarios
    - Understanding security, maintainability, and reliability guidelines
    
    Covers naming conventions, code organization, parameter usage, resource declarations, module composition, security recommendations, performance optimization, and testing approaches.
    The practices are maintained by the Bicep team and reflect current recommended approaches.
    """)]
    public BestPracticesResult GetBicepBestPractices() => new(BestPracticesMarkdownLazy.Value.ToString());

    [McpServerTool(Title = "List Azure Verified Modules (AVM)", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
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
    
    Use these modules in your Bicep files to reduce code and improve quality.
    """)]
    public async Task<AvmMetadataResult> ListAvmMetadata()
    {
        var metadata = await publicModuleIndexHttpClient.GetModuleIndexAsync();

        var modules = metadata.Select(entry => new AvmModuleMetadata(
            entry.ModulePath,
            entry.GetDescription(),
            [.. entry.Versions],
            entry.GetDocumentationUri())).ToImmutableArray();

        return new AvmMetadataResult(modules);
    }
}

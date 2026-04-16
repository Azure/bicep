// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.ComponentModel;
using Bicep.Core.Registry.Catalog.Implementation.PublicRegistries;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Providers.Extensibility;
using Bicep.McpServer.Extensions;
using Bicep.McpServer.ResourceProperties;
using Bicep.McpServer.ResourceProperties.Entities;
using Bicep.McpServer.ResourceProperties.Helpers;
using ModelContextProtocol.Server;

namespace Bicep.McpServer;

[McpServerToolType]
public sealed class BicepTools(
    AzResourceTypeLoader azResourceTypeLoader,
    IPublicModuleIndexHttpClient publicModuleIndexHttpClient,
    ResourceVisitor resourceVisitor,
    ExtensionTypeLoaderProvider extensionTypeLoaderProvider)
{
    public record ResourceTypeListResult(
        [Description("Array of resource types with API versions (e.g., Microsoft.KeyVault/vaults@2024-11-01 or Microsoft.Graph/applications@v1.0)")]
        ImmutableArray<string> ResourceTypes);

    public record ResourceTypeSchemaResult(
        [Description("JSON schema definition for the resource type including all properties, nested types, and constraints")]
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

    public record ExtensionInfo(
        [Description("Extension name (e.g., microsoftgraph/v1.0)")]
        string Name,
        [Description("Human-readable description of the extension")]
        string Description,
        [Description("Available versions (tags) of the extension")]
        ImmutableArray<string> AvailableTags);

    public record PublishedExtensionsResult(
        [Description("List of published Bicep extensions with their available versions")]
        ImmutableArray<ExtensionInfo> Extensions);

    private static Lazy<BinaryData> BestPracticesMarkdownLazy { get; } = new(() =>
        BinaryData.FromStream(
            typeof(BicepTools).Assembly.GetManifestResourceStream("Files/bestpractices.md") ??
            throw new InvalidOperationException("Could not find embedded resource 'Files/bestpractices.md'")));

    [McpServerTool(Title = "List available resource types", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Lists all available resource types and their API versions for a specific resource provider namespace.
    
    Use this tool to:
    - Discover what resource types are available in a provider (e.g., what can be created under Microsoft.Storage or Microsoft.Graph)
    - Find the latest API versions for resources
    - Explore the complete resource type catalog for a given provider
    
    For Azure (az) resources, data is sourced from Azure Resource Provider APIs.
    For extension resources (e.g., Microsoft Graph), specify the extensionName and extensionVersion parameters.
    Use ListPublishedExtensions to discover available extensions and their versions.
    
    Example provider namespaces: Microsoft.Compute, Microsoft.Storage, Microsoft.Network, Microsoft.Web, Microsoft.KeyVault, Microsoft.Graph
    """)]
    public async Task<ResourceTypeListResult> ListResourceTypes(
        [Description("The resource provider (or namespace) of the resource; e.g. Microsoft.KeyVault or Microsoft.Graph")] string providerNamespace,
        [Description("The extension name for non-Azure resources (e.g., microsoftgraph/v1.0). Omit for Azure resources. Use ListPublishedExtensions to discover available extensions.")] string? extensionName = null,
        [Description("The extension version/tag (e.g., 1.0.0). Required when extensionName is provided.")] string? extensionVersion = null)
    {
        if (!string.IsNullOrEmpty(extensionName) && !string.IsNullOrEmpty(extensionVersion))
        {
            var typeLoader = await extensionTypeLoaderProvider.GetTypeLoaderAsync(extensionName, extensionVersion);
            var loader = new ExtensionResourceTypeLoader(typeLoader);
            var extensionTypes = loader
                .GetAvailableTypes()
                .Where(type => type.TypeSegments[0].Equals(providerNamespace, StringComparison.OrdinalIgnoreCase));

            return new([.. extensionTypes.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase)]);
        }

        var azResourceTypes = azResourceTypeLoader
            .GetAvailableTypes()
            .Where(type => type.TypeSegments[0].Equals(providerNamespace, StringComparison.OrdinalIgnoreCase));

        return new([.. azResourceTypes.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase)]);
    }

    [McpServerTool(Title = "Get resource type schema", Destructive = false, Idempotent = true, OpenWorld = false, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Retrieves the complete JSON schema definition for a specific resource type and API version, including all properties, nested types, and constraints.
    
    Use this tool to:
    - Understand what properties are available on a resource
    - Learn about required vs optional properties, their types, and allowed values
    - Discover nested resource types and their schemas
    - Find available resource functions and their signatures
    - Generate accurate Bicep code with proper property names and types
    
    The returned JSON schema includes resource type definitions, nested complex types, resource function signatures (like list* operations), and property constraints.
    For Azure resources, data is sourced from Azure Resource Provider APIs.
    For extension resources (e.g., Microsoft Graph), specify the extensionName and extensionVersion parameters.
    Use ListPublishedExtensions to discover available extensions and their versions.
    Specify the resource type (e.g., Microsoft.KeyVault/vaults or Microsoft.Graph/applications) and API version (e.g., 2024-11-01 or beta).
    """)]
    public async Task<ResourceTypeSchemaResult> GetResourceTypeSchema(
        [Description("The resource type; e.g. Microsoft.KeyVault/vaults or Microsoft.Graph/applications")] string resourceType,
        [Description("The API version of the resource type; e.g. 2024-11-01 or beta")] string apiVersion,
        [Description("The extension name for non-Azure resources (e.g., microsoftgraph/v1.0). Omit for Azure resources. Use ListPublishedExtensions to discover available extensions.")] string? extensionName = null,
        [Description("The extension version/tag (e.g., 1.0.0). Required when extensionName is provided.")] string? extensionVersion = null)
    {
        TypesDefinitionResult typesDefinition;

        if (!string.IsNullOrEmpty(extensionName) && !string.IsNullOrEmpty(extensionVersion))
        {
            var typeLoader = await extensionTypeLoaderProvider.GetTypeLoaderAsync(extensionName, extensionVersion);
            typesDefinition = resourceVisitor.LoadSingleResourceType(typeLoader, resourceType, apiVersion);
        }
        else
        {
            typesDefinition = resourceVisitor.LoadSingleResourceType(resourceType, apiVersion);
        }

        return new ResourceTypeSchemaResult(JsonSchemaWriter.Write(typesDefinition));
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

    [McpServerTool(Title = "List published Bicep extensions", Destructive = false, Idempotent = true, OpenWorld = true, ReadOnly = true, UseStructuredContent = true)]
    [Description("""
    Lists published Bicep extensions (e.g., Microsoft Graph) with their available versions.
    
    Use this tool to:
    - Discover available Bicep extensions beyond Azure (az) resources
    - Find available versions for Microsoft Graph and other extensions
    - Get extension names and versions to use with ListResourceTypes and GetResourceTypeSchema
    
    Extensions provide resource types for non-Azure providers like Microsoft Graph.
    Use the returned extension name and version with other tools to explore extension resource types.
    """)]
    public async Task<PublishedExtensionsResult> ListPublishedExtensions()
    {
        var extensions = new List<ExtensionInfo>();

        foreach (var extension in PublishedExtension.All)
        {
            try
            {
                var tags = await extensionTypeLoaderProvider.GetAvailableTagsAsync(extension.Name);
                extensions.Add(new ExtensionInfo(extension.Name, extension.Description, [.. tags]));
            }
            catch
            {
                extensions.Add(new ExtensionInfo(extension.Name, extension.Description, []));
            }
        }

        return new PublishedExtensionsResult([.. extensions]);
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter.ApiVersions;
using Bicep.Core.Resources;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.McpServer.Core;
using Bicep.McpServer.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class BicepToolsTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    private static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services
            .AddBicepMcpServer();

        return services.BuildServiceProvider();
    }

    private readonly BicepTools tools = GetServiceProvider().GetRequiredService<BicepTools>();

    private static ServiceProvider GetServiceProviderWithResourceTypes(params string[] resourceTypes) => new ServiceCollection()
        .AddBicepMcpServer().Services
        .AddSingleton(ResourceTypeCatalogHelper.CreateTypeLoader(resourceTypes))
        .BuildServiceProvider();

    [TestMethod]
    public void ListAzureResourceTypes_returns_list_of_resource_types()
    {
        var response = tools.ListAzureResourceTypes("Microsoft.Compute");
        var result = response.ResourceTypes;

        result.Should().HaveCountGreaterThan(700);
        result.Should().AllSatisfy(x => x.Split('/').First().Equals("Microsoft.Compute", StringComparison.OrdinalIgnoreCase))
            .And.AllSatisfy(x => x.Contains('@'));
    }

    [TestMethod]
    public void ListAzureResourceTypes_returns_empty_array_for_invalid_provider()
    {
        var response = tools.ListAzureResourceTypes("Invalid.Provider");
        response.ResourceTypes.Should().BeEmpty();
    }

    [TestMethod]
    [EmbeddedFilesTestData(@"Files/GetAzResourceSchema/.*\.json")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public void GetAzureResourceTypeSchema_returns_resource_schema(EmbeddedFile jsonFile)
    {
        var baselineFile = BaselineFolder.BuildOutputFolder(TestContext, jsonFile).EntryFile;
        var split = Path.GetFileNameWithoutExtension(jsonFile.FileName).Split("@");
        var resourceType = split[0].Replace("-", "/");
        var apiVersion = split[1];

        var response = tools.GetAzureResourceTypeSchema(resourceType, apiVersion);

        baselineFile.WriteToOutputFolder(response.Schema);
        baselineFile.ShouldHaveExpectedJsonValue();
    }

    [TestMethod]
    [EmbeddedFilesTestData(@"Files/GetAzResourceSchemaTrimmed/.*\.json")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public void GetAzureResourceTypeSchema_returns_trimmed_resource_schema(EmbeddedFile jsonFile)
    {
        var baselineFile = BaselineFolder.BuildOutputFolder(TestContext, jsonFile).EntryFile;
        var split = Path.GetFileNameWithoutExtension(jsonFile.FileName).Split("@");
        var resourceType = split[0].Replace("-", "/");
        var apiVersion = split[1];

        var response = tools.GetAzureResourceTypeSchema(resourceType, apiVersion, excludeDescriptions: true, excludeReadOnlyProperties: true);

        baselineFile.WriteToOutputFolder(response.Schema);
        baselineFile.ShouldHaveExpectedJsonValue();
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("latest-stable")]
    public void GetAzureResourceTypeSchema_resolves_versions_from_bundled_catalog(string? apiVersion)
    {
        const string resourceType = "Microsoft.KeyVault/vaults";

        var response = tools.GetAzureResourceTypeSchema(resourceType, apiVersion);

        using var schema = JsonDocument.Parse(response.Schema);
        var selectedResourceType = ResourceTypeReference.Parse(schema.RootElement.GetProperty("title").GetString()!);
        selectedResourceType.FormatType().Should().Be(resourceType);
        selectedResourceType.ApiVersion.Should().NotBeNullOrEmpty();
        tools.ListAzureResourceTypes("Microsoft.KeyVault").ResourceTypes.Should().Contain(selectedResourceType.FormatName());
        response.Schema.Should().Be(tools.GetAzureResourceTypeSchema(resourceType, selectedResourceType.ApiVersion).Schema);

        if (apiVersion == "latest-stable")
        {
            AzureResourceApiVersion.Parse(selectedResourceType.ApiVersion!).IsStable.Should().BeTrue();
        }
    }

    [TestMethod]
    [DataRow("2025-01-01-preview", new[] { "2023-01-01", "2024-01-01", "2025-01-01-preview" })]
    [DataRow("2025-01-01-preview", new[] { "2025-01-01-preview", "2024-01-01", "2023-01-01" })]
    [DataRow("2025-01-01", new[] { "2024-01-01-preview", "2025-01-01" })]
    [DataRow("2025-01-01", new[] { "2025-01-01", "2024-01-01" })]
    [DataRow("2024-01-01", new[] { "2024-01-01-preview", "2024-01-01" })]
    [DataRow("2024-01-01", new[] { "2024-01-01", "2024-01-01-preview" })]
    [DataRow("2025-01-01-alpha", new[] { "2024-01-01", "2025-01-01-alpha" })]
    [DataRow("2025-01-01-privatepreview", new[] { "2024-01-01", "2025-01-01-privatepreview" })]
    [DataRow("2025-01-01-preview", new[] { "2024-01-01-preview", "2025-01-01-preview" })]
    [DataRow("2025-01-01-alpha", new[] { "2025-01-01-preview", "2025-01-01-beta", "2025-01-01-alpha" })]
    [DataRow("2025-01-01-PREVIEW", new[] { "2024-01-01-preview", "2025-01-01-PREVIEW" })]
    [DataRow("2024-01-01", new[] { "v1.0", "2024-01-01" })]
    public void GetAzureResourceTypeSchema_selects_latest_version(string expectedVersion, string[] versions)
    {
        using var services = GetServiceProviderWithResourceTypes([.. versions.Select(version => $"Test.Rp/widgets@{version}")]);
        var catalogTools = services.GetRequiredService<BicepTools>();

        var response = catalogTools.GetAzureResourceTypeSchema("Test.Rp/widgets");

        response.Schema.Should().Be(catalogTools.GetAzureResourceTypeSchema("Test.Rp/widgets", expectedVersion).Schema);
        response.Schema.Should().Be(catalogTools.GetAzureResourceTypeSchema("Test.Rp/widgets", null).Schema);
        using var schema = JsonDocument.Parse(response.Schema);
        schema.RootElement.GetProperty("title").GetString().Should().Be($"Test.Rp/widgets@{expectedVersion}");
        schema.RootElement.GetProperty("x-bicep-resource-functions").EnumerateArray().Should()
            .ContainSingle().Which.GetProperty("apiVersion").GetString().Should().Be(expectedVersion);
    }

    [TestMethod]
    [DataRow("latest-stable", "2024-01-01", new[] { "2023-01-01", "2024-01-01", "2025-01-01-preview" })]
    [DataRow("LATEST-STABLE", "2024-01-01", new[] { "2025-01-01-privatepreview", "2024-01-01", "2025-01-01-alpha" })]
    [DataRow("Latest-Stable", "2024-01-01", new[] { "2024-01-01-preview", "2024-01-01", "v1.0" })]
    [DataRow("latest-stable", "2025-01-01", new[] { "2024-01-01", "2025-01-01" })]
    public void GetAzureResourceTypeSchema_selects_latest_stable_version(string selector, string expectedVersion, string[] versions)
    {
        using var services = GetServiceProviderWithResourceTypes([.. versions.Select(version => $"Test.Rp/widgets@{version}")]);
        var catalogTools = services.GetRequiredService<BicepTools>();

        var response = catalogTools.GetAzureResourceTypeSchema("Test.Rp/widgets", selector);

        response.Schema.Should().Be(catalogTools.GetAzureResourceTypeSchema("Test.Rp/widgets", expectedVersion).Schema);
        using var schema = JsonDocument.Parse(response.Schema);
        schema.RootElement.GetProperty("title").GetString().Should().Be($"Test.Rp/widgets@{expectedVersion}");
        schema.RootElement.GetProperty("x-bicep-resource-functions").EnumerateArray().Should()
            .ContainSingle().Which.GetProperty("apiVersion").GetString().Should().Be(expectedVersion);
    }

    [TestMethod]
    [DataRow("2023-01-01")]
    [DataRow("2025-01-01-preview")]
    [DataRow("v1.0")]
    public void GetAzureResourceTypeSchema_honors_explicit_version(string apiVersion)
    {
        using var services = GetServiceProviderWithResourceTypes(
            "Test.Rp/widgets@2023-01-01",
            "Test.Rp/widgets@2024-01-01",
            "Test.Rp/widgets@2025-01-01-preview",
            "Test.Rp/widgets@v1.0");
        var catalogTools = services.GetRequiredService<BicepTools>();

        var response = catalogTools.GetAzureResourceTypeSchema("Test.Rp/widgets", apiVersion);

        using var schema = JsonDocument.Parse(response.Schema);
        schema.RootElement.GetProperty("title").GetString().Should().Be($"Test.Rp/widgets@{apiVersion}");
        schema.RootElement.GetProperty("x-bicep-resource-functions").EnumerateArray().Should()
            .ContainSingle().Which.GetProperty("apiVersion").GetString().Should().Be(apiVersion);
    }

    [TestMethod]
    [DataRow("test.rp/WIDGETS", null, "Test.Rp/widgets@2024-01-01")]
    [DataRow("test.rp/WIDGETS", "latest-stable", "Test.Rp/widgets@2024-01-01")]
    [DataRow("test.rp/WIDGETS", "2024-01-01", "Test.Rp/widgets@2024-01-01")]
    [DataRow("Test.Rp/widgets/children", null, "Test.Rp/widgets/children@2025-01-01")]
    [DataRow("Test.Rp/widgets/children", "latest-stable", "Test.Rp/widgets/children@2025-01-01")]
    public void GetAzureResourceTypeSchema_matches_full_type_case_insensitively(string resourceType, string? apiVersion, string expectedTitle)
    {
        using var services = GetServiceProviderWithResourceTypes(
            "Test.Rp/widgets@2024-01-01",
            "Test.Rp/widgets/children@2025-01-01",
            "Test.Rp/widgetsOther@2026-01-01",
            "Other.Rp/widgets@2027-01-01");

        var response = services.GetRequiredService<BicepTools>().GetAzureResourceTypeSchema(resourceType, apiVersion);

        using var schema = JsonDocument.Parse(response.Schema);
        schema.RootElement.GetProperty("title").GetString().Should().Be(expectedTitle);
    }

    [TestMethod]
    [DataRow("Unknown.Rp/widgets")]
    [DataRow("Test.Rp/missing")]
    [DataRow("Test.Rp/widget")]
    [DataRow("Test.Rp")]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("Test.Rp//widgets")]
    [DataRow("Test.Rp/widgets@2024-01-01")]
    public void GetAzureResourceTypeSchema_rejects_invalid_resource_type(string resourceType)
    {
        using var services = GetServiceProviderWithResourceTypes("Test.Rp/widgets@2024-01-01");
        var catalogTools = services.GetRequiredService<BicepTools>();

        var defaultVersionAction = () => catalogTools.GetAzureResourceTypeSchema(resourceType);
        var stableVersionAction = () => catalogTools.GetAzureResourceTypeSchema(resourceType, "latest-stable");
        var explicitVersionAction = () => catalogTools.GetAzureResourceTypeSchema(resourceType, "2024-01-01");

        defaultVersionAction.Should().Throw<InvalidDataException>()
            .WithMessage($"Resource type {resourceType} not found in Bicep's bundled Azure resource type catalog.");
        stableVersionAction.Should().Throw<InvalidDataException>()
            .WithMessage($"Resource type {resourceType} not found in Bicep's bundled Azure resource type catalog.");
        explicitVersionAction.Should().Throw<InvalidDataException>()
            .WithMessage($"Resource type {resourceType} not found in Bicep's bundled Azure resource type catalog.");
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("latest")]
    [DataRow("latest-preview")]
    [DataRow("latest-stable ")]
    [DataRow("2099-01-01")]
    [DataRow("2024-01-01 ")]
    public void GetAzureResourceTypeSchema_rejects_unsupported_explicit_version(string apiVersion)
    {
        using var services = GetServiceProviderWithResourceTypes(
            "Test.Rp/widgets@2023-01-01",
            "Test.Rp/widgets@2024-01-01-preview",
            "Test.Rp/widgets@v1.0",
            "Test.Rp/widgets@2025-01-01-preview",
            "Test.Rp/widgets@2024-01-01",
            "Test.Rp/widgets/children@2026-01-01",
            "Other.Rp/widgets@2027-01-01");

        var action = () => services.GetRequiredService<BicepTools>().GetAzureResourceTypeSchema("Test.Rp/widgets", apiVersion);

        action.Should().Throw<InvalidDataException>()
            .WithMessage($"Resource type Test.Rp/widgets with API version {apiVersion} not found. " +
                "Valid options: null (latest), \"latest-stable\", \"2025-01-01-preview\", \"2024-01-01\", \"2024-01-01-preview\", \"2023-01-01\", \"v1.0\".");
    }

    [TestMethod]
    [DataRow("latest-stable", "No stable API versions found for resource type Test.Rp/widgets in Bicep's bundled Azure resource type catalog.")]
    [DataRow("LATEST-STABLE", "No stable API versions found for resource type Test.Rp/widgets in Bicep's bundled Azure resource type catalog.")]
    [DataRow("latest-preview", "Resource type Test.Rp/widgets with API version latest-preview not found.")]
    public void GetAzureResourceTypeSchema_lists_only_usable_options_for_preview_only_resources(string apiVersion, string expectedError)
    {
        using var services = GetServiceProviderWithResourceTypes(
            "Test.Rp/widgets@2024-01-01-preview",
            "Test.Rp/widgets@2025-01-01-privatepreview",
            "Test.Rp/widgets@2025-01-01-alpha",
            "Test.Rp/widgets/children@2026-01-01");

        var action = () => services.GetRequiredService<BicepTools>().GetAzureResourceTypeSchema("Test.Rp/widgets", apiVersion);

        action.Should().Throw<InvalidDataException>()
            .WithMessage($"{expectedError} Valid options: null (latest), \"2025-01-01-alpha\", \"2025-01-01-privatepreview\", \"2024-01-01-preview\".");
    }

    [TestMethod]
    [DataRow("Test.Rp/widgets@v1.0", null, "supported", "\"v1.0\"")]
    [DataRow("Test.Rp/widgets@v1.0", "latest-stable", "stable", "\"v1.0\"")]
    [DataRow("Test.Rp/widgets", null, "supported", "none")]
    [DataRow("Test.Rp/widgets", "latest-stable", "stable", "none")]
    public void GetAzureResourceTypeSchema_rejects_catalog_without_usable_versions(string resourceTypeName, string? apiVersion, string versionKind, string expectedOptions)
    {
        using var services = GetServiceProviderWithResourceTypes(resourceTypeName);

        var action = () => services.GetRequiredService<BicepTools>().GetAzureResourceTypeSchema("Test.Rp/widgets", apiVersion);

        action.Should().Throw<InvalidDataException>()
            .WithMessage($"No {versionKind} API versions found for resource type Test.Rp/widgets in Bicep's bundled Azure resource type catalog. Valid options: {expectedOptions}.");
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void GetAzureResourceTypeSchema_preserves_trim_options_with_default_version(bool excludeDescriptions, bool excludeReadOnlyProperties)
    {
        using var services = GetServiceProviderWithResourceTypes("Test.Rp/widgets@2024-01-01");
        var catalogTools = services.GetRequiredService<BicepTools>();

        var response = catalogTools.GetAzureResourceTypeSchema("Test.Rp/widgets",
            excludeDescriptions: excludeDescriptions, excludeReadOnlyProperties: excludeReadOnlyProperties);

        response.Schema.Should().Be(catalogTools.GetAzureResourceTypeSchema(
            "Test.Rp/widgets", "2024-01-01", excludeDescriptions, excludeReadOnlyProperties).Schema);
        response.Schema.Should().Be(catalogTools.GetAzureResourceTypeSchema(
            "Test.Rp/widgets", "latest-stable", excludeDescriptions, excludeReadOnlyProperties).Schema);
        using var schema = JsonDocument.Parse(response.Schema);
        schema.RootElement.GetProperty("title").GetString().Should().Be("Test.Rp/widgets@2024-01-01");
        var properties = schema.RootElement.GetProperty("properties");
        properties.TryGetProperty("id", out _).Should().Be(!excludeReadOnlyProperties);
        properties.GetProperty("name").TryGetProperty("description", out _).Should().Be(!excludeDescriptions);
    }

    [TestMethod]
    public async Task ListExtensionResourceTypes_returns_graph_resource_types()
    {
        var response = await tools.ListExtensionResourceTypes("br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:1.0.0");
        var result = response.ResourceTypes;

        result.Should().BeEquivalentTo([
            "Microsoft.Graph/applications@v1.0",
            "Microsoft.Graph/applications/federatedIdentityCredentials@v1.0",
            "Microsoft.Graph/appRoleAssignedTo@v1.0",
            "Microsoft.Graph/groups@v1.0",
            "Microsoft.Graph/oauth2PermissionGrants@v1.0",
            "Microsoft.Graph/servicePrincipals@v1.0",
            "Microsoft.Graph/users@v1.0",
        ]);
    }

    [TestMethod]
    [EmbeddedFilesTestData(@"Files/GetExtensionResourceSchema/.*\.json")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public async Task GetExtensionResourceTypeSchema_returns_resource_schema(EmbeddedFile jsonFile)
    {
        var baselineFile = BaselineFolder.BuildOutputFolder(TestContext, jsonFile).EntryFile;
        var fileName = Path.GetFileNameWithoutExtension(jsonFile.FileName);

        // File name format: {repoPath}#{tag}#{resourceType}@{apiVersion}
        // e.g., microsoftgraph-v1.0#1.0.0#Microsoft.Graph-applications@v1.0
        var parts = fileName.Split('#');
        var repoPath = "bicep/extensions/" + parts[0].Replace("-", "/");
        var tag = parts[1];
        var resourcePart = parts[2];

        var extensionReference = $"br:{LanguageConstants.BicepPublicMcrRegistry}/{repoPath}:{tag}";

        var atIndex = resourcePart.LastIndexOf('@');
        var resourceType = resourcePart[..atIndex].Replace("-", "/");
        var apiVersion = resourcePart[(atIndex + 1)..];

        var response = await tools.GetExtensionResourceTypeSchema(extensionReference, resourceType, apiVersion);

        baselineFile.WriteToOutputFolder(response.Schema);
        baselineFile.ShouldHaveExpectedJsonValue();
    }

    [TestMethod]
    [EmbeddedFilesTestData(@"Files/GetExtensionResourceSchemaTrimmed/.*\.json")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public async Task GetExtensionResourceTypeSchema_returns_trimmed_resource_schema(EmbeddedFile jsonFile)
    {
        var baselineFile = BaselineFolder.BuildOutputFolder(TestContext, jsonFile).EntryFile;
        var fileName = Path.GetFileNameWithoutExtension(jsonFile.FileName);

        // File name format: {repoPath}#{tag}#{resourceType}@{apiVersion}
        // e.g., microsoftgraph-v1.0#1.0.0#Microsoft.Graph-applications@v1.0
        var parts = fileName.Split('#');
        var repoPath = "bicep/extensions/" + parts[0].Replace("-", "/");
        var tag = parts[1];
        var resourcePart = parts[2];

        var extensionReference = $"br:{LanguageConstants.BicepPublicMcrRegistry}/{repoPath}:{tag}";

        var atIndex = resourcePart.LastIndexOf('@');
        var resourceType = resourcePart[..atIndex].Replace("-", "/");
        var apiVersion = resourcePart[(atIndex + 1)..];

        var response = await tools.GetExtensionResourceTypeSchema(extensionReference, resourceType, apiVersion, excludeDescriptions: true, excludeReadOnlyProperties: true);

        baselineFile.WriteToOutputFolder(response.Schema);
        baselineFile.ShouldHaveExpectedJsonValue();
    }

    [TestMethod]
    public void GetBicepBestPractices_returns_best_practices_markdown()
    {
        var response = tools.GetBicepBestPractices();

        var expectedBestPractices = BinaryData.FromStream(typeof(BicepTools).Assembly.GetManifestResourceStream("Files/bestpractices.md")!).ToString();
        response.Content.Should().Be(expectedBestPractices);

        // Update this if the file content changes - it's just here as a sanity check to make sure we're decoding the content correctly
        expectedBestPractices.Should().StartWith("# Bicep best-practices");
    }

    [TestMethod]
    public async Task ListWellKnownExtensions_returns_extensions()
    {
        var response = await tools.ListWellKnownExtensions();
        var extensions = response.Extensions;

        extensions.Should().NotBeEmpty();
        extensions.Should().Contain(e => e.Name == "MicrosoftGraphBeta");
        extensions.Should().Contain(e => e.Name == "MicrosoftGraph");

        extensions.Should().AllSatisfy(ext =>
        {
            ext.Name.Should().NotBeNullOrWhiteSpace();
            ext.Description.Should().NotBeNullOrWhiteSpace();
            ext.OciReference.Should().StartWith("br:");
            ext.AvailableTags.Should().NotBeEmpty($"extension '{ext.Name}' should have at least one available tag");
        });
    }

    [TestMethod]
    public async Task ListAvmMetadata_returns_avm_metadata()
    {
        var response = await tools.ListAvmMetadata();
        var modules = response.Modules;

        modules.Should().HaveCountGreaterThan(200, "response should have more than 200 modules");

        modules.Should().AllSatisfy(module =>
        {
            // Verify module path
            module.ModulePath.Should().StartWith("avm/", "All modules should start with avm/");

            // Verify description
            module.Description.Should().NotBeNullOrWhiteSpace("Description should not be empty");

            // Verify versions
            module.Versions.Should().NotBeEmpty("Should have at least one version");
            module.Versions.Should().AllSatisfy(v => v.Should().MatchRegex(@"^\d+\.\d+\.\d+", "Each version should follow semantic versioning"));

            // Verify documentation URI
            if (module.DocumentationUri is not null)
            {
                module.DocumentationUri.Should().StartWith("https://", "Documentation URI should be a valid URL");
            }
        });
    }
}

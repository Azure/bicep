// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Bicep.McpServer.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class AzureResourceSchemaServerTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    [DataRow(false, null, "2025-01-01-preview")]
    [DataRow(true, null, "2025-01-01-preview")]
    [DataRow(true, "latest-stable", "2024-01-01")]
    [DataRow(true, "2023-01-01", "2023-01-01")]
    public async Task GetAzureResourceTypeSchema_accepts_optional_api_version(bool includeApiVersion, string? apiVersion, string expectedVersion)
    {
        await using var helper = await McpServerHelper.StartServer(TestContext, services =>
            services.AddSingleton(ResourceTypeCatalogHelper.CreateTypeLoader(
                "Test.Rp/widgets@2023-01-01",
                "Test.Rp/widgets@2024-01-01",
                "Test.Rp/widgets@2025-01-01-preview")));
        var arguments = new Dictionary<string, object?>
        {
            ["resourceType"] = "Test.Rp/widgets",
        };
        if (includeApiVersion)
        {
            arguments["apiVersion"] = apiVersion;
        }

        var response = await helper.Client.CallToolAsync("get_azure_resource_type_schema", arguments);

        response.IsError.Should().NotBe(true);
        response.StructuredContent.Should().NotBeNull();
        var content = response.Content.Should().ContainSingle().Which.Should().BeOfType<TextContentBlock>().Subject;
        using var result = JsonDocument.Parse(content.Text);
        using var schema = JsonDocument.Parse(result.RootElement.GetProperty("schema").GetString()!);
        schema.RootElement.GetProperty("title").GetString().Should().Be($"Test.Rp/widgets@{expectedVersion}");
    }

    [TestMethod]
    [DataRow("Test.Rp/missing", null)]
    [DataRow("Test.Rp/widgets", "2099-01-01")]
    public async Task GetAzureResourceTypeSchema_returns_error_for_invalid_inputs(string resourceType, string? apiVersion)
    {
        await using var helper = await McpServerHelper.StartServer(TestContext, services =>
            services.AddSingleton(ResourceTypeCatalogHelper.CreateTypeLoader("Test.Rp/widgets@2024-01-01")));
        var arguments = new Dictionary<string, object?>
        {
            ["resourceType"] = resourceType,
        };
        if (apiVersion is not null)
        {
            arguments["apiVersion"] = apiVersion;
        }

        var response = await helper.Client.CallToolAsync("get_azure_resource_type_schema", arguments);

        response.IsError.Should().BeTrue();
        response.Content.Should().ContainSingle().Which.Should().BeOfType<TextContentBlock>()
            .Which.Text.Should().Be(resourceType != "Test.Rp/widgets"
                ? $"Error: Resource type {resourceType} not found in Bicep's bundled Azure resource type catalog."
                : $"Error: Resource type {resourceType} with API version {apiVersion} not found. Valid options: null (latest), \"latest-stable\", \"2024-01-01\".");
    }

    [TestMethod]
    public async Task GetAzureResourceTypeSchema_returns_error_when_no_stable_version_exists()
    {
        await using var helper = await McpServerHelper.StartServer(TestContext, services =>
            services.AddSingleton(ResourceTypeCatalogHelper.CreateTypeLoader("Test.Rp/widgets@2024-01-01-preview")));

        var response = await helper.Client.CallToolAsync("get_azure_resource_type_schema", new Dictionary<string, object?>
        {
            ["resourceType"] = "Test.Rp/widgets",
            ["apiVersion"] = "latest-stable",
        });

        response.IsError.Should().BeTrue();
        response.Content.Should().ContainSingle().Which.Should().BeOfType<TextContentBlock>()
            .Which.Text.Should().Be("Error: No stable API versions found for resource type Test.Rp/widgets in Bicep's bundled Azure resource type catalog. " +
                "Valid options: null (latest), \"2024-01-01-preview\".");
    }
}
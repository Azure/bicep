// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
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

    [TestMethod]
    public void ListAzResourceTypesForProvider_returns_list_of_resource_types()
    {
        var response = tools.ListAzResourceTypesForProvider("Microsoft.Compute");
        var result = response.ResourceTypes;

        result.Should().HaveCountGreaterThan(700);
        result.Should().AllSatisfy(x => x.Split('/').First().Equals("Microsoft.Compute", StringComparison.OrdinalIgnoreCase))
            .And.AllSatisfy(x => x.Contains('@'));
    }

    [TestMethod]
    public void ListAzResourceTypesForProvider_returns_empty_array_for_invalid_provider()
    {
        var response = tools.ListAzResourceTypesForProvider("Invalid.Provider");
        response.ResourceTypes.Should().BeEmpty();
    }

    [TestMethod]
    [EmbeddedFilesTestData(@"Files/GetAzResourceSchema/.*\.json")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public void GetAzResourceTypeSchema_returns_resource_schema(EmbeddedFile jsonFile)
    {
        var baselineFile = BaselineFolder.BuildOutputFolder(TestContext, jsonFile).EntryFile;
        var split = Path.GetFileNameWithoutExtension(jsonFile.FileName).Split("@");
        var resourceType = split[0].Replace("-", "/");
        var apiVersion = split[1];

        var response = tools.GetAzResourceTypeSchema(resourceType, apiVersion);

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

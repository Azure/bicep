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
        var result = response.Split("\n").ToImmutableArray();

        result.Should().HaveCountGreaterThan(700);
        result.Should().AllSatisfy(x => x.Split('/').First().Equals("Microsoft.Compute", StringComparison.OrdinalIgnoreCase))
            .And.AllSatisfy(x => x.Contains('@'));
    }

    [TestMethod]
    public void ListAzResourceTypesForProvider_returns_empty_string_for_invalid_provider()
    {
        var response = tools.ListAzResourceTypesForProvider("Invalid.Provider");
        response.Should().BeEmpty();
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

        baselineFile.WriteToOutputFolder(response);
        baselineFile.ShouldHaveExpectedJsonValue();
    }

    [TestMethod]
    public void GetBicepBestPractices_returns_best_practices_markdown()
    {
        var response = tools.GetBicepBestPractices();

        var expectedBestPractices = BinaryData.FromStream(typeof(BicepTools).Assembly.GetManifestResourceStream("Files/bestpractices.md")!).ToString();
        response.Should().Be(expectedBestPractices);

        // Update this if the file content changes - it's just here as a sanity check to make sure we're decoding the content correctly
        expectedBestPractices.Should().StartWith("# Bicep best-practices");
    }

    [TestMethod]
    public async Task ListAvmMetadata_returns_avm_metadata()
    {
        var response = await tools.ListAvmMetadata();
        var lines = response.ReplaceLineEndings().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCountGreaterThan(200, "response should have more than 200 lines");

        lines.Should().AllSatisfy(line =>
        {
            // Parse the line format: "Module: {module}; Description: {description}; Versions: [{versions}]; Doc URI: {docUri}"
            var parts = line.Split("; ");
            parts.Should().HaveCount(4, $"Each line should have 4 parts separated by '; ', but got: {line}");

            // Extract Module
            var modulePart = parts[0];
            modulePart.Should().StartWith("Module: ");
            var module = modulePart["Module: ".Length..];
            module.Should().StartWith("avm/");

            // Extract Description
            var descriptionPart = parts[1];
            descriptionPart.Should().StartWith("Description: ");
            var description = descriptionPart["Description: ".Length..];
            description.Should().NotBeNullOrWhiteSpace("Description should not be empty");

            // Extract Versions
            var versionsPart = parts[2];
            versionsPart.Should().StartWith("Versions: [");
            versionsPart.Should().EndWith("]");
            var versionsContent = versionsPart.Substring("Versions: [".Length, versionsPart.Length - "Versions: [".Length - 1);
            versionsContent.Should().NotBeNullOrWhiteSpace("Versions should not be empty");
            // Versions should be comma-separated
            var versions = versionsContent.Split(", ");
            versions.Should().NotBeEmpty("Should have at least one version");
            versions.Should().AllSatisfy(v => v.Should().MatchRegex(@"^\d+\.\d+\.\d+", "Each version should follow semantic versioning"));

            // Extract Doc URI
            var docUriPart = parts[3];
            docUriPart.Should().StartWith("Doc URI: ");
            var docUri = docUriPart["Doc URI: ".Length..];
            docUri.Should().StartWith("https://");
        });
    }
}

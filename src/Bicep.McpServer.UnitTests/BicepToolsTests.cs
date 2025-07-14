// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class BicepToolsTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    private static IServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddBicepMcpServer();

        return services.BuildServiceProvider();
    }

    private readonly BicepTools tools = GetServiceProvider().GetRequiredService<BicepTools>();

    public static string GetText(CallToolResult result)
        => (result.Content[0] as TextContentBlock)!.Text;

    [TestMethod]
    public void ListAzResourceTypesForProvider_returns_list_of_resource_types()
    {
        var response = tools.ListAzResourceTypesForProvider("Microsoft.Compute");
        var result = GetText(response).Split("\n").ToImmutableArray();

        result.Should().HaveCountGreaterThan(700);
        result.Should().AllSatisfy(x => x.Split('/').First().Equals("Microsoft.Compute", StringComparison.OrdinalIgnoreCase))
            .And.AllSatisfy(x => x.Contains('@'));
    }

    [TestMethod]
    public void ListAzResourceTypesForProvider_returns_empty_string_for_invalid_provider()
    {
        var response = tools.ListAzResourceTypesForProvider("Invalid.Provider");
        GetText(response).Should().BeEmpty();
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

        baselineFile.WriteToOutputFolder(GetText(response));
        baselineFile.ShouldHaveExpectedJsonValue();
    }

    [TestMethod]
    public void GetBicepBestPractices_returns_best_practices_markdown()
    {
        var response = tools.GetBicepBestPractices();

        var expectedBestPractices = BinaryData.FromStream(typeof(BicepTools).Assembly.GetManifestResourceStream("Files/bestpractices.md")!).ToString();
        GetText(response).Should().Be(expectedBestPractices);

        // Update this if the file content changes - it's just here as a sanity check to make sure we're decoding the content correctly
        expectedBestPractices.Should().StartWith("# Bicep best-practices");
    }

    [TestMethod]
    public async Task GetBicepFileDiagnostics_returns_diagnostics()
    {
        var response = await tools.GetBicepFileDiagnostics("""
        var foo string = 123
        """);

        GetText(response).Should().EqualIgnoringNewlines("""
        dummy:///DUMMY(1,5) : Warning no-unused-vars: Variable "foo" is declared but never used. [https://aka.ms/bicep/linter-diagnostics#no-unused-vars]
        dummy:///DUMMY(1,18) : Error BCP033: Expected a value of type "string" but the provided value is of type "123". [https://aka.ms/bicep/core-diagnostics#BCP033]
        
        """);
    }
}

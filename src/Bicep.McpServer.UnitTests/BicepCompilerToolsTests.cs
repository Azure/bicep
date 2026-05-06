// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests.Utils;
using Bicep.McpServer.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class BicepCompilerToolsTests
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

    private readonly BicepCompilerTools tools = GetServiceProvider().GetRequiredService<BicepCompilerTools>();

    [TestMethod]
    public async Task FormatBicepFile_returns_formatted_bicep_content()
    {
        var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", """
            param          foo          string
            """);

        var response = await tools.FormatBicepFile(bicepFilePath);

        response.Content.Should().Contain("param foo string");
    }

    [TestMethod]
    public async Task GetFileReferences_returns_referenced_files()
    {
        var outputFolder = FileHelper.SaveResultFiles(TestContext, [
            new("main.bicep", """
                param location string
                """),
            new("main.bicepparam", """
                using 'main.bicep'

                param location = loadTextContent('location.txt')
                """),
            new("location.txt", "westus"),
            new("bicepconfig.json", """
                {
                }
                """),
        ]);

        var response = await tools.GetFileReferences(Path.Combine(outputFolder, "main.bicepparam"));
        response.FileUris.Select(u => u.AbsoluteUri.Split('/').Last()).Should().BeEquivalentTo([
            "main.bicep",
            "main.bicepparam",
            "bicepconfig.json",
            "location.txt",
        ]);
    }

    [TestMethod]
    public async Task BuildBicep_returns_compiled_template()
    {
        var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", """
            param location string = 'westus'
            output loc string = location
            """);

        var response = await tools.BuildBicep(bicepFilePath);

        response.Success.Should().BeTrue();
        response.Template.Should().NotBeNullOrEmpty();
        response.Template.Should().Contain("\"$schema\"");
        response.Diagnostics.Should().NotContain(x => x.Level == "Error");
    }

    [TestMethod]
    public async Task BuildBicep_returns_diagnostics_on_error()
    {
        var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", """
            var foo string = 123
            """);

        var response = await tools.BuildBicep(bicepFilePath);

        response.Success.Should().BeFalse();
        response.Template.Should().BeNull();
        response.Diagnostics.Should().HaveCountGreaterThanOrEqualTo(2);
        var diagnostic = response.Diagnostics.Should().ContainSingle(x => x.Code == "BCP033").Subject;
        diagnostic.Level.Should().Be("Error");
    }

    [TestMethod]
    public async Task BuildBicepparam_returns_compiled_parameters()
    {
        var outputFolder = FileHelper.SaveResultFiles(TestContext, [
            new("main.bicep", """
                param location string
                output loc string = location
                """),
            new("main.bicepparam", """
                using 'main.bicep'

                param location = 'westus'
                """),
        ]);

        var response = await tools.BuildBicepparam(Path.Combine(outputFolder, "main.bicepparam"));

        response.Success.Should().BeTrue();
        response.Parameters.Should().NotBeNullOrEmpty();
        response.Parameters.Should().Contain("\"$schema\"");
        response.Template.Should().NotBeNullOrEmpty();
        response.Template.Should().Contain("\"$schema\"");
        response.Diagnostics.Should().NotContain(x => x.Level == "Error");
    }

    [TestMethod]
    public async Task BuildBicepparam_returns_diagnostics_on_error()
    {
        var outputFolder = FileHelper.SaveResultFiles(TestContext, [
            new("main.bicep", """
                param location string
                """),
            new("main.bicepparam", """
                using 'main.bicep'

                param location = 123
                """),
        ]);

        var response = await tools.BuildBicepparam(Path.Combine(outputFolder, "main.bicepparam"));

        response.Success.Should().BeFalse();
        response.Parameters.Should().BeNull();
        response.Diagnostics.Should().HaveCountGreaterThanOrEqualTo(1);
        response.Diagnostics.Should().Contain(x => x.Level == "Error");
    }
}

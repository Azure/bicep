// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
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

    private static BicepCompilerTools CreateTools(MockFileSystemTestFileSet files)
    {
        var services = new ServiceCollection();
        services.AddBicepMcpServer();
        services
            .WithFileSystem(files.FileSystem)
            .WithFileExplorer(files.FileExplorer);

        return services.BuildServiceProvider().GetRequiredService<BicepCompilerTools>();
    }

    [TestMethod]
    public async Task FormatBicepFile_returns_formatted_bicep_content()
    {
        var files = MockFileSystemTestFileSet.Create(("main.bicep", """
            param          foo          string
            """));

        var response = await CreateTools(files).FormatBicepFile(files.GetUri("main.bicep").GetFilePath());

        response.Content.Should().Contain("param foo string");
    }

    [TestMethod]
    public async Task GetFileReferences_returns_referenced_files()
    {
        var files = MockFileSystemTestFileSet.Create(
            ("main.bicep", """
                param location string
                """),
            ("main.bicepparam", """
                using 'main.bicep'

                param location = loadTextContent('location.txt')
                """),
            ("location.txt", "westus"),
            ("bicepconfig.json", """
                {
                }
                """));

        var response = await CreateTools(files).GetFileReferences(files.GetUri("main.bicepparam").GetFilePath());
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
        var files = MockFileSystemTestFileSet.Create(("main.bicep", """
            param location string = 'westus'
            output loc string = location
            """));

        var response = await CreateTools(files).BuildBicep(files.GetUri("main.bicep").GetFilePath());

        response.Success.Should().BeTrue();
        response.Template.Should().NotBeNullOrEmpty();
        response.Template.Should().Contain("\"$schema\"");
        response.Diagnostics.Should().NotContain(x => x.Level == "Error");
    }

    [TestMethod]
    public async Task BuildBicep_returns_diagnostics_on_error()
    {
        var files = MockFileSystemTestFileSet.Create(("main.bicep", """
            var foo string = 123
            """));

        var response = await CreateTools(files).BuildBicep(files.GetUri("main.bicep").GetFilePath());

        response.Success.Should().BeFalse();
        response.Template.Should().BeNull();
        response.Diagnostics.Should().HaveCountGreaterThanOrEqualTo(2);
        var diagnostic = response.Diagnostics.Should().ContainSingle(x => x.Code == "BCP033").Subject;
        diagnostic.Level.Should().Be("Error");
    }

    [TestMethod]
    public async Task BuildBicepparam_returns_compiled_parameters()
    {
        var files = MockFileSystemTestFileSet.Create(
            ("main.bicep", """
                param location string
                output loc string = location
                """),
            ("main.bicepparam", """
                using 'main.bicep'

                param location = 'westus'
                """));

        var response = await CreateTools(files).BuildBicepparam(files.GetUri("main.bicepparam").GetFilePath());

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
        var files = MockFileSystemTestFileSet.Create(
            ("main.bicep", """
                param location string
                """),
            ("main.bicepparam", """
                using 'main.bicep'

                param location = 123
                """));

        var response = await CreateTools(files).BuildBicepparam(files.GetUri("main.bicepparam").GetFilePath());

        response.Success.Should().BeFalse();
        response.Parameters.Should().BeNull();
        response.Diagnostics.Should().HaveCountGreaterThanOrEqualTo(1);
        response.Diagnostics.Should().Contain(x => x.Level == "Error");
    }
}

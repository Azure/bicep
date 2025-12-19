// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
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
    public async Task GetBicepFileDiagnostics_returns_list_of_diagnostics()
    {
        var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", """
            var foo string = 123
            """);

        var response = await tools.GetBicepFileDiagnostics(bicepFilePath);

        response.Should().HaveCountGreaterThanOrEqualTo(2);
        var diagnostic = response.Should().ContainSingle(x => x.Code == "BCP033").Subject;
        diagnostic.FileUri.Should().Be(IOUri.FromFilePath(bicepFilePath).ToUri());
        diagnostic.Level.Should().Be("Error");
        diagnostic.Message.Should().Be("Expected a value of type \"string\" but the provided value is of type \"123\".");
        diagnostic.Position.Should().Be(17);
        diagnostic.Length.Should().Be(3);
        diagnostic.DocumentationUri.Should().Be(new Uri("https://aka.ms/bicep/core-diagnostics#BCP033"));
    }

    [TestMethod]
    public async Task FormatBicepFile_returns_formatted_bicep_content()
    {
        var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", """
            param          foo          string
            """);

        var response = await tools.FormatBicepFile(bicepFilePath);

        response.Content.Should().Contain("param foo string");
    }
}

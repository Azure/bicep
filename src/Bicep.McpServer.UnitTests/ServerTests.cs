// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.McpServer.Core;
using Bicep.McpServer.UnitTests.Helpers;
using FluentAssertions;
using Humanizer;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class ServerTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    [EmbeddedFilesTestData(@"Files/ServerTests/tools.json")]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public async Task List_tools_returns_full_list_of_tools(EmbeddedFile toolsJson)
    {
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, toolsJson);
        var toolsJsonFile = baselineFolder.EntryFile;
        await using var helper = await McpServerHelper.StartServer(TestContext);

        var tools = await helper.Client.ListToolsAsync();

        var toolDefinitions = tools.Select(x => new
        {
            x.Name,
            Description = x.Description?.ReplaceLineEndings("\n"),
            x.JsonSchema,
            x.ReturnJsonSchema,
        })
            .OrderByAscending(x => x.Name)
            .ToImmutableArray();

        toolsJsonFile.WriteStjJsonToOutputFolder(toolDefinitions);
        toolsJsonFile.ShouldHaveExpectedJsonValue();
    }

    [TestMethod]
    public async Task Server_returns_instructions()
    {
        await using var helper = await McpServerHelper.StartServer(TestContext);

        var instructions = helper.Client.ServerInstructions;
        instructions.Should().StartWith("This MCP server exposes a number of tools to improve accuracy and quality when authoring Bicep files.");
    }

    [TestMethod]
    public async Task Error_filter_returns_error_information()
    {
        await using var helper = await McpServerHelper.StartServer(TestContext);

        var instructions = await helper.Client.CallToolAsync("get_bicep_file_diagnostics", new Dictionary<string, object?>
        {
            ["filePath"] = "nonexistent.bicep",
        });

        instructions.IsError.Should().BeTrue();
        instructions.Content.Should().ContainSingle()
            .Which.Should().BeOfType<TextContentBlock>()
            .Which.Text.Should().Be("Error: File path must be absolute.");
    }

    [TestMethod]
    public void McpServer_does_not_require_aspnetcore_runtime()
    {
        // The local MCP server is launched by the VS Code extension via "dotnet Azure.Bicep.McpServer.dll",
        // which only provides the base .NET runtime (not ASP.NET Core). Adding a FrameworkReference
        // to Microsoft.AspNetCore.App would cause a startup failure (exit code 150).
        // We check the McpServer executable's runtimeconfig (not Core, which is a library without a runtimeconfig).
        var mcpServerAssemblyDir = Path.GetDirectoryName(typeof(IServiceCollectionExtensions).Assembly.Location)!;
        var runtimeConfigPath = Path.Combine(mcpServerAssemblyDir, "Azure.Bicep.McpServer.runtimeconfig.json");

        File.Exists(runtimeConfigPath).Should().BeTrue($"runtimeconfig.json should exist at {runtimeConfigPath}");

        var runtimeConfig = File.ReadAllText(runtimeConfigPath);
        runtimeConfig.Should().NotContain("Microsoft.AspNetCore.App",
            "the local MCP server must not depend on the ASP.NET Core runtime to avoid breaking VS Code extension users who only have the base .NET runtime");
    }
}

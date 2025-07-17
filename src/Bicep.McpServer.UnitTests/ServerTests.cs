// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests.Assertions;
using Bicep.McpServer.UnitTests.Helpers;
using FluentAssertions;
using ModelContextProtocol.Client;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class ServerTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public async Task List_tools_returns_full_list_of_tools()
    {
        await using var helper = await McpServerHelper.StartServer(TestContext);

        var tools = await helper.Client.ListToolsAsync();

        tools.OrderBy(x => x.Name).Should().SatisfyRespectively(
            x => x.Name.Should().Be("get_az_resource_type_schema"),
            x => x.Name.Should().Be("get_bicep_best_practices"),
            x => x.Name.Should().Be("list_az_resource_types_for_provider"));
    }

    [TestMethod]
    public async Task Server_returns_instructions()
    {
        await using var helper = await McpServerHelper.StartServer(TestContext);

        var instructions = helper.Client.ServerInstructions;
        instructions.Should().StartWith("This MCP server exposes a number of tools to improve accuracy and quality when authoring Bicep files.");
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.McpServer.UnitTests;

[TestClass]
public class BicepToolsTests
{
    private readonly BicepTools tools = new ServiceCollection()
        .AddMcpDependencies()
        .AddSingleton<BicepTools>()
        .BuildServiceProvider()
        .GetRequiredService<BicepTools>();

    [TestMethod]
    public void ListAzResourceTypesForProvider_returns_list_of_resource_types()
    {
        var response = tools.ListAzResourceTypesForProvider("Microsoft.Compute");
        var result = response.Split("\n").ToImmutableArray();

        result.Should().HaveCountGreaterThan(10);
        result.Should().AllSatisfy(x => x.Split('/').First().Equals("Microsoft.Compute", StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void GetAzResourceTypeSchema_returns_resource_schema()
    {
        var response = tools.GetAzResourceTypeSchema("Microsoft.KeyVault/vaults", "2024-11-01");
        response.FromJson<JToken>().Should().NotBeNull();
    }
}

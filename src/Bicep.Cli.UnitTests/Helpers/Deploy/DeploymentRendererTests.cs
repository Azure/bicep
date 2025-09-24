// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Bicep.Cli.Helpers.Deploy;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spectre.Console;
using Spectre.Console.Testing;

namespace Bicep.Cli.UnitTests.Helpers.Deploy;

[TestClass]
public class DeploymentRendererTests
{
    public static DeploymentWrapperView Create(DateTime timeNow)
        => new(new(
            Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/foo",
            Name: "foo",
            State: "Succeeded",
            StartTime: timeNow.Subtract(TimeSpan.FromSeconds(0.5)),
            EndTime: null,
            Operations: [
                new(
                    Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/fooNested",
                    Name: "fooNested",
                    SymbolicName: null,
                    Type: "Microsoft.Resources/deployments",
                    State: "Succeeded",
                    StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                    EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.5)),
                    Error: null),
                new(
                    Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/fooNestedErr",
                    Name: "fooNestedErr",
                    SymbolicName: null,
                    Type: "Microsoft.Resources/deployments",
                    State: "Failed",
                    StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                    EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.5)),
                    Error: "Oh dear oh dear"),
                new(
                    Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Compute/virtualMachines/blah",
                    Name: "blah",
                    SymbolicName: null,
                    Type: "Microsoft.Compute/virtualMachines",
                    State: "Succeeded",
                    StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                    EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.7)),
                    Error: null),
            ],
            Error: null,
            Outputs: new Dictionary<string, JsonNode>
            {
                ["output1"] = JsonValue.Create("value1")!,
                ["output2"] = JsonValue.Create(42)!,
            }.ToImmutableDictionary()), null);


    [TestMethod]
    public void Basic_formatting_works()
    {
        var console = new TestConsole();
        var renderer = new DeploymentRenderer(console);

        var timeNow = DateTime.UtcNow;
        var table = new Table().RoundedBorder();
        DeploymentWrapperView view = Create(timeNow);
        var isInitialized = false;
        renderer.RenderDeployment(table, view, ref isInitialized);

        console.Write(table);

        console.Output.Should().Be("""
        ╭──────────────┬──────────┬─────────────────╮
        │ Resource     │ Duration │ Status          │
        ├──────────────┼──────────┼─────────────────┤
        │ blah         │ 0.3s     │ Succeeded       │
        │ fooNested    │ 0.5s     │ Succeeded       │
        │ fooNestedErr │ 0.5s     │ Oh dear oh dear │
        ╰──────────────┴──────────┴─────────────────╯

        """);
    }
}
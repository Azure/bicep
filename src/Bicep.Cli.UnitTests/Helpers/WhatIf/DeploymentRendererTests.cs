// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Text.Json.Nodes;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Bicep.Cli.Arguments;
using Bicep.Cli.Commands.Helpers.Deploy;
using Bicep.Cli.Helpers.WhatIf;
using Bicep.Cli.Services;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Common;
using Json.Path;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests.Helpers.WhatIf;

[TestClass]
public class DeploymentRendererTests
{
    [TestMethod]
    public void Basic_formatting_works()
    {
        var timeNow = DateTime.UtcNow;
        var result = DeploymentRenderer.Format(timeNow, [
            new(
                Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/foo",
                Name: "foo",
                State: "Running",
                StartTime: timeNow.Subtract(TimeSpan.FromSeconds(0.5)),
                EndTime: null,
                Operations: [],
                IsEntryPoint: true,
                Error: null,
                Outputs: ImmutableDictionary<string, JsonNode>.Empty),
        ], 0);

        AnsiHelper.ReplaceCodes(result).Should().EqualIgnoringNewlines("""
        [EraseLine]foo [Bold][Gray]Running[Reset] (0.5s)

        """);
    }

    [TestMethod]
    public void Content_is_replaced_if_line_count_provided()
    {
        var timeNow = DateTime.UtcNow;
        var result = DeploymentRenderer.Format(timeNow, [
            new(
                Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/foo",
                Name: "foo",
                State: "Running",
                StartTime: timeNow,
                EndTime: null,
                Operations: [],
                IsEntryPoint: true,
                Error: null,
                Outputs: ImmutableDictionary<string, JsonNode>.Empty),
        ], 2);

        AnsiHelper.ReplaceCodes(result).Should().EqualIgnoringNewlines("""
        [HideCursor][RewindLines(2)][EraseLine]foo [Bold][Gray]Running[Reset] (0.0s)
        [ShowCursor]
        """);
    }

    [TestMethod]
    public void Nested_deployments_are_visualized()
    {
        var timeNow = DateTime.UtcNow;
        var result = DeploymentRenderer.Format(timeNow, [
            new(
                Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/foo",
                Name: "foo",
                State: "Running",
                StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                EndTime: null,
                Operations: [
                    new(
                        Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/fooNested",
                        Name: "fooNested",
                        Type: "Microsoft.Resources/deployments",
                        State: "Succeeded",
                        StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                        EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.5)),
                        Error: null),
                    new(
                        Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/fooNestedErr",
                        Name: "fooNestedErr",
                        Type: "Microsoft.Resources/deployments",
                        State: "Failed",
                        StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                        EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.5)),
                        Error: "Oh dear oh dear"),
                    new(
                        Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Compute/virtualMachines/blah",
                        Name: "blah",
                        Type: "Microsoft.Compute/virtualMachines",
                        State: "Succeeded",
                        StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                        EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.7)),
                        Error: null),
                ],
                IsEntryPoint: true,
                Error: null,
                Outputs: ImmutableDictionary<string, JsonNode>.Empty),
            new(
                Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/fooNested",
                Name: "fooNested",
                State: "Succeeded",
                StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.5)),
                Operations: [
                    new(
                        Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Compute/virtualMachines/blah2",
                        Name: "blah2",
                        Type: "Microsoft.Compute/virtualMachines",
                        State: "Succeeded",
                        StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                        EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.7)),
                        Error: null)
            ],
                IsEntryPoint: false,
                Error: null,
                Outputs: ImmutableDictionary<string, JsonNode>.Empty),
            new(
                Id: "/subscriptions/id/resourceGroups/rg/providers/Microsoft.Resources/deployments/fooNestedErr",
                Name: "fooNestedErr",
                State: "Failed",
                StartTime: timeNow.Subtract(TimeSpan.FromSeconds(1)),
                EndTime: timeNow.Subtract(TimeSpan.FromSeconds(0.5)),
                Operations: [],
                IsEntryPoint: false,
                Error: null,
                Outputs: ImmutableDictionary<string, JsonNode>.Empty),
        ], 0);

        var test = AnsiHelper.ReplaceCodes(result);

        AnsiHelper.ReplaceCodes(result).Should().EqualIgnoringNewlines("""
        [EraseLine]foo [Bold][Gray]Running[Reset] (1.0s)
          [EraseLine]blah [Bold][Green]Succeeded[Reset] (0.3s)
          [EraseLine]fooNested [Bold][Green]Succeeded[Reset] (0.5s)
            [EraseLine]blah2 [Bold][Green]Succeeded[Reset] (0.3s)
          [EraseLine]fooNestedErr [Bold][Red]Failed[Reset] (0.5s)
            [EraseLine][Red]Oh dear oh dear[Reset]

        """);
    }
}
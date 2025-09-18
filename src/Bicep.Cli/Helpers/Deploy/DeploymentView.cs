// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace Bicep.Cli.Commands.Helpers.Deploy;

public record DeploymentView(
    string Id,
    string Name,
    string State,
    DateTime StartTime,
    DateTime? EndTime,
    ImmutableArray<DeploymentOperationView> Operations,
    bool IsEntryPoint,
    string? Error,
    ImmutableDictionary<string, JsonNode> Outputs);

public record DeploymentOperationView(
    string Id,
    string Name,
    string Type,
    string State,
    DateTime StartTime,
    DateTime? EndTime,
    string? Error);
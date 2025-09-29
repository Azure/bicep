// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Nodes;

namespace Bicep.Cli.Helpers.Deploy;

public record GeneralOperationView(
    string Name,
    string State,
    string? Error);

public record DeploymentWrapperView(
    DeploymentView? Deployment,
    string? Error);

public record DeploymentView(
    string Id,
    string Name,
    string State,
    DateTime StartTime,
    DateTime? EndTime,
    ImmutableArray<DeploymentOperationView> Operations,
    string? Error,
    ImmutableDictionary<string, JsonNode> Outputs);

public record DeploymentOperationView(
    string Id,
    string Name,
    string? SymbolicName,
    string Type,
    string State,
    DateTime StartTime,
    DateTime? EndTime,
    string? Error);

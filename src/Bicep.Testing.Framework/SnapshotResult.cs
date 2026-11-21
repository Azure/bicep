// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Azure.Deployments.Core.Definitions.Identifiers;

namespace Bicep.Testing.Framework;

/// <summary>
/// Flattened resources, evaluation diagnostics, and outputs predicted by the CLI.
/// Check diagnostics before relying on a potentially incomplete prediction.
/// </summary>
public sealed record SnapshotResult
{
    /// <summary>
    /// Resources from the entry point and nested modules, including non-ARM and unresolved resources.
    /// </summary>
    public required ImmutableArray<SnapshotResource> PredictedResources { get; init; }

    /// <summary>
    /// Evaluation warnings and errors that may indicate an incomplete snapshot.
    /// </summary>
    public required ImmutableArray<string> Diagnostics { get; init; }

    /// <summary>
    /// Output names mapped to their evaluated JSON values.
    /// </summary>
    public required ImmutableDictionary<string, JsonElement> Outputs { get; init; }
}

/// <summary>
/// A predicted resource, including extensible resources and resources with unresolved IDs.
/// </summary>
public record SnapshotResource
{
    /// <summary>
    /// The complete evaluated JSON resource body, including unmodeled fields and unresolved expressions.
    /// </summary>
    public required JsonElement Body { get; init; }
}

/// <summary>
/// A predicted resource with a resolved, parseable ARM resource ID.
/// </summary>
public sealed record ArmSnapshotResource : SnapshotResource
{
    /// <summary>
    /// The parsed ARM resource ID, including its scope.
    /// </summary>
    public required ResourceId Id { get; init; }

    /// <summary>
    /// The fully qualified resource type derived from the ID.
    /// </summary>
    public string Type => Id.FormatFullyQualifiedType();

    /// <summary>
    /// The resource name derived from the ID, including parent resource names for nested resources.
    /// </summary>
    public string Name => Id.FormatName();
}
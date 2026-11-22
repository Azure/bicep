// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.RpcClient.Models;

namespace Bicep.Testing.Framework;

/// <summary>
/// A Bicep parameter file and optional deployment metadata. Omitted metadata uses CLI placeholder values.
/// </summary>
public sealed record SnapshotOptions
{
    /// <summary>
    /// The parameter-file path, resolved relative to the current working directory.
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// Tenant ID used when evaluating tenant-dependent expressions.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// Subscription ID used for subscription scope and resource IDs.
    /// </summary>
    public string? SubscriptionId { get; init; }

    /// <summary>
    /// Management group ID used for management-group scope. Requires CLI 0.47.16 or later.
    /// </summary>
    public string? ManagementGroupId { get; init; }

    /// <summary>
    /// Resource group name used for resource-group scope and resource IDs.
    /// </summary>
    public string? ResourceGroup { get; init; }

    /// <summary>
    /// Location used for deployment and resource-group metadata.
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Deployment name used when evaluating deployment metadata.
    /// </summary>
    public string? DeploymentName { get; init; }

    /// <summary>
    /// Overrides config-file discovery with a path relative to the working directory. Requires CLI 0.48.0 or later.
    /// </summary>
    public string? ConfigPath { get; init; }

    /// <summary>
    /// Offline values for parameter-file external inputs, avoiding external input provider calls.
    /// </summary>
    public ImmutableArray<GetSnapshotRequest.ExternalInputValue>? ExternalInputs { get; init; }
}

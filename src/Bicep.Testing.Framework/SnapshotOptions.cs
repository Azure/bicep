// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.RpcClient.Models;

namespace Bicep.Testing.Framework;

/// <summary>
/// Inputs for evaluating a <c>.bicepparam</c> file. Omitted metadata uses the CLI's placeholder values.
/// </summary>
public sealed record SnapshotOptions
{
    public required string FilePath { get; init; }

    public string? TenantId { get; init; }

    public string? SubscriptionId { get; init; }

    /// <summary>
    /// Requires Bicep CLI 0.47.16 or later.
    /// </summary>
    public string? ManagementGroupId { get; init; }

    public string? ResourceGroup { get; init; }

    public string? Location { get; init; }

    public string? DeploymentName { get; init; }

    /// <summary>
    /// Overrides config-file discovery. Requires Bicep CLI 0.48.0 or later.
    /// </summary>
    public string? ConfigPath { get; init; }

    /// <summary>
    /// Supplies values for external inputs without contacting an external input provider.
    /// </summary>
    public ImmutableArray<GetSnapshotRequest.ExternalInputValue>? ExternalInputs { get; init; }
}

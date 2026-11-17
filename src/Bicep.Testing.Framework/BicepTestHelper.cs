// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.RpcClient;
using Bicep.RpcClient.Models;

namespace Bicep.Testing.Framework;

/// <summary>
/// Evaluates Bicep parameter files through the Bicep CLI without deploying resources.
/// The caller owns the client factory; each snapshot acquires and disposes its own client.
/// </summary>
public sealed class BicepTestHelper(IBicepClientFactory clientFactory)
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        RespectNullableAnnotations = true,
    };

    /// <summary>
    /// Compiles and evaluates a parameter file. Evaluation diagnostics are returned in the snapshot;
    /// compilation and RPC failures are propagated to the caller.
    /// </summary>
    public async Task<SnapshotResult> SnapshotAsync(SnapshotOptions options, BicepClientConfiguration configuration, CancellationToken cancellationToken = default)
    {
        using var client = await clientFactory.Initialize(configuration, cancellationToken).ConfigureAwait(false);

        if (options.ManagementGroupId is not null)
        {
            var version = await client.GetVersion(cancellationToken).ConfigureAwait(false);
            if (!Version.TryParse(version.Split('+')[0], out var parsed) || parsed < new Version(0, 47, 16))
            {
                throw new NotSupportedException($"Bicep CLI 0.47.16 or later is required for management-group snapshots; detected '{version}'.");
            }
        }

        var response = await client.GetSnapshot(
            new(
                Path.GetFullPath(options.FilePath),
                new(
                    options.TenantId,
                    options.ManagementGroupId,
                    options.SubscriptionId,
                    options.ResourceGroup,
                    options.Location,
                    options.DeploymentName),
                options.ExternalInputs)
            {
                ConfigPath = options.ConfigPath is { } configPath ? Path.GetFullPath(configPath) : null,
            },
            cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<SnapshotResult>(response.Snapshot, SerializerOptions)
            ?? throw new InvalidDataException("The Bicep CLI returned a null snapshot.");
    }
}

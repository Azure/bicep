// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.RpcClient;
using Bicep.RpcClient.Models;

namespace Bicep.Testing.Framework;

/// <summary>
/// Evaluates Bicep parameter files without deploying resources.
/// The caller owns the factory; each snapshot acquires and disposes its own client.
/// </summary>
/// <param name="clientFactory">The factory used to acquire clients for the requested CLI configuration.</param>
public sealed class BicepTestHelper(IBicepClientFactory clientFactory)
{
    /// <summary>
    /// Compiles and evaluates a parameter file, preserving resource bodies and classifying resolved ARM resources.
    /// Evaluation diagnostics are returned; compilation, transport, and malformed-response errors throw.
    /// </summary>
    /// <param name="configuration">CLI version, executable, and connection settings.</param>
    /// <param name="options">The parameter file and optional deployment context.</param>
    /// <param name="cancellationToken">Cancels client initialization and snapshot evaluation.</param>
    /// <returns>The predicted resources, diagnostics, and outputs.</returns>
    public async Task<SnapshotResult> SnapshotAsync(BicepClientConfiguration configuration, SnapshotOptions options, CancellationToken cancellationToken = default)
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

        var snapshot = JsonSerializer.Deserialize(response.Snapshot, SnapshotSerializationContext.FileSerializer.Snapshot)
            ?? throw new InvalidDataException("The Bicep CLI returned a null snapshot.");

        return new SnapshotResult
        {
            PredictedResources = [.. GetSnapshotResources(snapshot)],
            Diagnostics = snapshot.Diagnostics,
            Outputs = snapshot.Outputs,
        };
    }

    private static IEnumerable<SnapshotResource> GetSnapshotResources(Snapshot snapshot)
    {
        foreach (var resource in snapshot.PredictedResources)
        {
            if (TryParseArmResourceId(resource) is { } resourceId)
            {
                yield return new ArmSnapshotResource
                {
                    Id = resourceId,
                    Body = resource
                };
                continue;
            }

            yield return new SnapshotResource
            {
                Body = resource
            };
        }
    }

    private static ResourceId? TryParseArmResourceId(JsonElement resource)
        => resource.ValueKind == JsonValueKind.Object &&
            resource.TryGetProperty("id", out var id) &&
            id.ValueKind == JsonValueKind.String &&
            id.GetString() is { } idString &&
            ResourceId.TryParse(idString, out var resourceId)
                ? resourceId
                : null;
}

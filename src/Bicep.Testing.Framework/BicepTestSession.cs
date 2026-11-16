// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.RpcClient;
using Bicep.RpcClient.Models;

namespace Bicep.Testing.Framework;

/// <summary>
/// Evaluates Bicep parameter files through the Bicep CLI without deploying resources.
/// </summary>
public sealed class BicepTestSession(IBicepClient client) : IDisposable, IAsyncDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        RespectNullableAnnotations = true,
    };

    private bool disposed;

    /// <summary>
    /// Creates a session using a pinned CLI version, downloading and caching it if necessary.
    /// </summary>
    public static Task<BicepTestSession> CreateAsync(string bicepVersion, CancellationToken cancellationToken = default)
        => CreateAsync(new BicepClientConfiguration { BicepVersion = bicepVersion }, cancellationToken);

    /// <summary>
    /// Creates a session with the specified CLI configuration, including support for an existing executable.
    /// </summary>
    public static async Task<BicepTestSession> CreateAsync(BicepClientConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var client = await new BicepClientFactory().Initialize(configuration, cancellationToken).ConfigureAwait(false);
        return new(client);
    }

    /// <summary>
    /// Compiles and evaluates a parameter file. Evaluation diagnostics are returned in the snapshot;
    /// compilation and RPC failures are propagated to the caller.
    /// </summary>
    public async Task<SnapshotResult> SnapshotAsync(SnapshotOptions options, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

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

    /// <summary>
    /// Disposes the session and its RPC client, including a client supplied to the constructor.
    /// </summary>
    public void Dispose()
    {
        if (!disposed)
        {
            disposed = true;
            client.Dispose();
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}

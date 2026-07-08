// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Bicep.RpcClient;

public interface IBicepClientFactory
{
    /// <summary>
    /// Initializes a Bicep client.
    /// </summary>
    /// <param name="configuration">The configuration for the Bicep client.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IBicepClient> Initialize(BicepClientConfiguration configuration, CancellationToken cancellationToken = default);

    [Obsolete($"Use {nameof(Initialize)} with a {nameof(BicepClientConfiguration)} that has {nameof(BicepClientConfiguration.ExistingCliPath)} set instead.")]
    Task<IBicepClient> InitializeFromPath(string bicepCliPath, CancellationToken cancellationToken = default);

    [Obsolete($"Use {nameof(Initialize)} instead.")]
    Task<IBicepClient> DownloadAndInitialize(BicepClientConfiguration configuration, CancellationToken cancellationToken = default);
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Bicep.RpcClient;

public interface IBicepClientFactory
{
    /// <summary>
    /// Initializes a Bicep client by downloading the specified version of the Bicep CLI.
    /// </summary>
    /// <param name="configuration">The configuration for the Bicep client.</param>
    /// <param name="bicepVersion">The version of the Bicep CLI to download.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IBicepClient> DownloadAndInitialize(BicepClientConfiguration configuration, string? bicepVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes a Bicep client from a file system path.
    /// </summary>
    /// <param name="bicepCliPath">The file system path to the Bicep CLI.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task<IBicepClient> InitializeFromPath(string bicepCliPath, CancellationToken cancellationToken = default);
}

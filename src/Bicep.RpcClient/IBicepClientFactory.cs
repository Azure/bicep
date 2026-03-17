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
}

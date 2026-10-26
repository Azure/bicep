// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.RpcClient.JsonRpc;

internal interface IJsonRpcClient : IDisposable
{
    Task<TResponse> SendRequest<TRequest, TResponse>(string method, TRequest request, CancellationToken cancellationToken);

    Task Listen(Action onComplete, CancellationToken cancellationToken);
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.IO.Pipelines;

namespace Bicep.RpcClient.JsonRpc;

internal class JsonRpcClient(PipeReader reader, PipeWriter writer) : IJsonRpcClient
{
    private int nextId = 0;
    private int disposed = 0;
    private readonly SemaphoreSlim writeSemaphore = new(1, 1);
    private readonly PendingRequestRegistry pendingRequests = new();

    public async Task<TResponse> SendRequest<TRequest, TResponse>(string method, TRequest request, CancellationToken cancellationToken)
    {
        var currentId = Interlocked.Increment(ref nextId);
        var requestBytes = JsonRpcFormatter.GetRequestBytes(method, request, currentId);

        // Register the pending response BEFORE writing to the wire. Otherwise the server can
        // respond so quickly that the listen loop observes the response before we've registered
        // the handler, silently dropping it and leaving this caller awaiting a response forever.
        // Disposing the registration releases the slot no matter how this method unwinds.
        using var registration = pendingRequests.Register(currentId);

        await writeSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await writer.WriteAsync(requestBytes, cancellationToken).ConfigureAwait(false);
            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            writeSemaphore.Release();
        }

        var responseContent = await registration.WaitForResponseAsync(cancellationToken).ConfigureAwait(false);
        return JsonRpcFormatter.GetResponse<TResponse>(responseContent);
    }

    public Task Listen(Action onComplete, CancellationToken cancellationToken)
        => Task.Run(async () =>
        {
            try
            {
                await ListenInternal(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected when disposing
            }
            finally
            {
                onComplete();
            }
        }, cancellationToken);

    private async Task ListenInternal(CancellationToken cancellationToken)
    {
        Exception? faultException = null;
        try
        {
            while (true)
            {
                var message = await JsonRpcFormatter.ReadMessage(reader, cancellationToken).ConfigureAwait(false);
                if (message is null)
                {
                    // The remote endpoint disconnected. Any outstanding callers are faulted below.
                    faultException = new IOException("The JSON-RPC connection was closed by the remote endpoint before all responses were received.");
                    return;
                }

                var responseId = JsonRpcFormatter.GetResponseId(message);

                if (!pendingRequests.TryComplete(responseId, message))
                {
                    // No caller is waiting on this id. This indicates a protocol bug (unknown or
                    // duplicate id), so surface it rather than silently dropping the message.
                    Debug.WriteLine($"Received a JSON-RPC response for unknown or already-completed request ID {responseId}.");
                }
            }
        }
        catch (Exception ex)
        {
            faultException = ex;
            if (cancellationToken.IsCancellationRequested)
            {
                await reader.CompleteAsync().ConfigureAwait(false);
                await writer.CompleteAsync().ConfigureAwait(false);
            }
            else
            {
                // A framing/protocol error is unrecoverable for this connection. Surface it on the listen
                // task (its established contract) after the finally block has faulted any pending callers.
                throw;
            }
        }
        finally
        {
            // Never leave a caller awaiting a response that can no longer arrive.
            if (cancellationToken.IsCancellationRequested)
            {
                pendingRequests.CancelAll(cancellationToken);
            }
            else
            {
                pendingRequests.FaultAll(faultException ?? new IOException("The JSON-RPC connection was closed before a response was received."));
            }
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0)
        {
            return;
        }

        // Unblock any callers still awaiting responses before tearing down the pipe.
        pendingRequests.FaultAll(new ObjectDisposedException(nameof(JsonRpcClient)));
        writer.Complete();
        reader.Complete();
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.RpcClient.JsonRpc;

/// <summary>
/// Owns the correlation between in-flight JSON-RPC request ids and the callers awaiting their
/// responses. All concurrency-sensitive bookkeeping (registration, completion, and connection
/// teardown) is localized here so the transport code can be read as intent.
/// </summary>
internal sealed class PendingRequestRegistry
{
    private readonly ConcurrentDictionary<int, TaskCompletionSource<byte[]>> pending = new();

    /// <summary>
    /// Registers a pending request for the given id. The caller awaits
    /// <see cref="Registration.WaitForResponseAsync"/> and must dispose the returned handle to release
    /// the slot (on success, failure, or cancellation).
    /// </summary>
    public Registration Register(int id)
    {
        var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!pending.TryAdd(id, tcs))
        {
            throw new InvalidOperationException($"A request with ID {id} is already pending.");
        }

        return new Registration(this, id, tcs);
    }

    /// <summary>
    /// Completes the pending request with the given id. Returns <see langword="false"/> if no caller is
    /// waiting on that id (unknown or already-completed request).
    /// </summary>
    public bool TryComplete(int id, byte[] message)
    {
        if (pending.TryRemove(id, out var tcs))
        {
            tcs.TrySetResult(message);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Faults every outstanding request so no caller is left awaiting a response that can no longer arrive.
    /// </summary>
    public void FaultAll(Exception exception)
    {
        foreach (var id in pending.Keys.ToArray())
        {
            if (pending.TryRemove(id, out var tcs))
            {
                tcs.TrySetException(exception);
            }
        }
    }

    /// <summary>
    /// Cancels every outstanding request using the given cancellation token.
    /// </summary>
    public void CancelAll(CancellationToken cancellationToken)
    {
        foreach (var id in pending.Keys.ToArray())
        {
            if (pending.TryRemove(id, out var tcs))
            {
                tcs.TrySetCanceled(cancellationToken);
            }
        }
    }

    /// <summary>
    /// A scoped handle for a single registered request. Disposing it releases the registry slot,
    /// ensuring the pending entry never leaks regardless of how the caller unwinds.
    /// </summary>
    public readonly struct Registration : IDisposable
    {
        private readonly PendingRequestRegistry registry;
        private readonly int id;
        private readonly TaskCompletionSource<byte[]> tcs;

        internal Registration(PendingRequestRegistry registry, int id, TaskCompletionSource<byte[]> tcs)
        {
            this.registry = registry;
            this.id = id;
            this.tcs = tcs;
        }

        /// <summary>
        /// Awaits the response, observing the caller's cancellation token without leaking the
        /// token registration once the response arrives.
        /// </summary>
        [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "The awaited task belongs to a TaskCompletionSource owned by this registry.")]
        public async Task<byte[]> WaitForResponseAsync(CancellationToken cancellationToken)
        {
            var completionSource = tcs;
            using (cancellationToken.Register(() => completionSource.TrySetCanceled(cancellationToken)))
            {
                return await completionSource.Task.ConfigureAwait(false);
            }
        }

        public void Dispose() => registry.pending.TryRemove(id, out _);
    }
}

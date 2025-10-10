// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.RpcClient.JsonRpc;

internal class JsonRpcClient(PipeReader reader, PipeWriter writer) : IDisposable
{
    private record JsonRpcRequest<T>(
        string Jsonrpc,
        string Method,
        T Params,
        int Id);

    private record MinimalJsonRpcResponse(
        int Id);

    private record JsonRpcResponse<T>(
        string Jsonrpc,
        T? Result,
        JsonRpcError? Error,
        int Id);

    private record JsonRpcError(
        int Code,
        string Message,
        JsonNode? Data);

    private int nextId = 0;
    private readonly SemaphoreSlim writeSemaphore = new(1, 1);
    private readonly ConcurrentDictionary<int, TaskCompletionSource<byte[]>> pendingResponses = new();

    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public async Task<TResponse> SendRequest<TRequest, TResponse>(string method, TRequest request, CancellationToken cancellationToken)
    {
        var currentId = Interlocked.Increment(ref nextId);

        var jsonRpcRequest = new JsonRpcRequest<TRequest>(Jsonrpc: "2.0", Method: method, Params: request, Id: currentId);
        var requestContent = JsonSerializer.Serialize(jsonRpcRequest, jsonSerializerOptions);
        var requestLength = Encoding.UTF8.GetByteCount(requestContent);
        var rawRequest = $"Content-Length: {requestLength}\r\n\r\n{requestContent}";

        await writeSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var requestBytes = Encoding.UTF8.GetBytes(rawRequest).AsMemory();
            await writer.WriteAsync(requestBytes, cancellationToken).ConfigureAwait(false);
            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            writeSemaphore.Release();
        }

        var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!pendingResponses.TryAdd(currentId, tcs))
        {
            throw new InvalidOperationException($"A request with ID {currentId} is already pending.");
        }

        var responseContent = await tcs.Task.ConfigureAwait(false);
        var jsonRpcResponse = JsonSerializer.Deserialize<JsonRpcResponse<TResponse>>(responseContent, jsonSerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize JSON-RPC response");

        if (jsonRpcResponse.Result is null)
        {
            var error = jsonRpcResponse.Error ?? throw new InvalidDataException("Failed to retrieve JSONRPC error");
            throw new InvalidOperationException(error.Message);
        }

        return jsonRpcResponse.Result;
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
        while (true)
        {
            try
            {
                var message = await ReadMessage(cancellationToken).ConfigureAwait(false);
                if (message is null)
                {
                    return;
                }

                var response = JsonSerializer.Deserialize<MinimalJsonRpcResponse>(message, jsonSerializerOptions)
                    ?? throw new InvalidOperationException("Failed to deserialize JSON-RPC response");

                if (pendingResponses.TryRemove(response.Id, out var tcs))
                {
                    tcs.SetResult(message);
                }
            }
            catch (Exception) when (cancellationToken.IsCancellationRequested)
            {
                await reader.CompleteAsync().ConfigureAwait(false);
                await writer.CompleteAsync().ConfigureAwait(false);
                break;
            }
        }
    }

    private record Headers(
        int ContentLength);

    private async Task<Headers?> ReadHeaders(CancellationToken cancellationToken)
    {
        int? contentLength = null;
        while (true)
        {
            var readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);

            if (readResult.Buffer.IsEmpty && readResult.IsCompleted)
            {
                return null; // remote end disconnected at a reasonable place.
            }

            var lf = readResult.Buffer.PositionOf((byte)'\n');
            if (!lf.HasValue)
            {
                if (readResult.IsCompleted)
                {
                    throw new EndOfStreamException();
                }

                // Indicate that we can't find what we're looking for and read again.
                reader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
                continue;
            }

            var line = readResult.Buffer.Slice(0, lf.Value);

            // Verify the line ends with an \r (that precedes the \n we already found)
            var cr = line.PositionOf((byte)'\r');
            if (!cr.HasValue || !line.GetPosition(1, cr.Value).Equals(lf))
            {
                throw new InvalidOperationException("Header does not end with expected \r\n character sequence");
            }

            // Trim off the \r now that we confirmed it was there.
            line = line.Slice(0, line.Length - 1);

            if (line.Length > 0)
            {
                var lineText = Encoding.UTF8.GetString(line.ToArray());
                var split = lineText.Split([':'], 2);
                if (split.Length != 2)
                {
                    throw new InvalidOperationException("Colon not found in header.");
                }

                var headerName = split[0].Trim();
                var headerValue = split[1].Trim();

                if (headerName == "Content-Length")
                {
                    contentLength = int.Parse(headerValue);
                }
            }

            // Advance to the next line.
            reader.AdvanceTo(readResult.Buffer.GetPosition(1, lf.Value));

            if (line.Length == 0)
            {
                // We found the empty line that constitutes the end of the HTTP headers.
                break;
            }
        }

        if (!contentLength.HasValue)
        {
            throw new InvalidOperationException("Failed to obtain Content-Length header");
        }

        return new(contentLength.Value);
    }

    protected async ValueTask<ReadResult> ReadAtLeastAsync(int requiredBytes, bool allowEmpty, CancellationToken cancellationToken)
    {
        var readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        while (readResult.Buffer.Length < requiredBytes && !readResult.IsCompleted && !readResult.IsCanceled)
        {
            reader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
            readResult = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        }

        if (allowEmpty && readResult.Buffer.Length == 0)
        {
            return readResult;
        }

        if (readResult.Buffer.Length < requiredBytes)
        {
            throw readResult.IsCompleted ? new EndOfStreamException() :
                readResult.IsCanceled ? new OperationCanceledException() :
                throw new InvalidOperationException(); // should be unreachable
        }

        return readResult;
    }

    private async Task<byte[]?> ReadMessage(CancellationToken cancellationToken)
    {
        var headers = await ReadHeaders(cancellationToken).ConfigureAwait(false);
        if (headers is null)
        {
            return null;
        }

        var readResult = await ReadAtLeastAsync(headers.ContentLength, allowEmpty: false, cancellationToken).ConfigureAwait(false);

        var contentBuffer = readResult.Buffer.Slice(0, headers.ContentLength);
        var output = contentBuffer.ToArray();

        reader.AdvanceTo(contentBuffer.End);

        return output;
    }

    public void Dispose()
    {
        writer.Complete();
        reader.Complete();
    }
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Bicep.RpcClient.JsonRpc;

internal static class JsonRpcFormatter
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

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private record Headers(
        int ContentLength);

    private static async Task<Headers?> ReadHeaders(PipeReader reader, CancellationToken cancellationToken)
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
                    contentLength = int.Parse(headerValue, CultureInfo.InvariantCulture);
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

    private static async ValueTask<ReadResult> ReadAtLeastAsync(PipeReader reader, int requiredBytes, bool allowEmpty, CancellationToken cancellationToken)
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

    public static async Task<byte[]?> ReadMessage(PipeReader reader, CancellationToken cancellationToken)
    {
        var headers = await ReadHeaders(reader, cancellationToken).ConfigureAwait(false);
        if (headers is null)
        {
            return null;
        }

        var readResult = await ReadAtLeastAsync(reader, headers.ContentLength, allowEmpty: false, cancellationToken).ConfigureAwait(false);

        var contentBuffer = readResult.Buffer.Slice(0, headers.ContentLength);
        var output = contentBuffer.ToArray();

        reader.AdvanceTo(contentBuffer.End);

        return output;
    }

    public static ReadOnlyMemory<byte> GetRequestBytes<TRequest>(string method, TRequest request, int currentId)
    {
        var jsonRpcRequest = new JsonRpcRequest<TRequest>(Jsonrpc: "2.0", Method: method, Params: request, Id: currentId);
        var requestContent = JsonSerializer.Serialize(jsonRpcRequest, JsonSerializerOptions);
        var requestLength = Encoding.UTF8.GetByteCount(requestContent);
        var rawRequest = $"Content-Length: {requestLength}\r\n\r\n{requestContent}";

        return Encoding.UTF8.GetBytes(rawRequest);
    }

    public static TResponse GetResponse<TResponse>(byte[] responseContent)
    {
        var jsonRpcResponse = JsonSerializer.Deserialize<JsonRpcResponse<TResponse>>(responseContent, JsonSerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize JSON-RPC response");

        if (jsonRpcResponse.Result is null)
        {
            var error = jsonRpcResponse.Error ?? throw new InvalidDataException("Failed to retrieve JSONRPC error");
            throw new InvalidOperationException(error.Message);
        }

        return jsonRpcResponse.Result;
    }

    public static int GetResponseId(byte[] responseContent)
    {
        var jsonRpcResponse = JsonSerializer.Deserialize<MinimalJsonRpcResponse>(responseContent, JsonSerializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize JSON-RPC response");

        return jsonRpcResponse.Id;
    }
}

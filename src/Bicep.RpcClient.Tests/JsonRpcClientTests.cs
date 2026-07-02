// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bicep.RpcClient.JsonRpc;
using FluentAssertions;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class JsonRpcClientTests
{
    public TestContext TestContext { get; set; } = null!;

    private record TestParams(string Name);

    private record TestResult(string Value);

    private CancellationToken Token => TestContext.CancellationTokenSource.Token;

    [TestMethod]
    public async Task SendRequest_writes_a_well_formed_framed_request()
    {
        using var harness = new TestHarness();
        harness.StartListening();

        var requestTask = harness.Client.SendRequest<TestParams, TestResult>("test/method", new("foo"), Token);

        var body = await harness.ReadRequestBodyAsync(Token);
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        root.GetProperty("jsonrpc").GetString().Should().Be("2.0");
        root.GetProperty("method").GetString().Should().Be("test/method");
        root.GetProperty("params").GetProperty("name").GetString().Should().Be("foo");
        root.GetProperty("id").GetInt32().Should().Be(1);

        await harness.WriteResultAsync(root.GetProperty("id").GetInt32(), """{"value":"bar"}""");

        (await requestTask).Value.Should().Be("bar");
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Observing the request result; not an issue in test code.")]
    public async Task SendRequest_throws_with_error_message_on_error_response()
    {
        using var harness = new TestHarness();
        harness.StartListening();

        var requestTask = harness.Client.SendRequest<TestParams, TestResult>("test/method", new("foo"), Token);

        var id = await harness.ReadRequestIdAsync(Token);
        await harness.WriteErrorAsync(id, -32000, "boom");

        await FluentActions.Invoking(() => requestTask)
            .Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Observing the request result; not an issue in test code.")]
    public async Task SendRequest_throws_when_response_has_neither_result_nor_error()
    {
        using var harness = new TestHarness();
        harness.StartListening();

        var requestTask = harness.Client.SendRequest<TestParams, TestResult>("test/method", new("foo"), Token);

        var id = await harness.ReadRequestIdAsync(Token);
        await harness.WriteEmptyResponseAsync(id);

        await FluentActions.Invoking(() => requestTask)
            .Should().ThrowAsync<InvalidDataException>();
    }

    [TestMethod]
    public async Task SendRequest_correlates_out_of_order_responses_by_id()
    {
        using var harness = new TestHarness();
        harness.StartListening();

        var firstTask = harness.Client.SendRequest<TestParams, TestResult>("test/method", new("first"), Token);
        var firstId = await harness.ReadRequestIdAsync(Token);

        var secondTask = harness.Client.SendRequest<TestParams, TestResult>("test/method", new("second"), Token);
        var secondId = await harness.ReadRequestIdAsync(Token);

        // Respond to the second request before the first to verify id-based correlation.
        await harness.WriteResultAsync(secondId, """{"value":"second-result"}""");
        await harness.WriteResultAsync(firstId, """{"value":"first-result"}""");

        (await secondTask).Value.Should().Be("second-result");
        (await firstTask).Value.Should().Be("first-result");
    }

    [TestMethod]
    public async Task SendRequest_reassembles_a_response_split_across_multiple_reads()
    {
        using var harness = new TestHarness();
        harness.StartListening();

        var requestTask = harness.Client.SendRequest<TestParams, TestResult>("test/method", new("foo"), Token);
        var id = await harness.ReadRequestIdAsync(Token);

        var json = "{\"jsonrpc\":\"2.0\",\"id\":" + id + ",\"result\":{\"value\":\"chunked\"}}";
        var frame = Encoding.UTF8.GetBytes($"Content-Length: {Encoding.UTF8.GetByteCount(json)}\r\n\r\n{json}");

        // Deliver the framed response one byte at a time, flushing between each, to exercise the partial-read loop.
        foreach (var b in frame)
        {
            await harness.WriteRawBytesAsync([b]);
        }

        (await requestTask).Value.Should().Be("chunked");
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Observing the listen loop result; not an issue in test code.")]
    public async Task Listen_invokes_onComplete_and_completes_on_clean_disconnect()
    {
        using var harness = new TestHarness();
        var onCompleteCalled = false;
        var listenTask = harness.Client.Listen(() => onCompleteCalled = true, Token);

        await harness.CompleteResponseAsync();

        await listenTask;
        onCompleteCalled.Should().BeTrue();
    }

    [TestMethod]
    public async Task Listen_invokes_onComplete_on_cancellation()
    {
        using var harness = new TestHarness();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
        var onComplete = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        // The outer Task.Run may itself observe cancellation, so assert via the onComplete callback rather than the returned task.
        _ = harness.Client.Listen(() => onComplete.TrySetResult(true), cts.Token);

        // Give the listen loop a moment to start and park inside a pending read before cancelling,
        // so cancellation unwinds through the loop body and its finally block.
        await Task.Delay(50);
        await cts.CancelAsync();

        (await Task.WhenAny(onComplete.Task, Task.Delay(TimeSpan.FromSeconds(5)))).Should().Be(onComplete.Task);
        onComplete.Task.IsCompletedSuccessfully.Should().BeTrue();
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Observing the listen loop result; not an issue in test code.")]
    public async Task Listen_faults_when_Content_Length_header_is_missing()
    {
        using var harness = new TestHarness();
        var listenTask = harness.Client.Listen(() => { }, Token);

        await harness.WriteRawBytesAsync(Encoding.UTF8.GetBytes("Header: value\r\n\r\n"));

        await FluentActions.Invoking(() => listenTask)
            .Should().ThrowAsync<InvalidOperationException>().WithMessage("*Content-Length*");
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Observing the listen loop result; not an issue in test code.")]
    public async Task Listen_faults_when_header_has_no_colon()
    {
        using var harness = new TestHarness();
        var listenTask = harness.Client.Listen(() => { }, Token);

        await harness.WriteRawBytesAsync(Encoding.UTF8.GetBytes("NoColonHere\r\n\r\n"));

        await FluentActions.Invoking(() => listenTask)
            .Should().ThrowAsync<InvalidOperationException>().WithMessage("*Colon*");
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Observing the listen loop result; not an issue in test code.")]
    public async Task Listen_faults_when_header_line_is_not_crlf_terminated()
    {
        using var harness = new TestHarness();
        var listenTask = harness.Client.Listen(() => { }, Token);

        await harness.WriteRawBytesAsync(Encoding.UTF8.GetBytes("Content-Length: 5\nbody!"));

        await FluentActions.Invoking(() => listenTask)
            .Should().ThrowAsync<InvalidOperationException>().WithMessage(@"*\r\n*");
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Observing the listen loop result; not an issue in test code.")]
    public async Task Listen_faults_with_EndOfStream_when_headers_are_truncated()
    {
        using var harness = new TestHarness();
        var listenTask = harness.Client.Listen(() => { }, Token);

        await harness.WriteRawBytesAsync(Encoding.UTF8.GetBytes("Content-Length: 5"));
        await harness.CompleteResponseAsync();

        await FluentActions.Invoking(() => listenTask)
            .Should().ThrowAsync<EndOfStreamException>();
    }

    [TestMethod]
    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Observing the listen loop result; not an issue in test code.")]
    public async Task Listen_faults_with_EndOfStream_when_body_is_shorter_than_content_length()
    {
        using var harness = new TestHarness();
        var listenTask = harness.Client.Listen(() => { }, Token);

        await harness.WriteRawBytesAsync(Encoding.UTF8.GetBytes("Content-Length: 100\r\n\r\n{}"));
        await harness.CompleteResponseAsync();

        await FluentActions.Invoking(() => listenTask)
            .Should().ThrowAsync<EndOfStreamException>();
    }

    private sealed class TestHarness : IDisposable
    {
        // requestPipe carries client -> server bytes; responsePipe carries server -> client bytes.
        private readonly Pipe requestPipe = new();
        private readonly Pipe responsePipe = new();

        public TestHarness()
        {
            Client = new JsonRpcClient(responsePipe.Reader, requestPipe.Writer);
        }

        public JsonRpcClient Client { get; }

        private Task? listenTask;

        public void StartListening() => listenTask = Client.Listen(() => { }, CancellationToken.None);

        public async Task<string> ReadRequestBodyAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var result = await requestPipe.Reader.ReadAsync(cancellationToken);
                var buffer = result.Buffer;
                if (TryParseFrame(buffer, out var body, out var consumed))
                {
                    requestPipe.Reader.AdvanceTo(consumed);
                    return body;
                }

                requestPipe.Reader.AdvanceTo(buffer.Start, buffer.End);
                if (result.IsCompleted)
                {
                    throw new EndOfStreamException();
                }
            }
        }

        public async Task<int> ReadRequestIdAsync(CancellationToken cancellationToken)
        {
            var body = await ReadRequestBodyAsync(cancellationToken);
            using var document = JsonDocument.Parse(body);
            return document.RootElement.GetProperty("id").GetInt32();
        }

        public Task WriteResultAsync(int id, string resultJson)
            => WriteRawResponseAsync("{\"jsonrpc\":\"2.0\",\"id\":" + id + ",\"result\":" + resultJson + "}");

        public Task WriteErrorAsync(int id, int code, string message)
            => WriteRawResponseAsync(JsonSerializer.Serialize(new { jsonrpc = "2.0", id, error = new { code, message } }));

        public Task WriteEmptyResponseAsync(int id)
            => WriteRawResponseAsync(JsonSerializer.Serialize(new { jsonrpc = "2.0", id }));

        public async Task WriteRawResponseAsync(string json)
        {
            var frame = $"Content-Length: {Encoding.UTF8.GetByteCount(json)}\r\n\r\n{json}";
            await WriteRawBytesAsync(Encoding.UTF8.GetBytes(frame));
        }

        public async Task WriteRawBytesAsync(byte[] bytes)
        {
            await responsePipe.Writer.WriteAsync(bytes);
            await responsePipe.Writer.FlushAsync();
        }

        public async Task CompleteResponseAsync()
        {
            await responsePipe.Writer.FlushAsync();
            await responsePipe.Writer.CompleteAsync();
        }

        private static bool TryParseFrame(ReadOnlySequence<byte> buffer, out string body, out SequencePosition consumed)
        {
            body = string.Empty;
            consumed = buffer.Start;

            var bytes = buffer.ToArray();
            var text = Encoding.UTF8.GetString(bytes);
            var headerEnd = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (headerEnd < 0)
            {
                return false;
            }

            var headerText = text.Substring(0, headerEnd);
            var match = Regex.Match(headerText, @"Content-Length:\s*(\d+)", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return false;
            }

            var contentLength = int.Parse(match.Groups[1].Value);
            var bodyStart = headerEnd + 4; // ASCII headers, so char offset == byte offset.
            if (bytes.Length - bodyStart < contentLength)
            {
                return false;
            }

            body = Encoding.UTF8.GetString(bytes, bodyStart, contentLength);
            consumed = buffer.GetPosition(bodyStart + contentLength);
            return true;
        }

        public void Dispose()
        {
            Client.Dispose();
            _ = listenTask;
        }
    }
}

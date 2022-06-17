// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.VSLanguageServerClient.Tracing
{
    public class EtwTeeStream : Stream
    {
        private readonly Stream _inner;
        private readonly byte[] _identifier;
        private static readonly EtwEvents s_etwEvents = new EtwEvents();
        private static int s_payloadId = 0;

        public EtwTeeStream(Stream inner, string sourceIdentifier)
        {
            _inner = inner;
            _identifier = Encoding.Default.GetBytes(sourceIdentifier);
        }

        public override bool CanRead => _inner.CanRead;

        public override bool CanSeek => _inner.CanSeek;

        public override bool CanWrite => _inner.CanWrite;

        public override long Length => _inner.Length;

        public override long Position { get => _inner.Position; set => _inner.Position = value; }

        public override void Flush()
        {
            _inner.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _inner.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _inner.Read(buffer, offset, count);

            int payloadId = Interlocked.Increment(ref s_payloadId);

            LogMessage(buffer, offset, bytesRead, isServerMessage: true, payloadId);

            return bytesRead;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int bytesRead = await _inner.ReadAsync(buffer, offset, count, cancellationToken);

            int payloadId = Interlocked.Increment(ref s_payloadId);

            LogMessage(buffer, offset, bytesRead, isServerMessage: true, payloadId);

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);

            int payloadId = Interlocked.Increment(ref s_payloadId);

            LogMessage(buffer, offset, count, isServerMessage: false, payloadId);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _inner.WriteAsync(buffer, offset, count, cancellationToken);

            int payloadId = Interlocked.Increment(ref s_payloadId);

            LogMessage(buffer, offset, count, isServerMessage: false, payloadId);
        }

        private void LogMessage(byte[] buffer, int offset, int count, bool isServerMessage, int payloadId)
        {
            if (s_etwEvents.IsEnabled())
            {
                byte[] payload = new byte[count];
                Buffer.BlockCopy(buffer, offset, payload, 0, count);

                s_etwEvents.LogMessage(_identifier, payload, isServerMessage, payloadId);
            }
        }

        [EventSource(Name = "Microsoft-VisualStudio-WebTools-LanguageServer")]
        private sealed class EtwEvents : EventSource
        {
            // TODO: fine tune this?
            // 64k, divided by 2 because UTF-16, minus a few bytes to allow unforeseen overhead
            private const int MaxChunkSize = (64 * 1024 / 2) - 32 - 16000;

            private readonly int _processId = Process.GetCurrentProcess().Id;

            #region Logger API
            // ***The methods which actually fire the event should be private and placed in the #region following this one.***
            // For logger events which require some preprocessing for the event data, it is preferred to
            // expose a method here which will conditionally do the processing only if the ETW provider has
            // been enabled.
            // Also, this allows for exposing a cleaner tracing API, since ETW cannot natively consume complex
            // types, and we can do the conversion in this API, and then only when necessary.

            [NonEvent]
            public void LogMessage(byte[] identifier, byte[] payload, bool isServerMessage, int payloadId)
            {
                if (IsEnabled())
                {
                    Action<byte[], byte[], int, int> invokeMessage = ClientMessage;
                    if (isServerMessage)
                    {
                        invokeMessage = ServerResponse;
                    }

                    if (payload.Length > MaxChunkSize) // need to chunk it to fit in ETW event payload
                    {
                        int offset = 0;
                        byte[] chunk = new byte[MaxChunkSize];
                        while (offset < payload.Length)
                        {
                            int chunkLength = Math.Min(MaxChunkSize, payload.Length - offset);
                            if (chunkLength < MaxChunkSize)
                            {
                                // only reallocate a (shorter) buffer on the last chunk
                                chunk = new byte[chunkLength];
                            }

                            Buffer.BlockCopy(payload, offset, chunk, 0, chunkLength);

                            invokeMessage(identifier, chunk, payloadId, _processId);
                            offset += chunkLength;
                        }
                    }
                    else
                    {
                        invokeMessage(identifier, payload, payloadId, _processId);
                    }
                }
            }

            #endregion

            #region Private event implementations
            // Here is where we provide the real implementation of each event
            // Some guidelines:
            // - Always use the [Event(#)] attribute.  Otherwise, the eventId passed to WriteEvent depends on the location of the method within this file (ew).
            // - Always be private.  This is the same reasoning as why properties exist over fields.
            // - Caller should always call IsEnabled() before calling this method.

            [Event(1)]
            private void ClientMessage(byte[] identifier, byte[] payload, int payloadId, int processId)
            {
                if (payload.Length > 0)
                {
                    WriteEvent(1, identifier, payload, payloadId, processId);
                }
            }

            [Event(2)]
            public void ServerResponse(byte[] identifier, byte[] payload, int payloadId, int processId)
            {
                if (payload.Length > 0)
                {
                    WriteEvent(2, identifier, payload, payloadId, processId);
                }
            }

            #endregion
        }
    }
}

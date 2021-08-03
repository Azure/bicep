// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Bicep.LanguageServer.Utils
{
    class ServerLogTraceListener : TraceListener
    {
        private readonly ILanguageServer server;

        public ServerLogTraceListener(ILanguageServer server)
        {
            this.server = server;
        }

        public override void Write(string? message)
        {
            server.Log(new LogMessageParams { Type = MessageType.Log, Message = $"TRACE: {message}" });
        }

        public override void WriteLine(string? message)
        {
            server.Log(new LogMessageParams { Type = MessageType.Log, Message = $"TRACE: {message}" });
        }
    }
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LanguageServer.Telemetry
{
    public class TelemetryProvider : ITelemetryProvider
    {
        private readonly ILanguageServerFacade server;

        public TelemetryProvider(ILanguageServerFacade server)
        {
            this.server = server;
        }

        public void PostEvent(BicepTelemetryEvent telemetryEvent)
        {
            if (string.IsNullOrWhiteSpace(telemetryEvent.EventName) || telemetryEvent.Properties?.Count == 0)
            {
                throw new ArgumentException("Invalid telemetryEvent. Event name is either null, empty, consists only " +
                    "of white-space characters or no properties are set on the event");
            }

            server.Window.SendTelemetryEvent(telemetryEvent);
        }
    }
}

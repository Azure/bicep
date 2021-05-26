// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
            if (telemetryEvent is null ||
                string.IsNullOrWhiteSpace(telemetryEvent.EventName) ||
                telemetryEvent.Properties?.Count == 0)
            {
                return;
            }

            server.Window.SendTelemetryEvent(telemetryEvent);
        }
    }
}

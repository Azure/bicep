// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace Bicep.LanguageServer.Telemetry
{
    public class TelemetryProvider : ITelemetryProvider
    {
        public ILanguageServer? LanguageServer { get; set; }

        public void PostEvent(TelemetryEvent telemetryEvent)
        {
            if (telemetryEvent == null || telemetryEvent.Properties?.Count == 0)
            {
                return;
            }

            LanguageServer?.Window.SendTelemetryEvent(telemetryEvent);
        }
    }
}

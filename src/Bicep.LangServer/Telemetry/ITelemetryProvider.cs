// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Telemetry
{
    public interface ITelemetryProvider
    {
        ITelemetryEventBuilder BuildTelemetryEvent(string eventName);

        void PostEvent(TelemetryEvent telemetryEvent);
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Telemetry
{
    public interface ITelemetryProvider
    {
        void PostEvent(BicepTelemetryEvent telemetryEvent);
    }
}

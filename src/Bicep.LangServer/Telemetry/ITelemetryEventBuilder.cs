// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Telemetry
{
    public interface ITelemetryEventBuilder
    {
        ITelemetryEventBuilder Set(string propertyName, object value);

        TelemetryEvent ToEvent();
    }
}

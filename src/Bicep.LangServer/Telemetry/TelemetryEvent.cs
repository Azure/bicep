// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Telemetry
{
    public class TelemetryEvent : TelemetryEventParams
    {
        public string EventName { get; set; }
        public Dictionary<string, string>? Properties { get; set; }

        public TelemetryEvent(string eventName, Dictionary<string, string>? properties)
        {
            EventName = eventName;
            Properties = properties;
        }
    }
}

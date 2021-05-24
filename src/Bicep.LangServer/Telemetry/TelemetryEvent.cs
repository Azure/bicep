// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Telemetry
{
    public class TelemetryEvent : TelemetryEventParams
    {
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public string? EventName { get; set; }

        public static TelemetryEvent Create(string eventName)
        {
            TelemetryEvent telemetryEvent = new TelemetryEvent();
            telemetryEvent.EventName = eventName;
            return telemetryEvent;
        }
    }

    public static class TelemetryExtensions
    {
        public static void Set(this TelemetryEvent telemetryEvent, string name, string value)
        {
            if (!telemetryEvent.Properties.ContainsKey(name))
            {
                telemetryEvent.Properties.Add(name, value);
            }
        }
    }
}

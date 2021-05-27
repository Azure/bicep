// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Telemetry
{
    public record BicepTelemetryEvent : TelemetryEventParams
    {
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public string? EventName { get; set; }

        public static BicepTelemetryEvent Create(string eventName)
        {
            BicepTelemetryEvent telemetryEvent = new BicepTelemetryEvent();
            telemetryEvent.EventName = eventName;
            return telemetryEvent;
        }
    }

    public static class TelemetryExtensions
    {
        public static void Set(this BicepTelemetryEvent telemetryEvent, string name, string value)
        {
            if (!telemetryEvent.Properties.ContainsKey(name))
            {
                telemetryEvent.Properties.Add(name, value);
            }
        }
    }
}

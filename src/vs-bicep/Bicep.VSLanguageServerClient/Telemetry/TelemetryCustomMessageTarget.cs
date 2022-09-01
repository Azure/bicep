// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.Telemetry;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace Bicep.VSLanguageServerClient.Telemetry
{
    /// <summary>
    /// VS lsp doesn't handle 'telemetry/event' and that's by design. This class receives 'telemetry/event' messages
    /// from bicep language server and posts the telemetry event using <see cref="TelemetrySession"/>
    /// </summary>
    public class TelemetryCustomMessageTarget
    {
        private readonly TelemetrySession TelemetrySession;

        public TelemetryCustomMessageTarget(TelemetrySession telemetrySession)
        {
            this.TelemetrySession = telemetrySession;
        }

        [JsonRpcMethod(Methods.TelemetryEventName)]
        public Task HandleTelemetryEventAsync(JToken jToken)
        {
            if (jToken is JObject jObject)
            {
                var telemetryEvent = GetTelemetryEvent(jObject);

                if (telemetryEvent is not null)
                {
                    TelemetrySession.PostEvent(telemetryEvent);
                }
            }

            return Task.CompletedTask;
        }

        public TelemetryEvent? GetTelemetryEvent(JObject jObject)
        {
            var eventName = jObject.Property("eventName", StringComparison.OrdinalIgnoreCase)?.Value.ToString();

            if (!string.IsNullOrWhiteSpace(eventName))
            {
                eventName = "vs/bicep/" + eventName;

                var telemetryEvent = new TelemetryEvent(eventName);

                var properties = jObject.Property("properties", StringComparison.OrdinalIgnoreCase);

                if (properties is not null &&
                    properties.HasValues &&
                    properties.Value.ToObject<IDictionary<string, string>>() is IDictionary<string, string> bicepTelemetryProperties)
                {
                    foreach (var kvp in bicepTelemetryProperties)
                    {
                        telemetryEvent.Properties[kvp.Key] = kvp.Value;
                    }
                }

                return telemetryEvent;
            }

            return null;
        }
    }
}

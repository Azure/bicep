// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Telemetry
{
    public class TelemetryEvent : TelemetryEventParams, ITelemetryEventBuilder
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public TelemetryEvent(string eventName)
        {
            EventName = eventName;
        }

        public string EventName { get; set; }
        public Dictionary<string, object>? Properties => _properties;

        public ITelemetryEventBuilder Set(string propertyName, object value)
        {
            AddProperty(propertyName, value);

            return this;
        }

        private void AddProperty(string key, object value)
        {
            _properties[key] = value;
        }

        public TelemetryEvent ToEvent() => this;
    }
}

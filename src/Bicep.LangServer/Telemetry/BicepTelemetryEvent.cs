// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Telemetry
{
    public record BicepTelemetryEvent : TelemetryEventParams
    {
        public string? EventName { get; set; }

        public Dictionary<string, string>? Properties { get; set; }

        public static BicepTelemetryEvent CreateTopLevelDeclarationSnippetInsertion(string name)
            => new BicepTelemetryEvent
            {
                EventName = TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion,
                Properties = new()
                {
                    ["name"] = name,
                },
            };

        public static BicepTelemetryEvent CreateNestedResourceDeclarationSnippetInsertion(string name)
            => new BicepTelemetryEvent
            {
                EventName = TelemetryConstants.EventNames.NestedResourceDeclarationSnippetInsertion,
                Properties = new()
                {
                    ["name"] = name,
                },
            };

        public static BicepTelemetryEvent CreateResourceBodySnippetInsertion(string name, string type)
            => new BicepTelemetryEvent
            {
                EventName = TelemetryConstants.EventNames.ResourceBodySnippetInsertion,
                Properties = new()
                {
                    ["name"] = name,
                    ["type"] = type,
                },
            };

        public static BicepTelemetryEvent CreateModuleBodySnippetInsertion(string name)
            => new BicepTelemetryEvent
            {
                EventName = TelemetryConstants.EventNames.ModuleBodySnippetInsertion,
                Properties = new()
                {
                    ["name"] = name,
                },
            };

        public static BicepTelemetryEvent CreateObjectBodySnippetInsertion(string name)
            => new BicepTelemetryEvent
            {
                EventName = TelemetryConstants.EventNames.ObjectBodySnippetInsertion,
                Properties = new()
                {
                    ["name"] = name
                },
            };

        public static BicepTelemetryEvent CreateDisableNextLineDiagnostics(string code)
            => new BicepTelemetryEvent
            {
                EventName = TelemetryConstants.EventNames.DisableNextLineDiagnostics,
                Properties = new()
                {
                    ["code"] = code,
                },
            };
    }
}

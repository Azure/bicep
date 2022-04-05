// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Telemetry
{
    public record BicepTelemetryEvent : TelemetryEventParams
    {
        public static class Result
        {
            public const string Succeeded = "Succeeded";
            public const string Canceled = "Canceled";
            public const string Failed = "Failed";
        }

        public string EventName { get; private set; }

        public Dictionary<string, string> Properties { get; private set; }

        private static string ToString(bool f)
        {
            return f ? "true" : "false";
        }

        public BicepTelemetryEvent(string eventName, Dictionary<string, string> properties)
        {
            this.EventName = eventName;
            this.Properties = properties;
        }

        public static BicepTelemetryEvent CreateTopLevelDeclarationSnippetInsertion(string name)
            => new BicepTelemetryEvent(
                eventName: TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                }
            );

        public static BicepTelemetryEvent CreateNestedResourceDeclarationSnippetInsertion(string name)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.NestedResourceDeclarationSnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                }
            );

        public static BicepTelemetryEvent CreateResourceBodySnippetInsertion(string name, string type)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.ResourceBodySnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                    ["type"] = type,
                }
            );

        public static BicepTelemetryEvent CreateModuleBodySnippetInsertion(string name)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.ModuleBodySnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                }
            );

        public static BicepTelemetryEvent CreateObjectBodySnippetInsertion(string name)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.ObjectBodySnippetInsertion,
                properties: new()
                {
                    ["name"] = name
                }
            );

        public static BicepTelemetryEvent InsertResourceSuccess(string resourceType, string apiVersion)
            => new BicepTelemetryEvent
            (
                eventName:  "InsertResource/success",
                properties: new()
                {
                    ["resourceType"] = resourceType,
                    ["apiVersion"] = apiVersion,
                }
            );

        public static BicepTelemetryEvent InsertResourceFailure(string failureType)
            => new BicepTelemetryEvent
            (
                eventName:  "InsertResource/failure",
                properties: new()
                {
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent CreateDisableNextLineDiagnostics(string code)
            => new BicepTelemetryEvent
            (
                eventName:  TelemetryConstants.EventNames.DisableNextLineDiagnostics,
                properties: new()
                {
                    ["code"] = code,
                }
            );

        public static BicepTelemetryEvent EditLinterRule(string code, bool newConfigFile, string? error)
            => new BicepTelemetryEvent
            (
                eventName:  TelemetryConstants.EventNames.EditLinterRule,
                properties: new()
                {
                    ["code"] = code,
                    ["newConfigFile"] = ToString(newConfigFile),
                    ["error"] = error ?? string.Empty,
                    ["result"] = error == null ? Result.Succeeded : Result.Failed,
                }
            );

        public static BicepTelemetryEvent CreateLinterRuleStateChangeInBicepConfig(string rule, string prevDiagnosticLevel, string curDiagnosticLevel)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.LinterRuleStateChange,
                properties: new()
                {
                    ["rule"] = rule,
                    ["previousDiagnosticLevel"] = prevDiagnosticLevel,
                    ["currentDiagnosticLevel"] = curDiagnosticLevel
                }
            );

        public static BicepTelemetryEvent CreateOverallLinterStateChangeInBicepConfig(string prevState, string curState)
            => new BicepTelemetryEvent
            (
                eventName:  TelemetryConstants.EventNames.LinterCoreEnabledStateChange,
                properties: new()
                {
                    ["previousState"] = prevState,
                    ["currentState"] = curState
                }
            );

        public static BicepTelemetryEvent CreateLinterStateOnBicepFileOpen(Dictionary<string, string> properties)
            => new BicepTelemetryEvent
            (
                eventName:  TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen,
                properties: properties
            );

        public static BicepTelemetryEvent CreateBicepFileOpen(Dictionary<string, string> properties)
            => new BicepTelemetryEvent
            (
                eventName:  TelemetryConstants.EventNames.BicepFileOpen,
                properties: properties
            );
            {
                EventName = TelemetryConstants.EventNames.BicepFileOpen,
                Properties = properties
            };

        public static BicepTelemetryEvent CreateDeployResult(string requestId, string result)
            => new BicepTelemetryEvent
            {
                EventName = TelemetryConstants.EventNames.DeployResult,
                Properties = new()
                {
                    ["requestId"] = requestId,
                    ["result"] = result
                }
            };
    }
}

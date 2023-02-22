// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Telemetry
{
    public static class EventResult
    {
        public const string Succeeded = "Succeeded";
        public const string Canceled = "Canceled";
        public const string Failed = "Failed";
    }

    public record BicepTelemetryEvent : TelemetryEventParams
    {
        public string EventName { get; private set; }

        public Dictionary<string, string> Properties { get; private set; }

        private static string ToTrueFalse(bool f)
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

        public static BicepTelemetryEvent CreateBicepRegistryOrTemplateSpecShemaCompletion(string schemaName)
            => new BicepTelemetryEvent(
                eventName: TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion,
                properties: new()
                {
                    ["schema"] = schemaName,
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
                eventName: TelemetryConstants.EventNames.InsertResourceSuccess,
                properties: new()
                {
                    ["resourceType"] = resourceType,
                    ["apiVersion"] = apiVersion,
                }
            );

        public static BicepTelemetryEvent InsertResourceFailure(string failureType)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.InsertResourceFailure,
                properties: new()
                {
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent ImportKubernetesManifestSuccess()
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.InsertKubernetesManifestSuccess,
                properties: new()
                {
                    // Properties has to contain some data
                    ["success"] = ToTrueFalse(true),
                }
            );

        public static BicepTelemetryEvent ImportKubernetesManifestFailure(string failureType)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.InsertKubernetesManifestFailure,
                properties: new()
                {
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent CreateDisableNextLineDiagnostics(string code)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.DisableNextLineDiagnostics,
                properties: new()
                {
                    ["code"] = code,
                }
            );

        public static BicepTelemetryEvent EditLinterRule(string code, bool newConfigFile, bool newRuleAdded, string? error)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.EditLinterRule,
                properties: new()
                {
                    ["code"] = code,
                    ["newConfigFile"] = ToTrueFalse(newConfigFile),
                    ["newRuleAdded"] = ToTrueFalse(newRuleAdded),
                    ["error"] = error ?? string.Empty,
                    ["result"] = error == null ? EventResult.Succeeded : EventResult.Failed,
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
                eventName: TelemetryConstants.EventNames.LinterCoreEnabledStateChange,
                properties: new()
                {
                    ["previousState"] = prevState,
                    ["currentState"] = curState
                }
            );

        public static BicepTelemetryEvent CreateLinterStateOnBicepFileOpen(Dictionary<string, string> properties)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen,
                properties: properties
            );

        public static BicepTelemetryEvent CreateBicepFileOpen(Dictionary<string, string> properties)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.BicepFileOpen,
                properties: properties
            );

        public static BicepTelemetryEvent CreateDeployStart(string deployId)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.DeployStart,
                properties: new()
                {
                    ["deployId"] = deployId
                }
            );

        public static BicepTelemetryEvent CreateDeployStartOrWaitForCompletionResult(string eventName, string deployId, bool isSuccess)
            => new BicepTelemetryEvent
            (
                eventName: eventName,
                properties: new()
                {
                    ["deployId"] = deployId,
                    ["result"] = isSuccess ? EventResult.Succeeded : EventResult.Failed
                }
            );

        public static BicepTelemetryEvent DecompileSuccess(string decompileId, int countOutputFiles, int countConflictingFiles)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.DecompileSuccess,
                properties: new()
                {
                    ["decompileId"] = decompileId,
                    ["countOutputFiles"] = countOutputFiles.ToString(),
                    ["countConflictingFiles"] = countConflictingFiles.ToString(),
                }
            );

        public static BicepTelemetryEvent DecompileFailure(string decompileId, string failureType)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.DecompileFailure,
                properties: new()
                {
                    ["decompileId"] = decompileId,
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent DecompileSaveSuccess(string decompileId)
            => new BicepTelemetryEvent
                (
                    eventName: TelemetryConstants.EventNames.DecompileSaveSuccess,
                    properties: new()
                    {
                        ["decompileId"] = decompileId,
                    }
                );

        public static BicepTelemetryEvent DecompileSaveFailure(string decompileId, string failureType)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.DecompileSaveFailure,
                properties: new()
                {
                    ["decompileId"] = decompileId,
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent DecompileForPaste(string decompileId, string? pasteType, int jsonSize, int? bicepSize)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.DecompileForPaste,
                properties: new()
                {
                    ["decompileId"] = decompileId,
                    ["pasteType"] = pasteType ?? string.Empty,
                    ["jsonSize"] = jsonSize.ToString(),
                    ["bicepSize"] = bicepSize?.ToString() ?? string.Empty,
                }
            );

        public static BicepTelemetryEvent UnhandledException(Exception exception)
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.UnhandledException,
                properties: new()
                {
                    ["exception"] = exception.ToString(),
                }
            );

        public static BicepTelemetryEvent MCRPathCompletion()
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.MCRPathCompletion,
                properties: new()
                {
                }
            );

        public static BicepTelemetryEvent ACRPathCompletion()
            => new BicepTelemetryEvent
            (
                eventName: TelemetryConstants.EventNames.ACRPathCompletion,
                properties: new()
                {
                }
            );
    }
}

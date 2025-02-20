// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Telemetry
{
    public static class EventResult
    {
        public const string Succeeded = "Succeeded";
        public const string Canceled = "Canceled";
        public const string Failed = "Failed";
    }

    public static class ModuleRegistryType
    {
        public const string MCR = "MCR";
        public const string ACR = "ACR";
        public const string AcrBasePathFromAlias = "basePathFromAlias";
    }

    public static class ModuleRegistryResolutionType
    {
        public const string AcrVersion = "acrVersion";
        public const string AcrModulePath = "acrPath";
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
            => new(
                eventName: TelemetryConstants.EventNames.TopLevelDeclarationSnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                }
            );

        public static BicepTelemetryEvent CreateNestedResourceDeclarationSnippetInsertion(string name)
            => new(
                eventName: TelemetryConstants.EventNames.NestedResourceDeclarationSnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                }
            );

        public static BicepTelemetryEvent CreateResourceBodySnippetInsertion(string name, string type)
            => new(
                eventName: TelemetryConstants.EventNames.ResourceBodySnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                    ["type"] = type,
                }
            );

        public static BicepTelemetryEvent CreateModuleBodySnippetInsertion(string name)
            => new(
                eventName: TelemetryConstants.EventNames.ModuleBodySnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                }
            );

        public static BicepTelemetryEvent CreateTestBodySnippetInsertion(string name)
            => new(
                eventName: TelemetryConstants.EventNames.TestBodySnippetInsertion,
                properties: new()
                {
                    ["name"] = name,
                }
            );

        public static BicepTelemetryEvent CreateObjectBodySnippetInsertion(string name)
            => new(
                eventName: TelemetryConstants.EventNames.ObjectBodySnippetInsertion,
                properties: new()
                {
                    ["name"] = name
                }
            );

        public static BicepTelemetryEvent InsertResourceSuccess(string resourceType, string apiVersion)
            => new(
                eventName: TelemetryConstants.EventNames.InsertResourceSuccess,
                properties: new()
                {
                    ["resourceType"] = resourceType,
                    ["apiVersion"] = apiVersion,
                }
            );

        public static BicepTelemetryEvent InsertResourceFailure(string failureType)
            => new(
                eventName: TelemetryConstants.EventNames.InsertResourceFailure,
                properties: new()
                {
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent ImportKubernetesManifestSuccess()
            => new(
                eventName: TelemetryConstants.EventNames.InsertKubernetesManifestSuccess,
                properties: new()
                {
                    // Properties has to contain some data
                    ["success"] = ToTrueFalse(true),
                }
            );

        public static BicepTelemetryEvent ImportKubernetesManifestFailure(string failureType)
            => new(
                eventName: TelemetryConstants.EventNames.InsertKubernetesManifestFailure,
                properties: new()
                {
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent CreateDisableNextLineDiagnostics(string code)
            => new(
                eventName: TelemetryConstants.EventNames.DisableNextLineDiagnostics,
                properties: new()
                {
                    ["code"] = code,
                }
            );

        public static BicepTelemetryEvent EditLinterRule(string code, bool newConfigFile, bool newRuleAdded, string? error)
            => new(
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
            => new(
                eventName: TelemetryConstants.EventNames.LinterRuleStateChange,
                properties: new()
                {
                    ["rule"] = rule,
                    ["previousDiagnosticLevel"] = prevDiagnosticLevel,
                    ["currentDiagnosticLevel"] = curDiagnosticLevel
                }
            );

        public static BicepTelemetryEvent CreateOverallLinterStateChangeInBicepConfig(string prevState, string curState)
            => new(
                eventName: TelemetryConstants.EventNames.LinterCoreEnabledStateChange,
                properties: new()
                {
                    ["previousState"] = prevState,
                    ["currentState"] = curState
                }
            );

        public static BicepTelemetryEvent CreateLinterStateOnBicepFileOpen(Dictionary<string, string> properties)
            => new(
                eventName: TelemetryConstants.EventNames.LinterRuleStateOnBicepFileOpen,
                properties: properties
            );

        public static BicepTelemetryEvent CreateBicepFileOpen(Dictionary<string, string> properties)
            => new(
                eventName: TelemetryConstants.EventNames.BicepFileOpen,
                properties: properties
            );

        public static BicepTelemetryEvent CreateBicepParamFileOpen(Dictionary<string, string> properties)
            => new(
                eventName: TelemetryConstants.EventNames.BicepParamFileOpen,
                properties: properties
            );

        public static BicepTelemetryEvent CreateDeployStart(string deployId)
            => new(
                eventName: TelemetryConstants.EventNames.DeployStart,
                properties: new()
                {
                    ["deployId"] = deployId
                }
            );

        public static BicepTelemetryEvent CreateDeployStartOrWaitForCompletionResult(string eventName, string deployId, bool isSuccess)
            => new(
                eventName: eventName,
                properties: new()
                {
                    ["deployId"] = deployId,
                    ["result"] = isSuccess ? EventResult.Succeeded : EventResult.Failed
                }
            );

        public static BicepTelemetryEvent DecompileSuccess(string decompileId, int countOutputFiles, int countConflictingFiles)
            => new(
                eventName: TelemetryConstants.EventNames.DecompileSuccess,
                properties: new()
                {
                    ["decompileId"] = decompileId,
                    ["countOutputFiles"] = countOutputFiles.ToString(),
                    ["countConflictingFiles"] = countConflictingFiles.ToString(),
                }
            );

        public static BicepTelemetryEvent DecompileFailure(string decompileId, string failureType)
            => new(
                eventName: TelemetryConstants.EventNames.DecompileFailure,
                properties: new()
                {
                    ["decompileId"] = decompileId,
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent DecompileSaveSuccess(string decompileId)
            => new(
                    eventName: TelemetryConstants.EventNames.DecompileSaveSuccess,
                    properties: new()
                    {
                        ["decompileId"] = decompileId,
                    }
                );

        public static BicepTelemetryEvent DecompileSaveFailure(string decompileId, string failureType)
            => new(
                eventName: TelemetryConstants.EventNames.DecompileSaveFailure,
                properties: new()
                {
                    ["decompileId"] = decompileId,
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent DecompileForPaste(string decompileId, string? pasteContext, string? pasteType, int jsonSize, int? bicepSize, string? languageId)
            => new(
                eventName: TelemetryConstants.EventNames.DecompileForPaste,
                properties: new()
                {
                    ["decompileId"] = decompileId,
                    ["pasteContext"] = pasteContext?.ToString() ?? string.Empty,
                    ["pasteType"] = pasteType ?? string.Empty,
                    ["jsonSize"] = jsonSize.ToString(),
                    ["bicepSize"] = bicepSize?.ToString() ?? string.Empty,
                    ["languageId"] = languageId ?? string.Empty,
                }
            );

        public static BicepTelemetryEvent UnhandledException(Exception exception)
            => new(
                eventName: TelemetryConstants.EventNames.UnhandledException,
                properties: new()
                {
                    ["exception"] = exception.ToString(),
                }
            );

        public static BicepTelemetryEvent ModuleRegistryPathCompletion(string moduleRegistryType, bool isAlias, int? totalRepos)
            => new(
                eventName: TelemetryConstants.EventNames.ModuleRegistryPathCompletion,
                properties: new()
                {
                    ["moduleRegistryType"] = moduleRegistryType,
                    ["isAlias"] = ToTrueFalse(isAlias),
                    ["totalRepos"] = totalRepos.HasValue ? totalRepos.Value.ToString() : string.Empty,
                }
            );

        public static BicepTelemetryEvent ModuleRegistryResolution(string resolutionType)
            => new(
                eventName: TelemetryConstants.EventNames.ModuleRegistryResolution,
                properties: new()
                {
                    ["type"] = resolutionType,
                }
            );

        public enum ExternalSourceRequestType
        {
            CompiledJson, // main.json
            BicepEntrypoint,
            Local, // A file included with the compilation group
            NestedExternal, // An external module
            Unknown,
        }

        public static BicepTelemetryEvent ExternalSourceRequestSuccess(bool hasSource, int archiveFilesCount, string fileExtension, ExternalSourceRequestType requestType)
            => new(
                eventName: TelemetryConstants.EventNames.ExternalSourceRequestSuccess,
                properties: new()
                {
                    ["hasSource"] = ToTrueFalse(hasSource),
                    ["archiveFilesCount"] = archiveFilesCount.ToString(),
                    ["fileExtension"] = fileExtension,
                    ["requestType"] = Enum.GetName(typeof(ExternalSourceRequestType), requestType) ?? string.Empty,
                }
            );

        public static BicepTelemetryEvent ExternalSourceRequestFailure(string failureType)
            => new(
                eventName: TelemetryConstants.EventNames.ExternalSourceRequestFailure,
                properties: new()
                {
                    ["failureType"] = failureType,
                }
            );

        public static BicepTelemetryEvent ExternalSourceDocLinkClickSuccess(ExternalSourceRequestType requestType, string moduleRegistryType)
            => new(
                eventName: TelemetryConstants.EventNames.ExternalSourceDocLinkClickSuccess,
                properties: new()
                {
                    ["requestType"] = Enum.GetName(typeof(ExternalSourceRequestType), requestType) ?? string.Empty,
                    ["registryType"] = moduleRegistryType,
                }
            );

        public static BicepTelemetryEvent ExternalSourceDocLinkClickFailure(string failureType, string? code = null)
            => new(
                eventName: TelemetryConstants.EventNames.ExternalSourceDocLinkClickFailure,
                properties: new()
                {
                    ["failureType"] = failureType,
                    ["code"] = code ?? string.Empty,
                }
            );

        public enum ExtractionKind
        {
            Variable,
            SimpleParam,    // Extract parameter when only simple type is available
            UserDefParam,          // Extract parameter with user-defined type (both simple and user-defined-type params are available)
            ResDerivedParam,    // Extract parameter with resource-derived type
            Type,
        }

        public record ExtractKindsAvailable(bool simpleTypeAvailable, bool userDefinedTypeAvailable, bool resourceDerivedTypeAvailable);

        public static BicepTelemetryEvent ExtractionRefactoring(
            ExtractionKind extractionKind,
            ExtractKindsAvailable extractKindsAvailable)
            => new(
                eventName: TelemetryConstants.EventNames.ExtractionRefactoring,
                properties: new()
                {
                    ["kind"] = StringUtils.ToCamelCase(extractionKind.ToString()),
                    ["simpleParamAvail"] = ToTrueFalse(extractKindsAvailable.simpleTypeAvailable),
                    ["userParamAvail"] = ToTrueFalse(extractKindsAvailable.userDefinedTypeAvailable),
                    ["resDerivedParamAvail"] = ToTrueFalse(extractKindsAvailable.resourceDerivedTypeAvailable),
                }
            );
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Telemetry
{
    public static class TelemetryConstants
    {
        public const string CommandName = "bicep.Telemetry";

        public static class EventNames
        {
            public const string BicepFileOpen = "file/bicepopen";
            public const string BicepParamFileOpen = "file/bicepparamopen";

            public const string NestedResourceDeclarationSnippetInsertion = "snippet/nestedresource";
            public const string TopLevelDeclarationSnippetInsertion = "snippet/toplevel";
            public const string ResourceBodySnippetInsertion = "snippet/resourcebody";
            public const string ModuleBodySnippetInsertion = "snippet/modulebody";
            public const string TestBodySnippetInsertion = "snippet/testbody";
            public const string ObjectBodySnippetInsertion = "snippet/object";

            public const string DecompileSuccess = "decompile/success";
            public const string DecompileFailure = "decompile/failure";
            public const string DecompileSaveSuccess = "decompileSave/success";
            public const string DecompileSaveFailure = "decompileSave/failure";

            public const string DecompileForPaste = "decompileForPaste";

            public const string InsertResourceSuccess = "InsertResource/success";
            public const string InsertResourceFailure = "InsertResource/failure";

            public const string InsertKubernetesManifestSuccess = "ImportKubernetesManifest/success";
            public const string InsertKubernetesManifestFailure = "ImportKubernetesManifest/failure";

            public const string DeployResult = "deploy/result";
            public const string DeployStart = "deploy/start";
            public const string DeployStartResult = "deploy/startresult";
            public const string DisableNextLineDiagnostics = "diagnostics/disablenextline";
            public const string EditLinterRule = "diagnostics/editLinterRule";

            // Rule names are all in lower case to help ease querying. The names get lowercased before they are stored.
            // So doing it upfront here will avoid confusion while querying.
            public const string LinterCoreEnabledStateChange = "linter/coreenabledstatechange";
            public const string LinterRuleStateChange = "linter/rulestatechange";
            public const string LinterRuleStateOnBicepFileOpen = "linter/rulestateonopen";
            public const string UnhandledException = "unhandledException";

            public const string ModuleRegistryPathCompletion = "ModuleRegistryPathCompletion";
            public const string ModuleRegistryResolution = "ModuleRegistryResolution";

            public const string ExternalSourceRequestSuccess = "ExternalSourceRequest/success";
            public const string ExternalSourceRequestFailure = "ExternalSourceRequest/failure";
            public const string ExternalSourceDocLinkClickSuccess = "ExternalSourceDocLinkClick/success";
            public const string ExternalSourceDocLinkClickFailure = "ExternalSourceDocLinkClick/failure";

            public const string ExtractionRefactoring = "refactoring/extraction";

            public const string ExtractToModuleSuccess = "ExtractToModule/success";
            public const string ExtractToModuleFailure = "ExtractToModule/failure";
        }
    }
}

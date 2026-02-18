// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer
{
    public static class LangServerConstants
    {
        public const string BuildCommand = "build";
        public const string DecompileForPasteCommand = "decompileForPaste";
        public const string DecompileCommand = "decompile";
        public const string DecompileParamsCommand = "decompileParams";
        public const string DecompileSaveCommand = "decompileSave";
        public const string GenerateParamsCommand = "generateParams";
        public const string BuildParamsCommand = "buildParams";
        public const string SnapshotCommand = "snapshot";
        public const string DeployCompleteMethod = "deploymentComplete";
        public const string DeployStartCommand = "deploy/start";
        public const string DeployWaitForCompletionCommand = "deploy/waitForCompletion";
        public const string GetDeploymentParametersCommand = "getDeploymentParameters";
        public const string GetDeploymentScopeCommand = "getDeploymentScope";
        public const string ForceModulesRestoreCommand = "forceModulesRestore";
        public const string ImportKubernetesManifestCommand = "importKubernetesManifest";
        public const string CreateConfigFile = "createConfigFile";
        // An internal-only command used in code actions to edit a particular rule in the bicepconfig.json file
        public const string EditLinterRuleCommandName = "bicep.EditLinterRule";

        // This is under "bicep.completions" in configuration
        public const string GetAllAzureContainerRegistriesForCompletionsSetting = "getAllAccessibleAzureContainerRegistries";

        public const string ExternalSourceFileScheme = "bicep-extsrc";
    }
}

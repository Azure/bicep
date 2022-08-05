// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer
{
    public static class LangServerConstants
    {
        public const string BuildCommand = "build";
        public const string GenerateParamsCommand = "generateParams";
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
    }
}

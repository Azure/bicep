// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer
{
    public static class LangServerConstants
    {
        public const string BuildCommand = "build";
        public const string DeployCommand = "deploy";
        public const string GetDeploymentScopeCommand = "getDeploymentScope";
        public const string ForceModulesRestoreCommand = "forceModulesRestore";
        public const string CreateConfigFile = "bicep.createConfigFile"; //asdfg?
        // An internal-only command used in code actions to edit a particular rule in the bicepconfig.json file
        public const string EditLinterRuleCommandName = "bicep.EditLinterRule";
    }
}

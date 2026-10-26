// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export { activateDeploymentFeature } from "./activation";
export { AzurePickers } from "./azure/azure-pickers";
export { AzureUIManager, type IAzureUIManager } from "./azure/azure-ui-manager";
export { DeployCommand } from "./commands";
export {
  registerDeploymentOutputNotifications,
  removePropertiesWithPossibleUserInfoInDeployParams,
} from "./deployment-output";
export { DeployPaneViewManager } from "./pane";
export { ShowDeployPaneCommand, ShowDeployPaneToSideCommand } from "./show-deploy-pane";
export {
  type BicepDeploymentParametersResponse,
  type BicepDeploymentScopeParams,
  type BicepDeploymentScopeResponse,
  type BicepDeploymentStartParams,
  type BicepDeploymentStartResponse,
  type BicepDeploymentWaitForCompletionParams,
  type BicepUpdatedDeploymentParameter,
  getDeploymentDataRequestType,
  type LocalDeployResponse,
  localDeployRequestType,
  ParametersFileUpdateOption,
} from "./protocol";

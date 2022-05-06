// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { BicepDeploymentScopeResponse } from "../language/protocol";
import { OutputChannelManager } from "./OutputChannelManager";

let bicepOperationsOutputChannelManager: OutputChannelManager | undefined;

export function handleDeploymentScopeNotification(
  bicepDeploymentScopeResponse: BicepDeploymentScopeResponse
) {
  const template = bicepDeploymentScopeResponse?.template;

  if (!bicepOperationsOutputChannelManager) {
    return;
  }

  if (!template) {
    bicepOperationsOutputChannelManager.appendToOutputChannel(
      "Unable to deploy. Please fix below errors:\n " +
        bicepDeploymentScopeResponse?.errorMessage
    );
    return;
  }
  bicepOperationsOutputChannelManager.appendToOutputChannel("test");
}

export function setOutputChannelManager(
  outputChannelManager: OutputChannelManager
) {
  bicepOperationsOutputChannelManager = outputChannelManager;
}

export function getOutputChannelManager(): OutputChannelManager | undefined {
  return bicepOperationsOutputChannelManager;
}

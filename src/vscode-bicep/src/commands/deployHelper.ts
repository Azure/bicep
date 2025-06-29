// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { OutputChannelManager } from "../utils/OutputChannelManager";

let _outputChannelManager: OutputChannelManager;

export async function writeDeploymentOutputMessageToBicepOperationsOutputChannel(outputMessage: string) {
  if (_outputChannelManager) {
    // Currently getting messages like this for failed deployments:
    //
    //   11:53:56 AM: Deployment failed for main.bicep. At least one resource deployment operation failed...
    //   Status: 200 (OK)
    //   ErrorCode: DeploymentFailed
    //   Service request succeeded. Response content and headers are not included to avoid logging sensitive data.
    //
    // The "service request succeeded" refers to the request to get the deployment status, not the deployment itself.
    // It's confusing so we'll just remove it.
    outputMessage = outputMessage.replaceAll("Service request succeeded. ", "");

    _outputChannelManager.appendToOutputChannel(outputMessage);
  }
}

export function setOutputChannelManagerAtTheStartOfDeployment(outputChannelManager: OutputChannelManager) {
  _outputChannelManager = outputChannelManager;
}

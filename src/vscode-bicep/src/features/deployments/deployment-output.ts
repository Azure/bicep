// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Disposable } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { OutputChannelManager } from "../../infrastructure/logging";

const deployParamsPattern = new RegExp('(?<lhs>"token":\\s*")(?<token>[^"]+)"', "g");

export function registerDeploymentOutputNotifications(
  languageClient: LanguageClient,
  outputChannelManager: OutputChannelManager,
): Disposable {
  return languageClient.onNotification("deploymentComplete", (outputMessage: string) => {
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

    outputChannelManager.appendToOutputChannel(outputMessage);
  });
}

export function removePropertiesWithPossibleUserInfoInDeployParams(value: string): string {
  return value.replace(deployParamsPattern, '$<lhs><REDACTED: token>"');
}

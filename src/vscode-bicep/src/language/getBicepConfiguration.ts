// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IAzExtOutputChannel } from "@microsoft/vscode-azext-utils";
import { ConfigurationScope, workspace, WorkspaceConfiguration } from "vscode";
import { bicepConfigurationPrefix } from "./constants";

export function getBicepConfiguration(
  scope?: ConfigurationScope,
  outputChannel?: IAzExtOutputChannel
): WorkspaceConfiguration {
  outputChannel?.appendLine("getBicepConfiguration start");
  const a = workspace.getConfiguration(bicepConfigurationPrefix, scope);
  outputChannel?.appendLine("getBicepConfiguration end");
  return a;
}

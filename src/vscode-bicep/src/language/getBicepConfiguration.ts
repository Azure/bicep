// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ConfigurationScope, workspace, WorkspaceConfiguration } from "vscode";
import { bicepConfigurationPrefix } from "./constants";

export function getBicepConfiguration(
  scope?: ConfigurationScope,
): WorkspaceConfiguration {
  return workspace.getConfiguration(bicepConfigurationPrefix, scope);
}

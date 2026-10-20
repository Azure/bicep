// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ConfigurationScope, workspace, WorkspaceConfiguration } from "vscode";

export const bicepConfigurationPrefix = "bicep";

export function getBicepConfiguration(scope?: ConfigurationScope): WorkspaceConfiguration {
  return workspace.getConfiguration(bicepConfigurationPrefix, scope);
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext } from "@microsoft/vscode-azext-utils";
import { ExtensionContext } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { CommandManager } from "../../infrastructure/commands";
import { DiagnosticsRouter } from "../../infrastructure/language-client";
import { Disposable } from "../../infrastructure/lifecycle";
import { OutputChannelManager } from "../../infrastructure/logging";
import { AzurePickers } from "./azure/azure-pickers";
import { AzureUIManager } from "./azure/azure-ui-manager";
import { DeployCommand } from "./commands";
import { registerDeploymentOutputNotifications } from "./deployment-output";
import { DeployPaneViewManager } from "./pane/deploy-pane-view-manager";
import { ShowDeployPaneCommand, ShowDeployPaneToSideCommand } from "./show-deploy-pane";

export async function activateDeploymentFeature(
  extension: Disposable,
  actionContext: IActionContext,
  extensionContext: ExtensionContext,
  commandManager: CommandManager,
  languageClient: LanguageClient,
  outputChannelManager: OutputChannelManager,
  diagnosticsRouter: DiagnosticsRouter,
): Promise<void> {
  const azurePickers = extension.register(new AzurePickers(outputChannelManager));
  const deployPaneViewManager = extension.register(
    new DeployPaneViewManager(
      actionContext,
      extensionContext,
      extensionContext.extensionUri,
      languageClient,
      new AzureUIManager(actionContext, azurePickers),
      diagnosticsRouter,
    ),
  );

  extension.register(registerDeploymentOutputNotifications(languageClient, outputChannelManager));
  await commandManager.registerCommands(
    new DeployCommand(languageClient, outputChannelManager, azurePickers),
    new ShowDeployPaneCommand(deployPaneViewManager),
    new ShowDeployPaneToSideCommand(deployPaneViewManager),
  );
}

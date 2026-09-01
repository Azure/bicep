// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ExtensionContext } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { CommandManager } from "../../infrastructure/commands";
import { DiagnosticsRouter } from "../../infrastructure/language-client";
import { Disposable } from "../../infrastructure/lifecycle";
import { OutputChannelManager } from "../../infrastructure/logging";
import { Prompts } from "../../infrastructure/prompts";
import { AzurePickers } from "./azure/azure-pickers";
import { AzureUIManager } from "./azure/azure-ui-manager";
import { DeployCommand } from "./commands";
import { registerDeploymentOutputNotifications } from "./deployment-output";
import { DeployPaneViewManager } from "./pane/deploy-pane-view-manager";
import { ShowDeployPaneCommand, ShowDeployPaneToSideCommand } from "./show-deploy-pane";

export async function activateDeploymentFeature(
  extension: Disposable,
  prompts: Prompts,
  extensionContext: ExtensionContext,
  commandManager: CommandManager,
  languageClient: LanguageClient,
  outputChannelManager: OutputChannelManager,
  diagnosticsRouter: DiagnosticsRouter,
): Promise<void> {
  const azurePickers = extension.register(new AzurePickers(prompts, outputChannelManager));
  const deployPaneViewManager = extension.register(
    new DeployPaneViewManager(
      prompts,
      extensionContext,
      extensionContext.extensionUri,
      languageClient,
      new AzureUIManager(azurePickers),
      diagnosticsRouter,
    ),
  );

  extension.register(registerDeploymentOutputNotifications(languageClient, outputChannelManager));
  await commandManager.registerCommands(
    new DeployCommand(prompts, languageClient, outputChannelManager, azurePickers),
    new ShowDeployPaneCommand(prompts, deployPaneViewManager),
    new ShowDeployPaneToSideCommand(prompts, deployPaneViewManager),
  );
}

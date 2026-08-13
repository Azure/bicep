// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { registerAzureUtilsExtensionVariables } from "@microsoft/vscode-azext-azureutils";
import { registerUIExtensionVariables } from "@microsoft/vscode-azext-utils";
import {
  ExtensionContext,
  ProgressLocation,
  Uri,
  window,
} from "vscode";
import * as lsp from "vscode-languageclient/node";
import { AzureUiManager } from "./azure/AzureUiManager";
import { CommandManager } from "./infrastructure/commands";
import { DeployCommand } from "./commands/deploy";
import { ShowDeployPaneCommand, ShowDeployPaneToSideCommand } from "./commands/showDeployPane";
import { ShowSourceFromVisualizerCommand } from "./commands/showSourceFromVisualizer";
import { ShowVisualizerCommand, ShowVisualizerToSideCommand } from "./commands/showVisualizer";
import { activateBuildFeature } from "./features/build";
import { activateConfigurationFeature } from "./features/configuration";
import { activateDecompileFeature } from "./features/decompile";
import { activateExternalSourceFeature } from "./features/external-source";
import { activateImportKubernetesManifestFeature } from "./features/import-kubernetes-manifest";
import { activateInsertResourceFeature } from "./features/insert-resource";
import { activateModuleRestoreFeature } from "./features/module-restore";
import { activateMcpFeature } from "./features/mcp";
import { activateParametersFeature } from "./features/parameters";
import { activatePasteAsBicepFeature } from "./features/paste-as-bicep";
import { activateRefactoringFeature } from "./features/refactoring";
import * as surveys from "./features/surveys";
import { activateWalkthroughFeature } from "./features/walkthrough";
import { setGlobalStateKeysToSyncBetweenMachines } from "./globalState";
import { bicepConfigurationPrefix } from "./infrastructure/configuration";
import { bicepLanguageId } from "./infrastructure/editor";
import { createLanguageService, ensureDotnetRuntimeInstalled } from "./infrastructure/language-client";
import { DeployPaneViewManager } from "./panes/deploy";
import { Disposable } from "./infrastructure/lifecycle";
import {
  activateWithTelemetryAndErrorHandling,
  createAzExtOutputChannel,
  createLogger,
  getLogger,
  OutputChannelManager,
  resetLogger,
} from "./infrastructure/logging";
import { AzurePickers } from "./utils/AzurePickers";
import { BicepVisualizerViewManager } from "./visualizer";

let languageClient: lsp.LanguageClient | null = null;

class BicepExtension extends Disposable {
  private constructor(public readonly extensionUri: Uri) {
    super();
  }

  public static create(context: ExtensionContext) {
    const extension = new BicepExtension(context.extensionUri);
    context.subscriptions.push(extension);

    return extension;
  }
}

export async function activate(extensionContext: ExtensionContext): Promise<void> {
  const extension = BicepExtension.create(extensionContext);
  const outputChannel = createAzExtOutputChannel("Bicep", bicepConfigurationPrefix);

  extension.register(outputChannel);
  extension.register(createLogger(extensionContext, outputChannel));

  registerUIExtensionVariables({ context: extensionContext, outputChannel });
  registerAzureUtilsExtensionVariables({
    context: extensionContext,
    outputChannel,
    prefix: bicepLanguageId,
  });

  // Activate and launch language server
  await activateWithTelemetryAndErrorHandling(async (actionContext) => {
    await window.withProgress(
      {
        location: ProgressLocation.Window,
      },
      async (progress) => {
        progress.report({ message: "Acquiring dotnet runtime" });
        const dotnetCommandPath = await ensureDotnetRuntimeInstalled(actionContext);

        progress.report({ message: "Launching language service" });
        languageClient = await createLanguageService(extensionContext, outputChannel, dotnetCommandPath);

        progress.report({ message: "Registering commands" });
        setGlobalStateKeysToSyncBetweenMachines(extensionContext.globalState);

        // Show appropriate surveys
        surveys.showSurveys(extensionContext.globalState);

        const viewManager = extension.register(new BicepVisualizerViewManager(extension.extensionUri, languageClient));

        const outputChannelManager = extension.register(
          new OutputChannelManager("Bicep Operations", bicepConfigurationPrefix),
        );

        const azurePickers = extension.register(new AzurePickers(outputChannelManager));

        const deployPaneViewManager = extension.register(
          new DeployPaneViewManager(
            actionContext,
            extensionContext,
            extension.extensionUri,
            languageClient,
            new AzureUiManager(actionContext, azurePickers),
          ),
        );

        // Register commands.
        const commandManager = extension.register(new CommandManager(extensionContext));
        await activateBuildFeature(commandManager, languageClient, outputChannelManager);
        await activateParametersFeature(commandManager, languageClient, outputChannelManager);
        await activateConfigurationFeature(commandManager, languageClient);
        await commandManager.registerCommands(new DeployCommand(languageClient, outputChannelManager, azurePickers));
        await activateDecompileFeature(extension, commandManager, languageClient, outputChannelManager);
        await activateModuleRestoreFeature(commandManager, languageClient, outputChannelManager);
        await activateInsertResourceFeature(commandManager, languageClient);
        await commandManager.registerCommands(
          new ShowDeployPaneCommand(deployPaneViewManager),
          new ShowDeployPaneToSideCommand(deployPaneViewManager),
          new ShowVisualizerCommand(viewManager),
          new ShowVisualizerToSideCommand(viewManager),
          new ShowSourceFromVisualizerCommand(viewManager),
        );
        await activateWalkthroughFeature(commandManager);
        await activatePasteAsBicepFeature(extension, commandManager, languageClient, outputChannelManager);
        await activateImportKubernetesManifestFeature(commandManager, languageClient);
        await activateExternalSourceFeature(extension, commandManager, languageClient);
        await activateRefactoringFeature(commandManager);

        await languageClient.start();
        getLogger().info("Bicep language service started.");

        activateMcpFeature(extension, extensionContext, dotnetCommandPath);

      },
    );
  });
}

export async function deactivate(): Promise<void> {
  await languageClient?.stop();
  getLogger().info("Bicep language service stopped.");

  resetLogger();
}

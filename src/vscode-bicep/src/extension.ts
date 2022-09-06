// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode from "vscode";
import { BicepVisualizerViewManager } from "./visualizer";
import { createAzExtOutputChannel } from "./utils/AzExtOutputChannel";
import { OutputChannelManager } from "./utils/OutputChannelManager";
import * as lsp from "vscode-languageclient/node";
import { registerAzureUtilsExtensionVariables } from "@microsoft/vscode-azext-azureutils";
import { registerUIExtensionVariables } from "@microsoft/vscode-azext-utils";
import { TreeManager } from "./tree/TreeManager";
import {
  BuildCommand,
  CommandManager,
  DeployCommand,
  ForceModulesRestoreCommand,
  GenerateParamsCommand,
  InsertResourceCommand,
  ShowSourceCommand,
  ShowVisualizerCommand,
  ShowVisualizerToSideCommand,
  WalkthroughCopyToClipboardCommand,
  WalkthroughCreateBicepFileCommand,
  WalkthroughOpenBicepFileCommand,
} from "./commands";
import { BicepCacheContentProvider, createLanguageService } from "./language";
import {
  activateWithTelemetryAndErrorHandling,
  createLogger,
  Disposable,
  getLogger,
  resetLogger,
} from "./utils";
import { CreateBicepConfigurationFile } from "./commands/createConfigurationFile";
import { ImportKubernetesManifestCommand } from "./commands/importKubernetesManifest";

let languageClient: lsp.LanguageClient | null = null;

class BicepExtension extends Disposable {
  private constructor(public readonly extensionUri: vscode.Uri) {
    super();
  }

  public static create(context: vscode.ExtensionContext) {
    const extension = new BicepExtension(context.extensionUri);
    context.subscriptions.push(extension);

    return extension;
  }
}

export async function activateWithProgressReport(
  activateFunc: () => Promise<void>
): Promise<void> {
  return await vscode.window.withProgress(
    {
      title: "Launching Bicep language service...",
      location: vscode.ProgressLocation.Notification,
    },
    activateFunc
  );
}

export async function activate(
  context: vscode.ExtensionContext
): Promise<void> {
  const extension = BicepExtension.create(context);
  const outputChannel = createAzExtOutputChannel("Bicep", "bicep");

  extension.register(outputChannel);
  extension.register(createLogger(context, outputChannel));

  registerUIExtensionVariables({ context, outputChannel });
  registerAzureUtilsExtensionVariables({
    context,
    outputChannel,
    prefix: "bicep",
  });

  await activateWithTelemetryAndErrorHandling(
    async (actionContext) =>
      await activateWithProgressReport(async () => {
        languageClient = await createLanguageService(
          actionContext,
          context,
          outputChannel
        );

        // go2def links that point to the bicep cache will have the bicep-cache scheme in their document URIs
        // this content provider will allow VS code to understand that scheme
        // and surface the content as a read-only file
        extension.register(
          vscode.workspace.registerTextDocumentContentProvider(
            "bicep-cache",
            new BicepCacheContentProvider(languageClient)
          )
        );

        const viewManager = extension.register(
          new BicepVisualizerViewManager(extension.extensionUri, languageClient)
        );

        const outputChannelManager = extension.register(
          new OutputChannelManager("Bicep Operations", "bicep")
        );

        const treeManager = extension.register(
          new TreeManager(outputChannelManager)
        );

        // Register commands.
        await extension
          .register(new CommandManager(context))
          .registerCommands(
            new BuildCommand(languageClient, outputChannelManager),
            new GenerateParamsCommand(languageClient, outputChannelManager),
            new CreateBicepConfigurationFile(languageClient),
            new DeployCommand(
              languageClient,
              outputChannelManager,
              treeManager
            ),
            new ForceModulesRestoreCommand(
              languageClient,
              outputChannelManager
            ),
            new InsertResourceCommand(languageClient),
            new ShowVisualizerCommand(viewManager),
            new ShowVisualizerToSideCommand(viewManager),
            new ShowSourceCommand(viewManager),
            new WalkthroughCopyToClipboardCommand(),
            new WalkthroughCreateBicepFileCommand(),
            new WalkthroughOpenBicepFileCommand(),
            new ImportKubernetesManifestCommand(languageClient)
          );

        await languageClient.start();
        getLogger().info("Bicep language service started.");
      })
  );
}

export async function deactivate(): Promise<void> {
  await languageClient?.stop();
  getLogger().info("Bicep language service stopped.");

  resetLogger();
}

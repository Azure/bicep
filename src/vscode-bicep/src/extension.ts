// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { registerAzureUtilsExtensionVariables } from "@microsoft/vscode-azext-azureutils";
import { registerUIExtensionVariables } from "@microsoft/vscode-azext-utils";
import {
  ExtensionContext,
  ProgressLocation,
  TextDocument,
  TextEditor,
  Uri,
  window,
  workspace,
} from "vscode";
import * as lsp from "vscode-languageclient/node";
import { CreateBicepConfigurationFile } from "./commands/createConfigurationFile";
import { DecompileCommand } from "./commands/decompile";
import { ImportKubernetesManifestCommand } from "./commands/importKubernetesManifest";
import { PasteAsBicepCommand } from "./commands/pasteAsBicep";
import { BicepCacheContentProvider, createLanguageService } from "./language";
import { TreeManager } from "./tree/TreeManager";
import { updateUiContext } from "./updateUiContext";
import { createAzExtOutputChannel } from "./utils/AzExtOutputChannel";
import { OutputChannelManager } from "./utils/OutputChannelManager";
import { BicepVisualizerViewManager } from "./visualizer";
import {
  bicepConfigurationPrefix,
  bicepLanguageId,
} from "./language/constants";
import { SuppressedWarningsManager } from "./commands/SuppressedWarningsManager";
import { Disposable } from "./utils/disposable";
import { createLogger, getLogger, resetLogger } from "./utils/logger";
import { activateWithTelemetryAndErrorHandling } from "./utils/telemetry";
import { InsertResourceCommand } from "./commands/insertResource";
import {
  ShowVisualizerCommand,
  ShowVisualizerToSideCommand,
} from "./commands/showVisualizer";
import { ShowSourceCommand } from "./commands/showSource";
import { WalkthroughCopyToClipboardCommand } from "./commands/gettingStarted/WalkthroughCopyToClipboardCommand";
import { WalkthroughCreateBicepFileCommand } from "./commands/gettingStarted/WalkthroughCreateBicepFileCommand";
import { WalkthroughOpenBicepFileCommand } from "./commands/gettingStarted/WalkthroughOpenBicepFileCommand";
import { ForceModulesRestoreCommand } from "./commands/forceModulesRestore";
import { DeployCommand } from "./commands/deploy";
import { GenerateParamsCommand } from "./commands/generateParams";
import { BuildCommand } from "./commands/build";
import { CommandManager } from "./commands/commandManager";

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

export async function activateWithProgressReport(
  activateFunc: () => Promise<void>
): Promise<void> {
  return await window.withProgress(
    {
      title: "Launching Bicep language service...",
      location: ProgressLocation.Notification,
    },
    activateFunc
  );
}

export async function activate(context: ExtensionContext): Promise<void> {
  const extension = BicepExtension.create(context);
  const outputChannel = createAzExtOutputChannel(
    "Bicep",
    bicepConfigurationPrefix
  );

  extension.register(outputChannel);
  extension.register(createLogger(context, outputChannel));

  registerUIExtensionVariables({ context, outputChannel });
  registerAzureUtilsExtensionVariables({
    context,
    outputChannel,
    prefix: bicepLanguageId,
  });

  // Launch language server
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
          workspace.registerTextDocumentContentProvider(
            "bicep-cache",
            new BicepCacheContentProvider(languageClient)
          )
        );

        const viewManager = extension.register(
          new BicepVisualizerViewManager(extension.extensionUri, languageClient)
        );

        const outputChannelManager = extension.register(
          new OutputChannelManager("Bicep Operations", bicepConfigurationPrefix)
        );

        const treeManager = extension.register(
          new TreeManager(outputChannelManager)
        );

        const suppressedWarningsManager = new SuppressedWarningsManager();

        // Register commands.
        const pasteAsBicepCommand = new PasteAsBicepCommand(
          languageClient,
          outputChannelManager,
          suppressedWarningsManager
        );
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
            new DecompileCommand(languageClient, outputChannelManager),
            new ForceModulesRestoreCommand(
              languageClient,
              outputChannelManager
            ),
            new InsertResourceCommand(languageClient),
            pasteAsBicepCommand,
            new ShowVisualizerCommand(viewManager),
            new ShowVisualizerToSideCommand(viewManager),
            new ShowSourceCommand(viewManager),
            new WalkthroughCopyToClipboardCommand(),
            new WalkthroughCreateBicepFileCommand(),
            new WalkthroughOpenBicepFileCommand(),
            new ImportKubernetesManifestCommand(languageClient)
          );

        pasteAsBicepCommand.registerForPasteEvents(extension);

        extension.register(
          window.onDidChangeActiveTextEditor(
            // eslint-disable-next-line @typescript-eslint/no-unused-vars
            async (editor: TextEditor | undefined) => {
              await updateUiContext(editor?.document, pasteAsBicepCommand);
            }
          )
        );

        extension.register(
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
          workspace.onDidCloseTextDocument(async (_d: TextDocument) => {
            await updateUiContext(
              window.activeTextEditor?.document,
              pasteAsBicepCommand
            );
          })
        );

        extension.register(
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
          workspace.onDidOpenTextDocument(async (_d: TextDocument) => {
            await updateUiContext(
              window.activeTextEditor?.document,
              pasteAsBicepCommand
            );
          })
        );

        extension.register(
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
          workspace.onDidSaveTextDocument(async (_d: TextDocument) => {
            await updateUiContext(window.activeTextEditor?.document);
          })
        );

        await updateUiContext(
          window.activeTextEditor?.document,
          pasteAsBicepCommand
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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { registerAzureUtilsExtensionVariables } from "@microsoft/vscode-azext-azureutils";
import { registerUIExtensionVariables } from "@microsoft/vscode-azext-utils";
import {
  ExtensionContext,
  lm,
  McpStdioServerDefinition,
  ProgressLocation,
  TextDocument,
  TextEditor,
  Uri,
  window,
  workspace,
} from "vscode";
import * as lsp from "vscode-languageclient/node";
import { AzureUiManager } from "./azure/AzureUiManager";
import { CommandManager } from "./infrastructure/commands";
import { DecompileCommand } from "./commands/decompile";
import { DecompileParamsCommand } from "./commands/decompileParams";
import { DeployCommand } from "./commands/deploy";
import { PasteAsBicepCommand } from "./commands/pasteAsBicep";
import { ShowDeployPaneCommand, ShowDeployPaneToSideCommand } from "./commands/showDeployPane";
import { ShowModuleSourceFileCommand } from "./commands/ShowModuleSourceFileCommand";
import { ShowSourceFromVisualizerCommand } from "./commands/showSourceFromVisualizer";
import { ShowVisualizerCommand, ShowVisualizerToSideCommand } from "./commands/showVisualizer";
import { SuppressedWarningsManager } from "./commands/SuppressedWarningsManager";
import { activateBuildFeature } from "./features/build";
import { activateConfigurationFeature } from "./features/configuration";
import { activateImportKubernetesManifestFeature } from "./features/import-kubernetes-manifest";
import { activateInsertResourceFeature } from "./features/insert-resource";
import { activateModuleRestoreFeature } from "./features/module-restore";
import { activateParametersFeature } from "./features/parameters";
import { activateRefactoringFeature } from "./features/refactoring";
import * as surveys from "./features/surveys";
import { activateWalkthroughFeature } from "./features/walkthrough";
import { setGlobalStateKeysToSyncBetweenMachines } from "./globalState";
import {
  BicepExternalSourceContentProvider,
  createLanguageService,
  ensureDotnetRuntimeInstalled,
  ensureMcpServerExists,
} from "./language";
import { bicepConfigurationPrefix, bicepLanguageId } from "./language/constants";
import { BicepExternalSourceScheme } from "./language/decodeExternalSourceUri";
import { DeployPaneViewManager } from "./panes/deploy";
import { updateUiContext } from "./updateUiContext";
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
        // go2def links that point to the bicep cache will have the bicep-extsrc scheme in their document URIs
        // this content provider will allow VS code to understand that scheme
        // and surface the content as a read-only file
        extension.register(
          workspace.registerTextDocumentContentProvider(
            BicepExternalSourceScheme,
            new BicepExternalSourceContentProvider(languageClient),
          ),
        );

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

        const suppressedWarningsManager = new SuppressedWarningsManager();

        // Register commands.
        const pasteAsBicepCommand = new PasteAsBicepCommand(
          languageClient,
          outputChannelManager,
          suppressedWarningsManager,
        );
        const commandManager = extension.register(new CommandManager(extensionContext));
        await activateBuildFeature(commandManager, languageClient, outputChannelManager);
        await activateParametersFeature(commandManager, languageClient, outputChannelManager);
        await activateConfigurationFeature(commandManager, languageClient);
        await commandManager.registerCommands(
          new DeployCommand(languageClient, outputChannelManager, azurePickers),
          new DecompileCommand(languageClient, outputChannelManager),
          new DecompileParamsCommand(languageClient, outputChannelManager),
        );
        await activateModuleRestoreFeature(commandManager, languageClient, outputChannelManager);
        await activateInsertResourceFeature(commandManager, languageClient);
        await commandManager.registerCommands(
          pasteAsBicepCommand,
          new ShowDeployPaneCommand(deployPaneViewManager),
          new ShowDeployPaneToSideCommand(deployPaneViewManager),
          new ShowVisualizerCommand(viewManager),
          new ShowVisualizerToSideCommand(viewManager),
          new ShowSourceFromVisualizerCommand(viewManager),
        );
        await activateWalkthroughFeature(commandManager);
        await activateImportKubernetesManifestFeature(commandManager, languageClient);
        await commandManager.registerCommands(new ShowModuleSourceFileCommand());
        await activateRefactoringFeature(commandManager);

        // Register events
        pasteAsBicepCommand.registerForPasteEvents(extension);

        extension.register(
          window.onDidChangeActiveTextEditor(async (editor: TextEditor | undefined) => {
            await updateUiContext(editor?.document);
          }),
        );

        extension.register(
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
          workspace.onDidCloseTextDocument(async (_d: TextDocument) => {
            await updateUiContext(window.activeTextEditor?.document);
          }),
        );

        extension.register(
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
          workspace.onDidOpenTextDocument(async (_d: TextDocument) => {
            await updateUiContext(window.activeTextEditor?.document);
          }),
        );

        extension.register(
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
          workspace.onDidSaveTextDocument(async (_d: TextDocument) => {
            await updateUiContext(window.activeTextEditor?.document);
          }),
        );

        await languageClient.start();
        getLogger().info("Bicep language service started.");

        extension.register(
          lm.registerMcpServerDefinitionProvider("bicep", {
            provideMcpServerDefinitions: async () => {
              const mcpServerPath = await ensureMcpServerExists(extensionContext);
              return [new McpStdioServerDefinition("Bicep", dotnetCommandPath, [mcpServerPath])];
            },
          }),
        );

        // Set initial UI context
        await updateUiContext(window.activeTextEditor?.document);
      },
    );
  });
}

export async function deactivate(): Promise<void> {
  await languageClient?.stop();
  getLogger().info("Bicep language service stopped.");

  resetLogger();
}

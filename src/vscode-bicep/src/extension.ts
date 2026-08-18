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
import { CommandManager } from "./infrastructure/commands";
import { activateBuildFeature } from "./features/build";
import { activateConfigurationFeature } from "./features/configuration";
import { activateDecompileFeature } from "./features/decompile";
import {
  activateDeploymentFeature,
  removePropertiesWithPossibleUserInfoInDeployParams,
} from "./features/deployments";
import { activateExternalSourceFeature } from "./features/external-source";
import { activateImportKubernetesManifestFeature } from "./features/import-kubernetes-manifest";
import { activateInsertResourceFeature } from "./features/insert-resource";
import { activateModuleRestoreFeature } from "./features/module-restore";
import { activateMcpFeature } from "./features/mcp";
import { activateParametersFeature } from "./features/parameters";
import { activatePasteAsBicepFeature } from "./features/paste-as-bicep";
import { activateRefactoringFeature } from "./features/refactoring";
import * as surveys from "./features/surveys";
import { activateVisualizationFeature } from "./features/visualization";
import { activateWalkthroughFeature } from "./features/walkthrough";
import { bicepConfigurationPrefix } from "./infrastructure/configuration";
import { bicepLanguageId } from "./infrastructure/editor";
import { createLanguageService, DiagnosticsRouter, ensureDotnetRuntimeInstalled } from "./infrastructure/language-client";
import { Disposable } from "./infrastructure/lifecycle";
import {
  activateWithTelemetryAndErrorHandling,
  createAzExtOutputChannel,
  createLogger,
  getLogger,
  OutputChannelManager,
  resetLogger,
} from "./infrastructure/logging";

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
  const outputChannel = createAzExtOutputChannel(
    "Bicep",
    bicepConfigurationPrefix,
    removePropertiesWithPossibleUserInfoInDeployParams,
  );

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
        surveys.setGlobalStateKeysToSyncBetweenMachines(extensionContext.globalState);

        // Show appropriate surveys
        surveys.showSurveys(extensionContext.globalState);

        const diagnosticsRouter = extension.register(new DiagnosticsRouter(languageClient.clientOptions));

        const outputChannelManager = extension.register(
          new OutputChannelManager(
            "Bicep Operations",
            bicepConfigurationPrefix,
            removePropertiesWithPossibleUserInfoInDeployParams,
          ),
        );

        // Register commands.
        const commandManager = extension.register(new CommandManager(extensionContext));
        await activateVisualizationFeature(
          extension,
          extension.extensionUri,
          commandManager,
          languageClient,
          diagnosticsRouter,
        );
        await activateDeploymentFeature(
          extension,
          actionContext,
          extensionContext,
          commandManager,
          languageClient,
          outputChannelManager,
          diagnosticsRouter,
        );
        await activateBuildFeature(commandManager, languageClient, outputChannelManager);
        await activateParametersFeature(commandManager, languageClient, outputChannelManager);
        await activateConfigurationFeature(commandManager, languageClient);
        await activateDecompileFeature(extension, commandManager, languageClient, outputChannelManager);
        await activateModuleRestoreFeature(commandManager, languageClient, outputChannelManager);
        await activateInsertResourceFeature(commandManager, languageClient);
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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ExtensionContext, ProgressLocation, Uri, window } from "vscode";
import * as lsp from "vscode-languageclient/node";
import { activateBuildFeature } from "./features/build";
import { activateConfigurationFeature } from "./features/configuration";
import { activateDecompileFeature } from "./features/decompile";
import { activateDeploymentFeature, removePropertiesWithPossibleUserInfoInDeployParams } from "./features/deployments";
import { activateExternalSourceFeature } from "./features/external-source";
import { activateImportKubernetesManifestFeature } from "./features/import-kubernetes-manifest";
import { activateInsertResourceFeature } from "./features/insert-resource";
import { activateMcpFeature } from "./features/mcp";
import { activateModuleRestoreFeature } from "./features/module-restore";
import { activateParametersFeature } from "./features/parameters";
import { activatePasteAsBicepFeature } from "./features/paste-as-bicep";
import { activateRefactoringFeature } from "./features/refactoring";
import * as surveys from "./features/surveys";
import { activateVisualizationFeature } from "./features/visualization";
import { activateWalkthroughFeature } from "./features/walkthrough";
import { CommandManager } from "./infrastructure/commands";
import {
  createLanguageService,
  DiagnosticsRouter,
  ensureDotnetRuntimeInstalled,
} from "./infrastructure/language-client";
import { Disposable } from "./infrastructure/lifecycle";
import {
  activateWithTelemetryAndErrorHandling,
  createLogger,
  createLogOutputChannel,
  getLogger,
  OutputChannelManager,
  resetLogger,
} from "./infrastructure/logging";
import { Prompts } from "./infrastructure/prompts";
import { BicepTelemetry } from "./infrastructure/telemetry";

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
  const outputChannel = createLogOutputChannel("Bicep", removePropertiesWithPossibleUserInfoInDeployParams);

  extension.register(outputChannel);
  extension.register(createLogger(extensionContext, outputChannel));
  const telemetry = extension.register(
    new BicepTelemetry(
      (extensionContext.extension.packageJSON as { aiKey: string }).aiKey,
      process.env.BICEP_TELEMETRY_DISABLED !== "true",
    ),
  );
  const prompts = new Prompts(extensionContext.globalState);

  // Activate and launch language server
  await activateWithTelemetryAndErrorHandling(telemetry, async () => {
    await window.withProgress(
      {
        location: ProgressLocation.Window,
      },
      async (progress) => {
        progress.report({ message: "Acquiring dotnet runtime" });
        const dotnetCommandPath = await ensureDotnetRuntimeInstalled();

        progress.report({ message: "Launching language service" });
        languageClient = await createLanguageService(extensionContext, outputChannel, dotnetCommandPath, telemetry);

        progress.report({ message: "Registering commands" });
        surveys.setGlobalStateKeysToSyncBetweenMachines(extensionContext.globalState);

        // Show appropriate surveys
        surveys.showSurveys(extensionContext.globalState);

        const diagnosticsRouter = extension.register(new DiagnosticsRouter(languageClient.clientOptions));

        const outputChannelManager = extension.register(
          new OutputChannelManager("Bicep Operations", removePropertiesWithPossibleUserInfoInDeployParams),
        );

        // Register commands.
        const commandManager = extension.register(new CommandManager(extensionContext, telemetry));
        await activateVisualizationFeature(
          extension,
          extension.extensionUri,
          prompts,
          commandManager,
          languageClient,
          diagnosticsRouter,
        );
        await activateDeploymentFeature(
          extension,
          prompts,
          extensionContext,
          commandManager,
          languageClient,
          outputChannelManager,
          diagnosticsRouter,
        );
        await activateBuildFeature(prompts, commandManager, languageClient, outputChannelManager);
        await activateParametersFeature(prompts, commandManager, languageClient, outputChannelManager);
        await activateConfigurationFeature(commandManager, languageClient);
        await activateDecompileFeature(extension, prompts, commandManager, languageClient, outputChannelManager);
        await activateModuleRestoreFeature(prompts, commandManager, languageClient, outputChannelManager);
        await activateInsertResourceFeature(prompts, commandManager, languageClient);
        await activateWalkthroughFeature(prompts, commandManager);
        await activatePasteAsBicepFeature(extension, prompts, commandManager, languageClient, outputChannelManager);
        await activateImportKubernetesManifestFeature(prompts, commandManager, languageClient);
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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { CommandManager } from "../../infrastructure/commands";
import { DiagnosticsRouter } from "../../infrastructure/language-client";
import { Disposable } from "../../infrastructure/lifecycle";
import { Prompts } from "../../infrastructure/prompts";
import { ShowSourceFromVisualizerCommand, ShowVisualizerCommand, ShowVisualizerToSideCommand } from "./commands";
import { BicepVisualizerViewManager } from "./visualizer-view-manager";

export async function activateVisualizationFeature(
  extension: Disposable,
  extensionUri: Uri,
  prompts: Prompts,
  commandManager: CommandManager,
  languageClient: LanguageClient,
  diagnosticsRouter: DiagnosticsRouter,
): Promise<void> {
  const viewManager = extension.register(
    new BicepVisualizerViewManager(extensionUri, languageClient, diagnosticsRouter),
  );

  await commandManager.registerCommands(
    new ShowVisualizerCommand(prompts, viewManager),
    new ShowVisualizerToSideCommand(prompts, viewManager),
    new ShowSourceFromVisualizerCommand(viewManager),
  );
}

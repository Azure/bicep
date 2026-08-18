// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { CommandManager } from "../../infrastructure/commands";
import { DiagnosticsRouter } from "../../infrastructure/language-client";
import { Disposable } from "../../infrastructure/lifecycle";
import {
  ShowSourceFromVisualizerCommand,
  ShowVisualizerCommand,
  ShowVisualizerToSideCommand,
} from "./commands";
import { BicepVisualizerViewManager } from "./visualizer-view-manager";

export async function activateVisualizationFeature(
  extension: Disposable,
  extensionUri: Uri,
  commandManager: CommandManager,
  languageClient: LanguageClient,
  diagnosticsRouter: DiagnosticsRouter,
): Promise<void> {
  const viewManager = extension.register(
    new BicepVisualizerViewManager(extensionUri, languageClient, diagnosticsRouter),
  );

  await commandManager.registerCommands(
    new ShowVisualizerCommand(viewManager),
    new ShowVisualizerToSideCommand(viewManager),
    new ShowSourceFromVisualizerCommand(viewManager),
  );
}

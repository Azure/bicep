// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode, { ViewColumn } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { IActionContext, parseError } from "../../infrastructure/action-context";
import { Command, CommandManager } from "../../infrastructure/commands";
import { importKubernetesManifestRequestType } from "./protocol";

export class ImportKubernetesManifestCommand implements Command {
  public readonly id = "bicep.importKubernetesManifest";
  public constructor(private readonly client: LanguageClient) {}

  public async execute(context: IActionContext): Promise<void> {
    const manifestPath = await context.ui.showOpenDialog({
      canSelectMany: false,
      openLabel: "Select Kubernetes Manifest File",
      filters: { "YAML files": ["yml", "yaml"] },
    });

    try {
      const response = await this.client.sendRequest(importKubernetesManifestRequestType, {
        manifestFilePath: manifestPath[0].fsPath,
      });

      const document = await vscode.workspace.openTextDocument(response.bicepFilePath);

      await vscode.window.showTextDocument(document, ViewColumn.Active);
    } catch (err) {
      this.client.error("Build failed", parseError(err).message, true);
    }
  }
}

export async function activateImportKubernetesManifestFeature(
  commandManager: CommandManager,
  client: LanguageClient,
): Promise<void> {
  await commandManager.registerCommands(new ImportKubernetesManifestCommand(client));
}

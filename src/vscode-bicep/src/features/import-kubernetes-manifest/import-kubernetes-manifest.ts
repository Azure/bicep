// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ViewColumn, window, workspace } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Command, CommandManager } from "../../infrastructure/commands";
import { parseError } from "../../infrastructure/errors";
import { Prompts } from "../../infrastructure/prompts";
import { importKubernetesManifestRequestType } from "./protocol";

export class ImportKubernetesManifestCommand implements Command {
  public readonly id = "bicep.importKubernetesManifest";
  public constructor(
    private readonly prompts: Prompts,
    private readonly client: LanguageClient,
  ) {}

  public async execute(): Promise<void> {
    const manifestPath = await this.prompts.showOpenDialog({
      canSelectMany: false,
      openLabel: "Select Kubernetes Manifest File",
      filters: { "YAML files": ["yml", "yaml"] },
    });

    try {
      const response = await this.client.sendRequest(importKubernetesManifestRequestType, {
        manifestFilePath: manifestPath[0].fsPath,
      });

      const document = await workspace.openTextDocument(response.bicepFilePath);

      await window.showTextDocument(document, ViewColumn.Active);
    } catch (err) {
      this.client.error("Build failed", parseError(err).message, true);
    }
  }
}

export async function activateImportKubernetesManifestFeature(
  prompts: Prompts,
  commandManager: CommandManager,
  client: LanguageClient,
): Promise<void> {
  await commandManager.registerCommands(new ImportKubernetesManifestCommand(prompts, client));
}

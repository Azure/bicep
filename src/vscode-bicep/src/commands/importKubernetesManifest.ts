// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import vscode, { ViewColumn } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { importKubernetesManifestRequestType } from "../language/protocol";
import { Command } from "./types";

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

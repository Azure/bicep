// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode, { ViewColumn } from "vscode";
import { importKubernetesManifestRequestType } from "../language/protocol";
import { Command } from "./types";
import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import { LanguageClient } from "vscode-languageclient/node";

export class ImportKubernetesManifestCommand implements Command {
  public readonly id = "bicep.importKubernetesManifest";
  public constructor(private readonly client: LanguageClient) {}

  public async execute(
    context: IActionContext,
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    documentUri?: vscode.Uri | undefined
  ): Promise<void> {
    const manifestPath = await context.ui.showOpenDialog({
      canSelectMany: false,
      openLabel: "Select Kubernetes Manifest File",
      filters: { "YAML files": ["yml", "yaml"] },
    });

    try {
      const response = await this.client.sendRequest(
        importKubernetesManifestRequestType,
        {
          manifestFilePath: manifestPath[0].fsPath,
        }
      );

      const document = await vscode.workspace.openTextDocument(
        response.bicepFilePath
      );

      await vscode.window.showTextDocument(document, ViewColumn.Active);
    } catch (err) {
      this.client.error("Build failed", parseError(err).message, true);
    }
  }
}

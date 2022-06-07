// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode, { Uri } from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import path from "path";

export class GenerateParamsCommand implements Command {
  public readonly id = "bicep.generateParams";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager
  ) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to generate parameters file for"
    );

    try {
      console.log(`Generating parameters file for ${documentUri.fsPath}...`);
      const generateParamsOutput: string = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "generateParams",
          arguments: [documentUri.fsPath],
        }
      );
      this.outputChannelManager.appendToOutputChannel(generateParamsOutput);

      const filePath = path.parse(documentUri.fsPath);

      const openPath = Uri.file(
        path.join(filePath.dir, `${filePath.name}.parameters.json`)
      );
      const doc = await vscode.workspace.openTextDocument(openPath);
      await vscode.window.showTextDocument(doc);
    } catch (err) {
      throw new Error(
        `Generating parameters failed: ${parseError(err).message}`
      );
    }
  }
}

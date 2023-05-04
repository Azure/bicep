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

      const outputFormat = await vscode.window.showQuickPick(['json', 'bicepparam'], { title: "Please select the output format" });
      const includeParams = await vscode.window.showQuickPick(['requiredonly', 'all'], { title: "Please select which parameters to include" });

      if (outputFormat === undefined || includeParams === undefined) {
        throw new Error(
          `Please select the format and which parameters ro include`
        );
      }

      const generateParamsOutput: string = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "generateParams",
          arguments: [{
            BicepFilePath: documentUri.fsPath,
            OutputFormat: outputFormat,
            IncludeParams: includeParams
          }],
        }
      );
      this.outputChannelManager.appendToOutputChannel(generateParamsOutput);

      const filePath = path.parse(documentUri.fsPath);

      const openPath = Uri.file(
        path.join(filePath.dir, `${filePath.name}.${outputFormat === 'json' ? 'parameters.json' : 'bicepparam'}`)
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

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import vscode, { Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Command, CommandManager } from "../../infrastructure/commands";
import { findOrCreateActiveBicepFile } from "../../infrastructure/editor";
import { parseError } from "../../infrastructure/errors";
import { OutputChannelManager } from "../../infrastructure/logging";
import { Prompts } from "../../infrastructure/prompts";

export class GenerateParamsCommand implements Command {
  public readonly id = "bicep.generateParams";
  public constructor(
    private readonly prompts: Prompts,
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      this.prompts,
      documentUri,
      "Choose which Bicep file to generate parameters file for",
    );

    try {
      console.log(`Generating parameters file for ${documentUri.fsPath}...`);

      const outputFormat = await vscode.window.showQuickPick(["json", "bicepparam"], {
        title: "Please select the output format",
      });
      const includeParams = await vscode.window.showQuickPick(["requiredonly", "all"], {
        title: "Please select which parameters to include",
      });

      if (outputFormat === undefined || includeParams === undefined) {
        throw new Error("Please select the output format and which parameters to include");
      }

      const generateParamsOutput: string = await this.client.sendRequest("workspace/executeCommand", {
        command: "generateParams",
        arguments: [
          {
            BicepFilePath: documentUri.fsPath,
            OutputFormat: outputFormat,
            IncludeParams: includeParams,
          },
        ],
      });
      this.outputChannelManager.appendToOutputChannel(generateParamsOutput);

      const filePath = path.parse(documentUri.fsPath);

      const openPath = Uri.file(
        path.join(filePath.dir, `${filePath.name}.${outputFormat === "json" ? "parameters.json" : "bicepparam"}`),
      );
      const doc = await vscode.workspace.openTextDocument(openPath);
      await vscode.window.showTextDocument(doc);
    } catch (err) {
      throw new Error(`Generating parameters failed: ${parseError(err).message}`, { cause: err });
    }
  }
}

export async function activateParametersFeature(
  prompts: Prompts,
  commandManager: CommandManager,
  client: LanguageClient,
  outputChannelManager: OutputChannelManager,
): Promise<void> {
  await commandManager.registerCommands(new GenerateParamsCommand(prompts, client, outputChannelManager));
}

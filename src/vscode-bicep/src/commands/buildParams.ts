// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { findOrCreateActiveBicepParamFile } from "./findOrCreateActiveBicepFile";
import { Command } from "./types";

export class BuildParamsCommand implements Command {
  public readonly id = "bicep.buildParams";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(context: IActionContext, documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri = await findOrCreateActiveBicepParamFile(
      context,
      documentUri,
      "Choose which Bicep Parameters file to build into an ARM parameters file",
    );

    if (documentUri.scheme.toLowerCase() !== "file") {
      this.client.error(
        "Bicep Parameters build failed. The active file must be saved to your local filesystem.",
        undefined,
        true,
      );
      return;
    }

    try {
      const buildOutput: string = await this.client.sendRequest("workspace/executeCommand", {
        command: "buildParams",
        arguments: [documentUri.toString()],
      });
      this.outputChannelManager.appendToOutputChannel(buildOutput);
    } catch (err) {
      this.client.error("Bicep Parameters build failed", parseError(err).message, true);
    }
  }
}

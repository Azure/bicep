// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";

export class BuildCommand implements Command {
  public readonly id = "bicep.build";
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
      "Choose which Bicep file to build into an ARM template"
    );

    try {
      const buildOutput: string = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "build",
          arguments: [documentUri.fsPath],
        }
      );
      this.outputChannelManager.appendToOutputChannel(buildOutput);
    } catch (err) {
      this.client.error("Bicep build failed", parseError(err).message, true);
    }
  }
}

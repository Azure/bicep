// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";

import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import vscode from "vscode";

export class PublishCommand implements Command {
  public readonly id = "bicep.publish";
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
      const targetModuleReference = await vscode.window.showInputBox({
        placeHolder: "Please enter target module reference",
      });
      const buildOutput: string = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "publish",
          arguments: [documentUri.fsPath, targetModuleReference],
        }
      );
      this.outputChannelManager.appendToOutputChannel(buildOutput);
    } catch (err) {
      this.client.error("Bicep build failed", parseError(err).message, true);
    }
  }
}

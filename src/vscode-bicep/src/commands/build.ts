// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";

export class BuildCommand implements Command {
  public readonly id = "bicep.build";
  public readonly outputChannel =
    vscode.window.createOutputChannel("Bicep Operations");

  public constructor(private readonly client: LanguageClient) {}

  public async execute(documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri ??= vscode.window.activeTextEditor?.document.uri;

    if (!documentUri) {
      return;
    }

    try {
      const buildOutput: string = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "build",
          arguments: [documentUri.fsPath],
        }
      );
      appendToOutputChannel(this.outputChannel, buildOutput);
    } catch (err) {
      this.client.error("Build failed", err.message, true);
    }
  }
}

function appendToOutputChannel(
  outputChannel: vscode.OutputChannel,
  text: string
) {
  outputChannel.clear();
  outputChannel.show();
  outputChannel.appendLine(text);
}

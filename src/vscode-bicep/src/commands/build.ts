// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { IAzExtOutputChannel } from "vscode-azureextensionui";
import { LanguageClient } from "vscode-languageclient/node";

import { Command } from "./types";

export class BuildCommand implements Command {
  public readonly id = "bicep.build";

  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannel: IAzExtOutputChannel
  ) {}

  public async execute(documentUri?: vscode.Uri | undefined) {
    try {
      if (documentUri == null) {
        return;
      }

      return await this.client.sendRequest("workspace/executeCommand", {
        command: "build",
        arguments: [documentUri.fsPath],
      });
    } catch (err) {
      this.outputChannel.appendLine(`Error: ${err}`);
    }
  }
}

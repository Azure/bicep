// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { Command } from "./types";

export class ForceModulesRestoreCommand implements Command {
  public readonly id = "bicep.forceModulesRestore";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(_context: IActionContext, documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri ??= vscode.window.activeTextEditor?.document.uri;

    if (!documentUri) {
      return;
    }

    if (documentUri.scheme === "output") {
      // The output panel in VS Code was implemented as a text editor by accident. Due to breaking change concerns,
      // it won't be fixed in VS Code, so we need to handle it on our side.
      // See https://github.com/microsoft/vscode/issues/58869#issuecomment-422322972 for details.

      // Don't wait
      void vscode.window.showInformationMessage(
        "We are unable to get restore modules in a Bicep file when the output panel is focused. Please focus a text editor first when running the command.",
      );

      return;
    }

    if (documentUri.scheme.toLowerCase() !== "file") {
      this.client.error(
        "Restore (force) failed. The active file must be saved to your local filesystem.",
        undefined,
        true,
      );
      return;
    }

    try {
      this.outputChannelManager.appendToOutputChannel(`Force restoring modules used by ${documentUri}...`);

      const forceModulesRestoreOutput: string = await this.client.sendRequest("workspace/executeCommand", {
        command: "forceModulesRestore",
        arguments: [documentUri.toString()],
      });
      this.outputChannelManager.appendToOutputChannel(forceModulesRestoreOutput);
    } catch (err) {
      this.client.error("Restore (force) failed", parseError(err).message, true);
    }
  }
}

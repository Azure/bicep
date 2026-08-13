// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Command, CommandManager } from "../../infrastructure/commands";
import { findOrCreateActiveBicepFile } from "../../infrastructure/editor";
import { OutputChannelManager } from "../../infrastructure/logging";

export class ForceModulesRestoreCommand implements Command {
  public readonly id = "bicep.forceModulesRestore";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(context: IActionContext, documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to restore modules for",
    );

    if (documentUri.scheme === "output") {
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

export async function activateModuleRestoreFeature(
  commandManager: CommandManager,
  client: LanguageClient,
  outputChannelManager: OutputChannelManager,
): Promise<void> {
  await commandManager.registerCommands(new ForceModulesRestoreCommand(client, outputChannelManager));
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Command, CommandManager } from "../../infrastructure/commands";
import { findOrCreateActiveBicepParamFile } from "../../infrastructure/editor";
import { parseError } from "../../infrastructure/errors";
import { OutputChannelManager } from "../../infrastructure/logging";
import { Prompts } from "../../infrastructure/prompts";

export class SnapshotCommand implements Command {
  public readonly id = "bicep.snapshot";
  public constructor(
    private readonly prompts: Prompts,
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(documentUri?: Uri | undefined): Promise<void> {
    documentUri = await findOrCreateActiveBicepParamFile(
      this.prompts,
      documentUri,
      "Choose which Bicep Parameters file to generate a snapshot from",
    );

    if (documentUri.scheme.toLowerCase() !== "file") {
      this.client.error(
        "Snapshot generation failed. The active file must be saved to your local filesystem.",
        undefined,
        true,
      );
      return;
    }

    try {
      const snapshotOutput: string = await this.client.sendRequest("workspace/executeCommand", {
        command: "snapshot",
        arguments: [documentUri.toString()],
      });
      this.outputChannelManager.appendToOutputChannel(snapshotOutput);
    } catch (err) {
      this.client.error("Snapshot generation failed", parseError(err).message, true);
    }
  }
}

export async function activateSnapshotFeature(
  prompts: Prompts,
  commandManager: CommandManager,
  client: LanguageClient,
  outputChannelManager: OutputChannelManager,
): Promise<void> {
  await commandManager.registerCommands(new SnapshotCommand(prompts, client, outputChannelManager));
}

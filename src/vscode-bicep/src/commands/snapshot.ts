// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { findOrCreateActiveBicepParamFile } from "./findOrCreateActiveBicepFile";
import { Command } from "./types";

export class SnapshotCommand implements Command {
  public readonly id = "bicep.snapshot";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(context: IActionContext, documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri = await findOrCreateActiveBicepParamFile(
      context,
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

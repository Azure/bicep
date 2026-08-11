// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Command, CommandManager } from "../../infrastructure/commands";
import {
  findOrCreateActiveBicepFile,
  findOrCreateActiveBicepParamFile,
} from "../../infrastructure/editor";
import { OutputChannelManager } from "../../infrastructure/logging";

export class BuildCommand implements Command {
  public readonly id = "bicep.build";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(context: IActionContext, documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to build into an ARM template",
    );

    if (documentUri.scheme.toLowerCase() !== "file") {
      this.client.error("Bicep build failed. The active file must be saved to your local filesystem.", undefined, true);
      return;
    }

    try {
      const buildOutput: string = await this.client.sendRequest("workspace/executeCommand", {
        command: "build",
        arguments: [documentUri.toString()],
      });
      this.outputChannelManager.appendToOutputChannel(buildOutput);
    } catch (err) {
      this.client.error("Bicep build failed", parseError(err).message, true);
    }
  }
}

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

export async function activateBuildFeature(
  commandManager: CommandManager,
  client: LanguageClient,
  outputChannelManager: OutputChannelManager,
): Promise<void> {
  await commandManager.registerCommands(
    new BuildCommand(client, outputChannelManager),
    new BuildParamsCommand(client, outputChannelManager),
  );
}

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
    private readonly outputChannelManager: OutputChannelManager
  ) {}

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<void> {
    documentUri = await findOrCreateActiveBicepParamFile(
      context,
      documentUri,
      "Choose which Bicep file to build into an ARM template"
    );

    try {
      const buildOutput: string = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "buildParams",
          arguments: [documentUri.fsPath],
        }
      );
      this.outputChannelManager.appendToOutputChannel(buildOutput);
    } catch (err) {
      this.client.error("Bicep build failed", parseError(err).message, true);
    }
  }
}

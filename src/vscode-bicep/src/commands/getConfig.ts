// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import path from "path";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import vscode, { window } from "vscode";
import { LanguageClient, ProtocolRequestType } from "vscode-languageclient/node";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { Command } from "./types";

interface GetConfigParams {
  bicepOrConfigPath: string;
}

interface GetConfigResult {
  configPath?: string; // Path to the configuration file
  effectiveConfig: string; // The effective configuration file content
  linterState: string; // The linter and rules state
}

const GetConfigRequestType = new ProtocolRequestType<GetConfigParams, GetConfigResult, never, void, void>(
  "bicep/getConfiguration",
);

export class GetConfigCommand implements Command {
  public readonly id = "bicep.getConfiguration";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(context: IActionContext, documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri ??= window.activeTextEditor?.document.uri;
    if (!documentUri || path.parse(documentUri?.fsPath ?? "").name != "bicepconfig.json") {
      documentUri = await findOrCreateActiveBicepFile(
        context,
        documentUri,
        "Choose which Bicep file to display configuration for",
      );
    }

    const bicepFilePath = documentUri.fsPath;
    const result: GetConfigResult = await this.client.sendRequest(GetConfigRequestType, {
      bicepOrConfigPath: bicepFilePath,
    });

    const output = `Current configuration for ${bicepFilePath}:
Configuration file path: ${result.configPath ?? "None"}
Effective configuration settings:
${result.effectiveConfig}
Linter state (core):
${result.linterState}

`;
    this.outputChannelManager.appendToOutputChannel(output);
  }
}

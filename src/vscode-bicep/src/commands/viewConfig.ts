// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import vscode, { window } from "vscode";
import { LanguageClient, ProtocolRequestType } from "vscode-languageclient/node";
import { indentLines } from "../utils/indentLines";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { Command } from "./types";

interface GetConfigInfoParams {
  bicepOrConfigPath: string;
}

interface getConfigInfoResult {
  configPath?: string; // Path to the configuration file
  effectiveConfig: string; // The effective configuration file content
  linterState: string; // The linter and rules state
}

const GetConfigInfoRequestType = new ProtocolRequestType<GetConfigInfoParams, getConfigInfoResult, never, void, void>(
  "bicep/getConfigInfo",
);

export class ViewConfigCommand implements Command {
  public readonly id = "bicep.viewConfig";

  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(context: IActionContext, documentUri?: vscode.Uri | undefined): Promise<void> {
    // If the user has a bicepconfig.json file open, use that
    let bicepOrConfigUri = ifBicepConfigFile(documentUri);
    bicepOrConfigUri ??= ifBicepConfigFile(window.activeTextEditor?.document.uri);

    // Otherwise check/ask for a bicep file
    bicepOrConfigUri ??= await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to display merged configuration for",
    );

    const bicepFilePath = bicepOrConfigUri.fsPath;
    const result: getConfigInfoResult = await this.client.sendRequest(GetConfigInfoRequestType, {
      bicepOrConfigPath: bicepFilePath,
    });

    const output = `Configuration information for ${bicepFilePath}:
Configuration file path: ${result.configPath ?? "None"}

Effective configuration after merging with default configuration:
${indentLines(result.effectiveConfig, 2)}

Linter rules state (core):
${indentLines(result.linterState, 2)}

`;
    this.outputChannelManager.appendToOutputChannel(output);
  }
}

function ifBicepConfigFile(documentUri?: vscode.Uri): vscode.Uri | undefined {
  if (documentUri?.scheme !== "file") {
    return undefined;
  }

  const parsed = path.parse(documentUri.fsPath);
  return parsed.name === "bicepconfig" && parsed.ext === ".json"
    ? documentUri
    : undefined;
}

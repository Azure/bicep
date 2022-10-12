// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  IActionContext,
  UserCancelledError
} from "@microsoft/vscode-azext-utils";
import * as fse from "fs-extra";
import vscode, { Uri, window } from "vscode";
import { DocumentUri, LanguageClient } from "vscode-languageclient/node";
import { sleep } from "../utils";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { Command } from "./types";

interface DecompileCommandParams {
  jsonUri: DocumentUri;
}
enum BicepDecompileCommandStatus {
  Canceled = 1,
  Failed = 2,
  Success = 3,
}
interface BicepDecompileCommandResult {
  status: BicepDecompileCommandStatus;
  bicepUri: DocumentUri;
  output: string;
  failureError: string | undefined;
}

export class DecompileCommand implements Command {
  public readonly id = "bicep.decompile";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager
  ) {
    // nothing to do
  }

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<void> {
    documentUri = documentUri ?? window.activeTextEditor?.document.uri;
    if (!documentUri) {
      throw new Error(
        "Please open a JSON ARM Template file before running this command"
      );
    }

    const canDecompile = await DecompileCommand.mightBeArmTemplate(documentUri);
    if (!canDecompile) {
      throw new Error(
        `Cannot decompile "${documentUri.fsPath}" into Bicep because it does not appear to be an ARM template.`
      );
    }

    // Give the language server time to settle to avoid "content modified" errors when the language server calls ShowMessageRequest,
    //   in the case that starting this command caused the language server to get loaded and the Bicep output files already exist.
    await sleep(500);

    const params: DecompileCommandParams = {
      jsonUri: documentUri.toString(),
    };
    const result: BicepDecompileCommandResult = await this.client.sendRequest(
      "workspace/executeCommand",
      {
        command: "decompile",
        arguments: [params],
      }
    );
    this.outputChannelManager.appendToOutputChannel(result.output);

    switch (result.status) {
      case BicepDecompileCommandStatus.Canceled:
        throw new UserCancelledError();
      case BicepDecompileCommandStatus.Failed:
        context.telemetry.properties.decompileStatus = "failed";
        break;
      case BicepDecompileCommandStatus.Success:
          context.telemetry.properties.decompileStatus = "success";
        break;
      default:
        throw new Error(
          `Unexpected BicepDecompileCommandStatus ${result.status}`
        );
    }
  }

  public static async mightBeArmTemplate(documentUri: Uri): Promise<boolean> {
    try {
      const contents = await fse.readFile(documentUri.fsPath);
      if (/\$schema.*deploymenttemplate\.json/i.test(contents.toString())) {
        return true;
      }
    } catch (err) {
      // ignore
    }

    return false;
  }
}

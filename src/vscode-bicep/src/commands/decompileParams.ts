// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import assert from "assert";
import * as path from "path";
import { IActionContext, IAzureQuickPickItem, UserCancelledError } from "@microsoft/vscode-azext-utils";
import * as fse from "fs-extra";
import vscode, { MessageItem, Uri, window } from "vscode";
import { DocumentUri, LanguageClient } from "vscode-languageclient/node";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { Command } from "./types";

interface DecompileParamsCommandParams {
  jsonUri: DocumentUri;
  bicepUri?: DocumentUri;
}

interface DecompiledBicepparamFile {
  contents: string;
  uri: DocumentUri;
}

interface DecompileParamsCommandResult {
  decompiledBicepparamFile?: DecompiledBicepparamFile;
  errorMessage?: string;
}

export class DecompileParamsCommand implements Command {
  public readonly id = "bicep.decompileParams";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
  ) {}

  public async execute(context: IActionContext, documentUri?: vscode.Uri | undefined): Promise<void> {
    documentUri = documentUri ?? window.activeTextEditor?.document.uri;
    if (!documentUri) {
      throw new Error("Please open a JSON Parameter file before running this command");
    }

    const canDecompile = await DecompileParamsCommand.mightBeArmParametersNoThrow(documentUri);
    if (!canDecompile) {
      this.outputChannelManager.appendToOutputChannel(
        `Cannot decompile "${documentUri.fsPath}" into Bicep because it does not appear to be an ARM template parameter file.`,
      );
      throw new UserCancelledError("Cannot decompile input because file provided is not a parameter file");
    }

    const bicepFileUri = await DecompileParamsCommand.selectBicepFile(context);

    const decompileParamsCommandParams: DecompileParamsCommandParams = {
      jsonUri: documentUri.path,
      bicepUri: bicepFileUri ? this.client.code2ProtocolConverter.asUri(bicepFileUri) : undefined,
    };

    this.outputChannelManager.appendToOutputChannel(`Decompiling file: ${documentUri.fsPath}`);

    const decompileParamsResult: DecompileParamsCommandResult = await this.client.sendRequest(
      "workspace/executeCommand",
      {
        command: "decompileParams",
        arguments: [decompileParamsCommandParams],
      },
    );

    if (decompileParamsResult.errorMessage) {
      throw new Error(decompileParamsResult.errorMessage);
    }

    assert(decompileParamsResult.decompiledBicepparamFile !== undefined);
    let bicepparamPath = this.client.protocol2CodeConverter.asUri(
      decompileParamsResult.decompiledBicepparamFile.uri,
    ).fsPath;

    if (await fse.pathExists(bicepparamPath)) {
      const fileSaveOption = await DecompileParamsCommand.getFileSaveOption(context);

      if (fileSaveOption === "Copy") {
        bicepparamPath = await DecompileParamsCommand.getUniquePath(bicepparamPath);
        this.outputChannelManager.appendToOutputChannel(`Saving Decompiled file (copy): ${bicepparamPath}`);
      }

      if (fileSaveOption === "Overwrite") {
        this.outputChannelManager.appendToOutputChannel(`Overwriting Decompiled file: ${bicepparamPath}`);
      }
    } else {
      this.outputChannelManager.appendToOutputChannel(`Saving Decompiled file: ${bicepparamPath}`);
    }

    await fse.writeFile(bicepparamPath, decompileParamsResult.decompiledBicepparamFile.contents);
  }

  public static async mightBeArmParametersNoThrow(documentUri: Uri): Promise<boolean> {
    try {
      const contents = await (await fse.readFile(documentUri.fsPath)).toString();
      return /\$schema.*deploymentParameters\.json/i.test(contents);
    } catch {
      // ignore
    }

    return false;
  }

  private static async selectBicepFile(context: IActionContext): Promise<Uri | undefined> {
    while (true) {
      const quickPickItems: IAzureQuickPickItem<string>[] = [
        {
          label: "None",
          data: "",
        },
        {
          label: "Browse",
          data: "",
        },
      ];

      const result: IAzureQuickPickItem<string> = await context.ui.showQuickPick(quickPickItems, {
        canPickMany: false,
        placeHolder: `Link to a Bicep file?`,
      });

      if (result.label === "None") {
        return undefined;
      }

      if (result.label === "Browse") {
        const bicepPaths: Uri[] | undefined = await vscode.window.showOpenDialog({
          canSelectMany: false,
          openLabel: "Select Bicep File",
          filters: {
            "Bicep Files": ["bicep"],
          },
        });
        if (bicepPaths) {
          assert(bicepPaths.length === 1, "Expected bicepPaths.length === 1");
          return bicepPaths[0];
        }
      }
    }
  }

  private static async getFileSaveOption(context: IActionContext): Promise<"Overwrite" | "Copy"> {
    const overwriteAction: MessageItem = {
      title: "Overwrite",
    };

    const copyAction: MessageItem = {
      title: "Copy",
    };

    const cancelAction: MessageItem = {
      title: "Cancel",
      isCloseAffordance: true,
    };

    const optionPicked: MessageItem = await context.ui.showWarningMessage(
      "The Bicep Parameters file already exist in the file system. Do you want to overwrite it or make a copy?",
      overwriteAction,
      copyAction,
      cancelAction,
    );

    if (optionPicked === cancelAction) {
      throw new UserCancelledError("getFileSaveOption");
    }

    assert(optionPicked === overwriteAction || optionPicked === copyAction);

    return optionPicked === overwriteAction ? "Overwrite" : "Copy";
  }

  private static async getUniquePath(bicepparamPath: DocumentUri): Promise<string> {
    const parsedPath = path.parse(bicepparamPath);
    let appendNumber = 2;
    while (true) {
      const uniquePath = path.join(parsedPath.dir, `${parsedPath.name}${appendNumber}${parsedPath.ext}`);
      if (!(await fse.pathExists(uniquePath))) {
        return uniquePath;
      }
      appendNumber++;
    }
  }
}

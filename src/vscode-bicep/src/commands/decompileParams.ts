// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IActionContext, IAzureQuickPickItem } from "@microsoft/vscode-azext-utils";
import vscode, { MessageItem, Uri, window } from "vscode";
import { DocumentUri, LanguageClient } from "vscode-languageclient/node";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { Command } from "./types";
import * as fse from "fs-extra";
import * as path from 'path';
import { UserCancelledError } from "@microsoft/vscode-azext-utils";
import assert from "assert";

interface DecompileParamsCommandParams {
  jsonUri: DocumentUri;
  bicepPath: string | undefined;
}

interface DecompiledBicepparamFile {
  contents: string;
  absolutePath: string;
}

interface DecompileParamsCommandResult {
  decompiledBicepparamFile: DecompiledBicepparamFile | undefined;
  errorMessage: string | undefined;
}

export class DecompileParamsCommand implements Command {
  public readonly id = "bicep.decompileParams";
  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager
  ) { }

  public async execute(
    context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<void> {
    documentUri = documentUri ?? window.activeTextEditor?.document.uri;
    if (!documentUri) {
      throw new Error(
        "Please open a JSON Parameter file before running this command"
      );
    }
    
    const canDecompile = await DecompileParamsCommand.mightBeArmParametersNoThrow(
      documentUri
    );
    if (!canDecompile) {
      this.outputChannelManager.appendToOutputChannel(
        `Cannot decompile "${documentUri.fsPath}" into Bicep because it does not appear to be an ARM template parameter file.`
      );
      throw new UserCancelledError("Cannot decompile input because file provided is not a parameter file");
    }

    const bicepFileRelativePath =   await DecompileParamsCommand.selectBicepFile(context, path.dirname(documentUri.fsPath));

    const decompileParamsCommandParams: DecompileParamsCommandParams = {
      jsonUri: documentUri.path,
      bicepPath: bicepFileRelativePath
    }

    this.outputChannelManager.appendToOutputChannel(
      `Decompiling file: ${documentUri.fsPath}`
    );

    const decompileParamsResult: DecompileParamsCommandResult =
      await this.client.sendRequest("workspace/executeCommand", {
        command: "decompileParams",
        arguments: [decompileParamsCommandParams]
      })

    if (decompileParamsResult.errorMessage) {
      throw new Error(decompileParamsResult.errorMessage);
    }

    assert(decompileParamsResult.decompiledBicepparamFile !== undefined)

    if(await fse.pathExists(decompileParamsResult.decompiledBicepparamFile.absolutePath.toString()))
    {
      const fileSaveOption = await DecompileParamsCommand.getFileSaveOption(context);

      if(fileSaveOption === "Copy")
      {
        decompileParamsResult.decompiledBicepparamFile.absolutePath = await DecompileParamsCommand.getUniquePath(decompileParamsResult.decompiledBicepparamFile.absolutePath)
        this.outputChannelManager.appendToOutputChannel(
          `Saving Decompiled file (copy): ${decompileParamsResult.decompiledBicepparamFile.absolutePath}`
        );
      }

      if(fileSaveOption == "Overwrite")
      {
        this.outputChannelManager.appendToOutputChannel(
          `Overwriting Decompiled file: ${decompileParamsResult.decompiledBicepparamFile.absolutePath}`
        );
      }
    }
    else
    {
      this.outputChannelManager.appendToOutputChannel(
        `Saving Decompiled file: ${decompileParamsResult.decompiledBicepparamFile.absolutePath}`
      );
    }

    fse.writeFile(decompileParamsResult.decompiledBicepparamFile.absolutePath, decompileParamsResult.decompiledBicepparamFile.contents)
  }

  public static async mightBeArmParametersNoThrow(
    documentUri: Uri
  ): Promise<boolean> {
    try {
      const contents = await (
        await fse.readFile(documentUri.fsPath)
      ).toString();
      return /\$schema.*deploymentParameters\.json/i.test(contents);
    } catch (err) {
      // ignore
    }

    return false;
  }

  private static async selectBicepFile(
    context: IActionContext,
    inputDirPath: string
    ): Promise<string | undefined> {
    while(true) {
      const quickPickItems: IAzureQuickPickItem<string>[] =
      [
        {
          label: "None",
          data: ""
        },
        {
          label: "Browse",
          data: ""
        }
      ]

      const result: IAzureQuickPickItem<string> = 
      await context.ui.showQuickPick(quickPickItems, {
        canPickMany: false,
        placeHolder: `Select a parameter file`,
      })
      
      if(result.label === "None")
      {
        return undefined;
      }

      if(result.label === "Browse"){
        const bicepPaths: Uri[] | undefined =
        await vscode.window.showOpenDialog({
          canSelectMany: false,
          openLabel: "Select Path File",
          filters: {
            "Bicp Files": ["bicep"]
          },
        });
      if (bicepPaths) {
        assert(bicepPaths.length === 1, "Expected bicepPaths.length === 1");
        const bicepFileAbsolutePath = bicepPaths[0].fsPath
        return path.relative(inputDirPath, bicepFileAbsolutePath).replaceAll("\\", "/"); 
        }
      }
    }
  }

  private static async getFileSaveOption(
    context: IActionContext,
  ): Promise<"Overwrite" | "Copy"> {
    const overwriteAction: MessageItem = {
      title: "Overwrite"
    }

    const copyAction: MessageItem = {
      title: "Copy"
    }

    const cancelAction: MessageItem = {
      title: "Cancel",
      isCloseAffordance: true
    }

    const optionPicked: MessageItem = await context.ui.showWarningMessage(
      "The Bicep Parameters file already exist in the file system. Would like to overwrite?",
      overwriteAction,
      copyAction,
      cancelAction
    );

    if (optionPicked === cancelAction) {
      throw new UserCancelledError("getFileSaveOption")
    }

    assert(optionPicked === overwriteAction || optionPicked == copyAction)

    return optionPicked === overwriteAction ? "Overwrite" : "Copy";
  }

  private static async getUniquePath(
    bicepparamPath: DocumentUri
    ): Promise<string> {
      const parsedPath = path.parse(bicepparamPath)
      let appendNumber = 2;
      while (true)
      {
        const uniquePath = path.join(parsedPath.dir, `${parsedPath.name}${appendNumber}${parsedPath.ext}`)
        if(!await fse.pathExists(uniquePath))
        {
          return uniquePath;
        }
        appendNumber++;
      }
  }
}



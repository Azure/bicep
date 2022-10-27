// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  IActionContext, UserCancelledError
} from "@microsoft/vscode-azext-utils";
import assert from "assert";
import * as fse from "fs-extra";
import vscode, { MessageItem, Uri, window } from "vscode";
import { DocumentUri, LanguageClient } from "vscode-languageclient/node";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { Command } from "./types";
import path from "path";

interface DecompileCommandParams {
  jsonUri: DocumentUri;
}

interface DecompiledFile {
  // absolute path to overwrite - could be outside input file's folder/subfolders (e.g. ".." in module path)
  absolutePath: DocumentUri;
  // relative path if choosing to copy to new folder - original paths outside input file's folder/subfolders may be renamed/moved
  clonableRelativePath: DocumentUri;
  bicepContents: string;
}

interface BicepDecompileCommandResult {
  decompileId: string; // Used to synchronize telemetry events
  output: string;
  errorMessage?: string;
  outputFiles: DecompiledFile[]; // First is assumed to be main output file
  conflictingOutputPaths: DocumentUri[]; // client should verify overwrite with user
}

interface BicepDecompileSaveCommandParams {
  decompileId: string;
  outputFiles: DecompiledFile[];
  overwrite: boolean
}

interface BicepDecompileSaveCommandResult {
  output: string;
  errorMessage?: string;
  mainSavedBicepPath?: string;
  savedPaths: string[];
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

    const decompileParams: DecompileCommandParams = {
      jsonUri: documentUri.toString(),
    };
    const decompileResult: BicepDecompileCommandResult = await this.client.sendRequest(
      "workspace/executeCommand",
      {
        command: "decompile",
        arguments: [decompileParams],
      }
    );

    this.outputChannelManager.appendToOutputChannel(decompileResult.output.trimEnd());
    context.telemetry.properties.decompileStatus = decompileResult.errorMessage ? "failed" : "success";
    context.telemetry.properties.countOutputFiles = String(decompileResult.outputFiles.length);
    context.telemetry.properties.countConflictFiles = String(decompileResult.conflictingOutputPaths.length);

    if (decompileResult.errorMessage) {
      // Language server will have already shown the message
      context.errorHandling.suppressDisplay = true;
      throw new Error("Decompilation failed");
    }

    // If there are conflicts, ask if we should overwrite
    var mainBicepPath = decompileResult.outputFiles[0].absolutePath;
    var outputFolder = path.dirname(mainBicepPath);
    var overwrite = await this.queryOverwrite(context, outputFolder, decompileResult.outputFiles, decompileResult.conflictingOutputPaths);

    // Save the output files
    const saveParams: BicepDecompileSaveCommandParams = {
      decompileId: decompileResult.decompileId,
      outputFiles: decompileResult.outputFiles,
      overwrite
    };
    const saveResult: BicepDecompileSaveCommandResult = await this.client.sendRequest(
      "workspace/executeCommand",
      {
        command: "decompileSave",
        arguments: [saveParams],
      }
    );

    this.outputChannelManager.appendToOutputChannel(saveResult.output.trimEnd());
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

  private async queryOverwrite(context: IActionContext, outputFolder: string, outputFiles: DecompiledFile[], conflictingOutputPaths: DocumentUri[]): Promise<boolean> {
    let overwrite: boolean;
    let isSingleFileDecompilation = outputFiles.length === 1;

    if (conflictingOutputPaths.length === 0) {
      // No conflicts - write to intended absolute paths
      overwrite = true;
    } else {
      const overwriteAction: MessageItem = {
        title: isSingleFileDecompilation ? "Overwrite" : "Overwrite all"
      };
      const createCopyAction: MessageItem = {
        title: isSingleFileDecompilation ? "Create copy" : "New subfolder"
      };
      const cancelAction: MessageItem = {
        title: "Cancel",
        isCloseAffordance: true
      };

      const conflictFilesWithQuotes = conflictingOutputPaths.map(f => `"${f}"`).join(", ");
      const message =
        isSingleFileDecompilation ?
          `Output file already exists: ${conflictFilesWithQuotes}` :
          `There are multiple decompilation output files and the following already exist: ${conflictFilesWithQuotes}`;
      this.outputChannelManager.appendToOutputChannel(message.trimEnd());

      var result = await context.ui.showWarningMessage(message, overwriteAction, createCopyAction, cancelAction);
      if (result === cancelAction) {
        this.outputChannelManager.appendToOutputChannel("Canceled.");
        throw new UserCancelledError("queryOverwrite");
      }

      assert(result === overwriteAction || result === createCopyAction);
      overwrite = (result === overwriteAction);
      this.outputChannelManager.appendToOutputChannel(result.title);
      context.telemetry.properties.conflictResolution = overwrite ? "overwrite" : "copy";
    }

    return overwrite;
  }
}

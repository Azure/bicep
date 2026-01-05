// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import fse from "fs-extra";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import { commands, Uri, window, workspace } from "vscode";
import type { LanguageClient } from "vscode-languageclient/node";
import { extractToModuleRequestType, ExtractToModuleParams, ExtractToModuleResult } from "../language/protocol";
import { bicepLanguageId } from "../language/constants";
import { Command } from "./types";

export class ExtractToModuleCommand implements Command {
  public readonly id = "bicep.extractToModule";

  public constructor(private readonly client: LanguageClient) {}

  public async execute(_: IActionContext, documentUri?: Uri): Promise<void> {
    const editor = window.activeTextEditor;
    if (!editor || editor.document.languageId !== bicepLanguageId) {
      await window.showErrorMessage("Extract to module is only available for Bicep files.");
      return;
    }

    const selection = editor.selection;
    if (selection.isEmpty) {
      await window.showWarningMessage("Select one or more top-level resources to extract.");
      return;
    }

    const targetUri = this.getTargetUri(documentUri ?? editor.document.uri);
    const targetPath = await window.showInputBox({
      prompt: "Enter a path for the new module file",
      value: targetUri.fsPath,
    });

    if (!targetPath) {
      return;
    }

    const resolvedTarget = this.resolveTargetUri(targetPath, editor.document.uri);

    if (await this.fileExists(resolvedTarget)) {
      const choice = await window.showWarningMessage(
        `File ${resolvedTarget.fsPath} already exists. Overwrite?`,
        { modal: true },
        "Overwrite",
      );

      if (choice !== "Overwrite") {
        return;
      }
    }

    const params: ExtractToModuleParams = {
      textDocument: this.client.code2ProtocolConverter.asTextDocumentIdentifier(editor.document),
      range: this.client.code2ProtocolConverter.asRange(selection),
      moduleFilePath: resolvedTarget.fsPath,
    };

    const result = await this.client.sendRequest<ExtractToModuleResult>(extractToModuleRequestType.method, params);

    if (!result || !result.moduleFileContents || !result.replacementText) {
      await window.showErrorMessage("Extract to module failed. See language server output for details.");
      return;
    }

    await fse.ensureDir(path.dirname(resolvedTarget.fsPath));
    await workspace.fs.writeFile(resolvedTarget, Buffer.from(result.moduleFileContents, "utf8"));

    const workspaceEdit = await this.client.protocol2CodeConverter.asWorkspaceEdit({
      changes: {
        [editor.document.uri.toString()]: [
          {
            range: result.replacementRange,
            newText: result.replacementText,
          },
        ],
      },
    });

    if (workspaceEdit) {
      await workspace.applyEdit(workspaceEdit);
    }

    if (result.renamePosition) {
      const position = this.client.protocol2CodeConverter.asPosition(result.renamePosition);
      await commands.executeCommand("editor.action.rename", [editor.document.uri, position]);
    }
  }

  private getTargetUri(documentUri: Uri): Uri {
    const defaultFileName = "extractedModule.bicep";
    const folder = path.dirname(documentUri.fsPath);
    return Uri.file(path.join(folder, defaultFileName));
  }

  private resolveTargetUri(inputPath: string, documentUri: Uri): Uri {
    if (path.isAbsolute(inputPath)) {
      return Uri.file(inputPath);
    }

    const folder = path.dirname(documentUri.fsPath);
    return Uri.file(path.resolve(folder, inputPath));
  }

  private async fileExists(uri: Uri): Promise<boolean> {
    try {
      await workspace.fs.stat(uri);
      return true;
    } catch {
      return false;
    }
  }
}

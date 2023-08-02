// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as os from "os";
import * as fse from "fs-extra";
import vscode, {
  TextDocument,
  TextEditor,
  Uri,
  window,
  workspace,
} from "vscode";
import { UserCancelledError } from "@microsoft/vscode-azext-utils";

import { Command } from "../types";
import { bicepFileExtension } from "../../language/constants";

export class WalkthroughCreateBicepFileCommand implements Command {
  public static id = "bicep.gettingStarted.createBicepFile";
  public readonly id = WalkthroughCreateBicepFileCommand.id;

  public async execute(): Promise<TextEditor> {
    return await createAndOpenBicepFile("");
  }
}

async function createAndOpenBicepFile(
  fileContents: string,
): Promise<vscode.TextEditor> {
  const folder: Uri =
    (workspace.workspaceFolders
      ? workspace.workspaceFolders[0].uri
      : undefined) ?? Uri.file(os.homedir());
  const uri: Uri | undefined = await window.showSaveDialog({
    title: "Save new Bicep file",
    defaultUri: Uri.joinPath(folder, "main"),
    filters: { "Bicep files": [bicepFileExtension] },
  });
  if (!uri) {
    throw new UserCancelledError("saveDialog");
  }

  const path = uri.fsPath;
  if (!path) {
    throw new Error(`Can't save file to location ${uri.toString()}`);
  }

  await fse.writeFile(path, fileContents, { encoding: "utf-8" });

  const document: TextDocument = await workspace.openTextDocument(uri);
  return await vscode.window.showTextDocument(
    document,
    vscode.ViewColumn.Beside,
  );
}

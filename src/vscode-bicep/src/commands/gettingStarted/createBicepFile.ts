// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as os from "os";
import * as fse from "fs-extra";
import vscode, { TextDocument, Uri, window, workspace } from "vscode";
import { UserCancelledError } from "vscode-azureextensionui";

import { Command } from "../types";

export class CreateBicepFileCommand implements Command {
  public readonly id = "bicep.gettingStarted.createBicepFile";

  public async execute(): Promise<void> {
    await createAndOpenBicepFile("");
  }
}

async function createAndOpenBicepFile(
  fileContents: string
): Promise<vscode.TextEditor | undefined> {
  const folder: Uri =
    (workspace.workspaceFolders
      ? workspace.workspaceFolders[0].uri
      : undefined) ?? Uri.file(os.tmpdir());
  const uri: Uri | undefined = await window.showSaveDialog({
    defaultUri: Uri.joinPath(folder, "untitled.bicep"),
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
    vscode.ViewColumn.Beside
  );
}

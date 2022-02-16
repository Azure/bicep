// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as os from "os";
import * as fse from "fs-extra";
import vscode, { TextDocument, Uri, window, workspace } from "vscode";
import {
  IActionContext,
  IAzureQuickPickItem,
  UserCancelledError,
} from "vscode-azureextensionui";

import { Command } from "../types";

export class OpenBicepFileCommand implements Command {
  public readonly id = "bicep.gettingStarted.openBicepFile";

  public async execute(): Promise<void> {
    await openBicepFile();
  }
}

async function openBicepFile(): Promise<vscode.TextEditor | undefined> {
  const folder: Uri =
    (workspace.workspaceFolders
      ? workspace.workspaceFolders[0].uri
      : undefined) ?? Uri.file(os.tmpdir());
  const uri: Uri | undefined = await window.showSaveDialog({
    defaultUri: Uri.joinPath(folder, "main.bicep"),
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

async function findBicepFilesInWorkspace(
  context: IActionContext
): Promise<string> {
  //asdfg time limit?
  const foundBicepFiles = await workspace.findFiles(
    "**/*.bicep",
    undefined,
    10
  );

  const entries: IAzureQuickPickItem<string | undefined>[] = foundBicepFiles
    .map((f) => f.fsPath)
    .filter((f) => !!f)
    .map(
      (f) =>
        <IAzureQuickPickItem<string>>{
          label: f,
          data: f,
        }
    );
  const browse: IAzureQuickPickItem<string | undefined> = {
    label: "Browse...",
    data: undefined,
  };
  entries.push(browse);

  const response = await context.ui.showQuickPick(entries, {});
  let path: string;

  if (response === browse) {
    const browsedFile: Uri = context.ui.showOpenDialog({
      filters: { "Bicep files": ["bicep"] },
      title: "Open a Bicep file",
    });
    path = browsedFile.fsPath;
  } else {
    path = response.data;
  }

  const folder: Uri = workspace.workspaceFolders
    ? workspace.workspaceFolders[0].uri
    : undefined;
  const uri: Uri | undefined = await window.showSaveDialog({
    defaultUri: Uri.joinPath(folder, "main.bicep"),
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

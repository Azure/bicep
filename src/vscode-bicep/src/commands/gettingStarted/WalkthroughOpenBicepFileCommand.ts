// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { TextDocument, Uri, workspace, window, ViewColumn } from "vscode";
import { IActionContext, IAzureQuickPickItem } from "vscode-azureextensionui";

import { Command } from "../types";

export class WalkthroughOpenBicepFileCommand implements Command {
  public readonly id = "bicep.gettingStarted.openBicepFile";

  public async execute(context: IActionContext): Promise<void> {
    await queryAndOpenBicepFile(context);
  }
}

async function queryAndOpenBicepFile(context: IActionContext): Promise<void> {
  const uri: Uri = await queryUserForBicepFile(context);
  const document: TextDocument = await workspace.openTextDocument(uri);
  await window.showTextDocument(document, ViewColumn.Beside);
}

async function queryUserForBicepFile(context: IActionContext): Promise<Uri> {
  //asdfg time limit?
  const foundBicepFiles = (
    await workspace.findFiles("**/*.bicep", undefined)
  ).filter((f) => !!f.fsPath);

  if (foundBicepFiles.length === 0) {
    return await browseForFile(context);
  }

  //asdfg
  // // Find the common file prefix so we can display relative paths
  // const prefix: string = foundBicepFiles[0].fsPath;
  // for (const uri of foundBicepFiles) {
  //   const a = path.relative(prefix, uri.fsPath);
  //   const b = path.relative(uri.fsPath, prefix);
  //   const c = a;
  // }

  const entries: IAzureQuickPickItem<Uri | undefined>[] = foundBicepFiles.map(
    (u) => {
      const workspaceRoot: string | undefined =
        workspace.getWorkspaceFolder(u)?.uri.fsPath;
      const relativePath = workspaceRoot
        ? path.relative(workspaceRoot, u.fsPath)
        : path.basename(u.fsPath);

      return <IAzureQuickPickItem<Uri>>{
        label: relativePath,
        data: u,
      };
    }
  );
  const browse: IAzureQuickPickItem<Uri | undefined> = {
    label: "Browse...",
    data: undefined,
  };
  entries.unshift(browse);

  const response = await context.ui.showQuickPick(entries, {
    placeHolder: "Select a Bicep file to open",
  });

  if (response === browse) {
    return await browseForFile(context);
  } else {
    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
    return response.data!;
  }
}

async function browseForFile(context: IActionContext): Promise<Uri> {
  const browsedFile: Uri[] = await context.ui.showOpenDialog({
    filters: { "Bicep files": ["bicep"] },
    title: "Open a Bicep file",
  });

  return browsedFile[0];
}

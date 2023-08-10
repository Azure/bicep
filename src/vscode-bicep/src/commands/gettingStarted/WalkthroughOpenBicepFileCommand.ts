// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import {
  TextDocument,
  Uri,
  workspace,
  window,
  ViewColumn,
  TextEditor,
} from "vscode";
import {
  IActionContext,
  IAzureQuickPickItem,
} from "@microsoft/vscode-azext-utils";

import { Command } from "../types";
import { bicepFileExtension } from "../../language/constants";

export class WalkthroughOpenBicepFileCommand implements Command {
  public static id = "bicep.gettingStarted.openBicepFile";
  public readonly id = WalkthroughOpenBicepFileCommand.id;

  public async execute(context: IActionContext): Promise<TextEditor> {
    return await queryAndOpenBicepFile(context);
  }
}

async function queryAndOpenBicepFile(
  context: IActionContext,
): Promise<TextEditor> {
  const uri: Uri = await queryUserForBicepFile(context);
  const document: TextDocument = await workspace.openTextDocument(uri);
  return await window.showTextDocument(document, ViewColumn.Beside);
}

async function queryUserForBicepFile(context: IActionContext): Promise<Uri> {
  const foundBicepFiles = (
    await workspace.findFiles("**/*.bicep", undefined)
  ).filter((f) => !!f.fsPath);

  if (foundBicepFiles.length === 0) {
    return await browseForFile(context);
  }

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
    },
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
    if (response.data) {
      return response.data;
    } else {
      throw new Error(
        "Internal error: queryUserForBicepFile: response.data should be truthy",
      );
    }
  }
}

async function browseForFile(context: IActionContext): Promise<Uri> {
  const browsedFile: Uri[] = await context.ui.showOpenDialog({
    title: "Open a Bicep file",
    filters: { "Bicep files": [bicepFileExtension] },
  });

  return browsedFile[0];
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import vscode, { Uri } from "vscode";

import {
  IActionContext,
  IAzureQuickPickItem,
} from "@microsoft/vscode-azext-utils";

import { appendToOutputChannel } from "../utils/logger";

export async function selectParameterFile(
  _context: IActionContext,
  sourceUri: Uri | undefined
): Promise<string> {
  const quickPickItems: IAzureQuickPickItem[] =
    await createParameterFileQuickPickList();
  const result: IAzureQuickPickItem =
    await _context.ui.showQuickPick(quickPickItems, {
      canPickMany: false,
      placeHolder: `Select a parameter file`,
      suppressPersistence: true,
    });

  if (result.label.includes("Browse...")) {
    const paramsPaths: Uri[] | undefined = await vscode.window.showOpenDialog({
      canSelectMany: false,
      defaultUri: sourceUri,
      openLabel: "Select Parameter File",
    });
    if (paramsPaths && paramsPaths.length == 1) {
      const parameterFilePath = paramsPaths[0].fsPath;
      appendToOutputChannel(
        `Parameter file used in deployment -> ${path.basename(
          parameterFilePath
        )}`
      );
      return parameterFilePath;
    }
  }

  appendToOutputChannel(`Parameter file was not provided`);

  return "";
}

async function createParameterFileQuickPickList(): Promise<IAzureQuickPickItem[]> {
  const none: IAzureQuickPickItem = {
    label: "$(circle-slash) None",
    data: undefined,
  };
  const browse: IAzureQuickPickItem = {
    label: "$(file-directory) Browse...",
    data: undefined,
  };

  return [
    none,
  ].concat([browse]);
}

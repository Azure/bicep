// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

export async function executeHoverProviderCommand(
  documentUri: vscode.Uri,
  position: vscode.Position
): Promise<vscode.Hover[] | undefined> {
  return await vscode.commands.executeCommand<vscode.Hover[]>(
    "vscode.executeHoverProvider",
    documentUri,
    position
  );
}

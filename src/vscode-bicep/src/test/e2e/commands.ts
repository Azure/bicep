// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

// More can be added as needed: https://code.visualstudio.com/api/references/commands.

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

export async function executeCompletionItemProviderCommand(
  documentUri: vscode.Uri,
  position: vscode.Position
): Promise<vscode.CompletionList | undefined> {
  return await vscode.commands.executeCommand<vscode.CompletionList>(
    "vscode.executeCompletionItemProvider",
    documentUri,
    position
  );
}

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

export async function executeShowVisualizerCommand(
  documentUri: vscode.Uri
): Promise<vscode.ViewColumn | undefined> {
  return await vscode.commands.executeCommand(
    "bicep.showVisualizer",
    documentUri
  );
}

export async function executeShowVisualizerToSideCommand(
  documentUri: vscode.Uri
): Promise<vscode.ViewColumn | undefined> {
  return await vscode.commands.executeCommand(
    "bicep.showVisualizerToSide",
    documentUri
  );
}

export async function executeShowSourceCommand(): Promise<
  vscode.TextEditor | undefined
> {
  return await vscode.commands.executeCommand("bicep.showSource");
}

export async function executeBuildCommand(
  documentUri: vscode.Uri
): Promise<void> {
  return await vscode.commands.executeCommand("bicep.build", documentUri);
}

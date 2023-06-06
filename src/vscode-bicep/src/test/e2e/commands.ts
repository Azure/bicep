// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as vscode from "vscode";

// More can be added as needed: https://code.visualstudio.com/api/references/commands.

export async function executeCloseAllEditors(): Promise<void> {
  await vscode.commands.executeCommand("workbench.action.closeAllEditors");
}

export async function executeHoverProvider(
  documentUri: vscode.Uri,
  position: vscode.Position
): Promise<vscode.Hover[] | undefined> {
  return await vscode.commands.executeCommand<vscode.Hover[]>(
    "vscode.executeHoverProvider",
    documentUri,
    position
  );
}

export async function executeCompletionItemProvider(
  documentUri: vscode.Uri,
  position: vscode.Position
): Promise<vscode.CompletionList | undefined> {
  return await vscode.commands.executeCommand<vscode.CompletionList>(
    "vscode.executeCompletionItemProvider",
    documentUri,
    position
  );
}

export async function executeAcceptSelectedSuggestion(): Promise<void> {
  await vscode.commands.executeCommand("acceptSelectedSuggestion");
}

export async function executeSelectNextSuggestion(): Promise<void> {
  await vscode.commands.executeCommand("selectNextSuggestion");
}

export async function executeTypeText(text: string): Promise<void> {
  return await vscode.commands.executeCommand("type", { text });
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

export async function executeBuildParamsCommand(
  documentUri: vscode.Uri
): Promise<void> {
  return await vscode.commands.executeCommand("bicep.buildParams", documentUri);
}

export async function executeDecompileCommand(
  documentUri: vscode.Uri
): Promise<void> {
  return await vscode.commands.executeCommand("bicep.decompile", documentUri);
}

export async function executeCreateConfigFileCommand(
  documentUri?: vscode.Uri
): Promise<string | undefined> {
  return await vscode.commands.executeCommand<string>(
    "bicep.createConfigFile",
    documentUri,
    true, // suppressQuery
    true // rethrow
  );
}

export async function executeForceModulesRestoreCommand(
  documentUri: vscode.Uri
): Promise<void> {
  return await vscode.commands.executeCommand(
    "bicep.forceModulesRestore",
    documentUri
  );
}

export async function executePasteAsBicepCommand(
  documentUri: vscode.Uri
): Promise<void> {
  return await vscode.commands.executeCommand(
    "bicep.pasteAsBicep",
    documentUri
  );
}

export async function executeEditorPasteCommand(): Promise<void> {
  return await vscode.commands.executeCommand(
    "editor.action.clipboardPasteAction"
  );
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { commands, CompletionList, Hover, Position, TextEditor, Uri, ViewColumn } from "vscode";
import { ShowSourceFromVisualizerCommand } from "../../../src/features/visualization";

// More can be added as needed: https://code.visualstudio.com/api/references/commands.

export async function executeCloseAllEditors(): Promise<void> {
  await commands.executeCommand("workbench.action.closeAllEditors");
}

export async function executeHoverProvider(documentUri: Uri, position: Position): Promise<Hover[] | undefined> {
  return await commands.executeCommand<Hover[]>("vscode.executeHoverProvider", documentUri, position);
}

export async function executeCompletionItemProvider(
  documentUri: Uri,
  position: Position,
): Promise<CompletionList | undefined> {
  return await commands.executeCommand<CompletionList>("vscode.executeCompletionItemProvider", documentUri, position);
}

export async function executeShowVisualizerCommand(documentUri: Uri): Promise<ViewColumn | undefined> {
  return await commands.executeCommand("bicep.showVisualizer", documentUri);
}

export async function executeShowVisualizerToSideCommand(documentUri: Uri): Promise<ViewColumn | undefined> {
  return await commands.executeCommand("bicep.showVisualizerToSide", documentUri);
}

export async function executeShowDeployPaneCommand(documentUri: Uri): Promise<ViewColumn | undefined> {
  return await commands.executeCommand("bicep.showDeployPane", documentUri);
}

export async function executeShowDeployPaneToSideCommand(documentUri: Uri): Promise<ViewColumn | undefined> {
  return await commands.executeCommand("bicep.showDeployPaneToSide", documentUri);
}

export async function executeShowSourceCommand(): Promise<TextEditor | undefined> {
  return await commands.executeCommand(ShowSourceFromVisualizerCommand.CommandId);
}

export async function executeBuildCommand(documentUri: Uri): Promise<void> {
  return await commands.executeCommand("bicep.build", documentUri);
}

export async function executeBuildParamsCommand(documentUri: Uri): Promise<void> {
  return await commands.executeCommand("bicep.buildParams", documentUri);
}

export async function executeGenerateParamsCommand(documentUri: Uri): Promise<void> {
  return await commands.executeCommand("bicep.generateParams", documentUri);
}

export async function executeDecompileCommand(documentUri: Uri): Promise<void> {
  return await commands.executeCommand("bicep.decompile", documentUri);
}

export async function executeCreateConfigFileCommand(documentUri?: Uri): Promise<string | undefined> {
  return await commands.executeCommand<string>(
    "bicep.createConfigFile",
    documentUri,
    true, // suppressQuery
    true, // rethrow
  );
}

export async function executePasteAsBicepCommand(documentUri: Uri, suppressErrorDisplay = false): Promise<void> {
  return await commands.executeCommand("bicep.pasteAsBicep", documentUri, suppressErrorDisplay);
}

export async function executeEditorPasteCommand(): Promise<void> {
  return await commands.executeCommand("editor.action.clipboardPasteAction");
}

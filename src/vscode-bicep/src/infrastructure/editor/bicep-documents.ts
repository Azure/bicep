// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as os from "os";
import * as path from "path";
import * as fse from "fs-extra";
import { TextDocument, TextEditor, Uri, window, workspace } from "vscode";
import { UserCancelledError } from "../errors";
import { PromptItem, Prompts } from "../prompts";

export const bicepFileExtension = "bicep";
export const bicepLanguageId = "bicep";
export const bicepParamLanguageId = "bicep-params";

/**
 * Determines which bicep file to target for a command.
 * @summary
 * In all cases, if a URL is passed in, it will be targeted (this handles all scenarios except invoking through a shortcut key or the command palette, e.g.
 *   context menus will always pass in a URI).
 * For shortcut keys and command palette, the behavior is:
 *   1) If there is an active bicep file, use that.
 *   2) If there's only a single bicep command available in the current workspace or visible editors, use that.
 *   3) If there are no bicep files in the workspace and visible editors, ask the user if they want to create a new file.
 *   4) Finally, show a pick list of available bicep files and allow them to choose (if the current editor is a bicep file, it will be at the top of the list be we will still ask).
 * @throws User-cancelled error
 */
export async function findOrCreateActiveBicepFile(
  prompts: Prompts,
  documentUri: Uri | undefined,
  prompt: string,
  includeBicepParam = false,
): Promise<Uri> {
  const matchesLanguageId = (editor: TextEditor) => {
    const languageId = editor.document.languageId;
    return languageId === bicepLanguageId || (includeBicepParam && languageId === bicepParamLanguageId);
  };

  if (documentUri) {
    // The command specified a specific URI, so act on that (right-click or context menu).
    // The only scenario where we should *not* have a documentUri is when a command is invoked through the command
    //   palette or through a shortcut key.
    return documentUri;
  }

  const activeEditor = window.activeTextEditor;
  if (activeEditor && matchesLanguageId(activeEditor)) {
    return activeEditor.document.uri;
  }

  const globPattern = includeBicepParam ? "**/*.{bicep, bicepparam}" : "**/*.bicep";

  const workspaceBicepFiles = (await workspace.findFiles(globPattern, undefined)).filter((f) => !!f.fsPath);
  const visibleBicepFiles = window.visibleTextEditors // List of the active editor in each editor tab group
    .filter(matchesLanguageId)
    .map((e) => e.document.uri);

  // Create deduped, sorted array of all available Bicep files (in workspace and visible editors)
  const map = new Map<string, Uri>();
  workspaceBicepFiles.concat(visibleBicepFiles).forEach((bf) => map.set(bf.fsPath, bf));
  const bicepFilesSorted = Array.from(map.values());
  bicepFilesSorted.sort((a, b) => compareStringsOrdinal(a.path, b.path));

  if (bicepFilesSorted.length === 1) {
    // Only a single Bicep file in the workspace/visible editors - choose it
    return bicepFilesSorted[0];
  }

  if (bicepFilesSorted.length === 0) {
    // Ask to create a new Bicep file...
    return await queryCreateBicepFile(prompts);
  }

  // We need to ask the user which existing file to use
  // Show quick pick
  const entries: PromptItem<Uri>[] = [];
  bicepFilesSorted.forEach((u) => addFileQuickPick(entries, u));

  const response = await prompts.showQuickPick(entries, {
    placeHolder: prompt,
  });
  return response.data;
}

export async function findOrCreateActiveBicepParamFile(
  prompts: Prompts,
  documentUri: Uri | undefined,
  prompt: string,
): Promise<Uri> {
  if (documentUri) {
    // The command specified a specific URI, so act on that (right-click or context menu).
    // The only scenario where we should *not* have a documentUri is when a command is invoked through the command
    //   palette or through a shortcut key.
    return documentUri;
  }

  const activeEditor = window.activeTextEditor;
  if (activeEditor?.document.languageId === bicepParamLanguageId) {
    return activeEditor.document.uri;
  }

  const workspaceBicepFiles = (await workspace.findFiles("**/*.bicepparam", undefined)).filter((f) => !!f.fsPath);
  const visibleBicepFiles = window.visibleTextEditors // List of the active editor in each editor tab group
    .filter((e) => e.document.languageId === bicepParamLanguageId)
    .map((e) => e.document.uri);

  // Create deduped, sorted array of all available Bicep files (in workspace and visible editors)
  const map = new Map<string, Uri>();
  workspaceBicepFiles.concat(visibleBicepFiles).forEach((bf) => map.set(bf.fsPath, bf));
  const bicepFilesSorted = Array.from(map.values());
  bicepFilesSorted.sort((a, b) => compareStringsOrdinal(a.path, b.path));

  if (bicepFilesSorted.length === 1) {
    // Only a single Bicep file in the workspace/visible editors - choose it
    return bicepFilesSorted[0];
  }

  if (bicepFilesSorted.length === 0) {
    // Ask to create a new Bicep file...
    return await queryCreateBicepFile(prompts);
  }

  // We need to ask the user which existing file to use
  // Show quick pick
  const entries: PromptItem<Uri>[] = [];
  bicepFilesSorted.forEach((u) => addFileQuickPick(entries, u));

  const response = await prompts.showQuickPick(entries, {
    placeHolder: prompt,
  });
  return response.data;
}

function addFileQuickPick(items: PromptItem<Uri>[], uri: Uri): void {
  if (items.find((i) => i.data === uri)) {
    return;
  }

  const workspaceRoot: string | undefined = workspace.getWorkspaceFolder(uri)?.uri.fsPath;
  const relativePath = workspaceRoot ? path.relative(workspaceRoot, uri.fsPath) : path.basename(uri.fsPath);

  items.push({
    label: relativePath,
    data: uri,
    alwaysShow: true,
    id: uri.path,
  });
}

function compareStringsOrdinal(left: string, right: string): number {
  return left < right ? -1 : left > right ? 1 : 0;
}

async function queryCreateBicepFile(prompts: Prompts): Promise<Uri> {
  const response = await prompts.showWarningMessage(
    "Couldn't find any Bicep files in your workspace. Would you like to create a Bicep file?",
    "Yes",
    "Cancel",
  );
  if (response !== "Yes") {
    throw new UserCancelledError("createBicepFile");
  }

  const startingFolder: Uri =
    (workspace.workspaceFolders ? workspace.workspaceFolders[0].uri : undefined) ?? Uri.file(os.homedir());
  const uri: Uri | undefined = await window.showSaveDialog({
    title: "Save new Bicep file",
    defaultUri: Uri.joinPath(startingFolder, "main"),
    filters: { "Bicep files": [bicepFileExtension] },
  });
  if (!uri) {
    throw new UserCancelledError("saveDialog");
  }

  const path = uri.fsPath;
  if (!path) {
    throw new Error(`Can't save file to location ${uri.toString()}`);
  }

  await fse.writeFile(
    path,
    "@description('Location of all resources')\nparam location string = resourceGroup().location\n",
    { encoding: "utf-8" },
  );

  const document: TextDocument = await workspace.openTextDocument(uri);
  await window.showTextDocument(document);

  return uri;
}

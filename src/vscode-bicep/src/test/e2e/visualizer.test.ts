// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import path from "path";
import fs from "fs";

import { readExampleFile } from "./examples";
import {
  executeShowSourceCommand,
  executeShowVisualizerCommand,
  executeShowVisualizerToSideCommand,
} from "./commands";
import { retryWhile, sleep } from "../utils/time";
import { expectDefined } from "../utils/assert";

const extensionLogPath = path.join(__dirname, "../../../bicep.log");

describe("visualizer", (): void => {
  let document: vscode.TextDocument;
  let content: string;
  let readyMessage: string;
  let sourceColumn: vscode.ViewColumn;

  beforeEach(async () => {
    content = readExampleFile("201", "sql");
    // Open the document once to warm up the language server.
    document = await vscode.workspace.openTextDocument({
      language: "bicep",
      content,
    });
    readyMessage = `Visualizer for ${document.uri.fsPath} is ready.`;

    const editor = await vscode.window.showTextDocument(document);
    expectDefined(editor.viewColumn);
    sourceColumn = editor.viewColumn;

    // Give the language server sometime to finish compilation.
    await sleep(2000);
  });

  afterEach(async () => {
    await vscode.commands.executeCommand("workbench.action.closeAllEditors");
  });

  it("should open visualizer webview", async () => {
    const viewColumn = await retryWhile(
      async () => await executeShowVisualizerCommand(document.uri),
      () => !exsitsInExtensionLog(readyMessage)
    );

    expect(exsitsInExtensionLog(readyMessage)).toBeTruthy();
    expectDefined(viewColumn);
    expect(viewColumn).toBe(sourceColumn);
  });

  it("should open visualizer webview to side", async () => {
    const viewColumn = await retryWhile(
      async () => await executeShowVisualizerToSideCommand(document.uri),
      () => !exsitsInExtensionLog(readyMessage)
    );

    expect(exsitsInExtensionLog(readyMessage)).toBeTruthy();
    expectDefined(viewColumn);
    expect(viewColumn).toBe(vscode.ViewColumn.Beside);
  });

  it("should open source", async () => {
    await vscode.commands.executeCommand("workbench.action.closeActiveEditor");
    expect(vscode.window.activeTextEditor).toBeUndefined();

    await retryWhile(
      async () => await executeShowVisualizerCommand(document.uri),
      () => !exsitsInExtensionLog(readyMessage)
    );

    expect(exsitsInExtensionLog(readyMessage)).toBeTruthy();

    const sourceEditor = await executeShowSourceCommand();

    expectDefined(sourceEditor);
    expect(sourceEditor).toBe(vscode.window.activeTextEditor);
  });

  function exsitsInExtensionLog(text: string): boolean {
    if (!fs.existsSync(extensionLogPath)) {
      return false;
    }

    return fs.readFileSync(extensionLogPath).indexOf(text) >= 0;
  }
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import vscode from "vscode";
import { expectDefined } from "../test-support/assert";
import { until } from "../test-support/polling";
import {
  executeCloseAllEditors,
  executeShowSourceCommand,
  executeShowVisualizerCommand,
  executeShowVisualizerToSideCommand,
} from "./commands";
import { getE2eLogPath } from "./e2e-log";
import { resolveExamplePath } from "./examples";

const extensionLogPath = getE2eLogPath();

describe("visualizer", (): void => {
  afterEach(executeCloseAllEditors);

  it("should open visualizer webview", async () => {
    const examplePath = resolveExamplePath("101", "vm-simple-linux");
    const document = await vscode.workspace.openTextDocument(examplePath);
    const editor = await vscode.window.showTextDocument(document);

    const viewColumn = await executeShowVisualizerCommand(document.uri);
    await until(() => visualizerIsReady(document.uri), {
      interval: 100,
      timeoutMs: 20000,
    });
    if (!visualizerIsReady(document.uri)) {
      throw new Error(`Expected visualizer to be ready for ${document.uri.toString()}`);
    }
    expectDefined(viewColumn);
    expect(viewColumn).toBe(editor.viewColumn);
  });

  it("should open visualizer webview to side", async () => {
    const examplePath = resolveExamplePath("201", "sql");
    const document = await vscode.workspace.openTextDocument(examplePath);
    await vscode.window.showTextDocument(document);

    const viewColumn = await executeShowVisualizerToSideCommand(document.uri);
    await until(() => visualizerIsReady(document.uri), {
      interval: 100,
      timeoutMs: 20000,
    });
    if (!visualizerIsReady(document.uri)) {
      throw new Error(`Expected visualizer to be ready for ${document.uri.toString()}`);
    }
    expectDefined(viewColumn);
    expect(viewColumn).toBe(vscode.ViewColumn.Beside);
  });

  it("should open source", async () => {
    const examplePath = resolveExamplePath("000", "empty");
    const document = await vscode.workspace.openTextDocument(examplePath);

    await executeShowVisualizerToSideCommand(document.uri);

    await until(() => visualizerIsReady(document.uri), {
      interval: 100,
      timeoutMs: 20000,
    });

    if (!visualizerIsReady(document.uri)) {
      throw new Error(`Expected visualizer to be ready for ${document.uri.toString()}`);
    }

    const sourceEditor = await executeShowSourceCommand();

    expectDefined(sourceEditor);
    expect(sourceEditor.document).toBe(document);
    expect(sourceEditor).toBe(vscode.window.activeTextEditor);
  });

  function visualizerIsReady(documentUri: vscode.Uri): boolean {
    if (!fs.existsSync(extensionLogPath)) {
      return false;
    }

    const readyMessage = `Visualizer for ${documentUri.fsPath} is ready.`;
    return fs.readFileSync(extensionLogPath).indexOf(readyMessage) >= 0;
  }
});

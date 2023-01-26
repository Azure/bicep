// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import path from "path";
import vscode from "vscode";
import { sleep } from "../../utils/time";

import { expectDefined } from "../utils/assert";
import { until } from "../utils/time";
import {
  executeCloseAllEditors,
  executeShowSourceCommand,
  executeShowVisualizerCommand,
  executeShowVisualizerToSideCommand,
} from "./commands";
import { resolveExamplePath } from "./examples";

const extensionLogPath = path.join(__dirname, "../../../bicep.log");

describe("visualizer", (): void => {
  afterEach(executeCloseAllEditors);

  it("should open visualizer webview", async () => {
    const examplePath = resolveExamplePath("101", "vm-simple-linux");
    const document = await vscode.workspace.openTextDocument(examplePath);
    const editor = await vscode.window.showTextDocument(document);

    // Give the language server some time to finish compilation.
    await sleep(2000);

    const viewColumn = await executeShowVisualizerCommand(document.uri);
    await until(() => visualizerIsReady(document.uri), {
      interval: 100,
    });
    if (!visualizerIsReady(document.uri)) {
      throw new Error(
        `Expected visualizer to be ready for ${document.uri.toString()}`
      );
    }
    expectDefined(viewColumn);
    expect(viewColumn).toBe(editor.viewColumn);
  });

  it("should open visualizer webview to side", async () => {
    const examplePath = resolveExamplePath("201", "sql");
    const document = await vscode.workspace.openTextDocument(examplePath);
    await vscode.window.showTextDocument(document);

    // Give the language server some time to finish compilation.
    await sleep(2000);
    const viewColumn = await executeShowVisualizerToSideCommand(document.uri);
    await until(() => visualizerIsReady(document.uri), {
      interval: 100,
    });
    if (!visualizerIsReady(document.uri)) {
      throw new Error(
        `Expected visualizer to be ready for ${document.uri.toString()}`
      );
    }
    expectDefined(viewColumn);
    expect(viewColumn).toBe(vscode.ViewColumn.Beside);
  });

  it("should open source", async () => {
    expect(vscode.window.activeTextEditor).toBeUndefined();

    const examplePath = resolveExamplePath("000", "empty");
    const document = await vscode.workspace.openTextDocument(examplePath);

    await executeShowVisualizerToSideCommand(document.uri);

    await until(() => visualizerIsReady(document.uri), {
      interval: 100,
    });

    if (!visualizerIsReady(document.uri)) {
      throw new Error(
        `Expected visualizer to be ready for ${document.uri.toString()}`
      );
    }

    const sourceEditor = await executeShowSourceCommand();

    expectDefined(sourceEditor);
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

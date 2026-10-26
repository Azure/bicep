// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";
import { ViewColumn, window, workspace } from "vscode";
import { resolveExamplePath } from "./examples";
import {
  executeCloseAllEditors,
  executeShowSourceCommand,
  executeShowVisualizerCommand,
  executeShowVisualizerToSideCommand,
} from "./utils/commands";

describe("visualizer", (): void => {
  afterEach(executeCloseAllEditors);

  it("should open visualizer webview", async () => {
    const examplePath = resolveExamplePath("101", "vm-simple-linux");
    const document = await workspace.openTextDocument(examplePath);
    const editor = await window.showTextDocument(document);

    const viewColumn = await executeShowVisualizerCommand(document.uri);
    assert(viewColumn !== undefined);
    expect(viewColumn).toBe(editor.viewColumn);
  });

  it("should open visualizer webview to side", async () => {
    const examplePath = resolveExamplePath("201", "sql");
    const document = await workspace.openTextDocument(examplePath);
    await window.showTextDocument(document);

    const viewColumn = await executeShowVisualizerToSideCommand(document.uri);
    assert(viewColumn !== undefined);
    expect(viewColumn).toBe(ViewColumn.Beside);
  });

  it("should open source", async () => {
    const examplePath = resolveExamplePath("000", "empty");
    const document = await workspace.openTextDocument(examplePath);

    await executeShowVisualizerToSideCommand(document.uri);

    const sourceEditor = await executeShowSourceCommand();

    assert(sourceEditor);
    expect(sourceEditor.document).toBe(document);
    expect(sourceEditor).toBe(window.activeTextEditor);
  });
});

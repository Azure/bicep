// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";
import vscode from "vscode";
import { resolveExamplePath } from "./examples";
import {
  executeCloseAllEditors,
  executeShowDeployPaneCommand,
  executeShowDeployPaneToSideCommand,
} from "./utils/commands";

describe("deploypane", (): void => {
  afterEach(executeCloseAllEditors);

  it.each([
    resolveExamplePath("101", "vm-simple-linux", "main.bicepparam"),
    resolveExamplePath("101", "vm-simple-linux", "main.bicep"),
  ])("should open deployment pane webview for %s", async (examplePath) => {
    const { document, editor } = await openDocument(examplePath);

    const viewColumn = await executeShowDeployPaneCommand(document.uri);
    assert(viewColumn !== undefined);
    expect(viewColumn).toBe(editor.viewColumn);
  });

  it.each([resolveExamplePath("201", "sql", "main.bicepparam"), resolveExamplePath("201", "sql", "main.bicep")])(
    "should open deployment pane webview to side for %s",
    async (examplePath) => {
      const { document } = await openDocument(examplePath);

      const viewColumn = await executeShowDeployPaneToSideCommand(document.uri);
      assert(viewColumn !== undefined);
      expect(viewColumn).toBe(vscode.ViewColumn.Beside);
    },
  );
});

async function openDocument(path: string) {
  const document = await vscode.workspace.openTextDocument(path);
  const editor = await vscode.window.showTextDocument(document);

  return { document, editor };
}

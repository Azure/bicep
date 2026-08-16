// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import vscode from "vscode";
import { expectDefined } from "../test-support/assert";
import { until } from "../test-support/polling";
import { executeCloseAllEditors, executeShowDeployPaneCommand, executeShowDeployPaneToSideCommand } from "./commands";
import { getE2eLogPath } from "./e2e-log";
import { resolveExamplePath } from "./examples";

const extensionLogPath = getE2eLogPath();

describe("deploypane", (): void => {
  afterEach(executeCloseAllEditors);

  it.each([
    resolveExamplePath("101", "vm-simple-linux", "main.bicepparam"),
    resolveExamplePath("101", "vm-simple-linux", "main.bicep"),
  ])("should open deployment pane webview for %s", async (examplePath) => {
    const { document, editor } = await openDocument(examplePath);

    const viewColumn = await executeShowDeployPaneCommand(document.uri);
    await waitForWebViewReady(document.uri);
    expectDefined(viewColumn);
    expect(viewColumn).toBe(editor.viewColumn);
  });

  it.each([resolveExamplePath("201", "sql", "main.bicepparam"), resolveExamplePath("201", "sql", "main.bicep")])(
    "should open deployment pane webview to side for %s",
    async (examplePath) => {
      const { document } = await openDocument(examplePath);

      const viewColumn = await executeShowDeployPaneToSideCommand(document.uri);
      await waitForWebViewReady(document.uri);
      expectDefined(viewColumn);
      expect(viewColumn).toBe(vscode.ViewColumn.Beside);
    },
  );
});

function webViewReady(documentUri: vscode.Uri): boolean {
  if (!fs.existsSync(extensionLogPath)) {
    return false;
  }

  const readyMessage = `Deployment Pane for ${documentUri.fsPath} is ready.`;
  return fs.readFileSync(extensionLogPath).indexOf(readyMessage) >= 0;
}

async function waitForWebViewReady(documentUri: vscode.Uri) {
  await until(() => webViewReady(documentUri), {
    interval: 100,
    timeoutMs: 30000,
  });
  if (!webViewReady(documentUri)) {
    throw new Error("Expected deployment pane to be ready");
  }
}

async function openDocument(path: string) {
  const document = await vscode.workspace.openTextDocument(path);
  const editor = await vscode.window.showTextDocument(document);

  return { document, editor };
}

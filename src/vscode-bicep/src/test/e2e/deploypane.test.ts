// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import path from "path";
import vscode from "vscode";
import { e2eLogName } from "../../utils/logger";
import { sleep } from "../../utils/time";
import { expectDefined } from "../utils/assert";
import { until } from "../utils/time";
import { executeCloseAllEditors, executeShowDeployPaneCommand, executeShowDeployPaneToSideCommand } from "./commands";
import { resolveExamplePath } from "./examples";

const extensionLogPath = path.join(__dirname, `../../../${e2eLogName}`);

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
    throw `Expected deployment pane to be ready`;
  }
}

async function openDocument(path: string) {
  const document = await vscode.workspace.openTextDocument(path);
  const editor = await vscode.window.showTextDocument(document);

  // Give the language server some time to finish compilation.
  await sleep(2000);

  return { document, editor };
}

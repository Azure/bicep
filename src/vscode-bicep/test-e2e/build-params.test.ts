// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import fs from "fs";
import path from "path";
import vscode from "vscode";
import { sleep } from "../src/infrastructure/timing";
import { executeBuildParamsCommand, executeCloseAllEditors } from "./commands";
import { resolveExamplePath } from "./examples";

describe("buildParams", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should generate compiled file if the source file has no errors", async () => {
    const examplePath = resolveExamplePath("201", "sql", "main.bicepparam");
    const compiledFilePath = path.join(path.dirname(examplePath), "main.parameters.json");
    fs.rmSync(compiledFilePath, { force: true });
    const textDocument = await vscode.workspace.openTextDocument(examplePath);

    try {
      // Give the language server some time to finish compilation.
      await sleep(2000);
      await executeBuildParamsCommand(textDocument.uri);
      expect(fs.existsSync(compiledFilePath)).toBe(true);
    } finally {
      fs.rmSync(compiledFilePath, { force: true });
    }
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import fs from "fs";
import path from "path";
import vscode from "vscode";
import { sleep } from "../src/infrastructure/timing";
import { executeBuildCommand, executeCloseAllEditors } from "./commands";
import { resolveExamplePath } from "./examples";

describe("build", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should generate compiled file if the source file has no errors", async () => {
    const examplePath = resolveExamplePath("201", "sql");
    const compiledFilePath = path.join(path.dirname(examplePath), "main.json");
    fs.rmSync(compiledFilePath, { force: true });
    const textDocument = await vscode.workspace.openTextDocument(examplePath);

    try {
      // Give the language server some time to finish compilation.
      await sleep(2000);
      await executeBuildCommand(textDocument.uri);
      expect(fs.existsSync(compiledFilePath)).toBe(true);
    } finally {
      fs.rmSync(compiledFilePath, { force: true });
    }
  });

  it("should not generate compiled file if the source file has errors", async () => {
    const examplePath = resolveExamplePath("files", "invalid-resources");
    const compiledFilePath = path.join(path.dirname(examplePath), "main.json");
    fs.rmSync(compiledFilePath, { force: true });
    const textDocument = await vscode.workspace.openTextDocument(examplePath);

    try {
      // Give the language server some time to finish compilation.
      await sleep(2000);
      await executeBuildCommand(textDocument.uri);
      expect(fs.existsSync(compiledFilePath)).toBe(false);
    } finally {
      fs.rmSync(compiledFilePath, { force: true });
    }
  });
});

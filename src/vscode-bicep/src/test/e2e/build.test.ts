// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import path from "path";
import fs from "fs";

import { executeBuildCommand, executeCloseAllEditors } from "./commands";
import { resolveExamplePath } from "./examples";
import { sleep } from "../utils/time";

describe("build", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should generate compiled file if the source file has no errors", async () => {
    const examplePath = resolveExamplePath("201", "sql");
    const textDocument = await vscode.workspace.openTextDocument(examplePath);

    // Give the language server sometime to finish compilation.
    await sleep(2000);

    await executeBuildCommand(textDocument.uri);

    const folderContainingSourceFile = path.dirname(examplePath);
    const compiledFilePath = path.join(folderContainingSourceFile, "main.json");

    expect(fs.existsSync(compiledFilePath)).toBe(true);

    // Delete the generated compiled file
    fs.unlinkSync(compiledFilePath);
  });

  it("should not generate compiled file if the source file has errors", async () => {
    const examplePath = resolveExamplePath("files", "invalid-resources");
    const textDocument = await vscode.workspace.openTextDocument(examplePath);

    // Give the language server sometime to finish compilation.
    await sleep(2000);

    await executeBuildCommand(textDocument.uri);

    const folderContainingSourceFile = path.dirname(examplePath);
    const compiledFilePath = path.join(folderContainingSourceFile, "main.json");

    expect(fs.existsSync(compiledFilePath)).toBe(false);
  });
});

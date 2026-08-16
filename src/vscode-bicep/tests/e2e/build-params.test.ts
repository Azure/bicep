// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import fs from "fs";
import path from "path";
import vscode from "vscode";
import { withWritableExampleDirectory } from "./examples";
import { executeBuildParamsCommand, executeCloseAllEditors } from "./utils/commands";

describe("buildParams", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should generate compiled file if the source file has no errors", async () => {
    await withWritableExampleDirectory("201", "sql", async (directory) => {
      const examplePath = path.join(directory, "main.bicepparam");
      const compiledFilePath = path.join(directory, "main.parameters.json");
      const textDocument = await vscode.workspace.openTextDocument(examplePath);

      await executeBuildParamsCommand(textDocument.uri);
      expect(fs.existsSync(compiledFilePath)).toBe(true);
    });
  });
});

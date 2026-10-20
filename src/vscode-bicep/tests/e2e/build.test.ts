// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import fs from "fs";
import path from "path";
import { workspace } from "vscode";
import { withWritableExampleDirectory } from "./examples";
import { executeBuildCommand, executeCloseAllEditors } from "./utils/commands";

describe("build", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should generate compiled file if the source file has no errors", async () => {
    await withWritableExampleDirectory("201", "sql", async (directory) => {
      const examplePath = path.join(directory, "main.bicep");
      const compiledFilePath = path.join(directory, "main.json");
      const textDocument = await workspace.openTextDocument(examplePath);

      await executeBuildCommand(textDocument.uri);
      expect(fs.existsSync(compiledFilePath)).toBe(true);
    });
  });

  it("should not generate compiled file if the source file has errors", async () => {
    await withWritableExampleDirectory("files", "invalid-resources", async (directory) => {
      const examplePath = path.join(directory, "main.bicep");
      const compiledFilePath = path.join(directory, "main.json");
      const textDocument = await workspace.openTextDocument(examplePath);

      await executeBuildCommand(textDocument.uri);
      expect(fs.existsSync(compiledFilePath)).toBe(false);
    });
  });
});

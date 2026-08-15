// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import path from "path";
import { Uri, window } from "vscode";
import { createUniqueTempFolder } from "../test-support/temp-folder";
import { normalizeMultilineString } from "../test-support/text-normalization";
import { executeCloseAllEditors, executeCreateConfigFileCommand } from "./commands";
import { expectedNewConfigFileContents } from "./expected-new-config-file-contents";

describe("bicep.createConfigFile", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should create valid config file and open it", async () => {
    const tempFolder = createUniqueTempFolder("createBicepConfigTest-");
    const fakeBicepPath = path.join(tempFolder, "main.bicep");

    try {
      const newConfigPath = await executeCreateConfigFileCommand(Uri.file(fakeBicepPath));
      if (!newConfigPath) {
        throw new Error(`Language server returned ${String(newConfigPath)} for bicep.createConfigFile`);
      }

      expect(path.basename(newConfigPath)).toBe("bicepconfig.json");
      expect(fs.existsSync(newConfigPath)).toBe(true);
      expect(fs.readFileSync(newConfigPath, "utf8")).toContain("rules");
      expect(path.dirname(newConfigPath).toLowerCase()).toBe(path.dirname(fakeBicepPath).toLowerCase());

      const editor = window.visibleTextEditors.find(
        (candidate) => candidate.document.uri.fsPath.toLowerCase() === newConfigPath.toLowerCase(),
      );
      if (!editor) {
        throw new Error("New config file should be opened in a visible editor");
      }

      expect(normalizeMultilineString(editor.document.getText())).toBe(
        normalizeMultilineString(expectedNewConfigFileContents),
      );
    } finally {
      try {
        fs.rmSync(tempFolder, {
          recursive: true,
          maxRetries: 5,
          retryDelay: 1000,
        });
      } catch {
        // post-test cleanup is strictly best-effort only
      }
    }
  });

});

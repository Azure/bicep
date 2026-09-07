// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import path from "path";
import { parse, ParseError } from "jsonc-parser";
import { Uri, window } from "vscode";
import { expectedNewConfigFileContents } from "./expected-new-config-file-contents";
import { executeCloseAllEditors, executeCreateConfigFileCommand } from "./utils/commands";
import { withTempDirectory } from "./utils/temp-directory";

describe("bicep.createConfigFile", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should create valid config file and open it", async () => {
    await withTempDirectory("createBicepConfigTest-", async (tempFolder) => {
      const fakeBicepPath = path.join(tempFolder, "main.bicep");

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

      const configText = editor.document.getText();
      expect(configText).toContain("// See https://aka.ms/bicep/config");
      expect(configText).toContain("// Press CTRL+SPACE");
      expect(parseJsonc(configText)).toEqual(parseJsonc(expectedNewConfigFileContents));
    });
  });
});

function parseJsonc(content: string): unknown {
  const errors: ParseError[] = [];
  const value: unknown = parse(content, errors);
  expect(errors).toEqual([]);

  return value;
}

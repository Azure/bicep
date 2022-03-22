// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode, { Uri, window } from "vscode";
import path from "path";
import fse from "fs-extra";

import * as os from "os";
import { executeCreateConfigFileCommand } from "./commands";

describe("bicep.createConfigFile", (): void => {
  afterEach(async () => {
    await vscode.commands.executeCommand("workbench.action.closeAllEditors");
  });

  it("should create valid config file and open it", async () => {
    const fakeBicepPath = path.join(os.tmpdir(), "main.bicep");

    let newConfigPath = await executeCreateConfigFileCommand(
      Uri.file(fakeBicepPath)
    );

    if (!newConfigPath) {
      throw new Error(`Language server returned ${String(newConfigPath)} for bicep.createConfigFile`);
    }
    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
    newConfigPath = newConfigPath!;

    try {
      expect(path.basename(newConfigPath)).toBe("bicepconfig.json");
      expect(fileExists(newConfigPath)).toBeTruthy();

      expect(fileContains(newConfigPath, "rules")).toBeTruthy();
      expect(fileIsValidJson(newConfigPath)).toBeTruthy();

      // Since the test instance of vscode does not have any workspace folders, the new file should be opened
      //   in the same folder as the bicep file
      expect(path.dirname(newConfigPath).toLowerCase()).toBe(
        path.dirname(fakeBicepPath).toLowerCase()
      );

      // Make sure the new config file has been opened in an editor
      const editor = window.visibleTextEditors.find(
        (ed) =>
          ed.document.uri.fsPath.toLowerCase() === newConfigPath?.toLowerCase()
      );
      if (!editor) {
        throw new Error("New config file should be opened in a visible editor");
      }
    } finally {
      fse.unlinkSync(newConfigPath);
    }
  });

  function fileExists(path: string): boolean {
    return fse.existsSync(path);
  }

  function fileContains(path: string, pattern: RegExp | string): boolean {
    const contents: string = fse.readFileSync(path).toString();
    return !!contents.match(pattern);
  }

  function fileIsValidJson(path: string): boolean {
    fse.readJsonSync(path, { throws: true });
    return true;
  }
});

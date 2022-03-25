// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode, { Uri, window } from "vscode";
import path from "path";
import fse from "fs-extra";
import os from "os";

import { executeCreateConfigFileCommand } from "./commands";
import {} from "fs";

describe("bicep.createConfigFile", (): void => {
  afterEach(async () => {
    await vscode.commands.executeCommand("workbench.action.closeAllEditors");
  });

  it("should create valid config file and open it", async () => {
    const tempFolder = createUniqueTempFolder("createBicepConfigTest-");
    const fakeBicepPath = path.join(tempFolder, "main.bicep");
    try {
      let newConfigPath = await executeCreateConfigFileCommand(
        Uri.file(fakeBicepPath)
      );

      if (!newConfigPath) {
        throw new Error(
          `Language server returned ${String(
            newConfigPath
          )} for bicep.createConfigFile`
        );
      }
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      newConfigPath = newConfigPath!;

      expect(path.basename(newConfigPath)).toBe("bicepconfig.json");
      if (!fileExists(newConfigPath)) {
        throw new Error(
          `Expected file ${newConfigPath} to exist but it doesn't`
        );
      }

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
      fse.rmdirSync(tempFolder, {
        recursive: true,
        maxRetries: 5,
        retryDelay: 1000,
      });
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

function createUniqueTempFolder(filenamePrefix: string): string {
  const tempFolder = os.tmpdir();
  if (!fse.existsSync(tempFolder)) {
    fse.mkdirSync(tempFolder, { recursive: true });
  }

  const tempSubfolder = fse.mkdtempSync(path.join(tempFolder, filenamePrefix));
  if (!fse.existsSync(tempSubfolder)) {
    fse.mkdirSync(tempSubfolder, { recursive: true });
  }

  return tempSubfolder;
}

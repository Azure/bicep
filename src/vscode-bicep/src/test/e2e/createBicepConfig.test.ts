// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { TextEditor, Uri, window } from "vscode";
import path from "path";
import fse from "fs-extra";
import os from "os";
import {
  executeCloseAllEditors,
  executeCreateConfigFileCommand,
} from "./commands";
import {} from "fs";
import { testScope } from "../utils/testScope";
import { expectedNewConfigFileContents } from "./expectedNewConfigFileContents";
import { normalizeMultilineString } from "../utils/normalizeMultilineString";

describe("bicep.createConfigFile", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("should create valid config file and open it", async () => {
    const tempFolder = createUniqueTempFolder("createBicepConfigTest-");
    const fakeBicepPath = path.join(tempFolder, "main.bicep");

    try {
      let newConfigPath: string;

      await testScope("Execute Create Config command", async () => {
        const newConfigPathOrUndefined = await executeCreateConfigFileCommand(
          Uri.file(fakeBicepPath)
        );

        if (!newConfigPathOrUndefined) {
          throw new Error(
            `Language server returned ${String(
              newConfigPathOrUndefined
            )} for bicep.createConfigFile`
          );
        }

        // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
        newConfigPath = newConfigPathOrUndefined!;

        expect(path.basename(newConfigPath)).toBe("bicepconfig.json");
        if (!fileExists(newConfigPath)) {
          throw new Error(
            `Expected file ${newConfigPath} to exist but it doesn't`
          );
        }

        expect(fileContains(newConfigPath, "rules")).toBeTruthy();

        // Since the test instance of vscode does not have any workspace folders, the new file should be opened
        //   in the same folder as the bicep file
        expect(path.dirname(newConfigPath).toLowerCase()).toBe(
          path.dirname(fakeBicepPath).toLowerCase()
        );
      });

      let editorOrUndefined: TextEditor | undefined;
      await testScope(
        "Make sure the new config file has been opened in an editor",
        async () => {
          editorOrUndefined = window.visibleTextEditors.find(
            (ed) =>
              ed.document.uri.fsPath.toLowerCase() ===
              newConfigPath?.toLowerCase()
          );
          if (!editorOrUndefined) {
            throw new Error(
              "New config file should be opened in a visible editor"
            );
          }
        }
      );
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      const editor = editorOrUndefined!;

      await testScope("Verify text", () => {
        const expectedText = expectedNewConfigFileContents;
        const actualText = editor.document.getText();
        const expectedTextNormalized = normalizeMultilineString(expectedText);
        const actualTextNormalized = normalizeMultilineString(actualText);
        expect(actualTextNormalized).toBe(expectedTextNormalized);
      });

      /* TODO: DISABLED (FLAKY) - see https://github.com/Azure/bicep/issues/6766
      await testScope(
        `Verify that vscode is in an "insertion" state with the dropdown for the first rule open to show the available diagnostic levels (the current one should be "warning"). Verify this by moving down to the next suggestion ("off") and selecting it`,
        async () => {
          const expectedAfterSelectingOffInsteadOfWarning =
            expectedNewConfigFileContents.replace(/warning/, "off");
          await executeSelectNextSuggestion();
          await executeAcceptSelectedSuggestion();
          const textAfterSelectingOffInsteadOfWarningtext =
            editor.document.getText();
          expect(
            normalizeMultilineString(textAfterSelectingOffInsteadOfWarningtext)
          ).toBe(
            normalizeMultilineString(expectedAfterSelectingOffInsteadOfWarning)
          );
        }
      );
      */
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
});

function createUniqueTempFolder(filenamePrefix: string): string {
  const tempFolder = os.tmpdir();
  if (!fse.existsSync(tempFolder)) {
    fse.mkdirSync(tempFolder, { recursive: true });
  }

  return fse.mkdtempSync(path.join(tempFolder, filenamePrefix));
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { TextEditor, Uri, window } from "vscode";
import path from "path";
import fse from "fs-extra";
import os from "os";
import {
  executeAcceptSelectedSuggestion,
  executeCloseAllEditors,
  executeCreateConfigFileCommand,
  executeSelectNextSuggestion,
} from "./commands";
import {} from "fs";
import { normalizeLineEndings } from "../utils/normalizeLineEndings";
import { testScope } from "../utils/testScope";
import { expectedNewConfigFileContents } from "./expectedNewConfigFileContents";
import { parseError } from "@microsoft/vscode-azext-utils";
import { removeIndentation } from "../utils/removeIndentation";

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
        verifyFileIsValidJsonWithComments(newConfigPath);

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

      await testScope("Verify inserted snippet", () => {
        const expectedText = expectedNewConfigFileContents;
        const actualText = editor.document.getText();
        // Not currently trying to get indentation consistent based on editor
        expect(normalize(actualText)).toBe(normalize(expectedText));
      });

      await testScope(
        `Verify that vscode is in an "insertion" state with the dropdown for the first rule open to show the available diagnostic levels (the current one should be "warning").
Verify this by moving down to the next suggestion ("info") and selecting it`,
        async () => {
          const expectedAfterSelectingOffInsteadOfWarning =
            expectedNewConfigFileContents.replace(/warning/, "info");
          await executeSelectNextSuggestion();
          await executeAcceptSelectedSuggestion();
          const textAfterSelectingOffInsteadOfWarningtext =
            editor.document.getText();
          expect(normalize(textAfterSelectingOffInsteadOfWarningtext)).toBe(
            normalize(expectedAfterSelectingOffInsteadOfWarning)
          );
        }
      );
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

  function verifyFileIsValidJsonWithComments(path: string): void {
    let contents = fse.readFileSync(path).toString();
    contents = stripComments(contents);
    try {
      JSON.parse(contents);
    } catch (err) {
      throw new Error(
        `Expected file '${path}' to contain valid JSON, but found error: ${
          parseError(err).message
        }`
      );
    }
  }
});

function stripComments(s: string): string {
  return s.replace(/\/\/.*/g, "");
}

function createUniqueTempFolder(filenamePrefix: string): string {
  const tempFolder = os.tmpdir();
  if (!fse.existsSync(tempFolder)) {
    fse.mkdirSync(tempFolder, { recursive: true });
  }

  return fse.mkdtempSync(path.join(tempFolder, filenamePrefix));
}

function normalize(s: string): string {
  // Not currently trying to get indentation consistent based on editor
  return removeIndentation(normalizeLineEndings(s)).trim();
}

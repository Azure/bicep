// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/* eslint-disable @typescript-eslint/no-non-null-assertion */

import vscode, {
  CompletionItem,
  SnippetString,
  window,
  workspace,
} from "vscode";
import path from "path";
import fse from "fs-extra";
import {
  executeAcceptSelectedSuggestion,
  executeCloseAllEditors,
  executeCompletionItemProvider,
  executeSelectNextSuggestion,
} from "./commands";
import {} from "fs";
import { createUniqueTempFolder } from "../utils/createUniqueTempFolder";
import { normalizeLineEndings } from "../utils/normalizeLineEndings";
import { testScope } from "../utils/testScope";
import { expectedNewConfigFileContents } from "./expectedNewConfigFileContents";

describe("empty config file snippets", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("scaffolding snippet should work as expected in an empty file", async () => {
    const expectedAfterInsertion = expectedNewConfigFileContents;

    const tempFolder = createUniqueTempFolder("emptyConfigSnippetsTest-");
    const configPath = path.join(tempFolder, "bicepconfig.json");
    fse.writeFileSync(configPath, "\n");

    try {
      const doc = await workspace.openTextDocument(configPath);
      const editor = await window.showTextDocument(doc);

      let items: CompletionItem[];
      await testScope("Find available completions", async () => {
        const completions = await executeCompletionItemProvider(
          doc.uri,
          new vscode.Position(0, 0)
        );
        expect(true).toBe(true);
        expect(completions).toBeDefined();
        items = completions!.items;
        expect(items.length).toBeGreaterThan(0);
      });

      let scaffoldSnippet: CompletionItem | undefined;
      await testScope("Find the snippet of interest", async () => {
        scaffoldSnippet = items.find(
          (i) => getCompletionLabelText(i) === "Default Bicep Configuration"
        );
        expect(scaffoldSnippet).toBeDefined();
      });
      await testScope(" ... and insert it", async () => {
        await editor.insertSnippet(<SnippetString>scaffoldSnippet!.insertText);
      });

      await testScope("Verify inserted snippet", () => {
        const textAfterInsertion = editor.document.getText();
        expect(normalizeLineEndings(textAfterInsertion)).toBe(
          normalizeLineEndings(expectedAfterInsertion)
        );
      });

      await testScope(
        `Verify that the snippet placed VS Code into an "insertion" state with the dropdown for the first rule open to show the available diagnostic levels (the current one should be "warning").
Verify this by moving down to the next suggestion ("off") and selecting it`,
        async () => {
          const expectedAfterSelectingOffInsteadOfWarning =
            expectedAfterInsertion.replace(/warning/, "off");
          await executeSelectNextSuggestion();
          await executeAcceptSelectedSuggestion();
          const textAfterSelectingOffInsteadOfWarningtext =
            editor.document.getText();
          expect(
            normalizeLineEndings(textAfterSelectingOffInsteadOfWarningtext)
          ).toBe(
            normalizeLineEndings(expectedAfterSelectingOffInsteadOfWarning)
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
});

function getCompletionLabelText(snippet: CompletionItem): string {
  return typeof snippet.label === "string"
    ? snippet.label
    : snippet.label.label;
}

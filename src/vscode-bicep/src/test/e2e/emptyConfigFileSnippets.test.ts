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
  executeCloseAllEditors,
  executeCompletionItemProvider,
} from "./commands";
import {} from "fs";
import { createUniqueTempFolder } from "../utils/createUniqueTempFolder";
import { normalizeLineEndings } from "../utils/normalizeLineEndings";

describe("empty config file snippets", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("scaffolding snippet should work as expected in an empty file", async () => {
    const expectedAfterInsertion = `{
    // See https://aka.ms/bicep/config for more information on Bicep configuration options
    // Press CTRL+SPACE/CMD+SPACE at any location to see IntelliSense suggestions
    "analyzers": {
        "core": {
            "rules": {
                "no-unused-params": {
                    "level": "warning"
                }
            }
        }
    }
}
`;

    const tempFolder = createUniqueTempFolder("emptyConfigSnippetsTest-");
    const configPath = path.join(tempFolder, "bicepconfig.json");
    fse.writeFileSync(configPath, "\n");

    try {
      const doc = await workspace.openTextDocument(configPath);
      const editor = await window.showTextDocument(doc);

      // Find available completions
      const completions = await executeCompletionItemProvider(
        doc.uri,
        new vscode.Position(0, 0)
      );
      expect(true).toBe(true);
      expect(completions).toBeDefined();
      const items = completions!.items;
      expect(items.length).toBeGreaterThan(0);

      // Find the snippet of interest
      const scaffoldSnippet = items.find(
        (i) => getCompletionLabelText(i) === "Default Bicep Configuration"
      );
      expect(scaffoldSnippet).toBeDefined();
      // ... and insert it
      await editor.insertSnippet(<SnippetString>scaffoldSnippet!.insertText);

      // Verify
      const textAfterInsertion = editor.document.getText();
      expect(normalizeLineEndings(textAfterInsertion)).toBe(
        normalizeLineEndings(expectedAfterInsertion)
      );

      // TODO: DISABLED (FLAKY) - see https://github.com/Azure/bicep/issues/6766
      //
      // // Verify that the snippet placed VS Code into an "insertion" state with the dropdown for the first rule open to show
      // //   the available diagnostic levels (the current one should be "warning").
      // // Verify this by moving down to the next suggestion ("off") and selecting it
      // const expectedAfterSelectingOffInsteadOfWarning =
      //   expectedAfterInsertion.replace(/warning/, "off");
      // await executeSelectNextSuggestion();
      // await executeAcceptSelectedSuggestion();
      // const textAfterSelectingOffInsteadOfWarningtext =
      //   editor.document.getText();
      // expect(
      //   normalizeLineEndings(textAfterSelectingOffInsteadOfWarningtext)
      // ).toBe(normalizeLineEndings(expectedAfterSelectingOffInsteadOfWarning));
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

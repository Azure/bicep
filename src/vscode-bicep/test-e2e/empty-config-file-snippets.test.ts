// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import path from "path";
import vscode, { CompletionItem, SnippetString, window, workspace } from "vscode";
import { createUniqueTempFolder } from "../test-support/temp-folder";
import { normalizeLineEndings } from "../test-support/text-normalization";
import { executeCloseAllEditors, executeCompletionItemProvider } from "./commands";
import { expectedNewConfigFileContents } from "./expected-new-config-file-contents";

describe("empty config file snippets", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("scaffolding snippet should work as expected in an empty file", async () => {
    const expectedAfterInsertion = expectedNewConfigFileContents;

    const tempFolder = createUniqueTempFolder("emptyConfigSnippetsTest-");
    const configPath = path.join(tempFolder, "bicepconfig.json");
    fs.writeFileSync(configPath, "\n");

    try {
      const doc = await workspace.openTextDocument(configPath);
      const editor = await window.showTextDocument(doc);

      const completions = await executeCompletionItemProvider(doc.uri, new vscode.Position(0, 0));
      if (!completions) {
        throw new Error("Expected completion provider to return a completion list");
      }

      const scaffoldSnippet = completions.items.find(
        (item) => getCompletionLabelText(item) === "Default Bicep Configuration",
      );
      if (!scaffoldSnippet) {
        throw new Error("Expected the default Bicep configuration completion");
      }
      if (!(scaffoldSnippet.insertText instanceof SnippetString)) {
        throw new Error("Expected the default Bicep configuration completion to contain a snippet");
      }

      await editor.insertSnippet(scaffoldSnippet.insertText);

      expect(normalizeLineEndings(editor.document.getText()).trimEnd()).toBe(
        normalizeLineEndings(expectedAfterInsertion).trimEnd(),
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

function getCompletionLabelText(snippet: CompletionItem): string {
  return typeof snippet.label === "string" ? snippet.label : snippet.label.label;
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import path from "path";
import vscode, { CompletionItem, SnippetString, window, workspace } from "vscode";
import { expectedNewConfigFileContents } from "./expected-new-config-file-contents";
import { executeCloseAllEditors, executeCompletionItemProvider } from "./utils/commands";
import { withTempDirectory } from "./utils/temp-directory";
import { normalizeLineEndings } from "./utils/text-normalization";

describe("empty config file snippets", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  it("scaffolding snippet should work as expected in an empty file", async () => {
    const expectedAfterInsertion = expectedNewConfigFileContents;

    await withTempDirectory("emptyConfigSnippetsTest-", async (tempFolder) => {
      const configPath = path.join(tempFolder, "bicepconfig.json");
      fs.writeFileSync(configPath, "\n");

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
    });
  });
});

function getCompletionLabelText(snippet: CompletionItem): string {
  return typeof snippet.label === "string" ? snippet.label : snippet.label.label;
}

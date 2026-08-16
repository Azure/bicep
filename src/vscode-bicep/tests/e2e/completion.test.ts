// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import assert from "assert";
import * as vscode from "vscode";
import { Range } from "vscode";
import { Position } from "vscode";
import { readExampleFile } from "./examples";
import { executeCloseAllEditors, executeCompletionItemProvider } from "./utils/commands";
import { waitFor } from "./utils/polling";

describe("completion", (): void => {
  let document: vscode.TextDocument;
  let editor: vscode.TextEditor;

  beforeAll(async () => {
    const content = readExampleFile("201", "sql");
    document = await vscode.workspace.openTextDocument({
      language: "bicep",
      content,
    });

    editor = await vscode.window.showTextDocument(document);
  });

  afterAll(async () => {
    await executeCloseAllEditors();
  });

  it("should provide completion while typing an identifier", async () => {
    await editor.edit((editBuilder) => editBuilder.insert(new Position(17, 0), "var foo = data"));

    const completionList = await waitFor(
      async () => await executeCompletionItemProvider(document.uri, new vscode.Position(17, 14)),
      (completionList) =>
        completionList !== undefined && completionList.items.map((item) => item.label).includes("dataUri"),
      { description: "the dataUri completion item" },
    );

    assert(completionList);
    expect(completionList.items.map((item) => item.label)).toContain("dataUri");

    await editor.edit((editBuilder) => editBuilder.delete(new Range(new Position(17, 0), new Position(17, 14))));
  });
});

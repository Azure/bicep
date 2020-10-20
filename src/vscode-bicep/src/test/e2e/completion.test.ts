// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import { Range } from "vscode";
import { Position } from "vscode";

import { readExampleFile } from "./examples";
import { executeCompletionItemProviderCommand } from "./commands";
import { expectDefined } from "../utils/assert";

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
    await vscode.commands.executeCommand("workbench.action.closeActiveEditor");
  });

  it("should provide completion while typing an indentifier", async () => {
    await editor.edit((editBuilder) =>
      editBuilder.insert(new Position(19, 0), "var foo = data")
    );

    const completionList = await executeCompletionItemProviderCommand(
      document.uri,
      new vscode.Position(19, 14)
    );

    expectDefined(completionList);
    expect(completionList.items.length).toBeGreaterThan(0);

    await editor.edit((editBuilder) =>
      editBuilder.delete(new Range(new Position(19, 0), new Position(19, 14)))
    );
  });
});

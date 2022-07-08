// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import { Range } from "vscode";
import { Position } from "vscode";

import { readExampleFile } from "./examples";
import {
  executeCloseAllEditors,
  executeCompletionItemProvider,
} from "./commands";
import { expectDefined } from "../utils/assert";
import { retryWhile, sleep } from "../utils/time";

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

    // Give the language server some time to finish compilation. If this is the first test
    // to run it may take long for the compilation to complete because JIT is not "warmed up".
    await sleep(2000);
  });

  afterAll(async () => {
    await executeCloseAllEditors();
  });

  it("should provide completion while typing an identifier", async () => {
    await editor.edit((editBuilder) =>
      editBuilder.insert(new Position(17, 0), "var foo = data")
    );

    const completionList = await retryWhile(
      async () =>
        await executeCompletionItemProvider(
          document.uri,
          new vscode.Position(17, 14)
        ),
      (completionList) =>
        completionList === undefined ||
        !completionList.items.map((item) => item.label).includes("dataUri")
    );

    expectDefined(completionList);
    expect(completionList.items.map((item) => item.label)).toContain("dataUri");

    await editor.edit((editBuilder) =>
      editBuilder.delete(new Range(new Position(17, 0), new Position(17, 14)))
    );
  });
});

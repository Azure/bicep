// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import { CompletionItem } from "vscode";
import { Position } from "vscode";
import { Range } from "vscode";
import { SnippetString } from "vscode";

import { readExampleFile } from "./examples";
import { executeCompletionItemProviderCommand } from "./commands";
import { expectDefined } from "../utils/assert";
import { retryWhile, sleep } from "../utils/time";

describe("completion", (): void => {
  let document: vscode.TextDocument;
  let editor: vscode.TextEditor;

  beforeAll(async () => {
    const content = readExampleFile("201", "armSnippets");
    document = await vscode.workspace.openTextDocument({
      language: "bicep",
      content,
    });

    editor = await vscode.window.showTextDocument(document);

    // Give the language server sometime to finish compilation. If this is the first test
    // to run it may take long for the compilation to complete because JIT is not "warmed up".
    await sleep(2000);
  });

  afterAll(async () => {
    await vscode.commands.executeCommand("workbench.action.closeActiveEditor");
  });

  it("should provide completion while typing an indentifier", async () => {
    await editor.edit((editBuilder) =>
      editBuilder.insert(new Position(0, 0), "arm")
    );

    const completionList = await retryWhile(
      async () =>
        await executeCompletionItemProviderCommand(
          document.uri,
          new vscode.Position(0, 3)
        ),
      (completionList) =>
        completionList === undefined ||
        !completionList.items.map((item) => item.label).includes("arm-plan")
    );

  let snippets = completionList?.items.filter(item => item.label.startsWith("arm")) as Array<CompletionItem>;

    await editor.edit((editBuilder) =>
      editBuilder.delete(new Range(new Position(0, 0), new Position(0, 3)))
    );

   for (var snippet in snippets)
   {
       var position = new Position(editor.document.lineCount + 1, 0);
       await editor.edit((editBuilder) =>
         editBuilder.insert(position, "// " + snippets[snippet]?.label)
       );
       position = new Position(editor.document.lineCount + 1, 0);
       await editor.edit((editBuilder) =>
         editBuilder.insert(position, "\r\n")
       );

       await editor.insertSnippet(snippets[snippet]?.insertText as SnippetString);
       position = new Position(editor.document.lineCount + 1, 0);
       editor.selection = new vscode.Selection(position, position);

       await editor.edit((editBuilder) =>
        editBuilder.insert(position, "\r\n")
     );
   }

    expectDefined(completionList);
    expect(completionList.items.map((item) => item.label)).toContain("arm-plan");

    await editor.edit((editBuilder) =>
      editBuilder.delete(new Range(new Position(0, 0), new Position(editor.document.lineCount, 0)))
    );
  });
});

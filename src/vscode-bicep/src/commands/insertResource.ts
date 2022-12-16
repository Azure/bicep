// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode, { Uri, window, workspace } from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { insertResourceRequestType } from "../language";
import { IActionContext } from "@microsoft/vscode-azext-utils";

export class InsertResourceCommand implements Command {
  public readonly id = "bicep.insertResource";

  public constructor(private readonly client: LanguageClient) {}

  public async execute(
    context: IActionContext,
    documentUri?: Uri
  ): Promise<void> {
    const editor = await findOrShowEditor(documentUri);
    if (!editor) {
      throw new Error(`Unable to find active editor for insert resource command`);
    }

    const resourceId = await vscode.window.showInputBox({
      prompt: "Enter a resourceId",
    });

    if (!resourceId) {
      return;
    }

    await this.client.sendNotification(insertResourceRequestType, {
      textDocument:
        this.client.code2ProtocolConverter.asTextDocumentIdentifier(editor.document),
      position: this.client.code2ProtocolConverter.asPosition(
        editor.selection.start
      ),
      resourceId: resourceId,
    });
  }
}

async function findOrShowEditor(documentUri?: Uri) {
  if (!documentUri) {
    const editor = vscode.window.activeTextEditor;
    if (editor?.document.languageId !== 'bicep') {
      return null;
    }

    return editor;
  } else {
    const document = await workspace.openTextDocument(documentUri);
    return await window.showTextDocument(document);
  }
}
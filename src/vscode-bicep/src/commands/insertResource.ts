// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode, { Uri, window, workspace } from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { insertResourceRequestType } from "../language";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { IActionContext } from "@microsoft/vscode-azext-utils";

export class InsertResourceCommand implements Command {
  public readonly id = "bicep.insertResource";

  public constructor(private readonly client: LanguageClient) {}

  public async execute(
    context: IActionContext,
    documentUri?: Uri
  ): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to insert a resource into",
      {
        // Since "Insert Resource" is acting on text in an editor, unlike most commands
        //   we will choose the active editor if it's a bicep file
        considerActiveEditor: true,
      }
    );

    const document = await workspace.openTextDocument(documentUri);
    const editor = await window.showTextDocument(document);

    const resourceId = await vscode.window.showInputBox({
      prompt: "Enter a resourceId",
    });

    if (!resourceId) {
      return;
    }

    await this.client.sendNotification(insertResourceRequestType, {
      textDocument:
        this.client.code2ProtocolConverter.asTextDocumentIdentifier(document),
      position: this.client.code2ProtocolConverter.asPosition(
        editor.selection.start
      ),
      resourceId: resourceId,
    });
  }
}

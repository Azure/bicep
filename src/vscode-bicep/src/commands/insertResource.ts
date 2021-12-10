// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { insertResourceRequestType } from "../language";

export class InsertResourceCommand implements Command {
  public readonly id = "bicep.insertResource";

  public constructor(private readonly client: LanguageClient) {}

  public async execute(): Promise<void> {
    const editor = vscode.window.activeTextEditor;
    if (!editor) {
      return;
    }

    const document = editor.document;
    if (document.uri.scheme === "output") {
      // The output panel in VS Code was implemented as a text editor by accident. Due to breaking change concerns,
      // it won't be fixed in VS Code, so we need to handle it on our side.
      // See https://github.com/microsoft/vscode/issues/58869#issuecomment-422322972 for details.
      vscode.window.showInformationMessage(
        "Unable to locate an active Bicep file, as the output panel is focused. Please focus a text editor first before running the command."
      );

      return;
    }

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

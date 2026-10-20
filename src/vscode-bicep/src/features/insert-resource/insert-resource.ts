// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Uri, window, workspace } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Command, CommandManager } from "../../infrastructure/commands";
import { findOrCreateActiveBicepFile } from "../../infrastructure/editor";
import { Prompts } from "../../infrastructure/prompts";
import { insertResourceRequestType } from "./protocol";

export class InsertResourceCommand implements Command {
  public readonly id = "bicep.insertResource";

  public constructor(
    private readonly prompts: Prompts,
    private readonly client: LanguageClient,
  ) {}

  public async execute(documentUri?: Uri): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      this.prompts,
      documentUri,
      "Choose which Bicep file to insert a resource into",
    );

    const document = await workspace.openTextDocument(documentUri);
    const editor = await window.showTextDocument(document);

    const resourceId = await window.showInputBox({
      prompt: "Enter a resourceId",
    });

    if (!resourceId) {
      return;
    }

    await this.client.sendNotification(insertResourceRequestType, {
      textDocument: this.client.code2ProtocolConverter.asTextDocumentIdentifier(document),
      position: this.client.code2ProtocolConverter.asPosition(editor.selection.start),
      resourceId: resourceId,
    });
  }
}

export async function activateInsertResourceFeature(
  prompts: Prompts,
  commandManager: CommandManager,
  client: LanguageClient,
): Promise<void> {
  await commandManager.registerCommands(new InsertResourceCommand(prompts, client));
}

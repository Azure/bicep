// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { commands, Position, Uri } from "vscode";
import { integer } from "vscode-languageclient";
import { Command, CommandManager } from "../../infrastructure/commands";

export class PostExtractionCommand implements Command {
  public readonly id = "bicep.internal.postExtraction";

  public async execute(
    _: Uri | undefined,
    targetUri: string,
    position: { line: integer; character: integer },
  ): Promise<void> {
    const uri = Uri.parse(targetUri, true);
    await commands.executeCommand("editor.action.rename", [uri, new Position(position.line, position.character)]);
  }
}

export async function activateRefactoringFeature(commandManager: CommandManager): Promise<void> {
  await commandManager.registerCommands(new PostExtractionCommand());
}

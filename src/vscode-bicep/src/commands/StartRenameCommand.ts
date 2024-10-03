// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext } from "@microsoft/vscode-azext-utils";
import { Uri, Position, commands } from "vscode";
import { Command } from "./types";
import { integer } from "vscode-languageclient";

export class StartRenameCommand implements Command {
  public readonly id = "bicep.internal.startRename";

  public async execute(_context: IActionContext, _: Uri, targetUri: string, position: {line:integer,character: integer}): Promise<void> {
    const uri = Uri.parse(targetUri, true);
    await commands.executeCommand("editor.action.rename", [uri, new Position(position.line, position.character)]);
  }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";
import { callWithTelemetryAndErrorHandlingSync, IActionContext } from "@microsoft/vscode-azext-utils";
import { commands, Position, Uri } from "vscode";
import { integer } from "vscode-languageclient";
import { Command, CommandManager } from "../../infrastructure/commands";

export class PostExtractionCommand implements Command {
  public readonly id = "bicep.internal.postExtraction";

  public async execute(
    _context: IActionContext,
    _: Uri,
    targetUri: string,
    position: { line: integer; character: integer },
    telemetryEvent: { EventName: string; Properties: { [key: string]: string } },
  ): Promise<void> {
    assert(!!telemetryEvent.EventName, "EventName is required");
    callWithTelemetryAndErrorHandlingSync(telemetryEvent.EventName, (telemetryActionContext) => {
      for (const key in telemetryEvent.Properties) {
        telemetryActionContext.telemetry.properties[key] = telemetryEvent.Properties[key];
      }
    });

    const uri = Uri.parse(targetUri, true);
    await commands.executeCommand("editor.action.rename", [uri, new Position(position.line, position.character)]);
  }
}

export async function activateRefactoringFeature(commandManager: CommandManager): Promise<void> {
  await commandManager.registerCommands(new PostExtractionCommand());
}

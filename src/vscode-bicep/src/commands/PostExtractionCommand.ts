// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { callWithTelemetryAndErrorHandlingSync, IActionContext } from "@microsoft/vscode-azext-utils";
import { Uri, Position, commands } from "vscode";
import { Command } from "./types";
import { integer } from "vscode-languageclient";
import assert from "assert";

// Called after an extract command is completed to start a rename operation and post telemetry
export class PostExtractionCommand implements Command {
  public readonly id = "bicep.internal.postExtraction";

  public async execute(
    _context: IActionContext, 
    _: Uri, 
    targetUri: string, 
    position: { line: integer, character: integer }, 
    telemetryEvent: { EventName: string, Properties: { [key: string]: string } }
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

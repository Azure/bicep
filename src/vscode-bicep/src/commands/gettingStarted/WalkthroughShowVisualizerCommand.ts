// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//asdfg telemetry?

import * as os from "os";
import * as fse from "fs-extra";
import vscode, { commands, TextDocument, TextEdit, TextEditor, Uri, window, workspace } from "vscode";
import { IActionContext, UserCancelledError } from "vscode-azureextensionui";
import { Command } from "../types";

function findCurrentBicepFile(
  context: IActionContext,
  documentUri?: vscode.Uri
): TextEditor {
  const editors = window.visibleTextEditors.filter(
    (ed) => ed.document.languageId === "bicep"
  );

  return editors[0];
}

export class WalkthroughShowVisualizerCommand implements Command {
  public readonly id = "bicep.gettingStarted.openVisualizer";

  public async execute(
    context: IActionContext,
    documentUri?: Uri
  ): Promise<void> {
    // asdfg what if not visible?
    const currentEditor = findCurrentBicepFile(context, documentUri);
    if (currentEditor) {
      window.activeTextEditor = currentEditor;
      await commands.executeCommand("bicep.showVisualizerToSide");
    }
  }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//asdfg telemetry?

import { commands, MessageItem, TextEditor, ViewColumn, window } from "vscode";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import { Command } from "../types";
import {
  WalkthroughCreateBicepFileCommand,
  WalkthroughOpenBicepFileCommand,
} from "..";

// Tries to find the visible bicep file that the user is editing alongside the walkthrough.
async function findAndActivateVisibleBicepFileEditor(
  context: IActionContext
): Promise<TextEditor | undefined> {
  const visibleBicepEditors = window.visibleTextEditors.filter(
    (ed) => ed.document.languageId === "bicep"
  );

  const editor: TextEditor | undefined = visibleBicepEditors[0];
  if (editor) {
    return window.showTextDocument(editor.document, ViewColumn.Beside);
  }

  const open: MessageItem = {
    title: "Open",
  };
  const create: MessageItem = {
    title: "Create",
  };
  const response = await context.ui.showWarningMessage(
    "Please open or create a Bicep file in a visible editor tab to run this command. If necessary, use Ctrl+\\ (Windows) or CMD+\\ (MacOs) to display it to the side.",
    open,
    create
  );
  if (response === open) {
    return await commands.executeCommand(WalkthroughOpenBicepFileCommand.id);
  } else if (response === create) {
    return await commands.executeCommand(WalkthroughCreateBicepFileCommand.id);
  } else {
    throw new Error(`Unexpected response ${response.title}`);
  }
}

export class WalkthroughShowVisualizerCommand implements Command {
  public readonly id = "bicep.gettingStarted.openVisualizer";

  public async execute(context: IActionContext): Promise<void> {
    // asdfg what if not visible?
    const currentEditor = await findAndActivateVisibleBicepFileEditor(context);
    if (currentEditor) {
      await commands.executeCommand("bicep.showVisualizerToSide");
    }
  }
}

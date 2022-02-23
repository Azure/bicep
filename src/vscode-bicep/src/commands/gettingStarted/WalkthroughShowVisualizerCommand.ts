// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//asdfg telemetry?

import { commands, MessageItem, TextEditor, window } from "vscode";
import { IActionContext } from "vscode-azureextensionui";
import { Command } from "../types";
import {
  WalkthroughCreateBicepFileCommand,
  WalkthroughOpenBicepFileCommand,
} from "..";

// Tries to find the visible bicep file that the user is editing alongside the walkthrough.
async function findVisibleBicepFileEditor(
  context: IActionContext
): Promise<TextEditor | undefined> {
  const editors = window.visibleTextEditors.filter(
    (ed) => ed.document.languageId === "bicep"
  );

  const editor: TextEditor | undefined = editors[0];
  if (editor) {
    return editor;
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
    const currentEditor = await findVisibleBicepFileEditor(context);
    if (currentEditor) {
      await commands.executeCommand("bicep.showVisualizerToSide");
    }
  }
}

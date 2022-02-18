// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//asdfg telemetry?

import * as os from "os";
import * as fse from "fs-extra";
import vscode, {
  commands,
  MessageItem,
  QuickPickItem,
  TextDocument,
  TextEdit,
  TextEditor,
  TextEditorOptions,
  Uri,
  window,
  workspace,
} from "vscode";
import {
  AzExtTreeItem,
  IActionContext,
  IAzureQuickPickItem,
  UserCancelledError,
} from "vscode-azureextensionui";
import { Command } from "../types";
import { WalkthroughCreateBicepFileCommand, WalkthroughOpenBicepFileCommand } from "..";

async function findCurrentBicepFile(
  context: IActionContext,
  documentUri?: vscode.Uri
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

  public async execute(
    context: IActionContext,
    documentUri?: Uri
  ): Promise<void> {
    // asdfg what if not visible?
    const currentEditor = await findCurrentBicepFile(context, documentUri);
    if (currentEditor) {
      await commands.executeCommand("bicep.showVisualizerToSide");
    }
  }
}

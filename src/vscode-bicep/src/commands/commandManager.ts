// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode from "vscode";

import { Disposable } from "../utils/disposable";
import { Command } from "./types";
import * as azureextensionui from "vscode-azureextensionui";

export class CommandManager extends Disposable {
  private static commandsRegistredContextKey = "commandsRegistered";

  public async registerCommands<T extends [Command, ...Command[]]>(
    ...commands: T
  ): Promise<void> {
    commands.map((command) => this.registerCommand(command));

    await vscode.commands.executeCommand(
      "setContext",
      CommandManager.commandsRegistredContextKey,
      true
    );
  }

  private registerCommand<T extends Command>(command: T): void {
    // The command will be added to the extension's subscriptions and therefore disposed automatically
    // when the extension is disposed.
    azureextensionui.registerCommand(
      command.id,
      async (context: azureextensionui.IActionContext, ...args: unknown[]) => {
        await command.execute(context, ...args);
      }
    );
  }
}

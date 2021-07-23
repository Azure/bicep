// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";

import { Disposable } from "../utils/disposable";
import { Command } from "./types";

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
    this.register(
      vscode.commands.registerCommand(command.id, command.execute, command)
    );
  }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";

import { Disposable } from "../utils/disposable";
import { Command } from "./types";

export class CommandManager extends Disposable {
  public registerCommand<T extends Command>(command: T): void {
    this.register(
      vscode.commands.registerCommand(command.id, command.execute, command)
    );
  }

  public registerCommands<T extends [Command, ...Command[]]>(
    ...commands: T
  ): void {
    commands.map((command) => this.registerCommand(command));
  }
}

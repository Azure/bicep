// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import assert from "assert";
import * as fse from "fs-extra";
import { commands, ExtensionContext, Uri } from "vscode";
import { IActionContext, runWithErrorHandling } from "../action-context";
import { Disposable } from "../lifecycle";
import { Telemetry } from "../telemetry";

export interface Command {
  readonly id: string;

  /**
   * Executes the command
   * @param context Provides user input and error-handling behavior
   * @param args Optional arguments that are being passed to the command
   */
  execute(context: IActionContext, documentUri: Uri | undefined, ...args: unknown[]): unknown | Promise<unknown>;
}

export class CommandManager extends Disposable {
  private _packageJson: IPackageJson | undefined;

  public constructor(
    private readonly extensionContext: ExtensionContext,
    private readonly telemetry: Telemetry,
  ) {
    super();
  }

  public async registerCommands<T extends [Command, ...Command[]]>(...commands: T): Promise<void> {
    commands.map((command) => this.registerCommand(command));
  }

  private registerCommand<T extends Command>(command: T): void {
    this.validateCommand(command);

    this.register(
      commands.registerCommand(command.id, async (...args: unknown[]) => {
        let documentUri: Uri | undefined = undefined;

        if (args[0] instanceof Uri) {
          // First argument is a Uri (this is how VsCode communicates the target URI for a comment invoked through a menu, context menu, etc.)
          documentUri = args[0];
          args = args.slice(1);
        }

        return await runWithErrorHandling(
          async (context) => await command.execute(context, documentUri, ...args),
          (error) => this.telemetry.sendError("command/error", error, { commandId: command.id }),
          this.extensionContext.globalState,
        );
      }),
    );
  }

  private validateCommand<T extends Command>(command: T): void {
    if (!this._packageJson) {
      this._packageJson = <IPackageJson>fse.readJsonSync(this.extensionContext.asAbsolutePath("package.json"));
    }

    assert(command.id.startsWith("bicep."), `Command ID doesn't start with 'bicep.': ${command.id}`);

    // Walkthrough commands shouldn't be shown in the command palette
    if (command.id.match(/gettingStarted/i)) {
      const commandPaletteWhen: string | undefined = this._packageJson.contributes?.menus?.commandPalette?.find(
        (m) => m.command === command.id,
      )?.when;
      assert(
        commandPaletteWhen === "never",
        `Internal error: Add an entry for '${command.id}' to package.json's contributes/menus/commandPalette array with a 'when' value of 'never'.`,
      );
    }
  }
}

interface IPackageJson {
  contributes: {
    commands?: {
      command: string;
    };
    menus?: {
      commandPalette?: {
        command: string;
        when?: string;
        group?: string;
      }[];
    };
  };
  activationEvents?: string[];
}

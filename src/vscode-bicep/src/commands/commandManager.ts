// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ExtensionContext } from "vscode";
import * as fse from "fs-extra";
import { Disposable } from "../utils/disposable";
import { Command } from "./types";
import * as azureextensionui from "@microsoft/vscode-azext-utils";
import assert from "assert";

export class CommandManager extends Disposable {
  private _packageJson: IPackageJson | undefined;

  public constructor(private readonly _ctx: ExtensionContext) {
    super();
  }

  public async registerCommands<T extends [Command, ...Command[]]>(
    ...commands: T
  ): Promise<void> {
    commands.map((command) => this.registerCommand(command));
  }

  private registerCommand<T extends Command>(command: T): void {
    this.validateCommand(command);

    // Prefix all command telemetry IDs with "command/" so we end up with telemetry IDs like "vscode-bicep/command/bicep.build"
    const telemetryId = `command/${command.id}`;

    // The command will be added to the extension's subscriptions and therefore disposed automatically
    // when the extension is disposed.
    azureextensionui.registerCommand(
      command.id,
      async (context: azureextensionui.IActionContext, ...args: unknown[]) => {
        return await command.execute(context, ...args);
      },
      undefined,
      telemetryId
    );
  }

  private validateCommand<T extends Command>(command: T): void {
    if (!this._packageJson) {
      this._packageJson = <IPackageJson>(
        fse.readJsonSync(this._ctx.asAbsolutePath("package.json"))
      );
    }

    // activationEvents
    const activationEvents = this._packageJson.activationEvents;
    assert(!!activationEvents, "Missing activationEvents in package.json");
    const activationKey = `onCommand:${command.id}`;
    const activation: string | undefined = activationEvents.find(
      (a) => a == activationKey
    );
    assert(
      !!activation,
      `Code error: Add an entry for '${activationKey}' to package.json's activationEvents array. This ensures that the command will be functional even if the extension is not yet activated.`
    );

    assert(
      command.id.startsWith("bicep."),
      `Command ID doesn't start with 'bicep.': ${command.id}`
    );
  }
}

interface IPackageJson {
  commands?: {
    command: string;
  };
  activationEvents?: string[];
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as azureextensionui from "@microsoft/vscode-azext-utils";
import assert from "assert";
import * as fse from "fs-extra";
import { ExtensionContext, Uri } from "vscode";
import { Disposable } from "../utils/disposable";
import { Command } from "./types";

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
        let documentUri: Uri | undefined = undefined;
        let isFromWalkthrough = false;

        if (args[0] instanceof Uri) {
          // First argument is a Uri (this is how VsCode communicates the target URI for a comment invoked through a menu, context menu, etc.)
          documentUri = args[0];
          args = args.slice(1);
        }

        // If the command is coming from a [command:xxx] inside a markdown file (e.g. from
        //   a walkthrough), and the command contains a query string, the query string values
        //   will be in an object in the next argument.
        if (
          args[0] instanceof Object &&
          (<{ walkthrough?: string }>args[0])["walkthrough"] === "true"
        ) {
          // Marked as a walkthrough via query string in markdow
          isFromWalkthrough = true;
        }

        // Commands starting with bicep.gettingStarted are obviously from the walkthrough
        if (command.id.startsWith("bicep.gettingStarted")) {
          isFromWalkthrough = true;
        }

        if (isFromWalkthrough) {
          context.telemetry.properties.contextValue = "walkthrough";
        }

        return await command.execute(context, documentUri, ...args);
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
      (a) => a === activationKey
    );
    assert(
      !!activation,
      `Internal error: Add an entry for '${activationKey}' to package.json's activationEvents array. This ensures that the command will be functional even if the extension is not yet activated.`
    );

    assert(
      command.id.startsWith("bicep."),
      `Command ID doesn't start with 'bicep.': ${command.id}`
    );

    // Walkthrough commands shouldn't be shown in the command palette
    if (command.id.match(/gettingStarted/i)) {
      const commandPaletteWhen: string | undefined =
        this._packageJson.contributes?.menus?.commandPalette?.find(
          (m) => m.command === command.id
        )?.when;
      assert(
        commandPaletteWhen === "never",
        `Internal error: Add an entry for '${command.id}' to package.json's contributes/menus/commandPalette array with a 'when' value of 'never'.`
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

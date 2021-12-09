// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext } from "vscode-azureextensionui";

export interface Command {
  readonly id: string;

  /**
   * Executes the command
   * @param context Optionally used to control telemetry and error-handling behavior
   * @param args Optional arguments that are being passed to the command
   */
  execute(context: IActionContext, ...args: unknown[]): unknown;
}

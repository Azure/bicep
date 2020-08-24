/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
import * as vscode from "vscode";
import { isError } from "util";

import { createLogger, getLogger } from "./uitls/logger";
import { launchLanugageServiceWithProgressReport } from "./language/client";

export async function activate(
  context: vscode.ExtensionContext
): Promise<void> {
  createLogger(context);

  try {
    await launchLanugageServiceWithProgressReport(context);
  } catch (err) {
    getLogger().error(err);
    vscode.window.showErrorMessage(isError(err) ? err.message : err);
  }
}

// eslint-disable-next-line @typescript-eslint/no-empty-function
export function deactivate(): void {}

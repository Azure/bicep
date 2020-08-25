// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
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

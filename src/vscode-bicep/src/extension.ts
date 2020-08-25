// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import { createLogger } from "./utils/logger";
import { launchLanugageServiceWithProgressReport } from "./language/client";
import { activateWithTelemetryAndErrorHandling } from "./utils/telemetry";

export async function activate(
  context: vscode.ExtensionContext
): Promise<void> {
  const outputChannel = vscode.window.createOutputChannel("Bicep");

  await activateWithTelemetryAndErrorHandling(
    context,
    outputChannel,
    async () => {
      createLogger(context, outputChannel);

      await launchLanugageServiceWithProgressReport(context);
    }
  );
}

// eslint-disable-next-line @typescript-eslint/no-empty-function
export function deactivate(): void {}

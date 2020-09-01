// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

import { createLogger } from "./utils/logger";
import { launchLanugageServiceWithProgressReport } from "./language/client";
import { activateWithTelemetryAndErrorHandling } from "./utils/telemetry";
import { createAzExtOutputChannel } from "vscode-azureextensionui";

export async function activate(
  context: vscode.ExtensionContext
): Promise<void> {
  const outputChannel = createAzExtOutputChannel("Bicep", "bicep");

  await activateWithTelemetryAndErrorHandling(
    context,
    outputChannel,
    async () => {
      createLogger(context, outputChannel);

      await launchLanugageServiceWithProgressReport(context, outputChannel);
    }
  );
}

// eslint-disable-next-line @typescript-eslint/no-empty-function
export function deactivate(): void {}

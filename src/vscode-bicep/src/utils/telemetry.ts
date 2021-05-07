// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  callWithTelemetryAndErrorHandling,
  IActionContext,
} from "vscode-azureextensionui";

import { getLogger } from "./logger";

export async function activateWithTelemetryAndErrorHandling(
  activateCallback: () => Promise<void>
): Promise<void> {
  await callWithTelemetryAndErrorHandling(
    "bicep.activate",
    async (activateContext: IActionContext) => {
      const startTime = Date.now();
      activateContext.telemetry.properties.isActivationEvent = "true";

      try {
        await activateCallback();
      } catch (e) {
        getLogger().error(e.message ?? e);
        throw e;
      }

      activateContext.telemetry.measurements.extensionLoad =
        (Date.now() - startTime) / 1000;
    }
  );
}

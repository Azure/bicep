// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  callWithTelemetryAndErrorHandling,
  IActionContext,
} from "@microsoft/vscode-azext-utils";

import { getLogger } from "./logger";

export async function activateWithTelemetryAndErrorHandling(
  activateCallback: (actionContext: IActionContext) => Promise<void>
): Promise<void> {
  await callWithTelemetryAndErrorHandling(
    "bicep.activate",
    async (actionContext: IActionContext) => {
      const startTime = Date.now();
      actionContext.telemetry.properties.isActivationEvent = "true";

      try {
        await activateCallback(actionContext);
      } catch (e) {
        getLogger().error(e.message ?? e);
        throw e;
      }

      actionContext.telemetry.measurements.extensionLoad =
        (Date.now() - startTime) / 1000;
    }
  );
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  callWithTelemetryAndErrorHandling,
  IActionContext,
  parseError,
} from "@microsoft/vscode-azext-utils";

import { getLogger } from "./logger";

export async function activateWithTelemetryAndErrorHandling(
  activateCallback: (actionContext: IActionContext) => Promise<void>,
): Promise<void> {
  await callWithTelemetryAndErrorHandling(
    "bicep.activate",
    async (actionContext: IActionContext) => {
      const startTime = Date.now();
      actionContext.telemetry.properties.isActivationEvent = "true";

      try {
        await activateCallback(actionContext);
      } catch (e) {
        getLogger().error(parseError(e).message);
        throw e;
      }

      actionContext.telemetry.measurements.extensionLoad =
        (Date.now() - startTime) / 1000;
    },
  );
}

// Creates a possible telemetry event scope.  But the event is only sent if there is a cancel or an error
export async function callWithTelemetryAndErrorHandlingOnlyOnErrors<T>(
  callbackId: string,
  callback: (context: IActionContext) => T | PromiseLike<T>,
): Promise<T | undefined> {
  return await callWithTelemetryAndErrorHandling<T | undefined>(
    callbackId,
    async (context): Promise<T | undefined> => {
      context.telemetry.suppressIfSuccessful = true;

      return await callback(context);
    },
  );
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import {
  registerUIExtensionVariables,
  AzureUserInput,
  createAzExtOutputChannel,
  callWithTelemetryAndErrorHandling,
  IActionContext,
} from "vscode-azureextensionui";

export async function activateWithTelemetryAndErrorHandling(
  context: vscode.ExtensionContext,
  outputChannel: vscode.OutputChannel,
  activateCallback: () => Promise<void>
): Promise<void> {
  const startTime = Date.now();
  registerUIExtensionVariables({
    context,
    outputChannel: createAzExtOutputChannel(outputChannel.name, "bicep"),
    ui: new AzureUserInput(context.globalState),
  });

  await callWithTelemetryAndErrorHandling(
    "bicep.activate",
    async (activateContext: IActionContext) => {
      activateContext.telemetry.properties.isActivationEvent = "true";

      await activateCallback();

      activateContext.telemetry.measurements.extensionLoad =
        (Date.now() - startTime) / 1000;
    }
  );
}

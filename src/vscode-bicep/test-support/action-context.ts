// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext } from "@microsoft/vscode-azext-utils";

export function createActionContext(): IActionContext {
  return {
    errorHandling: { issueProperties: {} },
    telemetry: { measurements: {}, properties: {} },
    ui: {} as IActionContext["ui"],
    valuesToMask: [],
  };
}

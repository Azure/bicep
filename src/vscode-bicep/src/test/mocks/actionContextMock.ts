// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext } from "@microsoft/vscode-azext-utils";

// CONSIDER: Update vscode-azext-azureutils and use createTestActionContext from there
export function createActionContextMock(): IActionContext {
  return {
    errorHandling: {
      issueProperties: {},
    },
    telemetry: {
      properties: {},
      measurements: {},
    },
    ui: {
      showWarningMessage: jest.fn(),
      showQuickPick: jest.fn(),
      showInputBox: jest.fn(),
      onDidFinishPrompt: jest.fn(),
      showOpenDialog: jest.fn(),
    },
    valuesToMask: [],
  };
}

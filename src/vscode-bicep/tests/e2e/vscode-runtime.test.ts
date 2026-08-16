// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <reference types="vitest/globals" />

import * as vscode from "vscode";

test("runs inside the VS Code extension host", () => {
  console.log(`VS Code version: ${vscode.version}`);
  // These checks prove the module received the real host API rather than a Node test double.
  expect(vscode.version).toMatch(/^\d+\.\d+\.\d+/);
  expect(vscode.extensions.getExtension("ms-azuretools.vscode-bicep")).toBeDefined();
});

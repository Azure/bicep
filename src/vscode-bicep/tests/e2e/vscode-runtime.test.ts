// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <reference types="vitest/globals" />

import { extensions, version } from "vscode";

test("runs inside the VS Code extension host", () => {
  console.log(`VS Code version: ${version}`);
  // These checks prove the module received the real host API rather than a Node test double.
  expect(version).toMatch(/^\d+\.\d+\.\d+/);
  expect(extensions.getExtension("ms-azuretools.vscode-bicep")).toBeDefined();
});

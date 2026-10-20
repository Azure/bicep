// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type * as vscode from "vscode";

import { vi } from "vitest";

declare global {
  // Set by VitestTestRunner before Vite starts evaluating setup and test modules.
  var __bicepVscodeApi: typeof vscode | undefined;
}

// `vscode` is not a package on disk. Redirect imports from Vite's module graph to the complete API
// object obtained from the extension host. This works because the custom pool stays in-process.
vi.mock("vscode", () => {
  if (!globalThis.__bicepVscodeApi) {
    throw new Error("The VS Code extension-host API was not provided to Vitest.");
  }

  return {
    ...globalThis.__bicepVscodeApi,
    default: globalThis.__bicepVscodeApi,
  };
});

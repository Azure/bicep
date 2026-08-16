// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { createExtensionHostTestRunner } from "./test-runner";
import { VitestTestRunner } from "./vitest/vitest-test-runner";

// VS Code requires this module to export a function named `run` with the ExtensionHostTestRunner shape.
export const run = createExtensionHostTestRunner(new VitestTestRunner());

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { defineConfig } from "@playwright/test";
export default defineConfig({
  testDir: "./tests",
  workers: 1,
  use: { browserName: "chromium", headless: true, trace: "off", actionTimeout: 10_000 },
  reporter: "line",
});

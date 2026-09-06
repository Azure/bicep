// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { defineConfig, devices } from "@playwright/test";

/**
 * Playwright config for the Visual Designer React app.
 *
 * Tests run against `vite dev`, which boots the React app together
 * with the {@link DevAppShell} that wires a {@link FakeMessageChannel}
 * and renders the {@link DevToolbar}.  The toolbar lets tests swap
 * between named sample deployment graphs without spinning up the
 * VS Code extension host.
 */
const PORT = Number(process.env.PLAYWRIGHT_PORT ?? 5173);
const BASE_URL = `http://127.0.0.1:${PORT}`;

export default defineConfig({
  testDir: "./e2e",
  testMatch: /.*\.spec\.ts/,
  outputDir: "./e2e/.results",

  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,

  timeout: 30_000,
  expect: {
    timeout: 10_000,
  },

  reporter: process.env.CI
    ? [["github"], ["html", { open: "never", outputFolder: "./e2e/.report" }], ["list"]]
    : [["list"], ["html", { open: "never", outputFolder: "./e2e/.report" }]],

  use: {
    baseURL: BASE_URL,
    headless: true,
    viewport: { width: 1440, height: 900 },
    trace: "retain-on-failure",
    screenshot: "only-on-failure",
    video: "retain-on-failure",
    actionTimeout: 10_000,
    navigationTimeout: 20_000,
  },

  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],

  webServer: {
    command: `npm run dev -- --host 127.0.0.1 --port ${PORT} --strictPort`,
    url: BASE_URL,
    reuseExistingServer: !process.env.CI,
    stdout: "pipe",
    stderr: "pipe",
    timeout: 120_000,
  },
});

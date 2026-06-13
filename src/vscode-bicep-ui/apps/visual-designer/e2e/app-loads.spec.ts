// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import { openVisualDesigner } from "./fixtures";

test.describe("Application bootstrap", () => {
  test("loads the visual designer shell", async ({ page }) => {
    await page.goto("/");

    await expect(page).toHaveTitle(/Visual Designer/i);
    await expect(page.getByTestId("app-root")).toBeVisible();
    await expect(page.getByTestId("graph-canvas")).toBeVisible();
    await expect(page.getByTestId("control-bar")).toBeVisible();
    await expect(page.getByTestId("status-bar")).toBeVisible();
  });

  test("renders without console errors", async ({ page }) => {
    const errors: string[] = [];
    page.on("console", (msg) => {
      if (msg.type() === "error") {
        errors.push(msg.text());
      }
    });
    page.on("pageerror", (err) => errors.push(err.message));

    await openVisualDesigner(page);

    // Filter out errors that are not actionable for E2E (e.g. dev-only
    // resource fetch warnings from external CDNs).  Surface anything else.
    const significant = errors.filter((text) => !/failed to load resource/i.test(text) && !/codicon/i.test(text));
    expect(significant, `Unexpected console errors:\n${significant.join("\n")}`).toEqual([]);
  });

  test("auto-receives the default deployment graph from the fake channel", async ({ page }) => {
    await openVisualDesigner(page);

    const count = await page.getByTestId("graph-node").count();
    expect(count).toBeGreaterThan(0);
  });
});

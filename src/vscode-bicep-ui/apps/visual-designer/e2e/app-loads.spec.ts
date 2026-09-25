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

  test("chrome matches the host theme on first load", async ({ page }) => {
    // The dev playground applies its persisted theme asynchronously, after the theme atom is created
    // with the light default. That used to leave app chrome light while the canvas followed the host.
    // Start on a non-light theme so the race is observable, and reload because it is timing-dependent.
    await page.addInitScript(() => localStorage.setItem("vscode-playground:theme", "dark-v2"));
    for (let attempt = 0; attempt < 3; attempt++) {
      await openVisualDesigner(page);
      await expect(page.locator("body")).toHaveAttribute("data-vscode-theme-kind", "vscode-dark");
      await expect(page.getByTestId("creation-dock")).toHaveCSS("background-color", "rgba(38, 38, 38, 0.92)");
    }
  });
});

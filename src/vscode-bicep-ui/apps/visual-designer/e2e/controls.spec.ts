// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import { getGraphTransform, loadSampleGraph, openVisualDesigner } from "./fixtures";

test.describe("Status bar", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
  });

  test("reports a ready state for a healthy graph", async ({ page }) => {
    await loadSampleGraph(page, "flat");

    await expect(page.getByTestId("status-bar")).toHaveAttribute("data-status", "ready");
    await expect(page.getByTestId("status-error-link")).toHaveCount(0);
    await expect(page.getByTestId("status-empty-message")).toHaveCount(0);
  });

  test("surfaces the error count for a graph that has diagnostics", async ({ page }) => {
    await loadSampleGraph(page, "error");

    const statusBar = page.getByTestId("status-bar");
    await expect(statusBar).toHaveAttribute("data-status", "errors");
    await expect(statusBar).toHaveAttribute("data-error-count", "3");
    await expect(page.getByTestId("status-error-link")).toHaveText(/3\s+errors/);
  });

  test("shows the empty-state message when the graph is null", async ({ page }) => {
    await loadSampleGraph(page, "empty");

    await expect(page.getByTestId("status-bar")).toHaveAttribute("data-status", "empty");
    await expect(page.getByTestId("status-empty-message")).toBeVisible();
  });
});

test.describe("Control bar", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
  });

  test("exposes all controls and disables graph-dependent ones when empty", async ({ page }) => {
    await expect(page.getByTestId("control-zoom-in")).toBeEnabled();
    await expect(page.getByTestId("control-zoom-out")).toBeEnabled();

    await loadSampleGraph(page, "empty");

    await expect(page.getByTestId("control-fit-view")).toBeDisabled();
    await expect(page.getByTestId("control-reset-layout")).toBeDisabled();
    await expect(page.getByTestId("control-export")).toBeDisabled();
  });

  test("re-enables graph-dependent controls when a graph is loaded", async ({ page }) => {
    await loadSampleGraph(page, "empty");
    await expect(page.getByTestId("control-fit-view")).toBeDisabled();

    await loadSampleGraph(page, "flat");
    await expect(page.getByTestId("control-fit-view")).toBeEnabled();
    await expect(page.getByTestId("control-reset-layout")).toBeEnabled();
    await expect(page.getByTestId("control-export")).toBeEnabled();
  });

  test("zoom in changes the pan-zoom transform", async ({ page }) => {
    await loadSampleGraph(page, "flat");

    const before = await getGraphTransform(page);
    await page.getByTestId("control-zoom-in").click();
    // The pan-zoom transform updates synchronously after the click,
    // but allow a frame for the styled-component to flush.
    await expect.poll(async () => await getGraphTransform(page), { timeout: 5_000 }).not.toBe(before);
  });

  test("zoom out also changes the pan-zoom transform", async ({ page }) => {
    await loadSampleGraph(page, "flat");

    const before = await getGraphTransform(page);
    await page.getByTestId("control-zoom-out").click();
    await expect.poll(async () => await getGraphTransform(page), { timeout: 5_000 }).not.toBe(before);
  });

  test("fit-view recenters the graph after zoom changes", async ({ page }) => {
    await loadSampleGraph(page, "flat");

    await page.getByTestId("control-zoom-in").click();
    await page.getByTestId("control-zoom-in").click();
    const zoomed = await getGraphTransform(page);

    await page.getByTestId("control-fit-view").click();
    await expect.poll(async () => await getGraphTransform(page), { timeout: 5_000 }).not.toBe(zoomed);
  });
});

test.describe("Export overlay", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "flat");
  });

  test("opens via the export control and closes with Escape", async ({ page }) => {
    await expect(page.getByTestId("export-overlay")).toHaveCount(0);

    await page.getByTestId("control-export").click();
    await expect(page.getByTestId("export-overlay")).toBeVisible();

    await page.keyboard.press("Escape");
    await expect(page.getByTestId("export-overlay")).toHaveCount(0);
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import {
  getGraphTransform,
  loadSampleGraph,
  nodeCount,
  openVisualDesigner,
  waitForStableNodePosition,
} from "./fixtures";

test.describe("Viewer mode with resource creation disabled", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page, { resourceEditing: "false" });
    await loadSampleGraph(page, "flat");
  });

  test("keeps navigation available without creation UI or edit shortcuts", async ({ page }) => {
    await expect(page.getByTestId("creation-dock")).toHaveCount(0);
    await expect(page.getByTestId("open-resource-palette")).toHaveCount(0);
    await expect(page.getByRole("complementary", { name: "Resource Palette" })).toHaveCount(0);
    await expect(page.getByTestId("control-bar")).toBeVisible();
    await expect(page.getByTestId("status-bar")).toHaveAttribute("data-status", "ready");

    const originalTransform = await getGraphTransform(page);
    await page.getByTestId("control-zoom-in").click();
    await expect.poll(() => getGraphTransform(page)).not.toBe(originalTransform);
    const zoomedTransform = await getGraphTransform(page);

    await page.getByTestId("control-fit-view").click();
    await expect.poll(() => getGraphTransform(page)).not.toBe(zoomedTransform);
    await waitForStableNodePosition(page, "vnet");

    const canvas = page.getByTestId("graph-canvas");
    const box = (await canvas.boundingBox())!;
    const start = { x: box.x + 40, y: box.y + 40 };
    const beforePan = await getGraphTransform(page);
    await page.mouse.move(start.x, start.y);
    await page.mouse.down();
    await page.mouse.move(start.x + 50, start.y + 20, { steps: 10 });
    await page.mouse.up();
    await expect.poll(() => getGraphTransform(page)).not.toBe(beforePan);
    await expect(canvas).not.toHaveClass(/\bgrabbing\b/);

    const count = await nodeCount(page);
    await page.locator('[data-node-id="vnet"]').click();
    await page.keyboard.press("Insert");
    await page.keyboard.press("Delete");
    await expect(page.getByTestId("graph-node")).toHaveCount(count);
    await expect(page.getByTestId("creation-dock")).toHaveCount(0);
    await expect(page.getByTestId("pending-resource-node")).toHaveCount(0);
  });

  test("retains source reveal, export, and status transitions", async ({ page }) => {
    const notifications: string[] = [];
    page.on("console", (message) => {
      if (message.type() === "log") notifications.push(message.text());
    });

    await page.locator('[data-node-id="nsg"]').dblclick();
    await expect
      .poll(() => notifications.find((text) => text.includes("[FakeMessageChannel] revealNode")))
      .toContain("nsg");

    // Keep the PNG capture real while replacing only the native save dialog with an in-memory writable.
    await page.evaluate(() => {
      const output = window as Window & { capturedExport?: Blob };
      Object.defineProperty(window, "showSaveFilePicker", {
        configurable: true,
        value: async () => ({
          createWritable: async () => ({
            write: async (blob: Blob) => {
              output.capturedExport = blob;
            },
            close: async () => {},
          }),
        }),
      });
    });
    await expect(page.getByTestId("control-export")).toBeEnabled();
    await page.getByTestId("control-export").click();
    await expect(page.getByTestId("export-overlay")).toBeVisible();
    await page
      .getByRole("toolbar", { name: "Export settings" })
      .getByRole("button", { name: "Export", exact: true })
      .click();
    await expect
      .poll(() => page.evaluate(() => (window as Window & { capturedExport?: Blob }).capturedExport?.type))
      .toBe("image/png");
    expect(
      await page.evaluate(() => (window as Window & { capturedExport?: Blob }).capturedExport?.size ?? 0),
    ).toBeGreaterThan(1_000);
    await page.keyboard.press("Escape");
    await expect(page.getByTestId("export-overlay")).toHaveCount(0);

    await loadSampleGraph(page, "error");
    await expect(page.getByTestId("status-bar")).toHaveAttribute("data-status", "errors");
    await expect(page.getByTestId("status-error-link")).toHaveText(/3\s+errors/);
    await page.getByTestId("status-error-link").click();
    await expect.poll(() => notifications.some((text) => text.includes("showProblems"))).toBe(true);

    await loadSampleGraph(page, "empty");
    await expect(page.getByTestId("status-empty-message")).toBeVisible();
    await expect(page.getByTestId("control-export")).toBeDisabled();
    await expect(page.getByTestId("creation-dock")).toHaveCount(0);
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import { getGraphTransform, loadSampleGraph, openVisualDesigner, waitForStableNodePosition } from "./fixtures";

async function interceptExportedImage(page: Page) {
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
}

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

  test("button groups are evenly inset from the divider and the bar edges", async ({ page }) => {
    const bar = (await page.getByTestId("control-bar").boundingBox())!;
    const first = (await page.getByTestId("control-zoom-in").boundingBox())!;
    const reset = (await page.getByTestId("control-reset-layout").boundingBox())!;
    const exportButton = (await page.getByTestId("control-export").boundingBox())!;
    const edgeInset = first.y - bar.y;

    expect(bar.y + bar.height - (exportButton.y + exportButton.height)).toBeCloseTo(edgeInset, 0);
    // The divider is 1px tall and sits between the two groups; each side of it matches the edge inset.
    expect(exportButton.y - (reset.y + reset.height)).toBeCloseTo(edgeInset * 2 + 1, 0);
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

  test("reset layout returns a dragged node to its laid-out position", async ({ page }) => {
    await loadSampleGraph(page, "flat");

    // Nodes spring into place after layout; measuring or dragging before that settles races it.
    const node = page.locator('[data-node-id="subnet"]');
    const laidOut = await waitForStableNodePosition(page, "subnet");
    const size = await node.boundingBox();

    const from = { x: laidOut.x + size!.width / 2, y: laidOut.y + size!.height / 2 };
    await page.mouse.move(from.x, from.y);
    await page.mouse.down();
    for (let step = 1; step <= 10; step++) {
      await page.mouse.move(from.x + step * 14, from.y + step * 11);
      // d3-drag tracks movement per event; dispatched back to back they can coalesce, so give each
      // move its own frame.
      await page.waitForTimeout(16);
    }
    await page.mouse.up();

    await expect
      .poll(async () => {
        const box = await node.boundingBox();
        return !!box && Math.abs(box.x - laidOut.x) > 100;
      })
      .toBe(true);

    // Layout is derived from topology and measured sizes only -- the client never sends positions
    // back -- so a reset is deterministic and restores the original coordinates exactly.
    await page.getByTestId("control-reset-layout").click();

    await expect
      .poll(async () => {
        const box = await node.boundingBox();
        return !!box && Math.abs(box.x - laidOut.x) <= 1 && Math.abs(box.y - laidOut.y) <= 1;
      })
      .toBe(true);
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

  test("solid background follows zoom and fills the exported image", async ({ page }) => {
    await waitForStableNodePosition(page, "pip");
    await interceptExportedImage(page);
    await page.getByTestId("control-export").click();
    await page.getByRole("toolbar", { name: "Export settings" }).getByRole("combobox").first().click();
    await page.getByRole("option", { name: "Solid" }).click();

    const cover = page.locator("[data-export-background]");
    await expect(cover).toBeVisible();
    const originalWidth = (await cover.boundingBox())!.width;
    const backgroundColor = await cover.evaluate((element) => getComputedStyle(element).backgroundColor);

    await page.getByTestId("control-zoom-in").click();
    await expect.poll(async () => (await cover.boundingBox())?.width).toBeGreaterThan(originalWidth * 1.4);
    await page.getByTestId("control-zoom-out").click();
    await expect.poll(async () => (await cover.boundingBox())?.width).toBeCloseTo(originalWidth, 0);

    await page.getByRole("toolbar", { name: "Export settings" }).getByRole("button", { name: "Export", exact: true }).click();
    await expect
      .poll(() => page.evaluate(() => Boolean((window as Window & { capturedExport?: Blob }).capturedExport)))
      .toBe(true);

    const corners = await page.evaluate(async () => {
      const blob = (window as Window & { capturedExport?: Blob }).capturedExport;
      if (!blob) throw new Error("Exported image not found");
      const bitmap = await createImageBitmap(blob);
      const canvas = document.createElement("canvas");
      canvas.width = bitmap.width;
      canvas.height = bitmap.height;
      const context = canvas.getContext("2d");
      if (!context) throw new Error("Could not inspect exported image");
      context.drawImage(bitmap, 0, 0);
      return [
        [...context.getImageData(1, 1, 1, 1).data],
        [...context.getImageData(canvas.width - 2, canvas.height - 2, 1, 1).data],
      ];
    });
    const colorComponents = backgroundColor.match(/\d+/g)?.map(Number);
    if (!colorComponents || colorComponents.length !== 3) throw new Error(`Unexpected background color: ${backgroundColor}`);
    for (const corner of corners) {
      expect(corner).toEqual([...colorComponents, 255]);
    }
  });

  test("captures every node after zooming the graph", async ({ page }) => {
    await waitForStableNodePosition(page, "pip");
    const graphScale = () =>
      page.locator("[data-export-graph]").evaluate((element) => new DOMMatrixReadOnly(element.style.transform).a);
    const initialScale = await graphScale();
    await page.getByTestId("control-zoom-in").click();
    await expect.poll(graphScale).toBeGreaterThan(initialScale * 1.4);

    const expectedCenters = await page.evaluate(() => {
      const graph = document.querySelector<HTMLElement>("[data-export-graph]");
      if (!graph) throw new Error("Graph content not found");
      const { left, top } = graph.getBoundingClientRect();
      const scale = new DOMMatrixReadOnly(graph.style.transform).a;
      const boxes = [...graph.querySelectorAll<HTMLElement>("[data-node-id]")].map((node) => {
        const rect = node.getBoundingClientRect();
        return {
          left: (rect.left - left) / scale,
          top: (rect.top - top) / scale,
          width: rect.width / scale,
          height: rect.height / scale,
        };
      });
      const minX = Math.min(...boxes.map((box) => box.left));
      const minY = Math.min(...boxes.map((box) => box.top));
      return boxes.map((box) => ({
        x: Math.round((box.left - minX + box.width / 2 + 40) * 2),
        y: Math.round((box.top - minY + box.height / 2 + 40) * 2),
      }));
    });
    expect(expectedCenters).toHaveLength(4);

    await interceptExportedImage(page);

    await page.getByTestId("control-export").click();
    await page.getByRole("toolbar", { name: "Export settings" }).getByRole("button", { name: "Export", exact: true }).click();
    await expect
      .poll(() => page.evaluate(() => Boolean((window as Window & { capturedExport?: Blob }).capturedExport)))
      .toBe(true);

    const capture = await page.evaluate(async (centers) => {
      const blob = (window as Window & { capturedExport?: Blob }).capturedExport;
      if (!blob) throw new Error("Exported image not found");
      const bitmap = await createImageBitmap(blob);
      const canvas = document.createElement("canvas");
      canvas.width = bitmap.width;
      canvas.height = bitmap.height;
      const context = canvas.getContext("2d");
      if (!context) throw new Error("Could not inspect exported image");
      context.drawImage(bitmap, 0, 0);
      const pixels = context.getImageData(0, 0, canvas.width, canvas.height).data;
      return {
        width: canvas.width,
        height: canvas.height,
        alphas: centers.map(({ x, y }) => pixels[(y * canvas.width + x) * 4 + 3]),
      };
    }, expectedCenters);

    for (const { x, y } of expectedCenters) {
      expect(x).toBeGreaterThan(0);
      expect(y).toBeGreaterThan(0);
      expect(x).toBeLessThan(capture.width);
      expect(y).toBeLessThan(capture.height);
    }
    expect(capture.alphas).toHaveLength(4);
    expect(capture.alphas.every((alpha) => alpha !== undefined && alpha > 0)).toBe(true);
  });
});

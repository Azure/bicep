// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import {
  clickHistoryButton,
  expectHistoryButtonEnabled,
  getGraphTransform,
  loadSampleGraph,
  openVisualDesigner,
  waitForStableNodePosition,
} from "./fixtures";

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

async function dragNode(page: Page, nodeId: string, dx: number, dy: number) {
  const node = page.locator(`[data-node-id="${nodeId}"]`);
  const before = await waitForStableNodePosition(page, nodeId);
  const size = await node.boundingBox();
  if (!size) {
    throw new Error(`Could not find node ${nodeId}.`);
  }

  const from = { x: before.x + size.width / 2, y: before.y + size.height / 2 };
  await page.mouse.move(from.x, from.y);
  await page.mouse.down();
  for (let step = 1; step <= 10; step++) {
    await page.mouse.move(from.x + (dx * step) / 10, from.y + (dy * step) / 10);
    // Give d3-drag a frame per move so its event stream does not coalesce.
    await page.waitForTimeout(16);
  }
  await page.mouse.up();

  return { before, after: await waitForStableNodePosition(page, nodeId) };
}

async function expectNodePosition(page: Page, nodeId: string, position: { x: number; y: number }) {
  await expect
    .poll(async () => {
      const box = await page.locator(`[data-node-id="${nodeId}"]`).boundingBox();
      return !!box && Math.abs(box.x - position.x) <= 2 && Math.abs(box.y - position.y) <= 2;
    })
    .toBe(true);
}

function nodeGraphX(page: Page, nodeId: string) {
  return page
    .locator(`[data-node-id="${nodeId}"]`)
    .evaluate((element) => Number.parseFloat((element as HTMLElement).style.translate));
}

async function trackNodeAnimation(page: Page, nodeId: string) {
  await page.evaluate((id) => {
    const node = document.querySelector<HTMLElement>(`[data-node-id="${id}"]`);
    if (!node) {
      throw new Error(`Could not find node ${id}.`);
    }
    const samples: number[] = [];
    (window as Window & { layoutAnimationSamples?: number[] }).layoutAnimationSamples = samples;
    const started = performance.now();
    const record = () => {
      samples.push(Number.parseFloat(node.style.translate));
      if (performance.now() - started < 800) {
        requestAnimationFrame(record);
      }
    };
    requestAnimationFrame(record);
  }, nodeId);
}

async function expectIntermediatePosition(page: Page, from: number, to: number) {
  const min = Math.min(from, to) + 2;
  const max = Math.max(from, to) - 2;
  await expect
    .poll(() =>
      page.evaluate(
        ({ min, max }) =>
          (window as Window & { layoutAnimationSamples?: number[] }).layoutAnimationSamples?.some(
            (position) => position > min && position < max,
          ) ?? false,
        { min, max },
      ),
    )
    .toBe(true);
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
    await expect(page.getByTestId("status-indicator")).toHaveCount(0);
  });

  test("shares the dock's bottom inset and truncates before reaching it", async ({ page }) => {
    for (const key of ["error", "empty"] as const) {
      await loadSampleGraph(page, key);
      for (const width of [1440, 520]) {
        await page.setViewportSize({ width, height: 700 });
        const status = (await page.getByTestId("status-bar").boundingBox())!;
        const dock = (await page.getByTestId("creation-dock").boundingBox())!;
        const app = (await page.getByTestId("app-root").boundingBox())!;

        expect(status.x - app.x).toBe(16);
        expect(app.y + app.height - (status.y + status.height)).toBeCloseTo(16, 0);
        expect(app.y + app.height - (dock.y + dock.height)).toBeCloseTo(16, 0);
        expect(status.height).toBe(34);
        expect(status.x + status.width).toBeLessThanOrEqual(dock.x - 12);
      }
    }
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

  test("floating panels never show a caret or select text", async ({ page }) => {
    for (const testId of ["control-bar", "history-bar", "creation-dock"]) {
      const style = await page.getByTestId(testId).evaluate((element) => ({
        caret: getComputedStyle(element).caretColor,
        select: getComputedStyle(element).userSelect,
      }));

      expect(style, testId).toEqual({ caret: "rgba(0, 0, 0, 0)", select: "none" });
    }
  });

  test("exposes all controls and disables graph-dependent ones when empty", async ({ page }) => {
    await expect(page.getByTestId("control-zoom-in")).toBeEnabled();
    await expect(page.getByTestId("control-zoom-out")).toBeEnabled();
    await expectHistoryButtonEnabled(page, "Undo", false);
    await expectHistoryButtonEnabled(page, "Redo", false);

    await loadSampleGraph(page, "empty");

    await expect(page.getByTestId("control-fit-view")).toBeDisabled();
    await expect(page.getByTestId("control-reset-layout")).toBeDisabled();
    await expect(page.getByTestId("control-export")).toBeDisabled();
  });

  test("keeps Undo and Redo in their own panel below the view controls", async ({ page }) => {
    const controlBar = page.getByTestId("control-bar");
    const historyBar = page.getByTestId("history-bar");
    await expect(controlBar.getByRole("button")).toHaveCount(5);
    await expect(controlBar.getByRole("separator")).toHaveCount(1);
    await expect(historyBar.getByRole("button")).toHaveCount(2);

    const bar = (await controlBar.boundingBox())!;
    const history = (await historyBar.boundingBox())!;
    const first = (await page.getByTestId("control-zoom-in").boundingBox())!;
    const reset = (await page.getByTestId("control-reset-layout").boundingBox())!;
    const exportButton = (await page.getByTestId("control-export").boundingBox())!;
    const edgeInset = first.y - bar.y;

    expect(bar.y + bar.height - (exportButton.y + exportButton.height)).toBeCloseTo(edgeInset, 0);
    expect(exportButton.y - (reset.y + reset.height)).toBeCloseTo(edgeInset * 2 + 1, 0);
    expect(history.y - (bar.y + bar.height)).toBeCloseTo(12, 0);
    expect(history.x + history.width).toBeCloseTo(bar.x + bar.width, 0);
  });

  test("re-enables graph-dependent controls when a graph is loaded", async ({ page }) => {
    await loadSampleGraph(page, "empty");
    await expect(page.getByTestId("control-fit-view")).toBeDisabled();

    await loadSampleGraph(page, "flat");
    await expect(page.getByTestId("control-fit-view")).toBeEnabled();
    await expect(page.getByTestId("control-reset-layout")).toBeEnabled();
    await expect(page.getByTestId("control-export")).toBeEnabled();
    await expectHistoryButtonEnabled(page, "Undo", false);
    await expectHistoryButtonEnabled(page, "Redo", false);
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

    const { before: laidOut, after: dragged } = await dragNode(page, "subnet", 140, 110);
    expect(Math.abs(dragged.x - laidOut.x)).toBeGreaterThan(100);

    // Layout is derived from topology and measured sizes only -- the client never sends positions
    // back -- so a reset is deterministic and restores the original coordinates exactly.
    await page.getByTestId("control-reset-layout").click();

    await expectNodePosition(page, "subnet", laidOut);
  });

  test("node drags and Reset Layout are separate undo steps without changing the camera", async ({ page }) => {
    await loadSampleGraph(page, "flat");
    const { before: laidOut, after: dragged } = await dragNode(page, "subnet", 140, 110);
    const camera = await getGraphTransform(page);
    expect(Math.abs(dragged.x - laidOut.x)).toBeGreaterThan(100);
    await expectHistoryButtonEnabled(page, "Undo", true);
    await expectHistoryButtonEnabled(page, "Redo", false);

    await page.keyboard.press("ControlOrMeta+z");
    await expectNodePosition(page, "subnet", laidOut);
    await expectHistoryButtonEnabled(page, "Redo", true);
    await page.keyboard.press("ControlOrMeta+Shift+z");
    await expectNodePosition(page, "subnet", dragged);
    await expectHistoryButtonEnabled(page, "Undo", true);

    await page.getByTestId("control-reset-layout").click();
    await expectNodePosition(page, "subnet", laidOut);
    await expectHistoryButtonEnabled(page, "Undo", true);

    await clickHistoryButton(page, "Undo");
    await expectNodePosition(page, "subnet", dragged);
    await clickHistoryButton(page, "Undo");
    await expectNodePosition(page, "subnet", laidOut);
    await expectHistoryButtonEnabled(page, "Undo", false);
    await expectHistoryButtonEnabled(page, "Redo", true);

    await clickHistoryButton(page, "Redo");
    await expectNodePosition(page, "subnet", dragged);
    await clickHistoryButton(page, "Redo");
    await expectNodePosition(page, "subnet", laidOut);
    expect(await getGraphTransform(page)).toBe(camera);
  });

  test("undoing a Reset Layout that interrupted an animation returns to where the nodes were heading", async ({
    page,
  }) => {
    await loadSampleGraph(page, "flat");
    const { before: laidOut, after: dragged } = await dragNode(page, "subnet", 140, 110);

    // Redo starts a spring back to the dragged position; Reset Layout interrupts it partway.
    await page.keyboard.press("ControlOrMeta+z");
    await expectNodePosition(page, "subnet", laidOut);
    await page.keyboard.press("ControlOrMeta+Shift+z");
    await page.getByTestId("control-reset-layout").click();
    await expectNodePosition(page, "subnet", laidOut);
    await expectHistoryButtonEnabled(page, "Undo", true);

    await clickHistoryButton(page, "Undo");
    await expectNodePosition(page, "subnet", dragged);
  });

  test("animates node positions when replaying layout history without moving the camera", async ({ page }) => {
    await loadSampleGraph(page, "flat");
    await waitForStableNodePosition(page, "subnet");
    const laidOutX = await nodeGraphX(page, "subnet");
    const { before: laidOut, after: dragged } = await dragNode(page, "subnet", 140, 110);
    const draggedX = await nodeGraphX(page, "subnet");
    const camera = await getGraphTransform(page);
    expect(Math.abs(draggedX - laidOutX)).toBeGreaterThan(20);

    await trackNodeAnimation(page, "subnet");
    await clickHistoryButton(page, "Undo");
    await expectIntermediatePosition(page, draggedX, laidOutX);
    await expectNodePosition(page, "subnet", laidOut);
    await waitForStableNodePosition(page, "subnet");

    await trackNodeAnimation(page, "subnet");
    await clickHistoryButton(page, "Redo");
    await expectIntermediatePosition(page, laidOutX, draggedX);
    await expectNodePosition(page, "subnet", dragged);
    expect(await getGraphTransform(page)).toBe(camera);
  });

  test("rapid undo and redo retarget an in-flight layout animation", async ({ page }) => {
    await loadSampleGraph(page, "flat");
    const { after: dragged } = await dragNode(page, "subnet", 140, 110);

    await clickHistoryButton(page, "Undo");
    await clickHistoryButton(page, "Redo");

    await waitForStableNodePosition(page, "subnet");
    await expectNodePosition(page, "subnet", dragged);
    await expectHistoryButtonEnabled(page, "Redo", false);
    await expectHistoryButtonEnabled(page, "Undo", true);
  });

  for (const { policy, systemPreference } of [
    { policy: "reduce", systemPreference: "no-preference" },
    { policy: "system", systemPreference: "reduce" },
  ] as const) {
    test(`snaps layout undo/redo under ${policy} reduced-motion policy`, async ({ page }) => {
      await page.emulateMedia({ reducedMotion: systemPreference });
      await openVisualDesigner(page, { motionPolicy: policy });
      await loadSampleGraph(page, "flat");
      await waitForStableNodePosition(page, "subnet");
      const laidOutX = await nodeGraphX(page, "subnet");
      await dragNode(page, "subnet", 140, 110);
      const draggedX = await nodeGraphX(page, "subnet");

      await clickHistoryButton(page, "Undo");
      expect(await nodeGraphX(page, "subnet")).toBeCloseTo(laidOutX, 0);
      await clickHistoryButton(page, "Redo");
      expect(await nodeGraphX(page, "subnet")).toBeCloseTo(draggedX, 0);
    });
  }

  test("a new layout move clears redo without recording an unmoved node", async ({ page }) => {
    await loadSampleGraph(page, "flat");
    const { before: laidOut } = await dragNode(page, "subnet", 140, 110);

    await clickHistoryButton(page, "Undo");
    await expectNodePosition(page, "subnet", laidOut);
    await expectHistoryButtonEnabled(page, "Redo", true);

    await page.locator('[data-node-id="subnet"]').click();
    await expectHistoryButtonEnabled(page, "Redo", true);

    const { after: movedAgain } = await dragNode(page, "subnet", -120, 80);
    expect(Math.abs(movedAgain.x - laidOut.x)).toBeGreaterThan(90);
    await expectHistoryButtonEnabled(page, "Redo", false);
    await clickHistoryButton(page, "Undo");
    await expectNodePosition(page, "subnet", laidOut);
  });

  test("source graph updates discard layout steps for removed IDs", async ({ page }) => {
    await loadSampleGraph(page, "flat");
    await dragNode(page, "subnet", 140, 110);
    await expectHistoryButtonEnabled(page, "Undo", true);

    await loadSampleGraph(page, "flat");
    await expectHistoryButtonEnabled(page, "Undo", true);

    await loadSampleGraph(page, "module");
    await expectHistoryButtonEnabled(page, "Undo", false);
    await loadSampleGraph(page, "flat");
    await expectHistoryButtonEnabled(page, "Undo", false);
  });

  test("keeps layout undo available when resource editing is disabled", async ({ page }) => {
    await openVisualDesigner(page, { resourceEditing: "false" });
    await loadSampleGraph(page, "flat");
    const { before: laidOut, after: dragged } = await dragNode(page, "subnet", 120, 80);
    expect(Math.abs(dragged.x - laidOut.x)).toBeGreaterThan(90);
    await expect(page.getByTestId("creation-dock")).toHaveCount(0);

    await expectHistoryButtonEnabled(page, "Undo", true);
    await page.keyboard.press("ControlOrMeta+z");
    await expectNodePosition(page, "subnet", laidOut);
    await clickHistoryButton(page, "Redo");
    await expectNodePosition(page, "subnet", dragged);
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

    await page
      .getByRole("toolbar", { name: "Export settings" })
      .getByRole("button", { name: "Export", exact: true })
      .click();
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
    if (!colorComponents || colorComponents.length !== 3)
      throw new Error(`Unexpected background color: ${backgroundColor}`);
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
    await page
      .getByRole("toolbar", { name: "Export settings" })
      .getByRole("button", { name: "Export", exact: true })
      .click();
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

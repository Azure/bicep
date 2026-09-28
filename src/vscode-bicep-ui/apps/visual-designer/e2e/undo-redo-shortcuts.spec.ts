// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import { loadSampleGraph, openVisualDesigner, waitForStableNodePosition } from "./fixtures";

async function dragSubnet(page: Page) {
  const node = page.locator('[data-node-id="subnet"]');
  const original = await waitForStableNodePosition(page, "subnet");
  const box = (await node.boundingBox())!;
  const start = { x: box.x + box.width / 2, y: box.y + box.height / 2 };
  await page.mouse.move(start.x, start.y);
  await page.mouse.down();
  await page.mouse.move(start.x + 80, start.y + 40, { steps: 10 });
  await page.mouse.up();
  await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeGreaterThan(original.x + 50);
  return { node, original, moved: await waitForStableNodePosition(page, "subnet") };
}

async function addInputToCanvas(page: Page) {
  await page.getByRole("region", { name: "Visual designer canvas" }).evaluate((canvas) => {
    const input = document.createElement("input");
    input.dataset.testid = "inline-canvas-input";
    input.style.position = "absolute";
    input.style.left = "40px";
    input.style.top = "40px";
    canvas.appendChild(input);
  });
  return page.getByTestId("inline-canvas-input");
}

test.describe("Undo/Redo shortcuts", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "flat");
  });

  test("undo and redo a node move from the keyboard", async ({ page }) => {
    const { node, original, moved } = await dragSubnet(page);

    await page.keyboard.press("ControlOrMeta+z");
    await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeCloseTo(original.x, 0);
    await page.keyboard.press("ControlOrMeta+Shift+z");
    await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeCloseTo(moved.x, 0);

    if (process.platform !== "darwin") {
      await page.keyboard.press("ControlOrMeta+z");
      await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeCloseTo(original.x, 0);
      await page.keyboard.press("Control+y");
      await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeCloseTo(moved.x, 0);
    }
  });

  test("keep working after focus moves to a control button", async ({ page }) => {
    const { node, original, moved } = await dragSubnet(page);
    const zoomIn = page.getByTestId("control-zoom-in");
    await zoomIn.focus();
    await expect(zoomIn).toBeFocused();

    await page.keyboard.press("ControlOrMeta+z");
    await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeCloseTo(original.x, 0);
    await page.keyboard.press("ControlOrMeta+Shift+z");
    await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeCloseTo(moved.x, 0);
  });

  test("stop the shortcut before VS Code's webview listener forwards it to the workbench", async ({ page }) => {
    // VS Code's webview host listens on `window` and forwards every key press it sees.
    await page.evaluate(() => {
      const forwardedKeys: string[] = [];
      (window as Window & { forwardedKeys?: string[] }).forwardedKeys = forwardedKeys;
      window.addEventListener("keydown", (event) => {
        if (!["Control", "Meta", "Shift"].includes(event.key)) {
          forwardedKeys.push(event.key);
        }
      });
    });
    await dragSubnet(page);

    await page.keyboard.press("ControlOrMeta+z");
    await page.keyboard.press("a");

    expect(await page.evaluate(() => (window as Window & { forwardedKeys?: string[] }).forwardedKeys)).toEqual(["a"]);
  });

  test("leave undo in a focused text field to the field", async ({ page }) => {
    const { node, original, moved } = await dragSubnet(page);
    const input = await addInputToCanvas(page);
    await input.focus();
    await input.pressSequentially("draft");

    await page.keyboard.press("ControlOrMeta+z");
    await expect(input).toHaveValue("");
    await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeCloseTo(moved.x, 0);

    await input.blur();
    await page.keyboard.press("ControlOrMeta+z");
    await expect.poll(async () => (await node.boundingBox())?.x ?? 0).toBeCloseTo(original.x, 0);
  });
});

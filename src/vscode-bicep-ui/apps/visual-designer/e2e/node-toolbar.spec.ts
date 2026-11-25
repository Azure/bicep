// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import { hoverNode, loadSampleGraph, openVisualDesigner, waitForStableNodePosition } from "./fixtures";

function toolbar(page: Page, name: string) {
  return page.getByRole("toolbar", { name: `Actions for ${name}` });
}

/** Every node toolbar, whichever node it is for; the dock is a toolbar too. */
function nodeToolbars(page: Page) {
  return page.locator("[data-node-toolbar]");
}

function selectedNodeIds(page: Page) {
  return page
    .locator('[data-node-id]:has([data-selected="true"])')
    .evaluateAll((nodes) => nodes.map((node) => (node as HTMLElement).dataset.nodeId));
}

function canvasRegion(page: Page) {
  return page.getByRole("region", { name: "Visual designer canvas" });
}

test.describe("Selected-node toolbar", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "flat");
    await waitForStableNodePosition(page, "vnet");
  });

  test("shows above a single selected node and reveals its source", async ({ page }) => {
    const reveals: string[] = [];
    page.on("console", (message) => {
      if (message.text().includes("[FakeMessageChannel] revealNode")) reveals.push(message.text());
    });

    await expect(nodeToolbars(page)).toHaveCount(0);
    await page.locator('[data-node-id="vnet"]').click();
    const bar = toolbar(page, "vnet");
    await expect(bar).toBeVisible();
    // A resource has nothing to fold.
    await expect(bar.getByRole("button")).toHaveCount(1);

    const node = (await page.locator('[data-node-id="vnet"]').boundingBox())!;
    const barBox = (await bar.boundingBox())!;
    expect(barBox.y + barBox.height).toBeLessThanOrEqual(node.y + 4);
    // Right-aligned with the node's frame, inside its 4px margin.
    expect(Math.abs(barBox.x + barBox.width - (node.x + node.width - 4))).toBeLessThan(2);

    await bar.getByRole("button", { name: "Go to Source" }).click();
    await expect.poll(() => reveals.at(-1)).toContain("vnet");

    // Showing it never takes focus; Escape from it returns focus to the canvas and keeps the selection.
    await bar.getByRole("button", { name: "Go to Source" }).focus();
    await page.keyboard.press("Escape");
    await expect(canvasRegion(page)).toBeFocused();
    await expect(bar).toBeVisible();
  });

  test("is absent for several nodes, in Hand, during a drag, and while the context menu is open", async ({ page }) => {
    await page.locator('[data-node-id="vnet"]').click();
    await page.locator('[data-node-id="subnet"]').click({ modifiers: ["Shift"] });
    // Only a node the pointer rests on shows a bar while several are selected.
    await page.mouse.move(4, 300);
    await expect(nodeToolbars(page)).toHaveCount(0);

    await page.locator('[data-node-id="vnet"]').click();
    await expect(toolbar(page, "vnet")).toBeVisible();
    await page.keyboard.press("h");
    await expect(nodeToolbars(page)).toHaveCount(0);
    await page.keyboard.press("v");
    await expect(toolbar(page, "vnet")).toBeVisible();

    const box = (await page.locator('[data-node-id="vnet"]').boundingBox())!;
    await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
    await page.mouse.down();
    await page.mouse.move(box.x + box.width / 2 + 60, box.y + box.height / 2 + 20, { steps: 8 });
    await expect(nodeToolbars(page)).toHaveCount(0);
    await page.mouse.up();
    await expect(toolbar(page, "vnet")).toBeVisible();

    await page.locator('[data-node-id="vnet"]').click({ button: "right" });
    await expect(page.getByRole("menu", { name: "Actions for vnet" })).toBeVisible();
    await expect(nodeToolbars(page)).toHaveCount(0);
    await page.keyboard.press("Escape");
    await expect(toolbar(page, "vnet")).toBeVisible();
  });

  test("folds a selected container from the toolbar and the context menu", async ({ page }) => {
    await loadSampleGraph(page, "module");
    await waitForStableNodePosition(page, "myModule");
    const module = page.locator('[data-node-id="myModule"]');
    await module
      .locator("[data-module-collapsed]")
      .first()
      .click({ position: { x: 60, y: 20 } });
    const bar = toolbar(page, "myModule");

    await bar.getByRole("button", { name: "Collapse" }).click();
    await expect(module.locator('[data-module-collapsed="true"]')).toHaveCount(1);
    await expect(bar.getByRole("button", { name: "Expand" })).toHaveAttribute("aria-expanded", "false");

    await module.click({ button: "right" });
    const menu = page.getByRole("menu", { name: "Actions for myModule" });
    await page.keyboard.press("ArrowDown");
    await expect(menu.getByRole("menuitem", { name: "Expand" })).toBeFocused();
    await page.keyboard.press("Enter");
    await expect(menu).toHaveCount(0);
    await expect(module.locator('[data-module-collapsed="false"]')).toHaveCount(1);
  });

  test("shows a bar for a hovered node, which the pointer can reach and use", async ({ page }) => {
    await loadSampleGraph(page, "module");
    await waitForStableNodePosition(page, "myModule");
    // Containers have no fold control of their own; the toolbar offers it.
    await expect(page.locator('[data-node-id="myModule"] button')).toHaveCount(0);

    const bar = await hoverNode(page, "myModule");
    await expect(bar).toHaveAttribute("data-transient", "true");
    await expect.poll(() => selectedNodeIds(page)).toEqual([]);

    // Crossing the gap into the bar keeps it.
    await bar.getByRole("button", { name: "Collapse module myModule", exact: true }).click();
    await expect(page.locator('[data-node-id="myModule"] [data-module-collapsed="true"]')).toHaveCount(1);
    await expect.poll(() => selectedNodeIds(page)).toEqual([]);

    // Leaving it hides it; a selected node's bar is opaque and stays.
    await page.mouse.move(4, 300);
    await expect(nodeToolbars(page)).toHaveCount(0);
    await page.locator('[data-node-id="networkInterface"]').click();
    await page.mouse.move(4, 300);
    await expect(toolbar(page, "networkInterface")).toHaveAttribute("data-transient", "false");

    // Hovering another node shows its bar instead, and the selected node's returns afterwards.
    await hoverNode(page, "myModule");
    await expect(toolbar(page, "networkInterface")).toHaveCount(0);
    await page.mouse.move(4, 300);
    await expect(toolbar(page, "networkInterface")).toBeVisible();
  });
});

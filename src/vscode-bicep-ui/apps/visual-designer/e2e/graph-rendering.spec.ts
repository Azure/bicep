// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import { loadSampleGraph, nodeCount, openVisualDesigner } from "./fixtures";

const FLAT_LAYOUT_ORDER = ["nsg", "pip", "subnet", "vnet"] as const;
const MODULE_LAYOUT_NODES = ["myModule", "myModule::storageAccount", "myModule::vmResource"] as const;

type NodeBox = { x: number; y: number; width: number; height: number };

async function getNodeBoxes(page: Page, nodeIds: readonly string[]): Promise<Record<string, NodeBox> | null> {
  return page.evaluate((ids) => {
    const boxes: Record<string, NodeBox> = {};

    for (const id of ids) {
      const element = document.querySelector<HTMLElement>(`[data-node-id="${id}"]`);
      if (!element) {
        return null;
      }

      const box = element.getBoundingClientRect();
      boxes[id] = { x: box.x, y: box.y, width: box.width, height: box.height };
    }

    return boxes;
  }, nodeIds);
}

function followsFlatServerLayout(boxes: Record<string, NodeBox> | null): boolean {
  if (!boxes) {
    return false;
  }

  return FLAT_LAYOUT_ORDER.every((nodeId, index) => {
    const nextNodeId = FLAT_LAYOUT_ORDER[index + 1];
    const box = boxes[nodeId];
    const nextBox = nextNodeId ? boxes[nextNodeId] : undefined;

    return !nextNodeId || (box !== undefined && nextBox !== undefined && box.y + box.height < nextBox.y);
  });
}

function followsModuleServerLayout(boxes: Record<string, NodeBox> | null): boolean {
  if (!boxes) {
    return false;
  }

  const moduleBox = boxes.myModule;
  const storageBox = boxes["myModule::storageAccount"];
  const vmBox = boxes["myModule::vmResource"];

  if (!moduleBox || !storageBox || !vmBox) {
    return false;
  }

  const storageIsInsideModule =
    storageBox.x > moduleBox.x &&
    storageBox.y > moduleBox.y &&
    storageBox.x + storageBox.width < moduleBox.x + moduleBox.width &&
    storageBox.y + storageBox.height < moduleBox.y + moduleBox.height;
  const vmIsInsideModule =
    vmBox.x > moduleBox.x &&
    vmBox.y > moduleBox.y &&
    vmBox.x + vmBox.width < moduleBox.x + moduleBox.width &&
    vmBox.y + vmBox.height < moduleBox.y + moduleBox.height;

  return storageIsInsideModule && vmIsInsideModule && storageBox.y + storageBox.height < vmBox.y;
}

test.describe("Graph rendering", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
  });

  test("renders the flat graph with the expected resource nodes", async ({ page }) => {
    await loadSampleGraph(page, "flat");

    // The flat sample has four atomic resources: vnet, subnet, nsg, pip.
    const atomicNodes = page.locator('[data-testid="graph-node"][data-node-kind="atomic"]');
    await expect(atomicNodes).toHaveCount(4);

    for (const id of ["vnet", "subnet", "nsg", "pip"]) {
      await expect(page.locator(`[data-node-id="${id}"]`)).toBeVisible();
    }
  });

  test("applies mocked server layout to the flat graph", async ({ page }) => {
    await loadSampleGraph(page, "flat");

    await expect
      .poll(async () => followsFlatServerLayout(await getNodeBoxes(page, FLAT_LAYOUT_ORDER)), { timeout: 5_000 })
      .toBe(true);
  });

  test("applies mocked server layout to module children", async ({ page }) => {
    await loadSampleGraph(page, "module");

    await expect
      .poll(async () => followsModuleServerLayout(await getNodeBoxes(page, MODULE_LAYOUT_NODES)), { timeout: 5_000 })
      .toBe(true);
  });

  test("renders compound (module) nodes for the module graph", async ({ page }) => {
    await loadSampleGraph(page, "module");

    const compoundNodes = page.locator('[data-testid="graph-node"][data-node-kind="compound"]');
    await expect(compoundNodes).toHaveCount(1);
    await expect(page.locator('[data-node-id="myModule"]')).toBeVisible();

    // The module contains two children, plus there are two top-level
    // atomic nodes.
    const atomicNodes = page.locator('[data-testid="graph-node"][data-node-kind="atomic"]');
    await expect(atomicNodes).toHaveCount(4);
  });

  test("renders a richer topology for the complex graph", async ({ page }) => {
    await loadSampleGraph(page, "complex");

    // The complex sample contains 13 modules + 2 top-level resource groups.
    const compoundNodes = page.locator('[data-testid="graph-node"][data-node-kind="compound"]');
    await expect(compoundNodes).toHaveCount(13);

    const total = await nodeCount(page);
    expect(total).toBeGreaterThan(20);
  });

  test("renders an empty canvas for a null graph", async ({ page }) => {
    await loadSampleGraph(page, "empty");

    await expect(page.getByTestId("graph-node")).toHaveCount(0);
    await expect(page.getByTestId("status-bar")).toHaveAttribute("data-status", "empty");
    await expect(page.getByTestId("status-empty-message")).toBeVisible();
  });

  test("swaps the graph in-place when a new sample is loaded", async ({ page }) => {
    await loadSampleGraph(page, "flat");
    const flatCount = await nodeCount(page);

    await loadSampleGraph(page, "complex");
    const complexCount = await nodeCount(page);

    expect(complexCount).toBeGreaterThan(flatCount);

    await loadSampleGraph(page, "empty");
    await expect(page.getByTestId("graph-node")).toHaveCount(0);
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import {
  clickHistoryButton,
  getGraphTransform,
  loadSampleGraph,
  nodeCount,
  openVisualDesigner,
  waitForStableNodePosition,
} from "./fixtures";

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

/** Drag a node by a screen-space offset, in small steps so the drag is not mistaken for a click. */
async function dragNode(page: Page, nodeId: string, dx: number, dy: number): Promise<void> {
  const box = await page.locator(`[data-node-id="${nodeId}"]`).boundingBox();
  if (!box) {
    throw new Error(`Node ${nodeId} is not visible.`);
  }

  const start = { x: box.x + box.width / 2, y: box.y + 12 };
  await page.mouse.move(start.x, start.y);
  await page.mouse.down();
  await page.mouse.move(start.x + dx, start.y + dy, { steps: 10 });
  await page.mouse.up();
}

/** Whether every node is within a pixel of `expected`, offset by `dx`, `dy`. */
async function nodesAreAt(page: Page, expected: Record<string, NodeBox> | null, dx = 0, dy = 0): Promise<boolean> {
  if (!expected) {
    return false;
  }

  const actual = await getNodeBoxes(page, Object.keys(expected));
  return Object.entries(expected).every(([nodeId, box]) => {
    const current = actual?.[nodeId];
    return current !== undefined && Math.abs(current.x - box.x - dx) <= 1 && Math.abs(current.y - box.y - dy) <= 1;
  });
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

  test("collapses and expands a module without moving the camera or unrelated nodes", async ({ page }) => {
    await loadSampleGraph(page, "module");
    await waitForStableNodePosition(page, "myModule::storageAccount");
    const unrelatedBefore = await waitForStableNodePosition(page, "networkInterface");
    const childrenBefore = await getNodeBoxes(page, ["myModule::storageAccount", "myModule::vmResource"]);
    const transformBefore = await getGraphTransform(page);

    await page.getByRole("button", { name: "Collapse module myModule" }).click();

    await expect(page.locator('[data-node-id="myModule"]')).toHaveAttribute("data-node-kind", "atomic");
    await expect(page.locator('[data-node-id="myModule"] [data-module-collapsed="true"]')).toBeVisible();
    await expect(page.locator('[data-node-id^="myModule::"]')).toHaveCount(0);
    await expect.poll(() => getGraphTransform(page)).toBe(transformBefore);
    await expect
      .poll(async () => (await waitForStableNodePosition(page, "networkInterface")).x)
      .toBeCloseTo(unrelatedBefore.x, 0);

    await page.getByRole("button", { name: "Expand module myModule" }).click();

    await expect(page.locator('[data-node-id="myModule"]')).toHaveAttribute("data-node-kind", "compound");
    await expect(page.locator('[data-node-id="myModule::storageAccount"]')).toBeVisible();
    await expect(page.locator('[data-node-id="myModule::vmResource"]')).toBeVisible();
    await expect.poll(() => getGraphTransform(page)).toBe(transformBefore);
    await expect
      .poll(async () => {
        const childrenAfter = await getNodeBoxes(page, ["myModule::storageAccount", "myModule::vmResource"]);
        if (!childrenBefore || !childrenAfter) {
          return false;
        }

        return Object.keys(childrenBefore).every((nodeId) => {
          const before = childrenBefore[nodeId];
          const after = childrenAfter[nodeId];
          return (
            before !== undefined &&
            after !== undefined &&
            Math.abs(before.x - after.x) <= 1 &&
            Math.abs(before.y - after.y) <= 1
          );
        });
      })
      .toBe(true);
  });

  test("resets layout while a module is collapsed and keeps its children inside it", async ({ page }) => {
    await loadSampleGraph(page, "module");
    await waitForStableNodePosition(page, "myModule::storageAccount");
    await page.getByRole("button", { name: "Collapse module myModule" }).click();
    await expect(page.locator('[data-node-id="myModule"] [data-module-collapsed="true"]')).toBeVisible();
    await dragNode(page, "myModule", 200, 0);

    await page.getByRole("button", { name: "Reset Layout" }).click();

    await expect(page.locator('[data-node-id="myModule"] [data-module-collapsed="true"]')).toBeVisible();
    await expect(page.locator('[data-node-id^="myModule::"]')).toHaveCount(0);
    await waitForStableNodePosition(page, "myModule");

    await page.getByRole("button", { name: "Expand module myModule" }).click();

    await expect
      .poll(async () => followsModuleServerLayout(await getNodeBoxes(page, MODULE_LAYOUT_NODES)), { timeout: 5_000 })
      .toBe(true);
  });

  test("moves a collapsed module's hidden children with it", async ({ page }) => {
    await loadSampleGraph(page, "module");
    await waitForStableNodePosition(page, "myModule::storageAccount");
    const childrenBefore = await getNodeBoxes(page, ["myModule::storageAccount", "myModule::vmResource"]);
    await page.getByRole("button", { name: "Collapse module myModule" }).click();
    await expect(page.locator('[data-node-id="myModule"] [data-module-collapsed="true"]')).toBeVisible();
    const moduleBefore = await waitForStableNodePosition(page, "myModule");

    await dragNode(page, "myModule", 150, 40);
    const moduleAfter = await waitForStableNodePosition(page, "myModule");
    await page.getByRole("button", { name: "Expand module myModule" }).click();

    await expect
      .poll(() => nodesAreAt(page, childrenBefore, moduleAfter.x - moduleBefore.x, moduleAfter.y - moduleBefore.y))
      .toBe(true);
  });

  test("undoes collapsing a module before what was done inside it, and redoes both", async ({ page }) => {
    const moduleNode = page.locator('[data-node-id="myModule"]');
    await loadSampleGraph(page, "module");
    await waitForStableNodePosition(page, "myModule::storageAccount");
    const storageBefore = await getNodeBoxes(page, ["myModule::storageAccount"]);
    await dragNode(page, "myModule::storageAccount", -160, 0);
    await waitForStableNodePosition(page, "myModule::storageAccount");
    const storageDragged = await getNodeBoxes(page, ["myModule::storageAccount"]);
    await expect.poll(() => nodesAreAt(page, storageBefore)).toBe(false);
    await page.getByRole("button", { name: "Collapse module myModule" }).click();
    await expect(page.locator('[data-node-id^="myModule::"]')).toHaveCount(0);
    await waitForStableNodePosition(page, "myModule");

    await clickHistoryButton(page, "Undo");
    await expect(moduleNode).toHaveAttribute("data-node-kind", "compound");
    await expect.poll(() => nodesAreAt(page, storageDragged)).toBe(true);

    await clickHistoryButton(page, "Undo");
    await expect.poll(() => nodesAreAt(page, storageBefore)).toBe(true);

    await clickHistoryButton(page, "Redo");
    await expect.poll(() => nodesAreAt(page, storageDragged)).toBe(true);

    await clickHistoryButton(page, "Redo");
    await expect(moduleNode).toHaveAttribute("data-node-kind", "atomic");
    await expect(page.locator('[data-node-id^="myModule::"]')).toHaveCount(0);
  });

  test("ignores a module toggle or node drag during Reset Layout", async ({ page }) => {
    await openVisualDesigner(page, { layoutDelay: "4000" });
    await loadSampleGraph(page, "module");
    await waitForStableNodePosition(page, "myModule::storageAccount");
    const reset = page.getByTestId("control-reset-layout");

    await reset.click();
    await expect(reset).toBeDisabled();

    await page.getByRole("button", { name: "Collapse module myModule" }).click();
    const unrelatedBefore = await waitForStableNodePosition(page, "networkInterface");
    await dragNode(page, "networkInterface", 200, 0);
    const unrelatedAfter = await page.locator('[data-node-id="networkInterface"]').boundingBox();
    // Both happened while the reset was still in flight.
    await expect(reset).toBeDisabled();
    await expect(page.locator('[data-node-id="myModule"]')).toHaveAttribute("data-node-kind", "compound");
    expect(unrelatedAfter?.x).toBeCloseTo(unrelatedBefore.x, 0);

    await expect(reset).toBeEnabled({ timeout: 15_000 });
    await page.getByRole("button", { name: "Collapse module myModule" }).click();
    await expect(page.locator('[data-node-id="myModule"] [data-module-collapsed="true"]')).toBeVisible();
  });

  test("preserves collapsed state across graph refreshes", async ({ page }) => {
    await loadSampleGraph(page, "module");
    await page.getByRole("button", { name: "Collapse module myModule" }).click();

    await page.getByTestId("dev-graph-module-graph").click();
    await page.waitForTimeout(350);

    await expect(page.locator('[data-node-id="myModule"] [data-module-collapsed="true"]')).toBeVisible();
    await expect(page.locator('[data-node-id^="myModule::"]')).toHaveCount(0);
  });

  test("paints edges below nodes, and a node's edges above a module it sits on", async ({ page }) => {
    await loadSampleGraph(page, "module");
    const zIndex = (selector: string) =>
      page.locator(selector).evaluate((element) => Number(getComputedStyle(element).zIndex) || 0);
    const minNodeZIndex = () =>
      page.evaluate(() =>
        Math.min(
          ...Array.from(document.querySelectorAll<HTMLElement>("[data-node-id]")).map(
            (element) => Number(getComputedStyle(element).zIndex) || 0,
          ),
        ),
      );
    const edge = '[data-edge-id="networkInterface->publicIp"]';
    const moduleNode = '[data-node-id="myModule"]';
    const moduleChildren = ['[data-node-id="myModule::vmResource"]', '[data-node-id="myModule::storageAccount"]'];

    // However nodes are ordered or raised, an edge between nodes that sit on nothing is below every node.
    await page.locator('[data-node-id="myModule::vmResource"]').click();
    await expect.poll(async () => (await zIndex(edge)) < (await minNodeZIndex())).toBe(true);
    await page.locator('[data-node-id="networkInterface"]').click();
    await expect.poll(async () => (await zIndex(edge)) < (await minNodeZIndex())).toBe(true);

    // An edge between a module's children is above the module and below its children.
    const innerEdge = '[data-edge-id="myModule::vmResource->myModule::storageAccount"]';
    await expect.poll(async () => (await zIndex(innerEdge)) > (await zIndex(moduleNode))).toBe(true);
    for (const child of moduleChildren) {
      await expect.poll(async () => (await zIndex(innerEdge)) < (await zIndex(child))).toBe(true);
    }

    // Dragged onto the module, publicIp sits on it: its edge is above the module and its children, and below
    // every other node.
    await waitForStableNodePosition(page, "publicIp");
    const moduleBox = (await page.locator(moduleNode).boundingBox())!;
    const publicIpBox = (await page.locator('[data-node-id="publicIp"]').boundingBox())!;
    const dx = moduleBox.x + moduleBox.width / 2 - (publicIpBox.x + publicIpBox.width / 2);
    const dy = moduleBox.y + moduleBox.height / 2 - (publicIpBox.y + 12);
    await dragNode(page, "publicIp", dx, dy);

    for (const below of [moduleNode, ...moduleChildren]) {
      await expect.poll(async () => (await zIndex(edge)) > (await zIndex(below))).toBe(true);
    }
    for (const above of ['[data-node-id="publicIp"]', '[data-node-id="networkInterface"]']) {
      await expect.poll(async () => (await zIndex(edge)) < (await zIndex(above))).toBe(true);
    }

    // Dragged off the module, it sits on nothing, and its edge goes back below every node.
    await waitForStableNodePosition(page, "publicIp");
    await dragNode(page, "publicIp", -dx, -dy);
    await expect.poll(async () => (await zIndex(edge)) < (await minNodeZIndex())).toBe(true);
  });

  test("paints a module the user opens above the neighbors it grew over", async ({ page }) => {
    await loadSampleGraph(page, "module");
    const zIndex = (nodeId: string) =>
      page.locator(`[data-node-id="${nodeId}"]`).evaluate((element) => Number(getComputedStyle(element).zIndex) || 0);
    const boxOf = async (nodeId: string) => (await page.locator(`[data-node-id="${nodeId}"]`).boundingBox())!;
    const overlap = (a: NodeBox, b: NodeBox) =>
      a.x < b.x + b.width && b.x < a.x + a.width && a.y < b.y + b.height && b.y < a.y + a.height;
    const moduleAndChildren = ["myModule", "myModule::vmResource", "myModule::storageAccount"];
    const isModuleAbovePublicIp = async () => {
      const below = await zIndex("publicIp");
      for (const nodeId of moduleAndChildren) {
        if ((await zIndex(nodeId)) <= below) {
          return false;
        }
      }
      return true;
    };

    await waitForStableNodePosition(page, "myModule::vmResource");
    const frame = await boxOf("myModule");
    await page.getByRole("button", { name: "Collapse module myModule" }).click();
    await expect(page.locator('[data-node-id="myModule"]')).toHaveAttribute("data-node-kind", "atomic");
    await waitForStableNodePosition(page, "myModule");
    const card = await boxOf("myModule");

    // publicIp moves beside the collapsed module, across the top edge of where its frame was.
    await waitForStableNodePosition(page, "publicIp");
    const before = await boxOf("publicIp");
    await dragNode(
      page,
      "publicIp",
      frame.x + frame.width / 2 - (before.x + before.width / 2),
      frame.y + 15 - (before.y + before.height / 2),
    );
    await waitForStableNodePosition(page, "publicIp");
    const publicIp = await boxOf("publicIp");
    expect(overlap(publicIp, frame)).toBe(true);
    expect(overlap(publicIp, card)).toBe(false);

    // Opened, the module grows over publicIp and comes to the front with its children.
    await page.getByRole("button", { name: "Expand module myModule" }).click();
    await expect(page.locator('[data-node-id="myModule"]')).toHaveAttribute("data-node-kind", "compound");
    await expect.poll(isModuleAbovePublicIp).toBe(true);

    // Focusing a node elsewhere leaves it in front.
    await page.locator('[data-node-id="networkInterface"]').click();
    await expect.poll(isModuleAbovePublicIp).toBe(true);
  });

  test("lays out a node whose size changed at its new size", async ({ page }) => {
    await loadSampleGraph(page, "flat");
    await waitForStableNodePosition(page, "vnet");

    // Becoming a collection changes a node's size without changing which nodes are shown.
    await page.getByRole("button", { name: "Toggle collection" }).click();
    for (const nodeId of FLAT_LAYOUT_ORDER) {
      await waitForStableNodePosition(page, nodeId);
    }
    const afterChange = await getNodeBoxes(page, FLAT_LAYOUT_ORDER);

    // Reset Layout lays out the same graph again, with every node certainly measured.
    await page.getByTestId("control-reset-layout").click();

    await expect.poll(() => nodesAreAt(page, afterChange)).toBe(true);
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

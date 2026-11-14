// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import { getGraphTransform, loadSampleGraph, openVisualDesigner, waitForStableNodePosition } from "./fixtures";

async function selectedNodeIds(page: Page) {
  return page
    .locator('[data-node-id]:has([data-selected="true"])')
    .evaluateAll((nodes) => nodes.map((node) => (node as HTMLElement).dataset.nodeId));
}

async function dragNode(page: Page, id: string, dx: number, dy: number) {
  const node = page.locator(`[data-node-id="${id}"]`);
  const box = (await node.boundingBox())!;
  const x = box.x + box.width / 2;
  const y = box.y + box.height / 2;
  await page.mouse.move(x, y);
  await page.mouse.down();
  await page.mouse.move(x + dx, y + dy, { steps: 10 });
  await page.mouse.up();
}

test.describe("Experimental navigation and selection", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "flat");
    await waitForStableNodePosition(page, "vnet");
  });

  test("Hand pans from a node without moving or selecting it; Select restores local dragging", async ({ page }) => {
    await page.locator('[data-node-id="subnet"]').click();
    const before = await waitForStableNodePosition(page, "vnet");
    const otherBefore = await waitForStableNodePosition(page, "subnet");
    const transform = await getGraphTransform(page);
    await page.keyboard.press("h");
    await expect(page.getByRole("button", { name: "Hand (H)" })).toHaveAttribute("aria-pressed", "true");
    await expect(page.locator('[data-node-id="vnet"]')).toHaveCSS("cursor", /data:image\/svg\+xml.*, grab$/);

    await dragNode(page, "vnet", 90, 40);

    await expect.poll(() => getGraphTransform(page)).not.toBe(transform);
    await expect.poll(() => selectedNodeIds(page)).toEqual(["subnet"]);
    const after = await waitForStableNodePosition(page, "vnet");
    const otherAfter = await waitForStableNodePosition(page, "subnet");
    expect(Math.abs(after.x - before.x - 90)).toBeLessThan(2);
    expect(Math.abs(after.y - before.y - 40)).toBeLessThan(2);
    expect(Math.abs(after.x - otherAfter.x - (before.x - otherBefore.x))).toBeLessThan(2);
    expect(Math.abs(after.y - otherAfter.y - (before.y - otherBefore.y))).toBeLessThan(2);
    await expect(page.getByTestId("history-bar").getByRole("button", { name: "Undo", exact: true })).toBeDisabled();

    await page.locator('[data-node-id="vnet"]').click();
    await page.getByTestId("graph-canvas").click({ position: { x: 8, y: 8 } });
    await expect.poll(() => selectedNodeIds(page)).toEqual(["subnet"]);

    await page.keyboard.press("v");
    await expect(page.getByRole("button", { name: "Select (V)" })).toHaveAttribute("aria-pressed", "true");
    const pannedTransform = await getGraphTransform(page);
    await dragNode(page, "vnet", 40, 20);
    await expect.poll(() => getGraphTransform(page)).toBe(pannedTransform);
    await expect.poll(() => selectedNodeIds(page)).toEqual(["subnet"]);
    const dragged = await waitForStableNodePosition(page, "vnet");
    expect(Math.abs(dragged.x - after.x - 40)).toBeLessThan(2);
    expect(Math.abs(dragged.y - after.y - 20)).toBeLessThan(2);
  });

  test("V/H ignore typing, modified shortcuts, and composition", async ({ page }) => {
    await page.getByRole("button", { name: "Add Resources" }).click();
    const search = page.getByRole("textbox", { name: "Filter resource types" });
    await search.fill("hv");
    await search.press("h");
    await expect(search).toHaveValue("hvh");
    await expect(page.getByRole("button", { name: "Select (V)" })).toHaveAttribute("aria-pressed", "true");

    await page.keyboard.press("Escape");
    await page.getByTestId("graph-canvas").click({ position: { x: 8, y: 8 } });
    await page.keyboard.press("Control+h");
    await page.keyboard.press("Alt+h");
    await page.keyboard.press("Shift+h");
    await page.getByTestId("graph-canvas").dispatchEvent("keydown", { key: "h", isComposing: true });
    await expect(page.getByRole("button", { name: "Select (V)" })).toHaveAttribute("aria-pressed", "true");

    await page.keyboard.press("h");
    await expect(page.getByRole("button", { name: "Hand (H)" })).toHaveAttribute("aria-pressed", "true");
    await page.getByRole("button", { name: "Select (V)" }).click();
    await expect(page.getByRole("button", { name: "Select (V)" })).toHaveAttribute("aria-pressed", "true");
  });

  test("a click selects one node, and Shift or Ctrl+click adds or removes nodes", async ({ page }) => {
    await page.locator('[data-node-id="vnet"]').click();
    await expect.poll(() => selectedNodeIds(page)).toEqual(["vnet"]);
    await page.locator('[data-node-id="subnet"]').click({ modifiers: ["Shift"] });
    await page.locator('[data-node-id="nsg"]').click({ modifiers: ["ControlOrMeta"] });
    await expect.poll(async () => (await selectedNodeIds(page)).sort()).toEqual(["nsg", "subnet", "vnet"]);

    await page.locator('[data-node-id="subnet"]').click({ modifiers: ["Shift"] });
    await expect.poll(async () => (await selectedNodeIds(page)).sort()).toEqual(["nsg", "vnet"]);
    // A modified click on the background keeps the selection; a plain one clears it.
    await page.getByTestId("graph-canvas").click({ position: { x: 8, y: 8 }, modifiers: ["Shift"] });
    await expect.poll(async () => (await selectedNodeIds(page)).sort()).toEqual(["nsg", "vnet"]);
    await page.locator('[data-node-id="pip"]').click();
    await expect.poll(() => selectedNodeIds(page)).toEqual(["pip"]);
    await page.getByTestId("graph-canvas").click({ position: { x: 8, y: 8 } });
    await expect.poll(() => selectedNodeIds(page)).toEqual([]);
  });

  test("dragging on the background draws a marquee that selects the nodes it touches, without panning", async ({
    page,
  }) => {
    const transform = await getGraphTransform(page);
    const pip = (await page.locator('[data-node-id="pip"]').boundingBox())!;
    const subnet = (await page.locator('[data-node-id="subnet"]').boundingBox())!;
    const start = { x: pip.x - 60, y: pip.y + 10 };

    await page.mouse.move(start.x, start.y);
    await page.mouse.down();
    await page.mouse.move(pip.x + pip.width + 40, subnet.y + 10, { steps: 10 });
    await expect(page.getByTestId("selection-marquee")).toBeVisible();
    await expect.poll(async () => (await selectedNodeIds(page)).sort()).toEqual(["pip", "subnet"]);
    await page.mouse.up();

    await expect(page.getByTestId("selection-marquee")).toBeHidden();
    expect(await page.evaluate(() => window.getSelection()?.toString() ?? "")).toBe("");
    expect(await getGraphTransform(page)).toBe(transform);
    await expect.poll(async () => (await selectedNodeIds(page)).sort()).toEqual(["pip", "subnet"]);

    // A Shift+marquee adds to the selection.
    const vnet = (await page.locator('[data-node-id="vnet"]').boundingBox())!;
    await page.keyboard.down("Shift");
    await page.mouse.move(vnet.x - 60, vnet.y + 10);
    await page.mouse.down();
    await page.mouse.move(vnet.x + 20, vnet.y + 20, { steps: 5 });
    await page.mouse.up();
    await page.keyboard.up("Shift");
    await expect.poll(async () => (await selectedNodeIds(page)).sort()).toEqual(["pip", "subnet", "vnet"]);
  });

  test("a marquee never selects text, even when it reaches past the graph", async ({ page }) => {
    const pip = (await page.locator('[data-node-id="pip"]').boundingBox())!;
    const scope = (await page.getByTestId("target-scope").boundingBox())!;

    await page.mouse.move(pip.x - 60, pip.y + 10);
    await page.mouse.down();
    await page.mouse.move(scope.x + 5, scope.y + 5, { steps: 15 });
    expect(await page.evaluate(() => window.getSelection()?.toString() ?? "")).toBe("");
    await page.mouse.up();
    expect(await page.evaluate(() => window.getSelection()?.toString() ?? "")).toBe("");
  });

  test("a marquee around a module selects it and its nodes, which then move together once", async ({ page }) => {
    await loadSampleGraph(page, "module");
    await waitForStableNodePosition(page, "myModule::vmResource");
    const module = (await page.locator('[data-node-id="myModule"]').boundingBox())!;
    const vm = await waitForStableNodePosition(page, "myModule::vmResource");

    await page.mouse.move(module.x - 30, module.y - 30);
    await page.mouse.down();
    await page.mouse.move(module.x + module.width + 10, module.y + module.height + 10, { steps: 10 });
    await page.mouse.up();
    await expect
      .poll(async () => (await selectedNodeIds(page)).filter((id) => id?.startsWith("myModule")).sort())
      .toEqual(["myModule", "myModule::storageAccount", "myModule::vmResource"]);

    await dragNode(page, "myModule::vmResource", 60, 0);
    const moved = await waitForStableNodePosition(page, "myModule::vmResource");
    expect(Math.abs(moved.x - vm.x - 60)).toBeLessThan(2);
  });

  for (const modifier of ["Alt", "Control"] as const) {
    test(`${modifier}+dragging inside a module's frame selects its nodes without moving the module`, async ({
      page,
    }) => {
      await loadSampleGraph(page, "module");
      await waitForStableNodePosition(page, "myModule::vmResource");
      const before = await waitForStableNodePosition(page, "myModule");
      const module = (await page.locator('[data-node-id="myModule"]').boundingBox())!;
      const start = { x: module.x + 8, y: module.y + 52 };

      await page.keyboard.down(modifier);
      await page.mouse.move(start.x, start.y);
      await page.mouse.down();
      await page.mouse.move(module.x + module.width - 8, module.y + module.height - 8, { steps: 10 });
      await expect(page.getByTestId("selection-marquee")).toBeVisible();
      await page.mouse.up();
      await page.keyboard.up(modifier);

      await expect
        .poll(async () => (await selectedNodeIds(page)).sort())
        .toEqual(["myModule::storageAccount", "myModule::vmResource"]);
      expect(await waitForStableNodePosition(page, "myModule")).toEqual(before);

      // Without the modifier, the same press drags the module.
      await page.mouse.move(start.x, start.y);
      await page.mouse.down();
      await page.mouse.move(start.x + 50, start.y, { steps: 5 });
      await page.mouse.up();
      const moved = await waitForStableNodePosition(page, "myModule");
      expect(Math.abs(moved.x - before.x - 50)).toBeLessThan(2);
    });
  }

  test("holding Space pans from anywhere until it is released, but not while a button has focus", async ({ page }) => {
    const hand = page.getByRole("button", { name: "Hand (H)" });
    const before = await waitForStableNodePosition(page, "vnet");
    const transform = await getGraphTransform(page);
    await page.getByTestId("graph-canvas").click({ position: { x: 8, y: 8 } });

    await page.keyboard.down(" ");
    await expect(hand).toHaveAttribute("aria-pressed", "true");
    await dragNode(page, "vnet", 70, 30);
    await page.keyboard.up(" ");

    await expect(page.getByRole("button", { name: "Select (V)" })).toHaveAttribute("aria-pressed", "true");
    await expect.poll(() => getGraphTransform(page)).not.toBe(transform);
    const after = await waitForStableNodePosition(page, "vnet");
    await expect.poll(() => selectedNodeIds(page)).toEqual([]);
    // The canvas panned under the pointer; the history has no node move.
    expect(Math.abs(after.x - before.x - 70)).toBeLessThan(2);
    await expect(page.getByTestId("history-bar").getByRole("button", { name: "Undo", exact: true })).toBeDisabled();

    await page.getByRole("button", { name: "Add Resources" }).focus();
    await page.keyboard.down(" ");
    await expect(hand).toHaveAttribute("aria-pressed", "false");
    await page.keyboard.up(" ");
  });

  test("dragging one of several selected nodes moves them together as one undo step", async ({ page }) => {
    await page.locator('[data-node-id="vnet"]').click();
    await page.locator('[data-node-id="subnet"]').click({ modifiers: ["Shift"] });
    const vnet = await waitForStableNodePosition(page, "vnet");
    const subnet = await waitForStableNodePosition(page, "subnet");
    const nsg = await waitForStableNodePosition(page, "nsg");

    await dragNode(page, "vnet", 80, 30);

    const movedVnet = await waitForStableNodePosition(page, "vnet");
    const movedSubnet = await waitForStableNodePosition(page, "subnet");
    expect(Math.abs(movedVnet.x - vnet.x - 80)).toBeLessThan(2);
    expect(Math.abs(movedSubnet.x - subnet.x - 80)).toBeLessThan(2);
    expect(Math.abs(movedSubnet.y - subnet.y - 30)).toBeLessThan(2);
    expect(await waitForStableNodePosition(page, "nsg")).toEqual(nsg);

    await page.keyboard.press("ControlOrMeta+z");
    await expect
      .poll(async () => Math.round((await waitForStableNodePosition(page, "subnet")).x))
      .toBe(Math.round(subnet.x));
    expect(Math.round((await waitForStableNodePosition(page, "vnet")).x)).toBe(Math.round(vnet.x));
  });

  test("a right drag pans in either tool, without moving nodes or opening a menu", async ({ page }) => {
    await page.locator('[data-node-id="subnet"]').click();
    for (const tool of ["Select (V)", "Hand (H)"]) {
      await page.getByRole("button", { name: tool }).click();
      const transform = await getGraphTransform(page);
      const before = await waitForStableNodePosition(page, "vnet");
      const vnet = (await page.locator('[data-node-id="vnet"]').boundingBox())!;
      const start = { x: vnet.x + vnet.width / 2, y: vnet.y + vnet.height / 2 };

      await page.mouse.move(start.x, start.y);
      await page.mouse.down({ button: "right" });
      await page.mouse.move(start.x + 70, start.y + 30, { steps: 10 });
      await page.mouse.up({ button: "right" });

      await expect.poll(() => getGraphTransform(page)).not.toBe(transform);
      const after = await waitForStableNodePosition(page, "vnet");
      expect(Math.abs(after.x - before.x - 70)).toBeLessThan(2);
      await expect(page.getByRole("menu")).toHaveCount(0);
      await expect.poll(() => selectedNodeIds(page)).toEqual(["subnet"]);
    }
  });

  test("the select tool's cursor is a custom arrow, and the active tool is not accented", async ({ page }) => {
    const cursor = await page.getByTestId("graph-canvas").evaluate((element) => getComputedStyle(element).cursor);
    expect(cursor).toContain("data:image/svg+xml");
    const select = page.getByRole("button", { name: "Select (V)" });
    const hand = page.getByRole("button", { name: "Hand (H)" });
    expect(await select.evaluate((element) => getComputedStyle(element).color)).toBe(
      await hand.evaluate((element) => getComputedStyle(element).color),
    );
    await expect(page.getByTestId("node-toolbar")).toHaveCount(0);
  });

  test("the select tool shows no grab indicator; a pan shows the closed-hand cursor", async ({ page }) => {
    const canvas = page.getByTestId("graph-canvas");
    const indicatorShown = () =>
      canvas.evaluate((element) =>
        Array.from(element.children).some(
          (child) =>
            child instanceof HTMLElement &&
            child.style.display === "block" &&
            getComputedStyle(child).borderRadius === "50%",
        ),
      );
    const vnet = (await page.locator('[data-node-id="vnet"]').boundingBox())!;
    const center = { x: vnet.x + vnet.width / 2, y: vnet.y + vnet.height / 2 };

    // Pressing and dragging a node with the select tool grabs nothing.
    await page.mouse.move(center.x, center.y);
    await page.mouse.down();
    await page.mouse.move(center.x + 30, center.y, { steps: 5 });
    await expect(canvas).not.toHaveClass(/\bgrabbing\b/);
    expect(await indicatorShown()).toBe(false);
    await page.mouse.up();

    // A right drag pans with the closed hand rather than the indicator.
    const empty = { x: vnet.x - 120, y: vnet.y };
    await page.mouse.move(empty.x, empty.y);
    await page.mouse.down({ button: "right" });
    await page.mouse.move(empty.x + 40, empty.y + 10, { steps: 5 });
    await expect(canvas).toHaveClass(/\bgrabbing\b/);
    expect(await canvas.evaluate((element) => getComputedStyle(element).cursor)).toMatch(
      /data:image\/svg\+xml.*, grabbing$/,
    );
    expect(await indicatorShown()).toBe(false);
    await page.mouse.up({ button: "right" });
    await expect(canvas).not.toHaveClass(/\bgrabbing\b/);
  });

  test("right-click targets its node and the menu supports keyboard invocation and dismissal", async ({ page }) => {
    const reveals: string[] = [];
    page.on("console", (message) => {
      if (message.text().includes("[FakeMessageChannel] revealNode")) reveals.push(message.text());
    });
    await page.locator('[data-node-id="subnet"]').click();
    await page.locator('[data-node-id="vnet"]').click({ button: "right" });
    const menu = page.getByRole("menu", { name: "Actions for vnet" });
    await expect(menu).toBeVisible();
    await expect(menu.getByRole("menuitem", { name: "Go to Source" })).toBeFocused();
    await expect.poll(() => selectedNodeIds(page)).toEqual(["vnet"]);
    await page.keyboard.press("Escape");
    await expect(menu).toHaveCount(0);
    await expect.poll(() => selectedNodeIds(page)).toEqual(["vnet"]);
    await expect(page.getByRole("region", { name: "Visual designer canvas" })).toBeFocused();

    await page.keyboard.press("Shift+F10");
    await expect(menu).toBeVisible();
    await page.keyboard.press("ArrowDown");
    await page.keyboard.press("Enter");
    await expect.poll(() => reveals.at(-1)).toContain("vnet");
    await expect(menu).toHaveCount(0);

    await page.keyboard.press("ContextMenu");
    await expect(menu).toBeVisible();
    await page.getByTestId("control-zoom-in").click();
    await expect(menu).toHaveCount(0);

    await page.getByRole("region", { name: "Visual designer canvas" }).focus();
    await page.keyboard.press("Escape");
    await expect.poll(() => selectedNodeIds(page)).toEqual([]);
  });

  test("no context menu opens while several nodes are selected", async ({ page }) => {
    await page.locator('[data-node-id="vnet"]').click();
    await page.locator('[data-node-id="subnet"]').click({ modifiers: ["Shift"] });
    await page.locator('[data-node-id="vnet"]').click({ button: "right" });
    await expect(page.getByRole("menu")).toHaveCount(0);
    await expect.poll(async () => (await selectedNodeIds(page)).sort()).toEqual(["subnet", "vnet"]);
    await page.getByRole("region", { name: "Visual designer canvas" }).focus();
    await page.keyboard.press("Shift+F10");
    await expect(page.getByRole("menu")).toHaveCount(0);

    // Right-clicking a node outside the selection selects it alone and offers its actions.
    await page.locator('[data-node-id="nsg"]').click({ button: "right" });
    await expect(page.getByRole("menu", { name: "Actions for nsg" })).toBeVisible();
    await expect.poll(() => selectedNodeIds(page)).toEqual(["nsg"]);
  });

  test("selection follows a collapsing container and disappears when its node leaves the graph", async ({ page }) => {
    await loadSampleGraph(page, "module");
    await page.locator('[data-node-id="myModule::vmResource"]').click();
    await expect.poll(() => selectedNodeIds(page)).toEqual(["myModule::vmResource"]);
    await page.getByRole("button", { name: "Collapse module myModule", exact: true }).click();
    await expect.poll(() => selectedNodeIds(page)).toEqual(["myModule"]);

    await loadSampleGraph(page, "flat");
    await expect.poll(() => selectedNodeIds(page)).toEqual([]);
  });

  test("with resource editing off, only the Resources tool goes away", async ({ page }) => {
    await openVisualDesigner(page, { resourceEditing: "false" });
    await loadSampleGraph(page, "flat");
    await expect(page.getByRole("button", { name: "Select (V)" })).toBeVisible();
    await expect(page.getByRole("button", { name: "Add Resources" })).toHaveCount(0);

    await page.locator('[data-node-id="vnet"]').click();
    await page.locator('[data-node-id="subnet"]').click({ modifiers: ["Shift"] });
    await expect.poll(async () => (await selectedNodeIds(page)).sort()).toEqual(["subnet", "vnet"]);
    await page.locator('[data-node-id="nsg"]').click({ button: "right" });
    await expect(page.getByRole("menu", { name: "Actions for nsg" })).toBeVisible();
    await page.keyboard.press("Escape");

    const transform = await getGraphTransform(page);
    await page.keyboard.press("h");
    await expect(page.getByRole("button", { name: "Hand (H)" })).toHaveAttribute("aria-pressed", "true");
    await dragNode(page, "vnet", 60, 20);
    await expect.poll(() => getGraphTransform(page)).not.toBe(transform);
  });
});

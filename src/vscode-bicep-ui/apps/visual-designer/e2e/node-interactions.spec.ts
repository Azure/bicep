// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import { loadSampleGraph, nodeCount, openVisualDesigner } from "./fixtures";

test.describe("Node interactions", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "flat");
  });

  test("clicking a node focuses it", async ({ page }) => {
    const target = page.locator('[data-node-id="vnet"]');
    await expect(target).toBeVisible();

    await target.click();

    // The focused node sits at the highest z-index in the layer.
    // The atom drives a re-render that boosts that node's stacking
    // order — we assert via the underlying state by re-querying that
    // the focused node id was set on the canvas.
    const focusedId = await page.evaluate(() => {
      const all = Array.from(document.querySelectorAll<HTMLElement>("[data-node-id]"));
      const sorted = all
        .map((el) => ({ id: el.dataset.nodeId ?? "", z: Number(getComputedStyle(el).zIndex) || 0 }))
        .sort((a, b) => b.z - a.z);
      return sorted[0]?.id ?? null;
    });
    expect(focusedId).toBe("vnet");
  });

  test("clicking blank canvas clears node focus", async ({ page }) => {
    const target = page.locator('[data-node-id="subnet"]');
    await target.click();

    const canvas = page.getByTestId("graph-canvas");
    const box = await canvas.boundingBox();
    expect(box).not.toBeNull();
    // Click far away from any node — top-left corner is usually empty.
    await canvas.click({ position: { x: 10, y: 10 } });

    // After clearing focus, all nodes should share the same baseline
    // z-index (no single node elevated above the rest).
    const elevated = await page.evaluate(() => {
      const all = Array.from(document.querySelectorAll<HTMLElement>("[data-node-id]"));
      const zs = all.map((el) => Number(getComputedStyle(el).zIndex) || 0);
      const max = Math.max(...zs);
      return zs.filter((z) => z === max).length;
    });
    expect(elevated).toBeGreaterThan(1);
  });

  test("double-clicking a node sends a reveal-node-source notification", async ({ page }) => {
    // The FakeMessageChannel logs reveal notifications to the console;
    // sniff that channel as a proxy for the outgoing message.
    const reveals: string[] = [];
    page.on("console", (msg) => {
      if (msg.type() === "log" && msg.text().includes("revealNodeSource")) {
        reveals.push(msg.text());
      }
    });

    await page.locator('[data-node-id="nsg"]').dblclick();

    await expect.poll(() => reveals.length, { timeout: 5_000 }).toBeGreaterThan(0);
    expect(reveals[0]).toContain("nsg");
  });

  test("a graph swap survives multiple updates without losing nodes", async ({ page }) => {
    await loadSampleGraph(page, "module");
    const moduleCount = await nodeCount(page);
    expect(moduleCount).toBeGreaterThan(0);

    await loadSampleGraph(page, "flat");
    await expect(page.getByTestId("graph-node")).toHaveCount(4);
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import {
  getGraphTransform,
  loadSampleGraph,
  nodeCount,
  openVisualDesigner,
  waitForStableNodePosition,
} from "./fixtures";

async function focusedNodeId(page: Page): Promise<string | null> {
  return page.evaluate(() => {
    const nodes = Array.from(document.querySelectorAll<HTMLElement>("[data-node-id]"))
      .map((element) => ({ id: element.dataset.nodeId ?? "", z: Number(getComputedStyle(element).zIndex) || 0 }))
      .sort((a, b) => b.z - a.z);
    const [top, next] = nodes;

    return top && (!next || top.z > next.z) ? top.id : null;
  });
}

test.describe("Node interactions", () => {
  test.beforeEach(async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "flat");
  });

  test("clicking a node focuses it", async ({ page }) => {
    const target = page.locator('[data-node-id="vnet"]');
    await expect(target).toBeVisible();

    await target.click();

    await expect.poll(() => focusedNodeId(page)).toBe("vnet");
  });

  test("a click with less than 4px of pointer movement still focuses a node", async ({ page }) => {
    const target = page.locator('[data-node-id="vnet"]');
    await waitForStableNodePosition(page, "vnet");
    const box = (await target.boundingBox())!;
    const start = { x: box.x + box.width / 2, y: box.y + box.height / 2 };

    await page.mouse.move(start.x, start.y);
    await page.mouse.down();
    await page.mouse.move(start.x + 3, start.y + 1, { steps: 3 });
    await page.mouse.up();

    await expect.poll(() => focusedNodeId(page)).toBe("vnet");
  });

  test("diagonal travel beyond 4px does not focus another node", async ({ page }) => {
    await page.locator('[data-node-id="subnet"]').click();
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
    const target = page.locator('[data-node-id="vnet"]');
    await waitForStableNodePosition(page, "vnet");
    const box = (await target.boundingBox())!;
    const start = { x: box.x + box.width / 2, y: box.y + box.height / 2 };

    await page.mouse.move(start.x, start.y);
    await page.mouse.down();
    await page.mouse.move(start.x + 3, start.y + 3, { steps: 3 });
    await page.mouse.up();

    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
  });

  test("travel of exactly 4px is a drag rather than a click", async ({ page }) => {
    await page.locator('[data-node-id="subnet"]').click();
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
    const target = page.locator('[data-node-id="vnet"]');
    await waitForStableNodePosition(page, "vnet");
    const box = (await target.boundingBox())!;
    const start = { x: box.x + box.width / 2, y: box.y + box.height / 2 };

    await page.mouse.move(start.x, start.y);
    await page.mouse.down();
    await page.mouse.move(start.x + 4, start.y, { steps: 4 });
    await page.mouse.up();

    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
  });

  test("clicking blank canvas clears node focus", async ({ page }) => {
    const target = page.locator('[data-node-id="subnet"]');
    await target.click();

    const canvas = page.getByTestId("graph-canvas");
    const box = await canvas.boundingBox();
    expect(box).not.toBeNull();
    // Click far away from any node — top-left corner is usually empty.
    await canvas.click({ position: { x: 10, y: 10 } });

    await expect.poll(() => focusedNodeId(page)).toBeNull();
  });

  test("right-clicking another node does not change focus", async ({ page }) => {
    await page.locator('[data-node-id="subnet"]').click();
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");

    await page.locator('[data-node-id="vnet"]').click({ button: "right" });

    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
  });

  test("slow multi-event panning does not clear focus", async ({ page }) => {
    await page.locator('[data-node-id="subnet"]').click();
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
    await waitForStableNodePosition(page, "subnet");

    const canvas = page.getByTestId("graph-canvas");
    const box = (await canvas.boundingBox())!;
    const start = { x: box.x + 40, y: box.y + 40 };
    const transform = await getGraphTransform(page);

    await page.mouse.move(start.x, start.y);
    await page.mouse.down();
    await expect(canvas).toHaveClass(/\bgrabbing\b/);
    for (let step = 1; step <= 10; step++) {
      await page.mouse.move(start.x + step, start.y);
      await page.waitForTimeout(16);
    }
    await page.mouse.up();

    await expect.poll(() => getGraphTransform(page)).not.toBe(transform);
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
    await expect(canvas).not.toHaveClass(/\bgrabbing\b/);
  });

  test("slow multi-event node dragging does not steal focus", async ({ page }) => {
    await page.locator('[data-node-id="subnet"]').click();
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
    const original = await waitForStableNodePosition(page, "vnet");

    const target = page.locator('[data-node-id="vnet"]');
    const box = (await target.boundingBox())!;
    const start = { x: box.x + box.width / 2, y: box.y + box.height / 2 };

    await page.mouse.move(start.x, start.y);
    await page.mouse.down();
    for (let step = 1; step <= 10; step++) {
      await page.mouse.move(start.x + step, start.y);
      await page.waitForTimeout(16);
    }
    await page.mouse.up();

    await expect.poll(async () => (await target.boundingBox())?.x).toBeGreaterThan(original.x + 4);
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
  });

  test("pointercancel discards pending focus and allows the next click", async ({ page }) => {
    await page.locator('[data-node-id="subnet"]').click();
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");
    await waitForStableNodePosition(page, "vnet");

    const box = (await page.locator('[data-node-id="vnet"]').boundingBox())!;
    const x = Math.round(box.x + box.width / 2);
    const y = Math.round(box.y + box.height / 2);
    const canvas = page.getByTestId("graph-canvas");
    const cdp = await page.context().newCDPSession(page);

    // CDP touchCancel produces a browser pointercancel, unlike dispatching a synthetic event.
    await cdp.send("Input.dispatchTouchEvent", { type: "touchStart", touchPoints: [{ x, y, id: 1 }] });
    await expect(canvas).toHaveClass(/\bgrabbing\b/);
    await cdp.send("Input.dispatchTouchEvent", { type: "touchCancel", touchPoints: [] });

    await expect(canvas).not.toHaveClass(/\bgrabbing\b/);
    await expect.poll(() => focusedNodeId(page)).toBe("subnet");

    await page.locator('[data-node-id="nsg"]').click();
    await expect.poll(() => focusedNodeId(page)).toBe("nsg");
  });

  test("double-clicking a node reveals its source without bubbling to pan-zoom", async ({ page }) => {
    // The FakeMessageChannel logs reveal notifications to the console;
    // sniff that channel as a proxy for the outgoing message.
    const reveals: string[] = [];
    page.on("console", (msg) => {
      if (msg.type() === "log" && msg.text().includes("revealNodeSource")) {
        reveals.push(msg.text());
      }
    });

    const canvas = page.getByTestId("graph-canvas");
    await canvas.evaluate((element) => {
      element.addEventListener(
        "dblclick",
        () => {
          element.setAttribute("data-node-double-click-bubbled", "true");
        },
        { once: true },
      );
    });

    await page.locator('[data-node-id="nsg"]').dblclick();

    await expect.poll(() => reveals.length, { timeout: 5_000 }).toBeGreaterThan(0);
    expect(reveals[0]).toContain("nsg");
    await expect(canvas).not.toHaveAttribute("data-node-double-click-bubbled");
  });

  test("a graph swap survives multiple updates without losing nodes", async ({ page }) => {
    await loadSampleGraph(page, "module");
    const moduleCount = await nodeCount(page);
    expect(moduleCount).toBeGreaterThan(0);

    await loadSampleGraph(page, "flat");
    await expect(page.getByTestId("graph-node")).toHaveCount(4);
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import { getGraphTransform, loadSampleGraph, openVisualDesigner, waitForStableNodePosition } from "./fixtures";

function getGraphLayerOpacity(page: Page) {
  return page.locator("[data-export-graph]").evaluate((layer) => getComputedStyle(layer).opacity);
}

async function getCenter(page: Page, nodeId: string) {
  const box = await page.locator(`[data-node-id="${nodeId}"]`).boundingBox();
  return box && { x: box.x + box.width / 2, y: box.y + box.height / 2 };
}

/** Where the remote sample stands before its nested registry module `storage::leaf` expands. */
async function captureBeforeExpandingLeaf(page: Page) {
  await expect.poll(() => getGraphLayerOpacity(page)).toBe("1");
  const plain = await waitForStableNodePosition(page, "plain");
  await waitForStableNodePosition(page, "storage::leaf");

  return { plain, collapsedCenter: await getCenter(page, "storage::leaf"), transform: await getGraphTransform(page) };
}

/** `storage::leaf` became a container and its restored contents are not left hidden. */
async function expectLeafContentsShown(page: Page) {
  await expect(page.locator('[data-node-id="storage::leaf"]')).toHaveAttribute("data-node-kind", "compound");
  await expect(page.locator('[data-node-id="storage::leaf::thing"]')).toHaveCSS("opacity", "1");
  await expect(page.locator('[data-node-id="storage::leaf::lock"]')).toHaveCSS("opacity", "1");
  await expect.poll(() => getGraphLayerOpacity(page)).toBe("1");
}

/** Only the module was laid out: it grew around where it was, and the rest of the graph and the camera stayed put. */
async function expectLeafExpandedInPlace(page: Page, before: Awaited<ReturnType<typeof captureBeforeExpandingLeaf>>) {
  await expectLeafContentsShown(page);
  await expect(page.getByRole("button", { name: "Collapse module leaf" })).toBeAttached();

  expect(await getGraphTransform(page)).toBe(before.transform);
  const plainAfter = await waitForStableNodePosition(page, "plain");
  expect(plainAfter.x).toBeCloseTo(before.plain.x, 0);
  expect(plainAfter.y).toBeCloseTo(before.plain.y, 0);
  const anchor = before.collapsedCenter!;
  await expect
    .poll(async () => {
      const center = await getCenter(page, "storage::leaf");
      return center ? Math.hypot(center.x - anchor.x, center.y - anchor.y) : Infinity;
    })
    .toBeLessThanOrEqual(2);
}

test.describe("Remote modules", () => {
  test("offers no toggle for a module whose contents can't be shown", async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "remote");

    await expect(page.locator('[data-node-id="plain"] button')).toHaveCount(0);
  });

  test("restores a nested registry module and expands it in place", async ({ page }) => {
    await openVisualDesigner(page, { restoreDelay: "800" });
    await loadSampleGraph(page, "remote");
    const before = await captureBeforeExpandingLeaf(page);

    await page.getByRole("button", { name: "Expand module leaf" }).click();

    await expect(page.getByRole("button", { name: "Restoring module leaf" })).toHaveAttribute("aria-busy", "true");
    await expectLeafExpandedInPlace(page, before);
  });

  test("lays a restored module out again, still in place, when the host reports the graph changed", async ({
    page,
  }) => {
    await openVisualDesigner(page, { moduleLayout: "graphChanged" });
    await loadSampleGraph(page, "remote");
    const before = await captureBeforeExpandingLeaf(page);

    await page.getByRole("button", { name: "Expand module leaf" }).click();

    await expectLeafExpandedInPlace(page, before);
  });

  for (const motionPolicy of ["animate", "reduce", "system"]) {
    test(`lays out the whole graph after module layout failure (${motionPolicy} motion)`, async ({ page }) => {
      await page.emulateMedia({ reducedMotion: "reduce" });
      await openVisualDesigner(page, { moduleLayout: "layoutFailed", motionPolicy, layoutDelay: "500" });
      await loadSampleGraph(page, "remote");
      await waitForStableNodePosition(page, "storage::leaf");

      await page.getByRole("button", { name: "Expand module leaf" }).click();

      // The contents are not left hidden: they show, and the layout of the whole graph spreads them out.
      await expectLeafContentsShown(page);
      const thing = await waitForStableNodePosition(page, "storage::leaf::thing");
      const lock = await waitForStableNodePosition(page, "storage::leaf::lock");
      expect(Math.hypot(thing.x - lock.x, thing.y - lock.y)).toBeGreaterThan(20);
    });
  }

  test("defers restored contents' layout while their parent is collapsed", async ({ page }) => {
    await openVisualDesigner(page, { restoreDelay: "1200", motionPolicy: "reduce" });
    await loadSampleGraph(page, "remote");
    const before = await captureBeforeExpandingLeaf(page);
    const accountBefore = await waitForStableNodePosition(page, "storage::account");

    await page.getByRole("button", { name: "Expand module leaf", exact: true }).click();
    await page.getByRole("button", { name: "Collapse module storage", exact: true }).click();
    await expect(page.locator('[data-node-id="storage::leaf"]')).toHaveCount(0);
    await page.waitForTimeout(1600);
    await page.getByRole("button", { name: "Expand module storage", exact: true }).click();

    await expectLeafExpandedInPlace(page, before);
    const accountAfter = await waitForStableNodePosition(page, "storage::account");
    expect(accountAfter.x).toBeCloseTo(accountBefore.x, 0);
    expect(accountAfter.y).toBeCloseTo(accountBefore.y, 0);
    const thing = await waitForStableNodePosition(page, "storage::leaf::thing");
    const lock = await waitForStableNodePosition(page, "storage::leaf::lock");
    expect(Math.hypot(thing.x - lock.x, thing.y - lock.y)).toBeGreaterThan(20);
  });

  test("blocks gestures during placement but not during the network restore", async ({ page }) => {
    await openVisualDesigner(page, { restoreDelay: "1000", layoutDelay: "1200", motionPolicy: "reduce" });
    await loadSampleGraph(page, "remote");
    const before = await captureBeforeExpandingLeaf(page);
    const accountBefore = await waitForStableNodePosition(page, "storage::account");
    const canvas = page.getByRole("region", { name: "Visual designer canvas" });

    await page.getByRole("button", { name: "Expand module leaf", exact: true }).click();
    await expect(page.getByRole("button", { name: "Restoring module leaf" })).toBeAttached();
    await expect(canvas).toHaveAttribute("aria-busy", "false");
    await expect(canvas).toHaveAttribute("aria-busy", "true");
    const parent = await page.locator('[data-node-id="storage"]').boundingBox();
    expect(parent).not.toBeNull();
    await page.mouse.move(parent!.x + 80, parent!.y + 25);
    await page.mouse.down();
    await page.mouse.move(parent!.x + 230, parent!.y - 75, { steps: 5 });
    await page.mouse.up();

    await expectLeafExpandedInPlace(page, before);
    await expect(canvas).toHaveAttribute("aria-busy", "false");
    const accountAfter = await waitForStableNodePosition(page, "storage::account");
    expect(accountAfter.x).toBeCloseTo(accountBefore.x, 0);
    expect(accountAfter.y).toBeCloseTo(accountBefore.y, 0);
  });

  test("reports a failed refresh and retries without restoring the module again", async ({ page }) => {
    await openVisualDesigner(page, { restoreRefreshFailures: "1", motionPolicy: "reduce" });
    await loadSampleGraph(page, "remote");
    const before = await captureBeforeExpandingLeaf(page);

    await page.getByRole("button", { name: "Expand module leaf", exact: true }).click();
    const retry = page.getByRole("button", { name: "Retry expanding module leaf" });
    await expect(retry).toBeEnabled();
    await expect(retry).toHaveAccessibleDescription("The restored module's graph could not be loaded.");
    await retry.focus();
    await page.keyboard.press("Enter");

    // Another restore resets the fake's failure count, so only a refresh-only retry can succeed.
    await expectLeafExpandedInPlace(page, before);
  });

  test("finishes the frame when reduced motion is enabled during placement", async ({ page }) => {
    await openVisualDesigner(page, { motionPolicy: "animate", layoutDelay: "1000" });
    await loadSampleGraph(page, "remote");
    const before = await captureBeforeExpandingLeaf(page);

    await page.getByRole("button", { name: "Expand module leaf", exact: true }).click();
    await expect(page.locator('[data-node-id="storage::leaf::thing"]')).toBeAttached();
    await page.evaluate(() =>
      window.postMessage(
        { method: "settings/didChange", params: { motionPolicy: "reduce", isResourceEditingEnabled: true } },
        "*",
      ),
    );

    await expectLeafExpandedInPlace(page, before);
    const frame = await page.locator('[data-node-id="storage::leaf"]').boundingBox();
    const child = await page.locator('[data-node-id="storage::leaf::thing"]').boundingBox();
    expect(frame).not.toBeNull();
    expect(child).not.toBeNull();
    expect(frame!.width).toBeGreaterThan(child!.width);
    expect(frame!.height).toBeGreaterThan(child!.height);
  });

  test("shows a failed restore on the toggle and retries it", async ({ page }) => {
    await openVisualDesigner(page, { restoreFailures: "1", restoreDelay: "100" });
    await loadSampleGraph(page, "remote");

    await page.getByRole("button", { name: "Expand module leaf" }).click();

    const retry = page.getByRole("button", { name: "Retry restoring module leaf" });
    await expect(retry).toBeEnabled();
    await expect(retry).toHaveAttribute(
      "title",
      /Couldn't restore the module[\s\S]*The registry could not be reached\./,
    );
    await expect(retry).toHaveCSS("opacity", "1");
    await expect(retry).toHaveAccessibleDescription("The registry could not be reached.");
    await expect(page.locator('[data-node-id="storage::leaf"]')).toHaveAttribute("data-node-kind", "atomic");

    await retry.click();

    await expect(page.locator('[data-node-id="storage::leaf::thing"]')).toBeVisible();
    await expect(page.getByRole("button", { name: "Collapse module leaf" })).toBeAttached();
  });
});

test.describe("Nested modules", () => {
  test("restores nested contents with one translation after moving their collapsed ancestor", async ({ page }) => {
    await openVisualDesigner(page, { motionPolicy: "reduce" });
    await loadSampleGraph(page, "nested");
    await expect.poll(() => getGraphLayerOpacity(page)).toBe("1");
    const before = await waitForStableNodePosition(page, "ama::identity::lock");
    const credentialBefore = await waitForStableNodePosition(page, "ama::identity::credentials::credential");
    const transform = await getGraphTransform(page);

    await page.getByRole("button", { name: "Collapse module identity", exact: true }).click();
    await expect(page.locator('[data-node-id="ama::identity"]')).toHaveAttribute("data-node-kind", "atomic");
    await page.getByRole("button", { name: "Collapse module ama", exact: true }).click();
    await expect(page.locator('[data-node-id="ama"]')).toHaveAttribute("data-node-kind", "atomic");

    const ancestor = page.locator('[data-node-id="ama"]');
    const box = await ancestor.boundingBox();
    if (!box) {
      throw new Error("The collapsed ancestor has no box.");
    }
    const x = box.x + box.width / 2;
    const y = box.y + box.height / 2;
    await page.mouse.move(x, y);
    await page.mouse.down();
    for (let step = 1; step <= 8; step++) {
      await page.mouse.move(x + step * 10, y + step * 5);
      await page.waitForTimeout(16);
    }
    await page.mouse.up();
    const moved = await waitForStableNodePosition(page, "ama");
    const dx = moved.x - box.x;
    const dy = moved.y - box.y;
    expect(dx).toBeGreaterThan(40);

    await page.getByRole("button", { name: "Expand module ama", exact: true }).click();
    await expect(page.locator('[data-node-id="ama::identity"]')).toHaveAttribute("data-node-kind", "atomic");
    await page.getByRole("button", { name: "Expand module identity", exact: true }).click();

    for (const [nodeId, original] of [
      ["ama::identity::lock", before],
      ["ama::identity::credentials::credential", credentialBefore],
    ] as const) {
      const after = await waitForStableNodePosition(page, nodeId);
      expect(after.x).toBeCloseTo(original.x + dx, 0);
      expect(after.y).toBeCloseTo(original.y + dy, 0);
    }
    expect(await getGraphTransform(page)).toBe(transform);
  });

  test("lays the graph out once a collapse that landed while its layout was in flight settles", async ({ page }) => {
    // The first graph waits hidden for its layout. The layout responds after the collapse, describing the graph as
    // it was, so it moves nothing and runs again rather than leaving the graph hidden.
    await openVisualDesigner(page, { layoutDelay: "800" });
    expect(await getGraphLayerOpacity(page)).toBe("0");

    await page.getByRole("button", { name: "Collapse module myModule" }).click();

    await expect(page.locator('[data-node-id="myModule"]')).toHaveAttribute("data-node-kind", "atomic");
    await expect.poll(() => getGraphLayerOpacity(page)).toBe("1");
  });

  test("keeps the graph visible when expanding a module that holds most of the graph", async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "nested");
    await waitForStableNodePosition(page, "ama::identity::lock");

    await page.getByRole("button", { name: "Collapse module ama" }).click();
    await expect(page.locator('[data-node-id="ama"]')).toHaveAttribute("data-node-kind", "atomic");
    await page.getByRole("button", { name: "Expand module ama" }).click();

    await expect(page.locator('[data-node-id="ama::identity::credentials::credential"]')).toBeVisible();
    await expect.poll(() => getGraphLayerOpacity(page)).toBe("1");
  });
});

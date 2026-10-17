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

  test("lays out the whole graph when a restored module can't be laid out on its own", async ({ page }) => {
    await openVisualDesigner(page, { moduleLayout: "layoutFailed" });
    await loadSampleGraph(page, "remote");
    await waitForStableNodePosition(page, "storage::leaf");

    await page.getByRole("button", { name: "Expand module leaf" }).click();

    // The contents are not left hidden: they show, and the layout of the whole graph spreads them out.
    await expectLeafContentsShown(page);
    const thing = await waitForStableNodePosition(page, "storage::leaf::thing");
    const lock = await waitForStableNodePosition(page, "storage::leaf::lock");
    expect(Math.hypot(thing.x - lock.x, thing.y - lock.y)).toBeGreaterThan(20);
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
    await expect(page.locator('[data-node-id="storage::leaf"]')).toHaveAttribute("data-node-kind", "atomic");

    await retry.click();

    await expect(page.locator('[data-node-id="storage::leaf::thing"]')).toBeVisible();
    await expect(page.getByRole("button", { name: "Collapse module leaf" })).toBeAttached();
  });
});

test.describe("Nested modules", () => {
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

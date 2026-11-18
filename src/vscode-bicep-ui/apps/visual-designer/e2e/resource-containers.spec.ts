// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import { loadSampleGraph, openVisualDesigner, waitForStableNodePosition } from "./fixtures";

test.describe("Resource containers", () => {
  test("draws a resource around its child resources, with its type in the header", async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "resourceContainers");

    const vnet = page.locator('[data-node-id="vnet"]');
    await expect(vnet).toHaveAttribute("data-node-kind", "compound");
    await expect(vnet).toContainText("Virtual Networks");
    await expect(page.locator('[data-node-id="subnetApp"]')).toBeVisible();
    // A resource with nothing inside it stays an ordinary node, with no toggle.
    await expect(page.locator('[data-node-id="vm"]')).toHaveAttribute("data-node-kind", "atomic");
    await expect(page.getByRole("button", { name: /resource vm$/ })).toHaveCount(0);
    // The dependency crossing into the container is drawn.
    await expect(page.locator('[data-edge-id="nic->subnetApp"] path')).toHaveAttribute("d", /^M /);
  });

  test("collapses and expands a resource container, ending crossing edges at the container", async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "resourceContainers");
    await waitForStableNodePosition(page, "subnetApp");

    await page.locator('[data-node-id="vnet"]').hover();
    await page.getByRole("button", { name: "Collapse resource vnet" }).click();

    await expect(page.locator('[data-node-id="vnet"]')).toHaveAttribute("data-node-kind", "atomic");
    await expect(page.locator('[data-node-id="subnetApp"]')).toHaveCount(0);
    await expect(page.locator('[data-edge-id="nic->vnet"] path')).toHaveAttribute("d", /^M /);

    await page.getByRole("button", { name: "Expand resource vnet" }).click();

    await expect(page.locator('[data-node-id="vnet"]')).toHaveAttribute("data-node-kind", "compound");
    await expect(page.locator('[data-node-id="subnetApp"]')).toBeVisible();
  });

  test("nests resource containers and keeps an edge into them above their frames", async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "nestedResourceContainers");
    await waitForStableNodePosition(page, "uploads");

    const zIndex = (selector: string) =>
      page.locator(selector).evaluate((element) => Number(getComputedStyle(element).zIndex));
    const edge = await zIndex('[data-edge-id="worker->uploads"]');

    expect(edge).toBeGreaterThan(await zIndex('[data-node-id="storage"]'));
    expect(edge).toBeGreaterThan(await zIndex('[data-node-id="blobService"]'));
    expect(edge).toBeLessThan(await zIndex('[data-node-id="uploads"]'));
  });
});

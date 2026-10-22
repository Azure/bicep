// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect, test } from "@playwright/test";
import {
  clickHistoryButton,
  getGraphTransform,
  loadSampleGraph,
  openVisualDesigner,
  waitForStableNodePosition,
} from "./fixtures";

async function getBox(page: Page, nodeId: string) {
  const box = await page.locator(`[data-node-id="${nodeId}"]`).boundingBox();
  if (!box) {
    throw new Error(`Node ${nodeId} is not visible.`);
  }

  return box;
}

test.describe("Remote modules", () => {
  test("shows a registry module's published children like a local module, and collapses and expands it", async ({
    page,
  }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "remote");

    await expect(page.locator('[data-node-id="storage"]')).toHaveAttribute("data-node-kind", "compound");
    await expect(page.locator('[data-node-id="storage::blobs"]')).toHaveAttribute("data-node-kind", "compound");
    await expect(page.locator('[data-node-id="storage::account"]')).toBeVisible();
    await expect(page.locator('[data-node-id="storage::blobs::container"]')).toBeVisible();
    await waitForStableNodePosition(page, "storage::account");

    await page.getByRole("button", { name: "Collapse module storage" }).click();

    await expect(page.locator('[data-node-id="storage"]')).toHaveAttribute("data-node-kind", "atomic");
    await expect(page.locator('[data-node-id^="storage::"]')).toHaveCount(0);

    await page.getByRole("button", { name: "Expand module storage" }).click();

    await expect(page.locator('[data-node-id="storage"]')).toHaveAttribute("data-node-kind", "compound");
    await expect(page.locator('[data-node-id="storage::blobs::container"]')).toBeVisible();
  });

  test("disables expanding a module whose source is unavailable, and says why", async ({ page }) => {
    await openVisualDesigner(page);
    await loadSampleGraph(page, "remote");
    const toggle = page.getByRole("button", { name: "Expand module network" });

    await expect(toggle).toBeDisabled();
    await expect(toggle).toHaveAttribute("title", "Bicep source is unavailable for this module.");
    await expect(toggle).toHaveAttribute("data-source-status", "unavailable");

    // It is disabled with `aria-disabled`, so its tooltip shows; a click does nothing.
    await toggle.click({ force: true });
    await expect(page.locator('[data-node-id="network"]')).toHaveAttribute("data-node-kind", "atomic");
    await expect(page.locator('[data-node-id="network"] [data-module-collapsed="true"]')).toHaveCount(0);

    // A module that can be restored is not disabled.
    await expect(page.getByRole("button", { name: "Expand module monitoring" })).toBeEnabled();
  });

  test("restores a module when it is expanded and opens it in place", async ({ page }) => {
    await openVisualDesigner(page, { restoreDelay: "500" });
    await loadSampleGraph(page, "remote");
    const unrelated = ["storage::account", "network"];
    const before = await Promise.all(unrelated.map((nodeId) => waitForStableNodePosition(page, nodeId)));
    const moduleBefore = await getBox(page, "monitoring");
    const transformBefore = await getGraphTransform(page);
    const toggle = page.getByRole("button", { name: "Expand module monitoring" });

    await toggle.click();

    // The toggle shows that the restore is running.
    await expect(toggle).toBeDisabled();
    await expect(toggle).toHaveAttribute("aria-busy", "true");
    await expect(toggle.getByTestId("module-restore-progress")).toBeVisible();
    await expect(toggle.getByTestId("unfold-codicon")).toHaveCount(0);

    await expect(page.locator('[data-node-id="monitoring"]')).toHaveAttribute("data-node-kind", "compound");
    await expect(page.getByRole("button", { name: "Collapse module monitoring" })).toBeEnabled();
    await expect(page.getByTestId("module-restore-progress")).toHaveCount(0);
    // A module among the children opens with them, around its own children.
    await expect(page.locator('[data-node-id="monitoring::diagnostics"]')).toHaveAttribute(
      "data-node-kind",
      "compound",
    );
    await expect(page.locator('[data-node-id="monitoring::diagnostics::settings"]')).toBeVisible();
    await expect(page.locator('[data-node-id="monitoring::diagnostics::settings"]')).toHaveCSS("opacity", "1");
    await waitForStableNodePosition(page, "monitoring::workspace");

    // The children are laid out around where the module was, and nothing else moves.
    const container = await getBox(page, "monitoring");
    for (const nodeId of ["monitoring::workspace", "monitoring::alerts", "monitoring::diagnostics"]) {
      const box = await getBox(page, nodeId);
      expect(box.x).toBeGreaterThan(container.x);
      expect(box.y).toBeGreaterThan(container.y);
      expect(box.x + box.width).toBeLessThan(container.x + container.width);
      expect(box.y + box.height).toBeLessThan(container.y + container.height);
    }
    // A collection's stacked offset widens the frame on one side, so allow a few pixels.
    expect(Math.abs(container.x + container.width / 2 - (moduleBefore.x + moduleBefore.width / 2))).toBeLessThan(10);
    expect(Math.abs(container.y + container.height / 2 - (moduleBefore.y + moduleBefore.height / 2))).toBeLessThan(10);
    for (const [index, nodeId] of unrelated.entries()) {
      const after = await waitForStableNodePosition(page, nodeId);
      expect(after.x).toBeCloseTo(before[index]!.x, 0);
      expect(after.y).toBeCloseTo(before[index]!.y, 0);
    }
    expect(await getGraphTransform(page)).toBe(transformBefore);

    // Opening it is one undo step.
    await clickHistoryButton(page, "Undo");
    await expect(page.locator('[data-node-id="monitoring"]')).toHaveAttribute("data-node-kind", "atomic");
    await expect(page.locator('[data-node-id^="monitoring::"]')).toHaveCount(0);
  });

  test("keeps children that arrive in the background hidden, so nothing moves", async ({ page }) => {
    await openVisualDesigner(page, { backgroundRestore: "3000" });
    await loadSampleGraph(page, "remote");
    const before = await waitForStableNodePosition(page, "network");
    const moduleBefore = await getBox(page, "monitoring");
    const transformBefore = await getGraphTransform(page);

    // Once the children arrive the module is collapsed rather than waiting to be restored.
    await expect(page.locator('[data-node-id="monitoring"] [data-module-collapsed="true"]')).toBeVisible();
    await expect(page.locator('[data-node-id^="monitoring::"]')).toHaveCount(0);
    await expect(page.locator('[data-node-id="monitoring"]')).toHaveAttribute("data-node-kind", "atomic");

    const after = await waitForStableNodePosition(page, "network");
    const moduleAfter = await getBox(page, "monitoring");
    expect(after.x).toBeCloseTo(before.x, 0);
    expect(after.y).toBeCloseTo(before.y, 0);
    expect(moduleAfter.x).toBeCloseTo(moduleBefore.x, 0);
    expect(moduleAfter.y).toBeCloseTo(moduleBefore.y, 0);
    expect(await getGraphTransform(page)).toBe(transformBefore);

    // Expanding it shows the children without a restore.
    await page.getByRole("button", { name: "Expand module monitoring" }).click();
    await expect(page.locator('[data-node-id="monitoring::workspace"]')).toBeVisible();
    expect(await getGraphTransform(page)).toBe(transformBefore);
  });

  test("reports a failed restore on the toggle, which tries again", async ({ page }) => {
    await openVisualDesigner(page, { restoreFailures: "1" });
    await loadSampleGraph(page, "remote");
    const toggle = page.getByRole("button", { name: "Expand module monitoring" });

    await toggle.click();

    // One toggle: it shows the error and stays enabled to retry.
    await expect(toggle).toHaveAttribute("data-source-status", "error");
    await expect(toggle).toHaveAttribute(
      "title",
      'Unable to restore the module "monitoring". Expand the module to try again.',
    );
    await expect(toggle).toBeEnabled();
    await expect(page.locator('[data-node-id="monitoring"] button')).toHaveCount(1);
    await expect(page.locator('[data-node-id="monitoring"]')).toHaveAttribute("data-node-kind", "atomic");

    await toggle.click();

    await expect(page.locator('[data-node-id="monitoring"]')).toHaveAttribute("data-node-kind", "compound");
    await expect(page.locator('[data-node-id="monitoring::workspace"]')).toBeVisible();
  });
});

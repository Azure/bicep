// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { expect, test } from "@playwright/test";
import { loadSampleGraph, nodeCount, openVisualDesigner } from "./fixtures";

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

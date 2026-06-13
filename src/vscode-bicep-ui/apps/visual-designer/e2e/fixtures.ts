// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect } from "@playwright/test";

/**
 * Sample graphs exposed by the dev toolbar.  The slugs match the
 * `data-testid` values generated in {@link DevToolbar.tsx}.
 */
export const SAMPLE_GRAPHS = {
  module: { slug: "dev-graph-module-graph", label: "Module graph" },
  flat: { slug: "dev-graph-flat-graph", label: "Flat graph" },
  error: { slug: "dev-graph-error-graph", label: "Error graph" },
  complex: { slug: "dev-graph-complex-graph", label: "Complex graph" },
  empty: { slug: "dev-graph-empty-null", label: "Empty (null)" },
} as const;

export type SampleGraphKey = keyof typeof SAMPLE_GRAPHS;

/**
 * Navigate to the visual designer and wait for the React app to mount
 * and the initial sample graph (the dev fake channel pushes the
 * "Module graph" 50 ms after the READY notification) to render.
 */
export async function openVisualDesigner(page: Page): Promise<void> {
  await page.goto("/");
  await expect(page.getByTestId("app-root")).toBeVisible();
  await expect(page.getByTestId("graph-canvas")).toBeVisible();
  await expect(page.getByTestId("dev-toolbar")).toBeVisible();
  await waitForAnyNode(page);
}

/**
 * Click a sample-graph button in the dev toolbar and wait for the
 * graph to settle: nodes must be present (or absent, for the empty
 * sample) and the status bar must reflect the new state.
 */
export async function loadSampleGraph(page: Page, key: SampleGraphKey): Promise<void> {
  const { slug } = SAMPLE_GRAPHS[key];
  await page.getByTestId(slug).click();

  if (key === "empty") {
    await expect(page.getByTestId("graph-node")).toHaveCount(0);
    await expect(page.getByTestId("status-bar")).toHaveAttribute("data-status", "empty");
    return;
  }

  await waitForAnyNode(page);
}

/** Wait until at least one graph node has been laid out and is visible. */
export async function waitForAnyNode(page: Page): Promise<void> {
  await expect(page.getByTestId("graph-node").first()).toBeVisible();
}

/** Return the count of graph nodes currently rendered. */
export function nodeCount(page: Page): Promise<number> {
  return page.getByTestId("graph-node").count();
}

/** Return the current pan-zoom transform on the inner graph layer. */
export async function getGraphTransform(page: Page): Promise<string> {
  return page.evaluate(() => {
    const layer = document.querySelector<HTMLElement>('[data-testid="graph-canvas"] [style*="transform"]');
    if (!layer) return "";
    return layer.style.transform || getComputedStyle(layer).transform;
  });
}

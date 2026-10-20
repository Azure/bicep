// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Page } from "@playwright/test";

import { expect } from "@playwright/test";

/**
 * Sample graphs exposed by the dev toolbar.  The slugs match the
 * `data-testid` values generated in {@link DevToolbar.tsx}. `nodeId` is a node found only in that
 * sample, so a test can tell when the sample has replaced the previous graph.
 */
export const SAMPLE_GRAPHS = {
  module: { slug: "dev-graph-module-graph", label: "Module graph", nodeId: "networkInterface" },
  flat: { slug: "dev-graph-flat-graph", label: "Flat graph", nodeId: "subnet" },
  error: { slug: "dev-graph-error-graph", label: "Error graph", nodeId: "brokenStorage" },
  complex: { slug: "dev-graph-complex-graph", label: "Complex graph", nodeId: "hubrg" },
  empty: { slug: "dev-graph-empty-null", label: "Empty (null)", nodeId: null },
} as const;

export type SampleGraphKey = keyof typeof SAMPLE_GRAPHS;

/**
 * Navigate to the visual designer and wait for the React app to mount
 * and the initial sample graph (the dev fake channel pushes the
 * "Module graph" 50 ms after the READY notification) to render.
 *
 * `query` is appended to the URL to drive the dev fake channel, for example
 * `{ catalogDelay: "3000" }` to hold resource-catalog loading states open.
 */
export async function openVisualDesigner(page: Page, query: Record<string, string> = {}): Promise<void> {
  const search = new URLSearchParams(query).toString();

  await page.goto(search ? `/?${search}` : "/");
  await expect(page.getByTestId("app-root")).toBeVisible();
  await expect(page.getByTestId("graph-canvas")).toBeVisible();
  await expect(page.getByTestId("dev-toolbar")).toBeVisible();
  await waitForAnyNode(page);
}

/**
 * Click a sample-graph button in the dev toolbar and wait for the graph to replace the previous one:
 * the sample's own node must be visible (or every node gone, for the empty sample), since the designer
 * fetches the graph only once document changes pause.
 */
export async function loadSampleGraph(page: Page, key: SampleGraphKey): Promise<void> {
  const { slug, nodeId } = SAMPLE_GRAPHS[key];
  await page.getByTestId(slug).click();

  if (nodeId === null) {
    await expect(page.getByTestId("graph-node")).toHaveCount(0);
    await expect(page.getByTestId("status-bar")).toHaveAttribute("data-status", "empty");
    return;
  }

  await expect(page.locator(`[data-node-id="${nodeId}"]`)).toBeVisible();
}

/** Wait until at least one graph node has been laid out and is visible. */
export async function waitForAnyNode(page: Page): Promise<void> {
  await expect(page.getByTestId("graph-node").first()).toBeVisible();
}

/** Return the count of graph nodes currently rendered. */
export function nodeCount(page: Page): Promise<number> {
  return page.getByTestId("graph-node").count();
}

/** Click Undo or Redo in the history bar. */
export async function clickHistoryButton(page: Page, action: "Undo" | "Redo") {
  await page.getByTestId("history-bar").getByRole("button", { name: action, exact: true }).click();
}

export async function expectHistoryButtonEnabled(page: Page, action: "Undo" | "Redo", isEnabled: boolean) {
  const button = page.getByTestId("history-bar").getByRole("button", { name: action, exact: true });
  if (isEnabled) {
    await expect(button).toBeEnabled();
  } else {
    await expect(button).toBeDisabled();
  }
}

/** Return the current pan-zoom transform on the inner graph layer. */
export async function getGraphTransform(page: Page): Promise<string> {
  return page.evaluate(() => {
    const layer = document.querySelector<HTMLElement>('[data-testid="graph-canvas"] [style*="transform"]');
    if (!layer) return "";
    return layer.style.transform || getComputedStyle(layer).transform;
  });
}

/**
 * Wait until a node has stopped moving.
 *
 * Two animations run after a graph loads: the fit-view transform, and each node's ~0.6s spring to its
 * laid-out position. They are independent, so a settled transform does not mean settled nodes — and a
 * node measured or dragged mid-spring keeps travelling to its target afterwards.
 */
export async function waitForStableNodePosition(page: Page, nodeId: string): Promise<{ x: number; y: number }> {
  const node = page.locator(`[data-node-id="${nodeId}"]`);
  let previous: string | null = null;

  await expect
    .poll(
      async () => {
        const box = await node.boundingBox();
        const current = box ? `${Math.round(box.x)},${Math.round(box.y)}` : null;
        const stable = current !== null && current === previous;
        previous = current;

        return stable;
      },
      { timeout: 10_000 },
    )
    .toBe(true);

  const settled = await node.boundingBox();

  return { x: settled!.x, y: settled!.y };
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { NodeState } from "../atoms/nodes";

import { createStore } from "jotai";
import { describe, expect, it, vi } from "vitest";
import {
  beginNodeResize,
  getNodeZIndex,
  mountedNodesByIdAtom,
  nodesByIdAtom,
  registerNodeMeasurer,
  waitForNodeSizes,
} from "../atoms/nodes";

/** Where `EdgeLayer` draws the edges between a compound's children. */
const scopeEdgesZ = (parentId: string, focusedId: string | null) => getNodeZIndex(parentId, "compound", focusedId) + 1;

/** Lets a test observe whether a promise has settled, once pending callbacks have run (see `flush`). */
function trackSettled(promise: Promise<void>) {
  const state = { isSettled: false };
  void promise.then(() => {
    state.isSettled = true;
  });
  return state;
}

async function flush() {
  for (let i = 0; i < 5; i++) {
    await Promise.resolve();
  }
}

describe("waitForNodeSizes", () => {
  /** A node map with an identity of its own, as each graph update sets. */
  const nodeMap = (): Record<string, NodeState> => ({});

  it("resolves at once when the canvas has mounted the current nodes, or no canvas is mounted", async () => {
    const store = createStore();
    store.set(nodesByIdAtom, nodeMap());
    await expect(waitForNodeSizes(store)).resolves.toBeUndefined();

    store.set(mountedNodesByIdAtom, store.get(nodesByIdAtom));
    await expect(waitForNodeSizes(store)).resolves.toBeUndefined();
  });

  it("waits for the canvas to mount the latest nodes, not ones it was given before", async () => {
    const store = createStore();
    const [first, second, latest] = [nodeMap(), nodeMap(), nodeMap()];
    store.set(nodesByIdAtom, first);
    store.set(mountedNodesByIdAtom, first);

    store.set(nodesByIdAtom, second);
    const waiting = trackSettled(waitForNodeSizes(store));
    store.set(nodesByIdAtom, latest);
    store.set(mountedNodesByIdAtom, second);
    await flush();
    expect(waiting.isSettled).toBe(false);

    store.set(mountedNodesByIdAtom, latest);
    await flush();
    expect(waiting.isSettled).toBe(true);
  });

  it("stops waiting when the canvas unmounts", async () => {
    const store = createStore();
    const [mounted, added] = [nodeMap(), nodeMap()];
    store.set(mountedNodesByIdAtom, mounted);
    store.set(nodesByIdAtom, added);
    const waiting = trackSettled(waitForNodeSizes(store));

    store.set(mountedNodesByIdAtom, undefined);
    await flush();
    expect(waiting.isSettled).toBe(true);
  });

  it("waits for every resize in progress to end", async () => {
    const store = createStore();
    const endFirst = beginNodeResize(store);
    const endSecond = beginNodeResize(store);
    const waiting = trackSettled(waitForNodeSizes(store));

    endFirst();
    // Ending a resize again does not end another.
    endFirst();
    await flush();
    expect(waiting.isSettled).toBe(false);

    endSecond();
    await flush();
    expect(waiting.isSettled).toBe(true);
  });

  it("has every mounted node read its size again once sizes settle, and not after it unmounts", async () => {
    const store = createStore();
    const measure = vi.fn();
    const unregister = registerNodeMeasurer(store, measure);
    const endResize = beginNodeResize(store);

    const waiting = waitForNodeSizes(store);
    expect(measure).not.toHaveBeenCalled();
    endResize();
    await waiting;
    expect(measure).toHaveBeenCalledTimes(1);

    unregister();
    await waitForNodeSizes(store);
    expect(measure).toHaveBeenCalledTimes(1);
  });

  it("keeps each store's nodes to itself", async () => {
    const [store, otherStore] = [createStore(), createStore()];
    const measure = vi.fn();
    registerNodeMeasurer(store, measure);

    await waitForNodeSizes(otherStore);
    expect(measure).not.toHaveBeenCalled();
  });
});

describe("getNodeZIndex", () => {
  it.each([null, "outside", "module::lock", "module::nested", "module::nested::child", "module"])(
    "draws a module's edges over its frame but under every child, nested frames included (focus: %s)",
    (focusedId) => {
      const edges = scopeEdgesZ("module", focusedId);

      expect(getNodeZIndex("module", "compound", focusedId)).toBeLessThan(edges);
      expect(getNodeZIndex("module::lock", "atomic", focusedId)).toBeGreaterThan(edges);
      expect(getNodeZIndex("module::nested", "compound", focusedId)).toBeGreaterThan(edges);
      expect(getNodeZIndex("module::nested::child", "atomic", focusedId)).toBeGreaterThan(
        scopeEdgesZ("module::nested", focusedId),
      );
    },
  );

  it("keeps every node above the edges between top-level nodes", () => {
    for (const focusedId of [null, "outside", "module::nested::child"]) {
      expect(getNodeZIndex("outside", "atomic", focusedId)).toBeGreaterThan(-1);
      expect(getNodeZIndex("module", "compound", focusedId)).toBeGreaterThan(-1);
    }
  });

  it("raises the focused node above its siblings however deeply they nest", () => {
    const focused = getNodeZIndex("outside", "atomic", "outside");

    expect(focused).toBeGreaterThan(getNodeZIndex("module", "compound", "outside"));
    expect(focused).toBeGreaterThan(getNodeZIndex("module::a::b::c::d::e::f", "atomic", "outside"));
  });
});

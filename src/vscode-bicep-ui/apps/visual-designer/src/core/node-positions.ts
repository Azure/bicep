// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Point } from "@/lib/math";

import { nodesByIdAtom } from "@/lib/graph";
import { translateBox } from "@/lib/math";

type Store = ReturnType<typeof createStore>;

/** Top-left corners of atomic nodes, keyed by node ID. */
export type NodePositions = ReadonlyMap<string, Point>;

/** Node IDs nest with `::`, so `module::storage` is inside `module`. */
function isInSubtree(nodeId: string, rootNodeId: string): boolean {
  return nodeId === rootNodeId || nodeId.startsWith(`${rootNodeId}::`);
}

/** IDs of the mounted atomic nodes: the only nodes with positions of their own. */
export function getAtomicNodeIds(store: Store): Set<string> {
  return new Set(
    Object.values(store.get(nodesByIdAtom))
      .filter((node) => node.kind === "atomic")
      .map((node) => node.id),
  );
}

/**
 * Read the positions of every atomic node, or only those in the subtree rooted at `rootNodeId`.
 *
 * Compound nodes are skipped: their boxes are derived from their children, so they have no
 * position of their own to save or restore.
 */
export function captureNodePositions(store: Store, rootNodeId?: string): NodePositions {
  const positions = new Map<string, Point>();

  for (const [nodeId, node] of Object.entries(store.get(nodesByIdAtom))) {
    if (node.kind === "atomic" && (rootNodeId === undefined || isInSubtree(nodeId, rootNodeId))) {
      const { x, y } = store.get(node.boxAtom).min;
      positions.set(nodeId, { x, y });
    }
  }

  return positions;
}

/** Move atomic nodes to the given positions immediately, keeping their sizes. Unknown IDs are ignored. */
export function applyNodePositions(store: Store, positions: NodePositions): void {
  const nodesById = store.get(nodesByIdAtom);

  for (const [nodeId, position] of positions) {
    const node = nodesById[nodeId];
    if (node?.kind === "atomic") {
      store.set(node.boxAtom, (box) => translateBox(box, position.x - box.min.x, position.y - box.min.y));
    }
  }
}

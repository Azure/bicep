// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { PositionTransition } from "@/lib/graph";
import type { Point } from "@/lib/math";
import type { ClientGraph } from "./graph-model";

import { nodePositionsAtom } from "@/lib/graph";
import { getDescendantIds } from "./graph-model";

type Store = ReturnType<typeof createStore>;

/**
 * Node centers keyed by node ID: the one record of where nodes are, including nodes hidden inside a
 * collapsed module, so they come back where they were. Containers have no position of their own; they
 * are placed by their contents. Snapshots are immutable, so keeping one is free.
 */
export type NodePositions = ReadonlyMap<string, Point>;

export function getNodePositions(store: Store): NodePositions {
  return store.get(nodePositionsAtom).centers;
}

/** Replace the node positions. `animate` moves the nodes there; `instant` puts them there. */
export function setNodePositions(store: Store, positions: NodePositions, transition: PositionTransition): void {
  store.set(nodePositionsAtom, { centers: positions, transition });
}

/**
 * Apply `changes` to `positions`.
 *
 * The hidden contents of a collapsed module move with its card, so they are still inside it when it
 * opens, unless `changes` moves them itself. This holds however the card moves: a drag, a layout, or
 * undo. Nested collapsed modules move with the outermost one that moves.
 */
export function moveNodes(
  positions: NodePositions,
  changes: NodePositions,
  graph: ClientGraph,
  collapsedModuleIds: ReadonlySet<string>,
): NodePositions {
  const next = new Map(positions);

  for (const [nodeId, position] of changes) {
    const previous = positions.get(nodeId);
    next.set(nodeId, position);

    if (!previous || !collapsedModuleIds.has(nodeId)) {
      continue;
    }

    const dx = position.x - previous.x;
    const dy = position.y - previous.y;

    for (const descendantId of getDescendantIds(graph, nodeId)) {
      const descendant = next.get(descendantId);
      if (descendant && !changes.has(descendantId)) {
        next.set(descendantId, { x: descendant.x + dx, y: descendant.y + dy });
      }
    }
  }

  return next;
}

/** The changes that move each of `nodeIds` that has a position by `dx`, `dy`. */
export function translateNodes(
  positions: NodePositions,
  nodeIds: Iterable<string>,
  dx: number,
  dy: number,
): NodePositions {
  const changes = new Map<string, Point>();

  for (const nodeId of nodeIds) {
    const position = positions.get(nodeId);
    if (position) {
      changes.set(nodeId, { x: position.x + dx, y: position.y + dy });
    }
  }

  return changes;
}

/** Drop the positions of nodes that are no longer in the graph. */
export function forgetRemovedNodePositions(positions: NodePositions, graph: ClientGraph): NodePositions {
  const kept = [...positions].filter(([nodeId]) => graph.nodes.has(nodeId));
  return kept.length === positions.size ? positions : new Map(kept);
}

/** The shown nodes that are positioned by their own center rather than by their contents. */
export function getPositionedNodeIds(shown: ClientGraph): string[] {
  const containerIds = new Set(
    [...shown.nodes.values()].flatMap((node) => (node.parentId === null ? [] : [node.parentId])),
  );
  return [...shown.nodes.keys()].filter((nodeId) => !containerIds.has(nodeId));
}

/** The center of the given positions, or null when there are none. */
export function getCentroid(positions: Iterable<Point>): Point | null {
  let count = 0;
  let x = 0;
  let y = 0;

  for (const position of positions) {
    count++;
    x += position.x;
    y += position.y;
  }

  return count === 0 ? null : { x: x / count, y: y / count };
}

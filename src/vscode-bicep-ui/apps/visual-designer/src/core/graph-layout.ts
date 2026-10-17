// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box, Point } from "@/lib/math";
import type { GraphBounds, NodePosition } from "./api";
import type { ClientGraph } from "./graph-model";

import { subtreeGraph } from "./graph-model";

/**
 * The node metadata fields that influence rendered size and therefore layout. Keep this consistent
 * with the measured graph comparison and the language server's layout validation. A module that
 * requires restoring shows an expand toggle, which can widen its node.
 */
const LAYOUT_AFFECTING_NODE_FIELDS = ["type", "isCollection", "hasChildren", "requiresRestore"] as const;

export interface LayoutExemptions {
  /** Nodes placed where the user dropped them. Adding one does not require a layout. */
  explicitlyPlacedNodeIds?: ReadonlySet<string>;
  /**
   * Nodes removed because the user undid their creation. Removing one leaves the other nodes where
   * they are, provided it is a top-level resource with no children.
   */
  pendingRemovalNodeIds?: ReadonlySet<string>;
  /**
   * Restored modules whose contents just arrived (see `findRestoredModules`). Each is laid out on its own, so what
   * that layout covers -- the module, its contents, and the dependencies among them (see `subtreeGraph`) -- and its
   * change into a container do not require laying out the graph.
   */
  restoredModuleIds?: ReadonlySet<string>;
}

/** Remote modules that required restoring in `previous` and have contents in `next`. */
export function findRestoredModules(previous: ClientGraph, next: ClientGraph): Set<string> {
  const restored = new Set<string>();

  for (const [nodeId, node] of previous.nodes) {
    if (node.kind === "module" && node.requiresRestore && !node.hasChildren && next.nodes.get(nodeId)?.hasChildren) {
      restored.add(nodeId);
    }
  }

  return restored;
}

/** Whether replacing `previous` with `next` may invalidate the current layout. */
export function graphChangeMayAffectLayout(
  previous: ClientGraph,
  next: ClientGraph,
  {
    explicitlyPlacedNodeIds = new Set(),
    pendingRemovalNodeIds = new Set(),
    restoredModuleIds = new Set(),
  }: LayoutExemptions = {},
): boolean {
  // What the restored modules' own layouts cover, collected once so each lookup below is a set lookup.
  const restoredSubtrees = [...restoredModuleIds].map((moduleId) => subtreeGraph(next, moduleId));
  const coveredNodeIds = new Set(restoredSubtrees.flatMap((subtree) => [...subtree.nodes.keys()]));
  const coveredEdgeIds = new Set(restoredSubtrees.flatMap((subtree) => [...subtree.edges.keys()]));

  for (const [nodeId, node] of previous.nodes) {
    const nextNode = next.nodes.get(nodeId);

    if (!nextNode) {
      const isIndependentResource = node.kind === "resource" && node.parentId === null && !node.hasChildren;
      if (pendingRemovalNodeIds.has(nodeId) && isIndependentResource) {
        continue;
      }
      return true;
    }

    // A node whose kind or container changed is replaced rather than updated. A restored module is expected to
    // gain children and stop requiring a restore.
    const fields = restoredModuleIds.has(nodeId)
      ? LAYOUT_AFFECTING_NODE_FIELDS.filter((field) => field !== "hasChildren" && field !== "requiresRestore")
      : LAYOUT_AFFECTING_NODE_FIELDS;
    if (
      nextNode.kind !== node.kind ||
      nextNode.parentId !== node.parentId ||
      fields.some((field) => nextNode[field] !== node[field])
    ) {
      return true;
    }
  }

  for (const nodeId of next.nodes.keys()) {
    if (!previous.nodes.has(nodeId) && !explicitlyPlacedNodeIds.has(nodeId) && !coveredNodeIds.has(nodeId)) {
      return true;
    }
  }

  for (const edgeId of next.edges.keys()) {
    if (!previous.edges.has(edgeId) && !coveredEdgeIds.has(edgeId)) {
      return true;
    }
  }

  return [...previous.edges.keys()].some((edgeId) => !next.edges.has(edgeId));
}

/**
 * Shift a server layout so what it laid out sits centred on `center`: the viewport's centre for the whole graph, or
 * where a module was for a module laid out on its own.
 *
 * Returns the shifted positions, keyed by node id, and the laid-out bounds in the same space, which is
 * what fit-view needs.
 */
export function centerGraphLayout(
  positions: readonly NodePosition[],
  graphBounds: GraphBounds | null,
  center: Point,
): { positions: Map<string, Point>; bounds: Box | null } {
  const offsetX = graphBounds ? center.x - graphBounds.width / 2 : 0;
  const offsetY = graphBounds ? center.y - graphBounds.height / 2 : 0;
  const centered = new Map(positions.map(({ nodeId, x, y }) => [nodeId, { x: x + offsetX, y: y + offsetY }]));

  return {
    positions: centered,
    bounds: graphBounds && {
      min: { x: offsetX, y: offsetY },
      max: { x: offsetX + graphBounds.width, y: offsetY + graphBounds.height },
    },
  };
}

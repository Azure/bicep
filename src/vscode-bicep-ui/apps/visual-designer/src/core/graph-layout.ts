// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box, Point } from "@/lib/math";
import type { GraphBounds, NodePosition } from "./api";
import type { ClientGraph } from "./graph-model";

/**
 * The node metadata fields that influence rendered size and therefore layout. Keep this consistent
 * with the measured graph comparison and the language server's layout validation.
 */
const LAYOUT_AFFECTING_NODE_FIELDS = ["type", "isCollection", "hasChildren"] as const;

export interface LayoutExemptions {
  /** Nodes placed where the user dropped them. Adding one does not require a layout. */
  explicitlyPlacedNodeIds?: ReadonlySet<string>;
  /**
   * Nodes removed because the user undid their creation. Removing one leaves the other nodes where
   * they are, provided it is a top-level resource with no children.
   */
  pendingRemovalNodeIds?: ReadonlySet<string>;
}

/** Whether replacing `previous` with `next` may invalidate the current layout. */
export function graphChangeMayAffectLayout(
  previous: ClientGraph,
  next: ClientGraph,
  { explicitlyPlacedNodeIds = new Set(), pendingRemovalNodeIds = new Set() }: LayoutExemptions = {},
): boolean {
  for (const [nodeId, node] of previous.nodes) {
    const nextNode = next.nodes.get(nodeId);

    if (!nextNode) {
      const isIndependentResource = node.kind === "resource" && node.parentId === null && !node.hasChildren;
      if (pendingRemovalNodeIds.has(nodeId) && isIndependentResource) {
        continue;
      }
      return true;
    }

    // A node whose kind or container changed is replaced rather than updated.
    if (
      nextNode.kind !== node.kind ||
      nextNode.parentId !== node.parentId ||
      LAYOUT_AFFECTING_NODE_FIELDS.some((field) => nextNode[field] !== node[field])
    ) {
      return true;
    }
  }

  for (const nodeId of next.nodes.keys()) {
    if (!previous.nodes.has(nodeId) && !explicitlyPlacedNodeIds.has(nodeId)) {
      return true;
    }
  }

  return (
    next.edges.size !== previous.edges.size || [...next.edges.keys()].some((edgeId) => !previous.edges.has(edgeId))
  );
}

/**
 * Shift a server layout so the graph sits centred on `viewportCenter`.
 *
 * Returns the shifted positions, keyed by node id, and the graph's bounds in the same space, which is
 * what fit-view needs.
 */
export function centerGraphLayout(
  positions: readonly NodePosition[],
  graphBounds: GraphBounds | null,
  viewportCenter: Point,
): { positions: Map<string, Point>; bounds: Box | null } {
  const offsetX = graphBounds ? viewportCenter.x - graphBounds.width / 2 : 0;
  const offsetY = graphBounds ? viewportCenter.y - graphBounds.height / 2 : 0;
  const centered = new Map(positions.map(({ nodeId, x, y }) => [nodeId, { x: x + offsetX, y: y + offsetY }]));

  return {
    positions: centered,
    bounds: graphBounds && {
      min: { x: offsetX, y: offsetY },
      max: { x: offsetX + graphBounds.width, y: offsetY + graphBounds.height },
    },
  };
}

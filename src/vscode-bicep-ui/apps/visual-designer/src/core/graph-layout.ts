// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box, Point } from "@/lib/math";
import type { GraphBounds, MeasuredGraph, NodePosition } from "./api";
import type { ClientGraph } from "./graph-model";

/**
 * The node metadata fields that influence rendered size and therefore layout. Node content is
 * measured, so keep this consistent with what the canvas renders.
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

    // A node that moved to another container, changed kind, or may render at another size needs a layout.
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

/**
 * The centers of the laid-out nodes that are positioned by their own center, from the top-left corners
 * the host returns. Containers are skipped: they are placed by their contents.
 */
export function getLayoutCenters(topLeftCorners: ReadonlyMap<string, Point>, graph: MeasuredGraph): Map<string, Point> {
  const containerIds = new Set(graph.nodes.flatMap((node) => (node.parentId === null ? [] : [node.parentId])));
  const centers = new Map<string, Point>();

  for (const { id, width, height } of graph.nodes) {
    const corner = topLeftCorners.get(id);
    if (corner && !containerIds.has(id)) {
      centers.set(id, { x: corner.x + width / 2, y: corner.y + height / 2 });
    }
  }

  return centers;
}

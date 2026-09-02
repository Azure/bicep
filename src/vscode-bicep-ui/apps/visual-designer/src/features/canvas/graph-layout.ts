// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box, Point } from "@/lib/math";
import type { GraphBounds, GraphNode, GraphPatch, NodeLayout } from "./api";

/**
 * The node metadata fields that influence rendered size and therefore layout. Keep this consistent
 * with the rendered graph comparison and the language server's layout validation.
 */
const LAYOUT_AFFECTING_NODE_FIELDS = ["type", "isCollection", "hasChildren"] as const;

type LayoutRelevantNode = Pick<GraphNode, (typeof LAYOUT_AFFECTING_NODE_FIELDS)[number]>;

interface LayoutRelevantGraph {
  nodes: ReadonlyMap<string, LayoutRelevantNode>;
}

/** Whether applying a patch may invalidate the current graph layout. */
export function patchMayAffectLayout(
  graph: LayoutRelevantGraph,
  patch: GraphPatch,
  explicitlyPlacedNodeIds: ReadonlySet<string> = new Set(),
): boolean {
  switch (patch.op) {
    case "clearGraph":
    case "removeNode":
    case "addEdge":
    case "removeEdge":
      return true;
    case "addNode":
      return !explicitlyPlacedNodeIds.has(patch.node.id);
    case "updateNode": {
      const node = graph.nodes.get(patch.nodeId);
      if (!node) {
        return false;
      }
      const { changes } = patch;
      return LAYOUT_AFFECTING_NODE_FIELDS.some(
        (field) => changes[field] !== undefined && changes[field] !== null && changes[field] !== node[field],
      );
    }
    case "setNodeLayout":
    case "setGraphBounds":
    case "setErrorCount":
      return false;
  }
}

/** Extract the server-computed positions and final bounds from a graph layout patch list. */
export function extractGraphLayout(patches: readonly GraphPatch[]): {
  nodeLayouts: Map<string, NodeLayout>;
  graphBounds: GraphBounds | null;
} {
  const nodeLayouts = new Map<string, NodeLayout>();
  let graphBounds: GraphBounds | null = null;

  for (const patch of patches) {
    if (patch.op === "setNodeLayout") {
      nodeLayouts.set(patch.nodeId, patch.layout);
    } else if (patch.op === "setGraphBounds") {
      graphBounds = patch.bounds;
    }
  }

  return { nodeLayouts, graphBounds };
}

/**
 * Shift a server layout so the graph sits centred on `viewportCenter`.
 *
 * Returns the shifted positions and the graph's bounds in the same space, which is what fit-view
 * needs.
 */
export function centerGraphLayout(
  nodeLayouts: Map<string, NodeLayout>,
  graphBounds: GraphBounds | null,
  viewportCenter: Point,
): { nodeLayouts: Map<string, NodeLayout>; bounds: Box | null } {
  if (!graphBounds) {
    return { nodeLayouts, bounds: null };
  }

  const offsetX = viewportCenter.x - graphBounds.width / 2;
  const offsetY = viewportCenter.y - graphBounds.height / 2;
  const centeredLayouts = new Map<string, NodeLayout>();

  for (const [nodeId, layout] of nodeLayouts) {
    centeredLayouts.set(nodeId, { x: layout.x + offsetX, y: layout.y + offsetY });
  }

  return {
    nodeLayouts: centeredLayouts,
    bounds: {
      min: { x: offsetX, y: offsetY },
      max: { x: offsetX + graphBounds.width, y: offsetY + graphBounds.height },
    },
  };
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNode, GraphPatch, RenderedGraph } from "./messages";

/**
 * The node metadata fields that influence a node's rendered size, and therefore the layout.
 *
 * "What affects layout" is decided in three places that must stay consistent:
 *
 * 1. {@link patchMayAffectLayout} here — the cheap client pre-filter that decides whether an
 *    applied `updateNode` patch is worth a re-measure.
 * 2. {@link renderedGraphsEqual} here — the authoritative check that compares the freshly measured
 *    graph (structure + measured sizes) against the last graph that produced a layout.
 * 3. The language server's `VisualGraphDiffer.HasTopologyChange` — which validates a measured
 *    layout request against the live compilation (node id/kind/parent and the edge set).
 *
 * If you add a field that changes a node's rendered size, add it here and confirm (2) and (3) still
 * capture it; otherwise range-only edits may wrongly reflow, or a real change may be missed.
 */
const LAYOUT_AFFECTING_NODE_FIELDS = ["type", "isCollection", "hasChildren"] as const;

type LayoutRelevantNode = Pick<GraphNode, (typeof LAYOUT_AFFECTING_NODE_FIELDS)[number]>;

interface LayoutRelevantGraph {
  nodes: ReadonlyMap<string, LayoutRelevantNode>;
}

/**
 * Whether applying `patch` to the client graph may change the layout. Used as a cheap pre-filter
 * before the (more expensive) render + measure + {@link renderedGraphsEqual} confirmation.
 *
 * Must be called BEFORE the patch is applied: for `updateNode` it compares the incoming values
 * against the node's CURRENT values. Only an actual change to a size-affecting field counts;
 * `hasError` never affects layout.
 */
export function patchMayAffectLayout(graph: LayoutRelevantGraph, patch: GraphPatch): boolean {
  switch (patch.op) {
    case "clearGraph":
    case "addNode":
    case "removeNode":
    case "addEdge":
    case "removeEdge":
      return true;
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

/**
 * Whether two measured graphs are equivalent layout inputs: the same node set (id, kind, parent,
 * and measured width/height) and the same edge set. This is the authoritative second tier after
 * {@link patchMayAffectLayout}; a layout request is sent only when this returns false. Node and
 * edge order is irrelevant (compared by id).
 */
export function renderedGraphsEqual(left: RenderedGraph | null, right: RenderedGraph): boolean {
  if (!left || left.nodes.length !== right.nodes.length || left.edges.length !== right.edges.length) {
    return false;
  }

  const leftNodes = new Map(left.nodes.map((node) => [node.id, node]));

  for (const rightNode of right.nodes) {
    const leftNode = leftNodes.get(rightNode.id);
    if (
      !leftNode ||
      leftNode.kind !== rightNode.kind ||
      leftNode.parentId !== rightNode.parentId ||
      leftNode.width !== rightNode.width ||
      leftNode.height !== rightNode.height
    ) {
      return false;
    }
  }

  const leftEdges = new Set(left.edges.map((edge) => `${edge.id}|${edge.sourceId}|${edge.targetId}`));

  return right.edges.every((edge) => leftEdges.has(`${edge.id}|${edge.sourceId}|${edge.targetId}`));
}

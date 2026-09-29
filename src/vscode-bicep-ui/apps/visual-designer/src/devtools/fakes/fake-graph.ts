// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Graph, GraphBounds, GraphNode, LayoutGraphResult, MeasuredGraph, NodePosition } from "@/core";
import type { SampleGraph, SampleGraphNode } from "./sample-graphs";

/**
 * Dev-only stand-ins for the language server's graph and layout requests, so the dev playground can run
 * the graph loop (`documentDidChange` → `graph/get` → `graph/layout`) without an extension or language server.
 */

/** Matches the edge identity the language server builds. */
function edgeId(sourceId: string, targetId: string): string {
  return `${sourceId}->${targetId}`;
}

/** Containment is encoded in the `::`-delimited id; the parent is everything before the last segment. */
function getParentId(id: string): string | null {
  const index = id.lastIndexOf("::");
  return index === -1 ? null : id.slice(0, index);
}

function toGraphNode(node: SampleGraphNode): GraphNode {
  return {
    id: node.id,
    kind: node.type === "<module>" ? "module" : "resource",
    parentId: getParentId(node.id),
    type: node.type,
    symbolName: node.id.split("::").pop() ?? node.id,
    isCollection: node.isCollection,
    hasChildren: node.hasChildren,
    hasError: node.hasError,
  };
}

/** The graph the language server would build for a sample. */
export function toGraph(sample: SampleGraph | null): Graph {
  return {
    nodes: (sample?.nodes ?? []).map(toGraphNode),
    edges: (sample?.edges ?? []).map(({ sourceId, targetId }) => ({
      id: edgeId(sourceId, targetId),
      sourceId,
      targetId,
    })),
    errorCount: sample?.errorCount ?? 0,
  };
}

/** Stack each scope's children vertically, sizing modules from their contents. */
function buildFakeLayout(nodes: GraphNode[]): { positions: NodePosition[]; bounds: GraphBounds } {
  const nodesByParent = new Map<string | null, GraphNode[]>();
  const positions: NodePosition[] = [];

  for (const node of nodes) {
    const siblings = nodesByParent.get(node.parentId) ?? [];
    siblings.push(node);
    nodesByParent.set(node.parentId, siblings);
  }

  for (const siblings of nodesByParent.values()) {
    siblings.sort((a, b) => a.id.localeCompare(b.id));
  }

  function layoutScope(parentId: string | null, offsetX: number, offsetY: number): { width: number; height: number } {
    const siblings = nodesByParent.get(parentId) ?? [];
    let maxWidth = 0;
    let cursorY = 0;

    for (const node of siblings) {
      positions.push({ nodeId: node.id, x: offsetX, y: offsetY + cursorY });

      let width = 220;
      let height = 80;

      if (node.hasChildren) {
        const childBounds = layoutScope(node.id, offsetX + 40, offsetY + cursorY + 50);
        width = childBounds.width + 80;
        height = childBounds.height + 90;
      }

      maxWidth = Math.max(maxWidth, width);
      cursorY += height + 80;
    }

    return { width: maxWidth, height: Math.max(cursorY - 80, 0) };
  }

  const root = layoutScope(null, 0, 0);

  return { positions, bounds: { width: root.width, height: root.height } };
}

/** Like the language server, refuse to lay out a measured graph whose topology is out of date. */
export function layoutGraph(measured: MeasuredGraph, sample: SampleGraph | null): LayoutGraphResult {
  const graph = toGraph(sample);
  const liveNodes = new Map(graph.nodes.map((node) => [node.id, node]));
  const liveEdgeIds = new Set(graph.edges.map((edge) => edge.id));
  const matches =
    measured.nodes.length === liveNodes.size &&
    measured.edges.length === liveEdgeIds.size &&
    measured.nodes.every(
      (node) => liveNodes.get(node.id)?.kind === node.kind && liveNodes.get(node.id)?.parentId === node.parentId,
    ) &&
    measured.edges.every((edge) => liveEdgeIds.has(edge.id));

  if (!matches) {
    return { status: "graphChanged", positions: [], bounds: null };
  }

  return { status: "ok", ...buildFakeLayout(graph.nodes) };
}

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
  };
}

/** Like the language server's revision: identifies the topology, ignoring metadata and order. */
export function getGraphRevision({ nodes, edges }: Graph): string {
  return JSON.stringify([
    nodes.map(({ id, kind, parentId }) => [id, kind, parentId]).sort(),
    edges.map(({ id, sourceId, targetId }) => [id, sourceId, targetId]).sort(),
  ]);
}

type LayoutNode = Pick<GraphNode, "id" | "parentId">;

/** Stack each scope's children vertically, sizing containers from their contents. */
function buildFakeLayout(nodes: LayoutNode[]): { positions: NodePosition[]; bounds: GraphBounds } {
  const nodesByParent = new Map<string | null, LayoutNode[]>();
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

      if (nodesByParent.has(node.id)) {
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

/** Like the language server, refuse a stale revision and otherwise lay out the measured graph as sent. */
export function layoutGraph(
  measured: MeasuredGraph,
  revision: string | null,
  sample: SampleGraph | null,
): LayoutGraphResult {
  if (revision !== getGraphRevision(toGraph(sample))) {
    return { status: "graphChanged", positions: [], bounds: null };
  }

  return { status: "ok", ...buildFakeLayout(measured.nodes) };
}

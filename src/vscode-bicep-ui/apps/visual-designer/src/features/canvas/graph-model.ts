// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";
import type { GraphEdge, GraphNode, GraphPatch, NodeLayout, RenderedGraph } from "./api";

/**
 * The client's local replica of the server's canonical graph.
 *
 * The server rebuilds its authoritative copy from the live compilation on every request, while this
 * replica is only as fresh as the last patch applied to it. That is why the server validates each
 * layout request against its own graph rather than trusting this one, and why a stale copy comes back
 * as a `graphChanged` response.
 *
 * Keyed by id, and mutated in place, so applying a patch delta is O(1) per patch.
 */
export interface ClientGraph {
  nodes: Map<string, GraphNode>;
  edges: Map<string, GraphEdge>;
  errorCount: number;
}

export function createClientGraph(): ClientGraph {
  return { nodes: new Map(), edges: new Map(), errorCount: 0 };
}

/** Apply one server patch to the client's copy. Layout patches are collected into `nodeLayouts` instead. */
export function applyGraphPatch(graph: ClientGraph, nodeLayouts: Map<string, NodeLayout>, patch: GraphPatch): void {
  switch (patch.op) {
    case "clearGraph":
      graph.nodes.clear();
      graph.edges.clear();
      graph.errorCount = 0;
      return;
    case "addNode":
      graph.nodes.set(patch.node.id, patch.node);
      return;
    case "removeNode":
      graph.nodes.delete(patch.nodeId);
      return;
    case "updateNode": {
      const node = graph.nodes.get(patch.nodeId);
      if (node) {
        // Only defined fields in `changes` override the node; the rest are left untouched.
        const next = { ...node };
        for (const [key, value] of Object.entries(patch.changes)) {
          if (value !== undefined && value !== null) {
            (next as Record<string, unknown>)[key] = value;
          }
        }
        graph.nodes.set(patch.nodeId, next);
      }
      return;
    }
    case "addEdge":
      graph.edges.set(patch.edge.id, patch.edge);
      return;
    case "removeEdge":
      graph.edges.delete(patch.edgeId);
      return;
    case "setNodeLayout":
      nodeLayouts.set(patch.nodeId, patch.layout);
      return;
    case "setGraphBounds":
      // Graph bounds drive fit-view in the layout flow, not the client's graph.
      return;
    case "setErrorCount":
      graph.errorCount = patch.errorCount;
      return;
  }
}

/**
 * Build the `RenderedGraph` to submit with a request: the topology the webview holds plus the size it
 * has measured for each node.
 *
 * `measuredBoxes` supplies those sizes, keyed by node id; a node with no entry reports zero, which is
 * the state before it has been laid out and measured.
 */
export function buildRenderedGraph(graph: ClientGraph, measuredBoxes: ReadonlyMap<string, Box>): RenderedGraph {
  return {
    nodes: [...graph.nodes.values()].map((node) => {
      const box = measuredBoxes.get(node.id);

      return {
        id: node.id,
        kind: node.kind,
        parentId: node.parentId,
        type: node.type,
        isCollection: node.isCollection,
        hasChildren: node.hasChildren,
        hasError: node.hasError,
        width: box ? box.max.x - box.min.x : 0,
        height: box ? box.max.y - box.min.y : 0,
      };
    }),
    edges: [...graph.edges.values()].map((edge) => ({
      id: edge.id,
      sourceId: edge.sourceId,
      targetId: edge.targetId,
    })),
  };
}

/**
 * Whether two client graphs would produce the same canvas.
 *
 * Compares exactly the fields the apply path reads. Nodes and edges are keyed by id, so ordering is
 * irrelevant.
 */
export function clientGraphsRenderEqually(left: ClientGraph | null, right: ClientGraph | null): boolean {
  if (left === right) {
    return true;
  }

  if (!left || !right) {
    return false;
  }

  if (
    left.errorCount !== right.errorCount ||
    left.nodes.size !== right.nodes.size ||
    left.edges.size !== right.edges.size
  ) {
    return false;
  }

  for (const [nodeId, rightNode] of right.nodes) {
    const leftNode = left.nodes.get(nodeId);

    if (
      !leftNode ||
      leftNode.type !== rightNode.type ||
      leftNode.isCollection !== rightNode.isCollection ||
      leftNode.hasChildren !== rightNode.hasChildren ||
      leftNode.hasError !== rightNode.hasError
    ) {
      return false;
    }
  }

  for (const [edgeId, rightEdge] of right.edges) {
    const leftEdge = left.edges.get(edgeId);

    if (!leftEdge || leftEdge.sourceId !== rightEdge.sourceId || leftEdge.targetId !== rightEdge.targetId) {
      return false;
    }
  }

  return true;
}

/**
 * Whether two measured graphs are equivalent layout inputs: the same node set, containment,
 * measured sizes, and edge set. Node and edge order is irrelevant.
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

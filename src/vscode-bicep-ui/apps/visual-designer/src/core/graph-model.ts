// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";
import type { Graph, GraphEdge, GraphNode, MeasuredGraph } from "./api";

export interface ClientGraphNode extends GraphNode {
  /** View-only state. Collapsing a module never changes the host graph. */
  readonly isCollapsed?: boolean;
}

/**
 * The latest graph from the host, indexed by id. Each graph update replaces it whole, so it is never
 * mutated and two updates can be compared by holding on to the previous one.
 */
export interface ClientGraph {
  readonly nodes: ReadonlyMap<string, ClientGraphNode>;
  readonly edges: ReadonlyMap<string, GraphEdge>;
}

export const EMPTY_CLIENT_GRAPH: ClientGraph = { nodes: new Map(), edges: new Map() };

export function indexGraph({ nodes, edges }: Graph): ClientGraph {
  return {
    nodes: new Map(nodes.map((node) => [node.id, node])),
    edges: new Map(edges.map((edge) => [edge.id, edge])),
  };
}

/** Whether `nodeId` is inside `ancestorId`, at any depth. */
export function isDescendant(graph: ClientGraph, nodeId: string, ancestorId: string): boolean {
  for (let id = graph.nodes.get(nodeId)?.parentId ?? null; id !== null; id = graph.nodes.get(id)?.parentId ?? null) {
    if (id === ancestorId) {
      return true;
    }
  }

  return false;
}

/**
 * Project the source graph into the locally visible graph.
 *
 * Descendants of collapsed modules are omitted. Dependencies that cross a collapsed boundary terminate
 * at the collapsed module so the relationship remains visible; dependencies wholly inside it disappear.
 */
export function projectGraph(graph: ClientGraph, collapsedNodeIds: ReadonlySet<string>): ClientGraph {
  const validCollapsedNodeIds = new Set(
    [...collapsedNodeIds].filter((nodeId) => {
      const node = graph.nodes.get(nodeId);
      return node?.kind === "module" && node.hasChildren;
    }),
  );

  if (validCollapsedNodeIds.size === 0) {
    return graph;
  }

  const visibleAncestorByNodeId = new Map<string, string>();

  const getVisibleAncestor = (nodeId: string): string => {
    const cached = visibleAncestorByNodeId.get(nodeId);
    if (cached) {
      return cached;
    }

    let currentId: string | null = nodeId;
    let visibleId = nodeId;

    while (currentId) {
      if (validCollapsedNodeIds.has(currentId)) {
        visibleId = currentId;
      }
      currentId = graph.nodes.get(currentId)?.parentId ?? null;
    }

    visibleAncestorByNodeId.set(nodeId, visibleId);
    return visibleId;
  };

  const nodes = new Map<string, ClientGraphNode>();
  for (const node of graph.nodes.values()) {
    const visibleId = getVisibleAncestor(node.id);
    if (visibleId !== node.id) {
      continue;
    }

    nodes.set(node.id, validCollapsedNodeIds.has(node.id) ? { ...node, hasChildren: false, isCollapsed: true } : node);
  }

  const edges = new Map<string, GraphEdge>();
  const endpointPairs = new Set<string>();

  for (const edge of graph.edges.values()) {
    const sourceId = getVisibleAncestor(edge.sourceId);
    const targetId = getVisibleAncestor(edge.targetId);
    const endpointPair = `${sourceId}\0${targetId}`;

    if (sourceId === targetId || endpointPairs.has(endpointPair)) {
      continue;
    }

    endpointPairs.add(endpointPair);
    const id = sourceId === edge.sourceId && targetId === edge.targetId ? edge.id : `${sourceId}->${targetId}`;
    edges.set(id, { id, sourceId, targetId });
  }

  return { nodes, edges };
}

/**
 * The graph as rendered, with each node's measured size: what the host lays out.
 *
 * `measuredBoxes` supplies those sizes, keyed by node id; a node with no entry reports zero, which is
 * the state before it has been laid out and measured.
 */
export function measureGraph(graph: ClientGraph, measuredBoxes: ReadonlyMap<string, Box>): MeasuredGraph {
  return {
    nodes: [...graph.nodes.values()].map(({ id, kind, parentId }) => {
      const box = measuredBoxes.get(id);

      return {
        id,
        kind,
        parentId,
        width: box ? box.max.x - box.min.x : 0,
        height: box ? box.max.y - box.min.y : 0,
      };
    }),
    edges: [...graph.edges.values()],
  };
}

/**
 * Whether two graphs would produce the same canvas.
 *
 * Compares exactly the fields the canvas renders. Nodes and edges are keyed by id, so ordering is
 * irrelevant.
 */
export function clientGraphsRenderEqually(left: ClientGraph, right: ClientGraph): boolean {
  if (left === right) {
    return true;
  }

  if (left.nodes.size !== right.nodes.size || left.edges.size !== right.edges.size) {
    return false;
  }

  for (const [nodeId, rightNode] of right.nodes) {
    const leftNode = left.nodes.get(nodeId);

    if (
      !leftNode ||
      leftNode.kind !== rightNode.kind ||
      leftNode.parentId !== rightNode.parentId ||
      leftNode.type !== rightNode.type ||
      leftNode.isCollection !== rightNode.isCollection ||
      leftNode.hasChildren !== rightNode.hasChildren ||
      leftNode.hasError !== rightNode.hasError ||
      leftNode.isCollapsed !== rightNode.isCollapsed
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
export function measuredGraphsEqual(left: MeasuredGraph | null, right: MeasuredGraph): boolean {
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

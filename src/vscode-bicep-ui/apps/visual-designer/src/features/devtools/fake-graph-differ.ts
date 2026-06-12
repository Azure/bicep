// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type {
  DeploymentGraph,
  DeploymentGraphNode,
  GraphEdge,
  GraphNode,
  GraphPatch,
  RenderedGraph,
} from "@/lib/messaging";

/**
 * A throwaway, dev-only stand-in for the language server's `VisualGraphDiffer`. It lets the
 * dev playground exercise the server-driven layout loop (`documentDidChange` → `getGraphUpdate`
 * → patches) without a running extension or language server.
 *
 * It is intentionally simpler than the real C# differ: it knows the full target graph (the dev
 * toolbar's sample/mutated `DeploymentGraph`) and emits a complete delta transforming the graph
 * the webview submitted into that target.
 */

/** Matches the edge identity the webview uses (`use-deployment-graph.ts` builds edges as `${source}>${target}`). */
function edgeId(sourceId: string, targetId: string): string {
  return `${sourceId}>${targetId}`;
}

/** Containment is encoded in the `::`-delimited id; the parent is everything before the last segment. */
function getParentId(id: string): string | null {
  const index = id.lastIndexOf("::");
  return index === -1 ? null : id.slice(0, index);
}

/** Depth = number of `::` segments. Used to remove children before parents and add parents before children. */
function getDepth(id: string): number {
  return id.split("::").length - 1;
}

function toCanonicalNode(node: DeploymentGraphNode): GraphNode {
  return {
    id: node.id,
    kind: node.type === "<module>" ? "module" : "resource",
    parentId: getParentId(node.id),
    type: node.type,
    symbolName: node.id.split("::").pop() ?? node.id,
    isCollection: node.isCollection,
    hasChildren: node.hasChildren,
    hasError: node.hasError,
    filePath: node.filePath,
    range: node.range,
  };
}

/**
 * Produce the patch list transforming `current` (the graph the webview displays) into `target`
 * (the dev toolbar's graph). Mirrors the ordering the real server guarantees: remove edges,
 * remove nodes deepest-first, add/update nodes shallowest-first, add edges, then error count.
 */
export function diffGraph(current: RenderedGraph | null, target: DeploymentGraph | null): GraphPatch[] {
  const targetNodes = new Map<string, GraphNode>();
  const targetEdges = new Map<string, GraphEdge>();
  let errorCount = 0;

  if (target) {
    for (const node of target.nodes) {
      targetNodes.set(node.id, toCanonicalNode(node));
    }
    for (const edge of target.edges) {
      const id = edgeId(edge.sourceId, edge.targetId);
      targetEdges.set(id, { id, sourceId: edge.sourceId, targetId: edge.targetId });
    }
    errorCount = target.errorCount;
  }

  const currentNodeIds = new Set((current?.nodes ?? []).map((node) => node.id));
  const currentEdgeIds = new Set((current?.edges ?? []).map((edge) => edge.id));

  // Empty target: clear in one shot rather than emitting a removal per node.
  if (targetNodes.size === 0) {
    return currentNodeIds.size === 0 ? [] : [{ op: "clearGraph" }];
  }

  const patches: GraphPatch[] = [];

  // 1. Remove edges that are gone.
  for (const id of currentEdgeIds) {
    if (!targetEdges.has(id)) {
      patches.push({ op: "removeEdge", edgeId: id });
    }
  }

  // 2. Remove nodes that are gone, deepest-first so children leave before their parents.
  const removedNodeIds = [...currentNodeIds]
    .filter((id) => !targetNodes.has(id))
    .sort((a, b) => getDepth(b) - getDepth(a));
  for (const id of removedNodeIds) {
    patches.push({ op: "removeNode", nodeId: id });
  }

  // 3. Add new nodes shallowest-first; refresh metadata on survivors.
  // The webview only submits node identity (id/kind/parentId/size), so the server can't tell which
  // metadata changed — it just re-pushes the current metadata for every survivor. updateNode never
  // triggers a relayout, so this is cheap.
  const orderedNodes = [...targetNodes.values()].sort((a, b) => getDepth(a.id) - getDepth(b.id));
  for (const node of orderedNodes) {
    if (currentNodeIds.has(node.id)) {
      patches.push({
        op: "updateNode",
        nodeId: node.id,
        changes: {
          type: node.type,
          isCollection: node.isCollection,
          hasChildren: node.hasChildren,
          hasError: node.hasError,
          filePath: node.filePath,
          range: node.range,
        },
      });
    } else {
      patches.push({ op: "addNode", node });
    }
  }

  // 4. Add new edges.
  for (const [id, edge] of targetEdges) {
    if (!currentEdgeIds.has(id)) {
      patches.push({ op: "addEdge", edge });
    }
  }

  // 5. Error count.
  patches.push({ op: "setErrorCount", errorCount });

  return patches;
}

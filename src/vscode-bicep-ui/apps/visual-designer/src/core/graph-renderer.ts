// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { NodeState } from "@/lib/graph";
import type { Box, Point } from "@/lib/math";
import type { ClientGraph } from "./graph-model";

import { createAtomicNode, createCompoundNode, edgesAtom, layoutReadyAtom, nodesByIdAtom } from "@/lib/graph";

type Store = Pick<ReturnType<typeof createStore>, "get" | "set">;

/**
 * Snapshot the current position (box.min) of every node so we can
 * restore positions for nodes that survive a graph update, giving
 * them a smooth transition to their new server-computed location
 * instead of jumping from (0,0).
 */
function snapshotNodePositions(store: Store): Map<string, Point> {
  const positions = new Map<string, Point>();
  const nodes = store.get(nodesByIdAtom);

  for (const [id, node] of Object.entries(nodes)) {
    const box = store.get(node.boxAtom);
    // Use the node's center so the centroid of existing positions
    // matches the visual center of the graph, not the top-left bias.
    positions.set(id, {
      x: (box.min.x + box.max.x) / 2,
      y: (box.min.y + box.max.y) / 2,
    });
  }

  return positions;
}

/**
 * How a node that is about to be mounted should arrive, for transitions that replace one node with
 * another under the same id (a module collapsing into a single node, or expanding back into a container).
 */
export interface NodeEntrance {
  /** Where an atomic node's center starts. */
  origin?: Point;
  /** The box a compound node grows from; its `enterProgressAtom` drives the growth. */
  enterFrom?: Box;
  /** Extra data for the node's content, read once when it mounts. */
  data?: Record<string, unknown>;
}

export interface ApplyGraphOptions {
  /** Where newly created nodes were dropped. */
  newNodeOrigins?: ReadonlyMap<string, Point>;
  /** How particular new nodes arrive. */
  entrances?: ReadonlyMap<string, NodeEntrance>;
  /**
   * Whether a layout of the whole graph will position the new nodes. When most of the graph is new, the graph is
   * then hidden until that layout reveals it, rather than showing new nodes piled at their origins. A caller that
   * positions every new node itself (a module expanding in place) passes false, since no such reveal follows.
   */
  awaitsLayout?: boolean;
}

/**
 * Mount, update, and remove canvas nodes and edges to match a graph. Positions
 * are left to layout; new nodes start at their `newNodeOrigins`, where they were dropped, or near the others.
 */
export function applyGraph(
  store: Store,
  graph: ClientGraph,
  viewportCenter: Point,
  { newNodeOrigins = new Map(), entrances = new Map(), awaitsLayout = true }: ApplyGraphOptions = {},
) {
  if (graph.nodes.size === 0) {
    // Empty graph — clear everything and re-engage the
    // visibility gate so the next non-empty graph can spawn
    // from the center without flashing.
    store.set(nodesByIdAtom, {});
    store.set(edgesAtom, []);
    store.set(layoutReadyAtom, false);
    return;
  }

  // Snapshot positions before modifying so surviving nodes
  // can animate from their current location.
  const previousPositions = snapshotNodePositions(store);

  // ── Classify incoming nodes ──
  const parentChildMap = new Map<string, string[]>(); // parentId → childIds[]

  for (const node of graph.nodes.values()) {
    if (node.hasChildren) {
      parentChildMap.set(node.id, []);
    }
  }

  // Build the visible parent-child relationships from the graph contract.
  for (const node of graph.nodes.values()) {
    if (node.parentId) {
      parentChildMap.get(node.parentId)?.push(node.id);
    }
  }

  // Demote compound nodes that ended up with no actual children
  // (e.g. after a mutation removed all child nodes). They become
  // atomic (leaf) nodes so they are draggable and render properly.
  for (const [id, children] of parentChildMap) {
    if (children.length === 0) {
      parentChildMap.delete(id);
    }
  }

  // ── Diff-and-patch: update in place instead of clear-and-rebuild ──
  const currentNodes = store.get(nodesByIdAtom);
  const survivingCount = Object.keys(currentNodes).filter((id) => graph.nodes.has(id)).length;

  // Hide the graph layer when most of the topology is being replaced
  // so the user doesn't see new nodes piled at the spawn origin while
  // graph layout computes. Incremental edits (adding/removing a few nodes)
  // keep the graph visible for smooth in-place animation.
  if (awaitsLayout && survivingCount / graph.nodes.size < 0.5) {
    store.set(layoutReadyAtom, false);
  }

  // Default origin for brand-new nodes.
  // When the graph was previously empty (no existing positions),
  // use the viewport center so nodes spawn at the center of the
  // canvas and animate outward.  On subsequent updates, use the
  // centroid of existing positions so new nodes animate in from
  // a natural location.
  const positions = [...previousPositions.values()];
  const defaultOrigin =
    positions.length > 0
      ? {
          x: positions.reduce((sum, p) => sum + p.x, 0) / positions.length,
          y: positions.reduce((sum, p) => sum + p.y, 0) / positions.length,
        }
      : viewportCenter;

  // Build the next node map in one pass and set it once, so the canvas updates once per graph rather
  // than once per node. Nodes that no longer exist are simply left out.
  const nextNodes: Record<string, NodeState> = {};

  for (const node of graph.nodes.values()) {
    const existing = currentNodes[node.id];
    const symbolicName = node.id.split("::").pop() ?? node.id;
    const childIds = parentChildMap.get(node.id);

    if (childIds) {
      const data = {
        nodeKind: node.kind,
        symbolicName,
        isCollection: node.isCollection,
        hasError: node.hasError,
        isCollapsed: false,
        canCollapse: true,
      };
      if (existing?.kind === "compound") {
        store.set(existing.childIdsAtom, childIds);
        store.set(existing.dataAtom, data);
        nextNodes[node.id] = existing;
      } else {
        // New, or an atomic node that gained children: a different node type, so it is replaced.
        const entrance = entrances.get(node.id);
        nextNodes[node.id] = createCompoundNode(node.id, childIds, { ...data, ...entrance?.data }, entrance?.enterFrom);
      }
      continue;
    }

    // A module that requires restoring is collapsed in effect: expanding it restores, then shows its contents.
    const isCollapsed = node.isCollapsed === true || node.requiresRestore;
    const data = {
      nodeKind: node.kind,
      symbolicName,
      resourceType: node.type,
      isCollection: node.isCollection,
      hasError: node.hasError,
      isCollapsed,
      canCollapse: node.kind === "module" && (node.hasChildren || isCollapsed),
    };
    if (existing?.kind === "atomic") {
      store.set(existing.dataAtom, data);
      nextNodes[node.id] = existing;
    } else {
      const entrance = entrances.get(node.id);
      const origin = newNodeOrigins.get(node.id) ?? entrance?.origin ?? previousPositions.get(node.id) ?? defaultOrigin;
      nextNodes[node.id] = createAtomicNode(node.id, origin, {
        ...data,
        ...entrance?.data,
        replacesPlaceholder: newNodeOrigins.has(node.id),
      });
    }
  }

  store.set(nodesByIdAtom, nextNodes);

  // Edges are plain values with no atom identity to preserve, so replace them whenever the set changes.
  const currentEdgeIds = new Set(store.get(edgesAtom).map((edge) => edge.id));
  const nextEdges = [...graph.edges.values()].map(({ sourceId, targetId }) => ({
    id: `${sourceId}>${targetId}`,
    fromId: sourceId,
    toId: targetId,
  }));

  if (nextEdges.length !== currentEdgeIds.size || nextEdges.some((edge) => !currentEdgeIds.has(edge.id))) {
    store.set(edgesAtom, nextEdges);
  }

  // Node positions and the visibility reveal are applied separately via
  // applyGraphLayout once the server returns the computed layout. The visibility
  // gate set above is preserved until then.
}

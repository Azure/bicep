// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { NodeState } from "@/lib/graph";
import type { Point } from "@/lib/math";
import type { ClientGraph } from "../graph-model";

import { useStore } from "jotai";
import { useCallback } from "react";
import { createAtomicNode, createCompoundNode, edgesAtom, layoutReadyAtom, nodesByIdAtom } from "@/lib/graph";

type Store = ReturnType<typeof createStore>;

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
 * Returns a function that mounts, updates, and removes canvas nodes and edges to match a graph. Positions
 * are left to layout; new nodes start at `newNodeOrigins`, where they were dropped, or near the others.
 */
export function useApplyGraph(getViewportCenter: () => Point) {
  const store = useStore();

  return useCallback(
    (graph: ClientGraph, newNodeOrigins: ReadonlyMap<string, Point> = new Map()) => {
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

      // Build parent-child relationships from :: delimited IDs
      for (const node of graph.nodes.values()) {
        const segments = node.id.split("::");
        if (segments.length > 1) {
          parentChildMap.get(segments.slice(0, -1).join("::"))?.push(node.id);
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
      if (survivingCount / graph.nodes.size < 0.5) {
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
          : getViewportCenter();

      // Build the next node map in one pass and set it once, so the canvas updates once per graph rather
      // than once per node. Nodes that no longer exist are simply left out.
      const nextNodes: Record<string, NodeState> = {};

      for (const node of graph.nodes.values()) {
        const existing = currentNodes[node.id];
        const symbolicName = node.id.split("::").pop() ?? node.id;
        const childIds = parentChildMap.get(node.id);

        if (childIds) {
          const data = { symbolicName, isCollection: node.isCollection, hasError: node.hasError };
          if (existing?.kind === "compound") {
            store.set(existing.childIdsAtom, childIds);
            store.set(existing.dataAtom, data);
            nextNodes[node.id] = existing;
          } else {
            // New, or an atomic node that gained children: a different node type, so it is replaced.
            nextNodes[node.id] = createCompoundNode(node.id, childIds, data);
          }
          continue;
        }

        const data = {
          symbolicName,
          resourceType: node.type,
          isCollection: node.isCollection,
          hasError: node.hasError,
        };
        if (existing?.kind === "atomic") {
          store.set(existing.dataAtom, data);
          nextNodes[node.id] = existing;
        } else {
          const origin = newNodeOrigins.get(node.id) ?? previousPositions.get(node.id) ?? defaultOrigin;
          nextNodes[node.id] = createAtomicNode(node.id, origin, {
            ...data,
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
    },
    [getViewportCenter, store],
  );
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Point } from "@/lib/math";
import type { ClientGraph } from "../graph-model";

import { useSetAtom, useStore } from "jotai";
import { useCallback, useRef } from "react";
import { reportGraphStatusAtom } from "@/features/status";
import {
  addAtomicNodeAtom,
  addCompoundNodeAtom,
  addEdgeAtom,
  edgesAtom,
  layoutReadyAtom,
  nodesByIdAtom,
  removeNodesAtom,
} from "@/lib/graph";
import { clientGraphsRenderEqually } from "../graph-model";

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

export function useApplyGraph(getViewportCenter: () => Point) {
  const store = useStore();
  const setEdgesAtom = useSetAtom(edgesAtom);
  const addAtomicNode = useSetAtom(addAtomicNodeAtom);
  const addCompoundNode = useSetAtom(addCompoundNodeAtom);
  const addEdge = useSetAtom(addEdgeAtom);
  const removeNodes = useSetAtom(removeNodesAtom);
  const setLayoutReady = useSetAtom(layoutReadyAtom);
  const appliedGraphRef = useRef<ClientGraph | null>(null);

  return useCallback(
    (graph: ClientGraph | null, newNodeOrigins: ReadonlyMap<string, Point> = new Map()) => {
      // Report the graph facts that features/status derives its display from.
      store.set(reportGraphStatusAtom, {
        errorCount: graph?.errorCount ?? 0,
        hasNodes: (graph?.nodes.size ?? 0) > 0,
      });

      // Nothing the canvas shows has changed, so leave the mounted nodes alone rather than tearing
      // the graph down and re-laying it out. Most keystrokes land here.
      if (clientGraphsRenderEqually(appliedGraphRef.current, graph)) {
        return;
      }

      // The graph is mutated in place, so keep a shallow copy: holding the live reference would
      // compare it against itself on the next pass and report equal every time.
      appliedGraphRef.current = graph && {
        nodes: new Map(graph.nodes),
        edges: new Map(graph.edges),
        errorCount: graph.errorCount,
      };

      if (!graph || graph.nodes.size === 0) {
        // Empty graph — clear everything and re-engage the
        // visibility gate so the next non-empty graph can spawn
        // from the center without flashing.
        removeNodes(new Set(Object.keys(store.get(nodesByIdAtom))));
        setEdgesAtom([]);
        setLayoutReady(false);
        return;
      }

      // Snapshot positions before modifying so surviving nodes
      // can animate from their current location.
      const previousPositions = snapshotNodePositions(store);

      // ── Classify incoming nodes ──
      const compoundNodeIds = new Set<string>();
      const parentChildMap = new Map<string, string[]>(); // parentId → childIds[]

      for (const node of graph.nodes.values()) {
        if (node.hasChildren) {
          compoundNodeIds.add(node.id);
          parentChildMap.set(node.id, []);
        }
      }

      // Build parent-child relationships from :: delimited IDs
      for (const node of graph.nodes.values()) {
        const segments = node.id.split("::");
        if (segments.length > 1) {
          const parentId = segments.slice(0, -1).join("::");
          if (parentChildMap.has(parentId)) {
            parentChildMap.get(parentId)!.push(node.id);
          }
        }
      }

      // Demote compound nodes that ended up with no actual children
      // (e.g. after a mutation removed all child nodes). They become
      // atomic (leaf) nodes so they are draggable and render properly.
      for (const [id, children] of parentChildMap) {
        if (children.length === 0) {
          compoundNodeIds.delete(id);
          parentChildMap.delete(id);
        }
      }

      // ── Diff-and-patch: update in-place instead of clear-and-rebuild ──
      const currentNodes = store.get(nodesByIdAtom);
      const newNodeIds = new Set(graph.nodes.keys());
      const currentNodeIds = new Set(Object.keys(currentNodes));

      // Phase 1: Remove nodes that no longer exist.
      const idsToRemove = new Set<string>();
      for (const id of currentNodeIds) {
        if (!newNodeIds.has(id)) {
          idsToRemove.add(id);
        }
      }
      if (idsToRemove.size > 0) {
        removeNodes(idsToRemove);
      }

      // Hide the graph layer when most of the topology is being replaced
      // so the user doesn't see new nodes piled at the spawn origin while
      // graph layout computes. Incremental edits (adding/removing a few nodes)
      // keep the graph visible for smooth in-place animation.
      const survivingCount = currentNodeIds.size - idsToRemove.size;
      const survivalRatio = graph.nodes.size > 0 ? survivingCount / graph.nodes.size : 0;
      if (survivalRatio < 0.5) {
        setLayoutReady(false);
      }

      // Phase 2: Default origin for brand-new nodes.
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

      // Phase 3: Update surviving nodes in-place / add new atomic nodes.
      for (const node of graph.nodes.values()) {
        if (compoundNodeIds.has(node.id)) {
          continue; // Compound nodes handled in Phase 4.
        }

        const existing = currentNodes[node.id];
        const symbol = node.id.split("::").pop()!;

        if (existing && !idsToRemove.has(node.id)) {
          // Node survived — check if its kind changed.
          const newKind = "atomic";
          if (existing.kind !== newKind) {
            // Kind changed (compound → atomic): remove and re-add.
            removeNodes(new Set([node.id]));
          } else {
            // Same kind — update data in-place, skip re-creation.
            store.set(existing.dataAtom, () => ({
              symbolicName: symbol,
              resourceType: node.type,
              isCollection: node.isCollection,
              hasError: node.hasError,
            }));
            continue;
          }
        }

        // New node (or re-added after kind change) — create it.
        const origin = newNodeOrigins.get(node.id) ?? previousPositions.get(node.id) ?? defaultOrigin;
        addAtomicNode(node.id, origin, {
          symbolicName: symbol,
          resourceType: node.type,
          isCollection: node.isCollection,
          hasError: node.hasError,
        });
      }

      // Phase 4: Update surviving compound nodes / add new ones.
      for (const node of graph.nodes.values()) {
        if (!compoundNodeIds.has(node.id)) {
          continue;
        }

        const existing = currentNodes[node.id];
        const symbol = node.id.split("::").pop()!;
        const childIds = parentChildMap.get(node.id) ?? [];

        if (existing && !idsToRemove.has(node.id) && existing.kind === "compound") {
          // Compound node survived — update children and data in-place.
          store.set(existing.childIdsAtom, childIds);
          store.set(existing.dataAtom, () => ({
            symbolicName: symbol,
            isCollection: node.isCollection,
            hasError: node.hasError,
          }));
        } else {
          // New compound node (or kind changed from atomic → compound).
          if (existing && !idsToRemove.has(node.id)) {
            // Kind changed — remove old atomic node first.
            removeNodes(new Set([node.id]));
          }
          addCompoundNode(node.id, childIds, {
            symbolicName: symbol,
            isCollection: node.isCollection,
            hasError: node.hasError,
          });
        }
      }

      // Phase 5: Diff edges — replace only if the set changed.
      const currentEdges = store.get(edgesAtom);
      const newEdgeIds = new Set([...graph.edges.values()].map((e) => `${e.sourceId}>${e.targetId}`));
      const currentEdgeIds = new Set(currentEdges.map((e) => e.id));

      const edgesChanged =
        newEdgeIds.size !== currentEdgeIds.size || [...newEdgeIds].some((id) => !currentEdgeIds.has(id));

      if (edgesChanged) {
        // Rebuild edges in one shot (edges are lightweight value objects
        // with no atom identity to preserve).
        setEdgesAtom([]);
        for (const edge of graph.edges.values()) {
          addEdge(`${edge.sourceId}>${edge.targetId}`, edge.sourceId, edge.targetId);
        }
      }

      // Node positions and the visibility reveal are applied separately via
      // applyGraphLayout once the server returns the computed layout. The visibility
      // gate set above is preserved until then.
    },
    [setEdgesAtom, addAtomicNode, addCompoundNode, addEdge, removeNodes, setLayoutReady, getViewportCenter, store],
  );
}

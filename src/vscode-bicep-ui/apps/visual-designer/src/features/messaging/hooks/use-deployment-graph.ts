// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DeploymentGraph } from "../../../messages";
import type { Point } from "../../../utils/math/geometry";

import { getDefaultStore, useSetAtom } from "jotai";
import { useCallback, useRef } from "react";
import { isDeploymentGraphEqual } from "../../../utils/deployment-graph-equality";
import {
  addAtomicNodeAtom,
  addCompoundNodeAtom,
  addEdgeAtom,
  edgesAtom,
  errorCountAtom,
  graphVersionAtom,
  hasNodesAtom,
  layoutReadyAtom,
  nodesByIdAtom,
  removeNodesAtom,
} from "../../graph-engine";

const store = getDefaultStore();

/**
 * Snapshot the current position (box.min) of every node so we can
 * restore positions for nodes that survive a graph update, giving
 * them a smooth transition to their new ELK-computed location
 * instead of jumping from (0,0).
 */
function snapshotNodePositions(): Map<string, Point> {
  const positions = new Map<string, Point>();
  const nodes = store.get(nodesByIdAtom);

  for (const [id, node] of Object.entries(nodes)) {
    const box = store.get(node.boxAtom);
    positions.set(id, { x: box.min.x, y: box.min.y });
  }

  return positions;
}

export function useApplyDeploymentGraph(getViewportCenter: () => Point) {
  const setEdgesAtom = useSetAtom(edgesAtom);
  const addAtomicNode = useSetAtom(addAtomicNodeAtom);
  const addCompoundNode = useSetAtom(addCompoundNodeAtom);
  const addEdge = useSetAtom(addEdgeAtom);
  const removeNodes = useSetAtom(removeNodesAtom);
  const setGraphVersion = useSetAtom(graphVersionAtom);
  const setLayoutReady = useSetAtom(layoutReadyAtom);
  const previousGraphRef = useRef<DeploymentGraph | null>(null);

  return useCallback(
    (graph: DeploymentGraph | null) => {
      // Update status bar atoms
      store.set(errorCountAtom, graph?.errorCount ?? 0);
      store.set(hasNodesAtom, (graph?.nodes.length ?? 0) > 0);

      // If the graph topology hasn't changed (only ranges differ due
      // to trivial edits like adding blank lines), update ranges on
      // existing nodes in-place without tearing down and re-laying out.
      if (isDeploymentGraphEqual(previousGraphRef.current, graph)) {
        if (graph) {
          const nodes = store.get(nodesByIdAtom);
          for (const node of graph.nodes) {
            const existing = nodes[node.id];
            if (existing) {
              store.set(existing.dataAtom, (prev: Record<string, unknown>) => ({
                ...prev,
                range: node.range,
                filePath: node.filePath,
              }));
            }
          }
        }
        previousGraphRef.current = graph;
        return;
      }
      previousGraphRef.current = graph;

      if (!graph || graph.nodes.length === 0) {
        // Empty graph — clear everything and re-engage the
        // visibility gate so the next non-empty graph can spawn
        // from the center without flashing.
        removeNodes(new Set(Object.keys(store.get(nodesByIdAtom))));
        setEdgesAtom([]);
        setGraphVersion(0);
        setLayoutReady(false);
        return;
      }

      // Snapshot positions before modifying so surviving nodes
      // can animate from their current location.
      const previousPositions = snapshotNodePositions();

      // ── Classify incoming nodes ──
      const compoundNodeIds = new Set<string>();
      const parentChildMap = new Map<string, string[]>(); // parentId → childIds[]

      for (const node of graph.nodes) {
        if (node.hasChildren) {
          compoundNodeIds.add(node.id);
          parentChildMap.set(node.id, []);
        }
      }

      // Build parent-child relationships from :: delimited IDs
      for (const node of graph.nodes) {
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
      const newNodeIds = new Set(graph.nodes.map((n) => n.id));
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
      for (const node of graph.nodes) {
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
            store.set(existing.dataAtom, () =>
              node.type === "<module>"
                ? {
                    symbolicName: symbol,
                    path: node.filePath,
                    isCollection: node.isCollection,
                    hasError: node.hasError,
                    range: node.range,
                    filePath: node.filePath,
                  }
                : {
                    symbolicName: symbol,
                    resourceType: node.type,
                    isCollection: node.isCollection,
                    hasError: node.hasError,
                    range: node.range,
                    filePath: node.filePath,
                  },
            );
            continue;
          }
        }

        // New node (or re-added after kind change) — create it.
        const origin = previousPositions.get(node.id) ?? defaultOrigin;
        if (node.type === "<module>") {
          addAtomicNode(node.id, origin, {
            symbolicName: symbol,
            path: node.filePath,
            isCollection: node.isCollection,
            hasError: node.hasError,
            range: node.range,
            filePath: node.filePath,
          });
        } else {
          addAtomicNode(node.id, origin, {
            symbolicName: symbol,
            resourceType: node.type,
            isCollection: node.isCollection,
            hasError: node.hasError,
            range: node.range,
            filePath: node.filePath,
          });
        }
      }

      // Phase 4: Update surviving compound nodes / add new ones.
      for (const node of graph.nodes) {
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
            path: node.filePath,
            isCollection: node.isCollection,
            hasError: node.hasError,
            range: node.range,
            filePath: node.filePath,
          }));
        } else {
          // New compound node (or kind changed from atomic → compound).
          if (existing && !idsToRemove.has(node.id)) {
            // Kind changed — remove old atomic node first.
            removeNodes(new Set([node.id]));
          }
          addCompoundNode(node.id, childIds, {
            symbolicName: symbol,
            path: node.filePath,
            isCollection: node.isCollection,
            hasError: node.hasError,
            range: node.range,
            filePath: node.filePath,
          });
        }
      }

      // Phase 5: Diff edges — replace only if the set changed.
      const currentEdges = store.get(edgesAtom);
      const newEdgeIds = new Set(graph.edges.map((e) => `${e.sourceId}>${e.targetId}`));
      const currentEdgeIds = new Set(currentEdges.map((e) => e.id));

      const edgesChanged =
        newEdgeIds.size !== currentEdgeIds.size || [...newEdgeIds].some((id) => !currentEdgeIds.has(id));

      if (edgesChanged) {
        // Rebuild edges in one shot (edges are lightweight value objects
        // with no atom identity to preserve).
        setEdgesAtom([]);
        for (const edge of graph.edges) {
          addEdge(`${edge.sourceId}>${edge.targetId}`, edge.sourceId, edge.targetId);
        }
      }

      // Phase 6: Bump graph version so subscribers (e.g. useLayoutEffect
      // in App) can trigger ELK layout after the DOM reflects the new graph.
      setGraphVersion((v) => v + 1);
    },
    [
      setEdgesAtom,
      addAtomicNode,
      addCompoundNode,
      addEdge,
      removeNodes,
      setGraphVersion,
      setLayoutReady,
      getViewportCenter,
    ],
  );
}

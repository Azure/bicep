// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "../../../utils/math/geometry";

import { getDefaultStore, useSetAtom } from "jotai";
import { useCallback, useRef } from "react";
import {
  addAtomicNodeAtom,
  addCompoundNodeAtom,
  addEdgeAtom,
  edgesAtom,
  errorCountAtom,
  graphVersionAtom,
  hasNodesAtom,
  nodesByIdAtom,
} from "../../graph-engine";
import type { DeploymentGraph } from "../../../messages";
import { isDeploymentGraphEqual } from "../../../utils/deployment-graph-equality";

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

export function useApplyDeploymentGraph() {
  const setNodesByIdAtom = useSetAtom(nodesByIdAtom);
  const setEdgesAtom = useSetAtom(edgesAtom);
  const addAtomicNode = useSetAtom(addAtomicNodeAtom);
  const addCompoundNode = useSetAtom(addCompoundNodeAtom);
  const addEdge = useSetAtom(addEdgeAtom);
  const setGraphVersion = useSetAtom(graphVersionAtom);
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

      // Snapshot positions before clearing so surviving nodes
      // can animate from their current location.
      const previousPositions = snapshotNodePositions();

      // Clear existing state
      setNodesByIdAtom({});
      setEdgesAtom([]);

      if (!graph || graph.nodes.length === 0) {
        return;
      }

      // Classify nodes
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

      // Phase 1: Add all atomic nodes
      // Use the previous position if the node existed before.
      // For brand-new nodes, use the center of the previous graph
      // (so they animate in from a natural location) or off-screen
      // if there was no previous graph (first render).
      const positions = [...previousPositions.values()];
      const defaultOrigin =
        positions.length > 0
          ? {
              x: positions.reduce((sum, p) => sum + p.x, 0) / positions.length,
              y: positions.reduce((sum, p) => sum + p.y, 0) / positions.length,
            }
          : { x: -200, y: -200 };
      for (const node of graph.nodes) {
        if (!compoundNodeIds.has(node.id)) {
          const symbol = node.id.split("::").pop()!;
          const origin = previousPositions.get(node.id) ?? defaultOrigin;

          if (node.type === "<module>") {
            // Module demoted to leaf (no children) — use module data shape
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
      }

      // Phase 2: Add compound nodes
      for (const node of graph.nodes) {
        if (compoundNodeIds.has(node.id)) {
          const symbol = node.id.split("::").pop()!;
          const childIds = parentChildMap.get(node.id) ?? [];
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

      // Phase 3: Add edges
      for (const edge of graph.edges) {
        addEdge(`${edge.sourceId}>${edge.targetId}`, edge.sourceId, edge.targetId);
      }

      // Phase 4: Bump graph version so subscribers (e.g. useLayoutEffect
      // in App) can trigger ELK layout after the DOM reflects the new graph.
      setGraphVersion((v) => v + 1);
    },
    [setNodesByIdAtom, setEdgesAtom, addAtomicNode, addCompoundNode, addEdge, setGraphVersion],
  );
}

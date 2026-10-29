// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Point } from "@/lib/math";
import type { GraphNodeSourceStatus } from "./api";
import type { ClientGraph, ClientGraphNode } from "./graph-model";

import { layoutReadyAtom, setGraphAtom } from "@/lib/graph";
import { pendingSubgraphLayoutsAtom, shownGraphAtom } from "./atoms";
import { isCollapsible, isDescendant } from "./graph-model";
import { getCentroid, getNodePositions, getPositionedNodeIds, setNodePositions } from "./node-positions";

type Store = ReturnType<typeof createStore>;

/** What the canvas renders for a node. */
export interface CanvasNodeData {
  nodeKind: ClientGraphNode["kind"];
  symbolicName: string;
  resourceType: string;
  isCollection: boolean;
  hasError: boolean;
  /** A module shown with its children hidden. */
  isCollapsed: boolean;
  /** A module with children to show or hide. */
  canCollapse: boolean;
  /** Why a remote module shows no children, or null. */
  sourceStatus: GraphNodeSourceStatus | null;
  /** What went wrong, when `sourceStatus` is `error`. */
  sourceError: string | null;
  /** The node replaces the placeholder shown where it was dropped, so it grows from the placeholder's size. */
  replacesPlaceholder: boolean;
}

function toCanvasNodeData(node: ClientGraphNode, replacesPlaceholder: boolean): CanvasNodeData {
  return {
    nodeKind: node.kind,
    symbolicName: node.symbolName,
    resourceType: node.type,
    isCollection: node.isCollection,
    hasError: node.hasError,
    isCollapsed: node.isCollapsed === true,
    canCollapse: isCollapsible(node),
    sourceStatus: node.sourceStatus,
    sourceError: node.sourceError,
    replacesPlaceholder,
  };
}

export interface ShowGraphOptions {
  /** Where particular new nodes appear, such as a dropped resource or the children of an opening module. */
  origins?: ReadonlyMap<string, Point>;
  /** Where other new nodes appear when no shown node has a position yet. */
  getDefaultOrigin: () => Point;
  /** New nodes that replace a placeholder. */
  placeholderNodeIds?: ReadonlySet<string>;
}

/** Whether most of `next` is new, as when another graph replaces the one shown. */
export function isMostlyNewGraph(previous: ClientGraph, next: ClientGraph): boolean {
  const survivingCount = [...next.nodes.keys()].filter((nodeId) => previous.nodes.has(nodeId)).length;
  return survivingCount / next.nodes.size < 0.5;
}

/**
 * Show `graph` on the canvas.
 *
 * A node without a position appears at its origin, or among the other nodes, until a layout places it. One inside a
 * subgraph waiting to be laid out (see `pendingSubgraphLayoutsAtom`) is left without one instead: it renders hidden,
 * so it is measured, and its subgraph's root stays as it was until the layout places them together.
 * Returns the IDs of the nodes that had no position.
 */
export function showGraph(
  store: Store,
  graph: ClientGraph,
  { origins = new Map(), getDefaultOrigin, placeholderNodeIds = new Set() }: ShowGraphOptions,
): string[] {
  store.set(shownGraphAtom, graph);

  if (graph.nodes.size === 0) {
    // Hide the canvas again, so the next graph does not flash at its spawn point.
    store.set(setGraphAtom, { nodes: [], edges: [] });
    store.set(layoutReadyAtom, false);
    return [];
  }

  const positions = getNodePositions(store);
  const positionedNodeIds = getPositionedNodeIds(graph);
  const unplacedNodeIds = positionedNodeIds.filter((nodeId) => !positions.has(nodeId));
  const subgraphRootIds = [...store.get(pendingSubgraphLayoutsAtom).keys()];
  const placedNodeIds = unplacedNodeIds.filter(
    (nodeId) => !subgraphRootIds.some((rootId) => isDescendant(graph, nodeId, rootId)),
  );

  if (placedNodeIds.length > 0) {
    const defaultOrigin =
      getCentroid(positionedNodeIds.flatMap((nodeId) => positions.get(nodeId) ?? [])) ?? getDefaultOrigin();
    const next = new Map(positions);

    for (const nodeId of placedNodeIds) {
      next.set(nodeId, origins.get(nodeId) ?? defaultOrigin);
    }

    setNodePositions(store, next, "instant");
  }

  store.set(setGraphAtom, {
    nodes: [...graph.nodes.values()].map((node) => ({
      id: node.id,
      parentId: node.parentId,
      data: toCanvasNodeData(node, placeholderNodeIds.has(node.id)),
    })),
    edges: [...graph.edges.values()],
  });

  return unplacedNodeIds;
}

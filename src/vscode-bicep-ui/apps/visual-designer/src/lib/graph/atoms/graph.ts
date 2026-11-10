// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { createStore } from "jotai";
import type { Box, Point } from "@/lib/math";
import type { EdgeAtomValue } from "./edges";
import type { NodeState, Size } from "./nodes";

import { atom } from "jotai";
import { getBoxCenter } from "@/lib/math";
import { edgesAtom } from "./edges";
import { createNodeState, nodesByIdAtom } from "./nodes";

export interface GraphNodeInput {
  id: string;
  /** The container node this one is drawn inside, or null at the root. */
  parentId: string | null;
  /** Passed to the node content renderer. */
  data: unknown;
}

export interface GraphEdgeInput {
  id: string;
  sourceId: string;
  targetId: string;
}

export interface GraphInput {
  nodes: readonly GraphNodeInput[];
  edges: readonly GraphEdgeInput[];
}

/** Counts `setGraphAtom` calls. */
export const graphVersionAtom = atom(0);

/** The `graphVersionAtom` value whose nodes React last committed. Set by `NodeLayer`. */
export const committedGraphVersionAtom = atom(0);

/**
 * Whether React has committed the graph last set: every node in it has rendered its current data and
 * measured itself, so the nodes' target boxes are up to date.
 */
export const graphCommittedAtom = atom((get) => get(committedGraphVersionAtom) === get(graphVersionAtom));

function sameIds(left: readonly string[], right: readonly string[]): boolean {
  return left.length === right.length && left.every((id, index) => id === right[index]);
}

function sameEdges(left: readonly EdgeAtomValue[], right: readonly EdgeAtomValue[]): boolean {
  return (
    left.length === right.length &&
    left.every(
      (edge, index) =>
        edge.id === right[index]?.id && edge.fromId === right[index].fromId && edge.toId === right[index].toId,
    )
  );
}

/**
 * Shows a graph. Nodes are matched by ID: a node whose parent is unchanged keeps its state, so it
 * stays where it is drawn and animates to any new position, and nodes not in `graph` are removed.
 * Positions come from `nodePositionsAtom`.
 */
export const setGraphAtom = atom(null, (get, set, graph: GraphInput) => {
  const currentNodes = get(nodesByIdAtom);
  const childIdsByParent = new Map<string, string[]>();
  const nodeIds = new Set(graph.nodes.map((node) => node.id));

  for (const node of graph.nodes) {
    if (node.parentId !== null && nodeIds.has(node.parentId)) {
      const childIds = childIdsByParent.get(node.parentId);
      if (childIds) {
        childIds.push(node.id);
      } else {
        childIdsByParent.set(node.parentId, [node.id]);
      }
    }
  }

  const nextNodes: Record<string, NodeState> = {};
  let isNodeSetChanged = Object.keys(currentNodes).length !== graph.nodes.length;

  for (const { id, parentId, data } of graph.nodes) {
    const childIds = childIdsByParent.get(id) ?? [];
    const existing = currentNodes[id];

    if (existing && existing.parentId === parentId) {
      if (!sameIds(get(existing.childIdsAtom), childIds)) {
        set(existing.childIdsAtom, childIds);
      }
      set(existing.dataAtom, data);
      nextNodes[id] = existing;
    } else {
      nextNodes[id] = createNodeState(id, parentId, childIds, data);
      isNodeSetChanged = true;
    }
  }

  if (isNodeSetChanged) {
    set(nodesByIdAtom, nextNodes);
  }

  set(graphVersionAtom, (version) => version + 1);

  const nextEdges = graph.edges.map(({ id, sourceId, targetId }) => ({ id, fromId: sourceId, toId: targetId }));
  if (!sameEdges(get(edgesAtom), nextEdges)) {
    set(edgesAtom, nextEdges);
  }
});

/**
 * Whether the graph may be shown. The app keeps it hidden until its nodes have positions, so a new
 * graph does not flash at the spot where its nodes were spawned.
 */
export const layoutReadyAtom = atom(false);

/** The box enclosing every node's target box, or null when no node has one. */
export const graphBoundsAtom = atom<Box | null>((get) => {
  const boxes = Object.values(get(nodesByIdAtom)).flatMap((node) => {
    const box = get(node.targetBoxAtom);
    return box ? [box] : [];
  });

  if (boxes.length === 0) {
    return null;
  }

  return {
    min: { x: Math.min(...boxes.map((box) => box.min.x)), y: Math.min(...boxes.map((box) => box.min.y)) },
    max: { x: Math.max(...boxes.map((box) => box.max.x)), y: Math.max(...boxes.map((box) => box.max.y)) },
  };
});

/** A shown node's target box: where it belongs, at its measured size, whether or not it has arrived. */
export function getNodeTargetBox(store: ReturnType<typeof createStore>, nodeId: string): Box | null {
  const node = store.get(nodesByIdAtom)[nodeId];
  return node ? store.get(node.targetBoxAtom) : null;
}

/** Each shown node's target box: where it belongs, at its measured size, whether or not it has arrived. */
export function getNodeTargetBoxes(store: ReturnType<typeof createStore>): Map<string, Box> {
  const boxes = new Map<string, Box>();

  for (const [id, node] of Object.entries(store.get(nodesByIdAtom))) {
    const box = store.get(node.targetBoxAtom);
    if (box) {
      boxes.set(id, box);
    }
  }

  return boxes;
}

/**
 * Each shown node's measured content size. A node renders, and so measures itself, before it is placed; a
 * container's is the size it last had as a leaf, since containers are sized by their children.
 */
export function getNodeSizes(store: ReturnType<typeof createStore>): Map<string, Size> {
  const sizes = new Map<string, Size>();

  for (const [id, node] of Object.entries(store.get(nodesByIdAtom))) {
    const size = store.get(node.sizeAtom);
    if (size) {
      sizes.set(id, size);
    }
  }

  return sizes;
}

/**
 * Where each of `nodeIds` is drawn now, which differs from its position while it animates there. A
 * gesture that grabs a moving node starts from here.
 */
export function getDrawnCenters(store: ReturnType<typeof createStore>, nodeIds: Iterable<string>): Map<string, Point> {
  const nodesById = store.get(nodesByIdAtom);
  const centers = new Map<string, Point>();

  for (const nodeId of nodeIds) {
    const node = nodesById[nodeId];
    const box = node ? store.get(node.boxAtom) : null;
    if (box) {
      centers.set(nodeId, getBoxCenter(box));
    }
  }

  return centers;
}

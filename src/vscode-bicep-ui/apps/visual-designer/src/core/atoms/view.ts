// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";

import { atom } from "jotai";
import { sourceGraphAtom } from "./graph";

/**
 * The ID of the focused node, or null when nothing is focused. Clicking a node focuses it and
 * clicking the canvas background clears it.
 */
export const focusedNodeIdAtom = atom<string | null>(null);

/**
 * The nodes painted above their siblings, from least to most recently raised, as windows are stacked: those the
 * user focused, dragged, or opened. Dragging a node raises it without focusing it. Write it only through
 * `raiseNodeAtom`.
 */
export const raisedNodeIdsAtom = atom<readonly string[]>([]);

/** Paint a node above its siblings and every node raised before it. */
export const raiseNodeAtom = atom(null, (get, set, nodeId: string) => {
  const raisedNodeIds = get(raisedNodeIdsAtom);
  if (raisedNodeIds.at(-1) === nodeId) {
    return;
  }

  // Nodes that left the graph are forgotten as others are raised, so the stack never outgrows the graph.
  const nodes = get(sourceGraphAtom).nodes;
  set(raisedNodeIdsAtom, [...raisedNodeIds.filter((id) => id !== nodeId && nodes.has(id)), nodeId]);
});

/** Focus a node, which also raises it, or clear focus with null. */
export const focusNodeAtom = atom(null, (_get, set, nodeId: string | null) => {
  set(focusedNodeIdAtom, nodeId);
  if (nodeId !== null) {
    set(raiseNodeAtom, nodeId);
  }
});

/**
 * The modules the user collapsed: a local view choice that never edits Bicep source and is not an
 * undo step. It is kept through graph updates, even while a module is missing or empty, so a module
 * that comes back is still collapsed.
 */
export const collapsedModuleIdsAtom = atom<ReadonlySet<string>>(new Set<string>());

/**
 * Subgraphs to lay out on their own, by root node, and the point each is centered on: a module that opened showing
 * children never laid out, centered where the module was. Until then those children stay hidden and the module as it
 * was, so it opens with one morph, as a module whose children were laid out does. A `subgraph` layout places them,
 * leaving the other nodes and the camera where they are; any whole-graph layout places them too.
 */
export const pendingSubgraphLayoutsAtom = atom<ReadonlyMap<string, Point>>(new Map<string, Point>());

/** Modules being restored because the user expanded them. Each shows progress until it opens, or says why not. */
export const restoringModuleIdsAtom = atom<ReadonlySet<string>>(new Set<string>());

/** How many node drags are in progress. Undo and Redo wait for them to end, so a replay never races a drag. */
export const activeNodeDragCountAtom = atom(0);

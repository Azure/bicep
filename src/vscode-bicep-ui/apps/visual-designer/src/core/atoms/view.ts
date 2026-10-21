// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";

/**
 * The ID of the focused node, or null when nothing is focused. Clicking a node focuses it and
 * clicking the canvas background clears it.
 */
export const focusedNodeIdAtom = atom<string | null>(null);

/**
 * The node painted above the others: the one most recently focused or dragged. Dragging a node raises
 * it without focusing it, and it stays raised until another node is focused or dragged.
 */
export const raisedNodeIdAtom = atom<string | null>(null);

/** Focus a node, which also raises it, or clear focus with null. */
export const focusNodeAtom = atom(null, (_get, set, nodeId: string | null) => {
  set(focusedNodeIdAtom, nodeId);
  if (nodeId !== null) {
    set(raisedNodeIdAtom, nodeId);
  }
});

/**
 * The modules the user collapsed: a local view choice that never edits Bicep source and is not an
 * undo step. It is kept through graph updates, even while a module is missing or empty, so a module
 * that comes back is still collapsed.
 */
export const collapsedModuleIdsAtom = atom<ReadonlySet<string>>(new Set<string>());

/** How many node drags are in progress. Undo and Redo wait for them to end, so a replay never races a drag. */
export const activeNodeDragCountAtom = atom(0);

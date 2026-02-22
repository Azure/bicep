// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, PrimitiveAtom } from "jotai";
import type { Box, Point } from "../../../utils/math/geometry";

import { atom } from "jotai";
import { nodeConfigAtom } from "./configs";

export interface AtomicNodeState {
  kind: "atomic";
  id: string;
  boxAtom: PrimitiveAtom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
}

export interface CompoundNodeState {
  kind: "compound";
  id: string;
  childIdsAtom: PrimitiveAtom<string[]>;
  boxAtom: Atom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
}

export type NodeState = AtomicNodeState | CompoundNodeState;

export type NodeKind = NodeState["kind"];

export const nodesByIdAtom = atom<Record<string, NodeState>>({});

/**
 * The ID of the currently focused node, or `null` if nothing is focused.
 * Set on mousedown; cleared when clicking the canvas background.
 */
export const focusedNodeIdAtom = atom<string | null>(null);

/**
 * Extracts the parent node ID from a composite `::` delimited ID.
 * Returns `null` for top-level nodes.
 */
export function getParentId(nodeId: string): string | null {
  const lastSep = nodeId.lastIndexOf("::");
  return lastSep >= 0 ? nodeId.slice(0, lastSep) : null;
}

/**
 * Computes how closely related `nodeId` is to `focusedId`.
 *
 * Tier values (lower = closer):
 *  - 0: the focused node itself, or a descendant of the focused node
 *  - 1: a sibling of the focused node, or a descendant of such a sibling
 *  - 2: the parent of the focused node
 *  - 3: a sibling of the parent (or descendant thereof)
 *  - 4: the grandparent
 *  - …and so on recursively
 *  - -1: unrelated (should not happen for valid graph IDs)
 */
export function computeFocusTier(nodeId: string, focusedId: string): number {
  // Self.
  if (nodeId === focusedId) return 0;

  // Descendant of the focused node (e.g. children inside a focused module).
  if (nodeId.startsWith(focusedId + "::")) return 0;

  // Walk up the focused node's ancestor chain.
  let ancestorId: string | null = focusedId;
  let tier = 0;

  while (ancestorId !== null) {
    if (nodeId === ancestorId) return tier;

    const parentOfAncestor = getParentId(ancestorId);

    // Sibling: same parent as this ancestor.
    if (getParentId(nodeId) === parentOfAncestor) {
      return tier + 1;
    }

    // Descendant of a sibling of this ancestor.
    if (parentOfAncestor !== null && nodeId.startsWith(parentOfAncestor + "::")) {
      return tier + 1;
    }

    // Reached the root — every remaining node is a top-level peer.
    if (parentOfAncestor === null) {
      return tier + 1;
    }

    ancestorId = parentOfAncestor;
    tier += 2;
  }

  return -1;
}

/**
 * Returns the effective z-index for a node given the current focus state.
 *
 * Without focus every compound sits at 0 and every atomic at 2.
 * With focus the focused group is boosted highest, siblings next, then
 * parent, and so on up the ancestor chain.
 */
export function getNodeZIndex(nodeId: string, nodeKind: NodeKind, focusedId: string | null): number {
  const baseZ = nodeKind === "atomic" ? 2 : 0;

  if (focusedId === null) return baseZ;

  const tier = computeFocusTier(nodeId, focusedId);
  if (tier < 0) return baseZ;

  // Decrease boost as the relationship becomes more distant.
  const boost = Math.max(10, 100 - tier * 15);
  return baseZ + boost;
}

export const addAtomicNodeAtom = atom(null, (get, set, id: string, origin: Point, data: unknown) => {
  if (get(nodesByIdAtom)[id] !== undefined) {
    throw new Error(`Cannot add atomic node ${id} because it already exists.`);
  }

  set(nodesByIdAtom, (nodes) => ({
    ...nodes,
    [id]: {
      kind: "atomic",
      id,
      boxAtom: atom({ min: { ...origin }, max: { ...origin } }),
      dataAtom: atom(data),
    },
  }));
});

export const addCompoundNodeAtom = atom(null, (_, set, id: string, childIds: string[], data: unknown) => {
  const childIdsAtom = atom(childIds);
  const boxAtom = atom((get) => {
    const nodes = get(nodesByIdAtom);
    const { padding } = get(nodeConfigAtom);
    const childBoxes = get(childIdsAtom)
      .map((id) => nodes[id])
      .filter((child) => child !== undefined)
      .map((child) => get(child.boxAtom));

    let minX = Math.min(...childBoxes.map((box) => box.min.x)) - padding.left;
    let minY = Math.min(...childBoxes.map((box) => box.min.y)) - padding.top;
    let maxX = Math.max(...childBoxes.map((box) => box.max.x)) + padding.right;
    let maxY = Math.max(...childBoxes.map((box) => box.max.y)) + padding.bottom;

    minX = isNaN(minX) ? 0 : minX;
    minY = isNaN(minY) ? 0 : minY;
    maxX = isNaN(maxX) ? 0 : maxX;
    maxY = isNaN(maxY) ? 0 : maxY;

    return {
      min: { x: minX, y: minY },
      max: { x: maxX, y: maxY },
    };
  });

  set(nodesByIdAtom, (nodes) => ({
    ...nodes,
    [id]: {
      kind: "compound",
      id,
      childIdsAtom,
      boxAtom,
      dataAtom: atom(data),
    },
  }));
});

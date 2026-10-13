// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, PrimitiveAtom } from "jotai";
import type { Box, Point } from "@/lib/math";

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
  /**
   * How far the box has grown from the box it entered with (0) to the box enclosing its children (1).
   * Always 1 for a compound node created without an entry box.
   */
  enterProgressAtom: PrimitiveAtom<number>;
}

export type NodeState = AtomicNodeState | CompoundNodeState;

export type NodeKind = NodeState["kind"];

export const nodesByIdAtom = atom<Record<string, NodeState>>({});

/**
 * The ID of the currently focused node, or `null` if nothing is focused.
 * Set after a node click; cleared after a click on the canvas background.
 */
export const focusedNodeIdAtom = atom<string | null>(null);

/**
 * Nodes drawn fully transparent, along with their edges. Adding a node fades it out and removing it
 * fades it in, so nodes can leave or arrive without popping.
 */
export const fadedNodeIdsAtom = atom<ReadonlySet<string>>(new Set<string>());

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

/**
 * A leaf node whose box starts as a point at `origin`; it grows to its measured size when it mounts.
 * Add it to `nodesByIdAtom`.
 */
export function createAtomicNode(id: string, origin: Point, data: unknown): AtomicNodeState {
  return {
    kind: "atomic",
    id,
    boxAtom: atom({ min: { ...origin }, max: { ...origin } }),
    dataAtom: atom(data),
  };
}

/**
 * A container node whose box encloses its children's boxes plus the configured padding. Add it to
 * `nodesByIdAtom`.
 *
 * With `enterFrom`, the box starts there instead and grows to enclose the children as
 * `enterProgressAtom` goes from 0 to 1.
 */
export function createCompoundNode(id: string, childIds: string[], data: unknown, enterFrom?: Box): CompoundNodeState {
  const childIdsAtom = atom(childIds);
  const enterProgressAtom = atom(enterFrom ? 0 : 1);
  const enclosingBoxAtom = atom((get) => {
    const nodes = get(nodesByIdAtom);
    const { padding } = get(nodeConfigAtom);
    const childBoxes = get(childIdsAtom)
      .map((childId) => nodes[childId])
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
  const boxAtom = atom((get) => {
    const box = get(enclosingBoxAtom);
    const progress = get(enterProgressAtom);

    if (!enterFrom || progress >= 1) {
      return box;
    }

    const lerp = (from: number, to: number) => from + (to - from) * progress;

    return {
      min: { x: lerp(enterFrom.min.x, box.min.x), y: lerp(enterFrom.min.y, box.min.y) },
      max: { x: lerp(enterFrom.max.x, box.max.x), y: lerp(enterFrom.max.y, box.max.y) },
    };
  });

  return { kind: "compound", id, childIdsAtom, boxAtom, dataAtom: atom(data), enterProgressAtom };
}

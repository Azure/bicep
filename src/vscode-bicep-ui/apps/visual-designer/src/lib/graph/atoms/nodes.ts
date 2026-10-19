// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, PrimitiveAtom } from "jotai";
import type { Box, Point } from "@/lib/math";

import { atom } from "jotai";
import { nodeConfigAtom } from "./configs";

export interface AtomicNodeState {
  kind: "atomic";
  id: string;
  /** The container node this node is drawn inside, or null at the root. */
  parentId: string | null;
  boxAtom: PrimitiveAtom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
}

export interface CompoundNodeState {
  kind: "compound";
  id: string;
  /** The container node this node is drawn inside, or null at the root. */
  parentId: string | null;
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
 * Nodes drawn fully transparent, along with their edges. Adding a node fades it out and removing it
 * fades it in, so nodes can leave or arrive without popping.
 */
export const fadedNodeIdsAtom = atom<ReadonlySet<string>>(new Set<string>());

/**
 * A leaf node whose box starts as a point at `origin`; it grows to its measured size when it mounts.
 * Add it to `nodesByIdAtom`.
 */
export function createAtomicNode(id: string, parentId: string | null, origin: Point, data: unknown): AtomicNodeState {
  return {
    kind: "atomic",
    id,
    parentId,
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
export function createCompoundNode(
  id: string,
  parentId: string | null,
  childIds: string[],
  data: unknown,
  enterFrom?: Box,
): CompoundNodeState {
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

  return { kind: "compound", id, parentId, childIdsAtom, boxAtom, dataAtom: atom(data), enterProgressAtom };
}

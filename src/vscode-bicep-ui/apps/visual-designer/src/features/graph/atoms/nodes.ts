// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, PrimitiveAtom } from "jotai";
import type { Box, Point } from "../../../utils/math/geometry";

import { atom } from "jotai";
import { nodeConfigAtom } from "./configs";

export interface AtomicNodeState {
  kind: "atomic";
  id: string;
  originAtom: PrimitiveAtom<Point>;
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

export const nodesAtom = atom<Record<string, NodeState>>({});

export const addAtomicNodeAtom = atom(null, (get, set, id: string, origin: Point, data: unknown) => {
  if (get(nodesAtom)[id] !== undefined) {
    throw new Error(`Cannot add atomic node ${id} because it already exists.`);
  }

  set(nodesAtom, (nodes) => ({
    ...nodes,
    [id]: {
      kind: "atomic",
      id,
      originAtom: atom(origin),
      boxAtom: atom({ min: { ...origin }, max: { ...origin } }),
      dataAtom: atom(data),
    },
  }));
});

export const addCompoundNodeAtom = atom(null, (_, set, id: string, childIds: string[], data: unknown) => {
  const childIdsAtom = atom(childIds);
  const boxAtom = atom((get) => {
    const nodes = get(nodesAtom);
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

  set(nodesAtom, (nodes) => ({
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

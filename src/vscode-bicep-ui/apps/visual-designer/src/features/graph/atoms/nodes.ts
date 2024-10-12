import type { Atom, PrimitiveAtom } from "jotai";
import type { Box, Point } from "../../../utils/math/geometry";

import { atom } from "jotai";

export interface PrimitiveNodeState {
  kind: "primitive";
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

export type NodeState = PrimitiveNodeState | CompoundNodeState;

export const nodesAtom = atom<Record<string, NodeState>>({});

export const addPrimitiveNodeAtom = atom(null, (get, set, id: string, origin: Point, data: unknown) => {
  if (get(nodesAtom)[id] !== undefined) {
    throw new Error(`Cannot add primitive node ${id} because it already exists.`);
  }

  set(nodesAtom, (nodes) => ({
    ...nodes,
    [id]: {
      kind: "primitive",
      id,
      originAtom: atom(origin),
      boxAtom: atom({ min: origin, max: origin }),
      dataAtom: atom(data),
    },
  }));
});

export const addCompoundNodeAtom = atom(null, (_, set, id: string, childIds: string[], data: unknown) => {
  const childIdsAtom = atom(childIds);
  const boxAtom = atom((get) => {
    const nodes = get(nodesAtom);
    const childBoxes = get(childIdsAtom)
      .map((id) => nodes[id])
      .filter((child) => child !== undefined)
      .map((child) => get(child.boxAtom));

    const minX = Math.min(...childBoxes.map((box) => box.min.x));
    const minY = Math.min(...childBoxes.map((box) => box.min.y));
    const maxX = Math.max(...childBoxes.map((box) => box.max.x));
    const maxY = Math.max(...childBoxes.map((box) => box.max.y));

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

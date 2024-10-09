import type { Atom, PrimitiveAtom } from "jotai";
import type { Box, Point } from "../../../utils/math/geometry";

import { atom } from "jotai";

export interface PrimitiveNodeState {
  id: string;
  originAtom: PrimitiveAtom<Point>;
  boxAtom: PrimitiveAtom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
}

export interface CompoundNodeState {
  id: string;
  childIdsAtom: PrimitiveAtom<string[]>;
  boxAtom: Atom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
}

export type NodeState = PrimitiveNodeState | CompoundNodeState;

export function isPrimitive(node: NodeState): node is PrimitiveNodeState {
  return "originAtom" in node;
}

export function isCompound(node: NodeState): node is CompoundNodeState {
  return "childIdsAtom" in node;
}

export const nodesByIdAtom = atom<Record<string, NodeState>>({});

export const addPrimitiveNodeAtom = atom(null, (_, set, id: string, origin: Point, box: Box, data: unknown) => {
  set(nodesByIdAtom, (nodes) => ({
    ...nodes,
    [id]: {
      id,
      originAtom: atom(origin),
      boxAtom: atom(box),
      dataAtom: atom(data),
    },
  }));
});

export const addCompoundNodeAtom = atom(null, (_, set, id: string, childIds: string[], data: unknown) => {
  const childIdsAtom = atom(childIds);
  const boxAtom = atom((get) => {
    const childBoxes = get(childIdsAtom).map((id) => {
      const node = get(nodesByIdAtom)[id];

      if (!node) {
        return null;
      }

      return get(node.boxAtom);
    }).filter(box => box !== null);

    const minX = Math.min(...childBoxes.map((box) => box.min.x));
    const minY = Math.min(...childBoxes.map((box) => box.min.y));
    const maxX = Math.max(...childBoxes.map((box) => box.max.x));
    const maxY = Math.max(...childBoxes.map((box) => box.max.y));

    return {
      min: { x: minX, y: minY },
      max: { x: maxX, y: maxY },
    };
  });

  set(nodesByIdAtom, (nodes) => ({
    ...nodes,
    [id]: {
      id,
      childIdsAtom,
      boxAtom,
      dataAtom: atom(data),
    },
  }));
});

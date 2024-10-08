import { atom, type Atom, type PrimitiveAtom } from "jotai";
import type { Box, Point } from "../../../math/geometry";

export interface PrimitiveNodeAtomValue {
  id: string;
  originAtom: PrimitiveAtom<Point>;
  boxAtom: PrimitiveAtom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
}

export interface CompoundNodeAtomValue {
  id: string;
  childIdsAtom: PrimitiveAtom<string[]>;
  boxAtom: Atom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
}

export type NodeAtomValue = PrimitiveNodeAtomValue | CompoundNodeAtomValue;

export type NodesAtomValue = Record<string, NodeAtomValue>;

export function isPrimitive(node: NodeAtomValue): node is PrimitiveNodeAtomValue {
  return "originAtom" in node;
}

export function isCompound(node: NodeAtomValue): node is CompoundNodeAtomValue {
  return "childIdsAtom" in node;
}

export const nodesAtom = atom<NodesAtomValue>({});

export const addPrimitiveNodeAtom = atom(null, (_, set, id: string, origin: Point, box: Box, data: unknown) => {
  set(nodesAtom, (nodes) => ({
    ...nodes,
    [id]: {
      id,
      originAtom: atom(origin),
      boxAtom: atom(box),
      dataAtom: atom(data),
    }
  }));
});

export const addCompoundNodeAtom = atom(null, (_, set, id: string, childIds: string[], data: unknown) => {
  const childIdsAtom = atom(childIds);
  const boxAtom = atom((get) => {
    const childBoxes = get(childIdsAtom).map((id) => {
      const node = get(nodesAtom)[id];
      return get(node.boxAtom);
    });

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
      id,
      childIdsAtom,
      boxAtom,
      dataAtom: atom(data),
    }
  }));
});


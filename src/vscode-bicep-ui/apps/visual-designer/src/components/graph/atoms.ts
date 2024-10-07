import { atom, type Atom, type PrimitiveAtom } from "jotai";

const createSubgraph = (id: string, initialChildNodeIds: string[]) => {
  const childNodeIdsAtom = atom(initialChildNodeIds);
  const boxAtom = atom((get) => {
    const childBoxes = get(childNodeIdsAtom).map((id) => {
      const node = get(nodesAtom)[id];
      return get(node.box);
    });

    const left = Math.min(...childBoxes.map((box) => box.center.x - box.width / 2));
    const right = Math.max(...childBoxes.map((box) => box.center.x + box.width / 2));
    const top = Math.min(...childBoxes.map((box) => box.center.y - box.height / 2));
    const bottom = Math.max(...childBoxes.map((box) => box.center.y + box.height / 2));

    const width = right - left;
    const height = bottom - top;
    const center = {
      x: left + width / 2,
      y: top + height / 2,
    };

    return { center, width, height };
  });

  return {
    id,
    childNodeIds: childNodeIdsAtom,
    box: boxAtom,
  }
};

export interface Position {
  x: number,
  y: number
}

export interface Box {
    center: Position;
    width: number;
    height: number;
}

export interface Node {
  id: string;
  origin: PrimitiveAtom<Position>;
  box: PrimitiveAtom<Box>;
}

export interface Subgraph {
  id: string;
  childNodeIds: PrimitiveAtom<string[]>;
  box: Atom<Box>;
}

const subgraph = createSubgraph("D", ["A", "B"]);

const subgraph2 = createSubgraph("E", ["C", "D"]);

export function isSubgraph(node: Node | Subgraph): node is Subgraph {
  return "childNodeIds" in node;
}

export function isNode(node: Node | Subgraph): node is Node {
  return "origin" in node;
}

export const nodesAtom = atom<Record<string, Node | Subgraph>>({
  A: {
    id: "A",
    origin: atom({
      x: 100,
      y: 100,
    }),
    box: atom({
      center: {
        x: 200,
        y: 200,
      },
      width: 100,
      height: 100,
    }),
  },
  B: {
    id: "B",
    origin: atom({
      x: 600,
      y: 400,
    }),
    box: atom({
      center: {
        x: 400,
        y: 200,
      },
      width: 100,
      height: 100,
    }),
  },
  C: {
    id: "C",
    origin: atom({
      x: 55,
      y: 66,
    }),
    box: atom({
      center: {
        x: 90,
        y: 190,
      },
      width: 200,
      height: 100,
    }),
  },
  D: subgraph,
  E: subgraph2,
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, PrimitiveAtom } from "jotai";
import type { Box } from "@/lib/math";

import { atom } from "jotai";
import { nodeConfigAtom } from "./configs";
import { nodePositionsAtom } from "./positions";

export interface Size {
  width: number;
  height: number;
}

/**
 * A node the graph draws. A node keeps its state for as long as its ID and parent stay the same, so it
 * is not recreated when it becomes a container or stops being one.
 */
export interface NodeState {
  readonly id: string;
  /** The container this node is drawn inside, or null at the root. */
  readonly parentId: string | null;
  /** The nodes drawn inside this one. A node with children is a container. */
  readonly childIdsAtom: PrimitiveAtom<readonly string[]>;
  readonly dataAtom: PrimitiveAtom<unknown>;
  /** The size of the node's content, measured once it renders. Containers are sized by their children. */
  readonly sizeAtom: PrimitiveAtom<Size | null>;
  /**
   * Where the node belongs: around its position at its measured size, or around its children's target
   * boxes plus padding. Null until the node has both a position and a size.
   */
  readonly targetBoxAtom: Atom<Box | null>;
  /** Where the node is drawn, which moves toward the target box. Only the node's motion writes it. */
  readonly boxAtom: PrimitiveAtom<Box | null>;
}

export type NodeKind = "atomic" | "compound";

export const nodesByIdAtom = atom<Readonly<Record<string, NodeState>>>({});

/** The smallest box enclosing `boxes`, grown by the container padding. Null when there are none. */
export function encloseWithPadding(
  boxes: readonly Box[],
  padding: { top: number; right: number; bottom: number; left: number },
): Box | null {
  if (boxes.length === 0) {
    return null;
  }

  return {
    min: {
      x: Math.min(...boxes.map((box) => box.min.x)) - padding.left,
      y: Math.min(...boxes.map((box) => box.min.y)) - padding.top,
    },
    max: {
      x: Math.max(...boxes.map((box) => box.max.x)) + padding.right,
      y: Math.max(...boxes.map((box) => box.max.y)) + padding.bottom,
    },
  };
}

export function createNodeState(
  id: string,
  parentId: string | null,
  childIds: readonly string[],
  data: unknown,
): NodeState {
  const childIdsAtom = atom(childIds);
  const sizeAtom = atom<Size | null>(null);
  const centerAtom = atom((get) => get(nodePositionsAtom).centers.get(id));
  const targetBoxAtom: Atom<Box | null> = atom((get) => {
    const ids = get(childIdsAtom);

    if (ids.length > 0) {
      const nodesById = get(nodesByIdAtom);
      const childBoxes = ids.flatMap((childId) => {
        const child = nodesById[childId];
        const box = child ? get(child.targetBoxAtom) : null;
        return box ? [box] : [];
      });

      return encloseWithPadding(childBoxes, get(nodeConfigAtom).padding);
    }

    const center = get(centerAtom);
    const size = get(sizeAtom);

    if (!center || !size) {
      return null;
    }

    return {
      min: { x: center.x - size.width / 2, y: center.y - size.height / 2 },
      max: { x: center.x + size.width / 2, y: center.y + size.height / 2 },
    };
  });

  return {
    id,
    parentId,
    childIdsAtom,
    dataAtom: atom(data),
    sizeAtom,
    targetBoxAtom,
    boxAtom: atom<Box | null>(null),
  };
}

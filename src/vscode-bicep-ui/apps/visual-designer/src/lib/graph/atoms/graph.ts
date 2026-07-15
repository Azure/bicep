// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/utils/math";

import { atom } from "jotai";
import { nodesByIdAtom } from "./nodes";

/**
 * Whether the first server layout has completed and nodes are in their final positions.
 * The graph layer uses this to stay invisible until layout is ready, preventing a flash
 * of nodes at their spawn position before the viewport transform is applied.
 */
export const layoutReadyAtom = atom(false);

/**
 * Derived atom that computes the axis-aligned bounding box enclosing
 * every node in the graph.  Returns `null` when the graph is empty.
 */
export const graphBoundsAtom = atom<Box | null>((get) => {
  const nodesById = get(nodesByIdAtom);
  const nodes = Object.values(nodesById);

  if (nodes.length === 0) {
    return null;
  }

  let minX = Infinity;
  let minY = Infinity;
  let maxX = -Infinity;
  let maxY = -Infinity;

  for (const node of nodes) {
    const box = get(node.boxAtom);

    minX = Math.min(minX, box.min.x);
    minY = Math.min(minY, box.min.y);
    maxX = Math.max(maxX, box.max.x);
    maxY = Math.max(maxY, box.max.y);
  }

  return {
    min: { x: minX, y: minY },
    max: { x: maxX, y: maxY },
  };
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "../../../utils/math";

import { atom } from "jotai";
import { nodesByIdAtom } from "./nodes";

/**
 * Monotonically increasing version number, bumped each time
 * the deployment graph is replaced.  Components can subscribe
 * to this atom to re-run layout after the new graph is committed
 * to the DOM.
 */
export const graphVersionAtom = atom(0);

/**
 * The number of diagnostics errors in the file.
 * Updated by the deployment graph notification handler.
 */
export const errorCountAtom = atom(0);

/**
 * Whether the current deployment graph has any nodes.
 * Updated by the deployment graph notification handler.
 */
export const hasNodesAtom = atom(false);

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

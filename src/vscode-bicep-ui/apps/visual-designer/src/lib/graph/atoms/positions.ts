// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";

import { atom } from "jotai";

/**
 * How the graph moves nodes to new positions: `animate` springs them there, `instant` puts them there
 * at once. Direct manipulation, such as dragging, is instant.
 */
export type PositionTransition = "animate" | "instant";

export interface NodePositions {
  /**
   * The center of each node that is not a container, keyed by node ID. A container is placed by its
   * contents. Entries for nodes not in the graph are kept and ignored, so a node that is hidden and
   * shown again comes back where it was.
   */
  readonly centers: ReadonlyMap<string, Point>;
  /** How the nodes whose centers changed in this update move there. */
  readonly transition: PositionTransition;
}

/** Where nodes are going. Written by the app; the graph animates the drawn nodes toward it. */
export const nodePositionsAtom = atom<NodePositions>({ centers: new Map(), transition: "instant" });

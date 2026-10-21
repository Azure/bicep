// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom } from "jotai";

import { atom } from "jotai";
import { createContext } from "react";
import { nodesByIdAtom } from "../atoms";
import { computePaintOrder } from "../paint-order";

/** What users do to nodes. The graph reports gestures; what they change is up to the app. */
export interface GraphNodeEvents {
  /** A node started moving. A press that never moves is a click and does not call this. */
  onNodeDragStart: (nodeId: string) => void;
  /** A node being dragged moved by `dx`, `dy` in graph coordinates. */
  onNodeDrag: (nodeId: string, dx: number, dy: number) => void;
  /** A node that started moving was released. */
  onNodeDragEnd: (nodeId: string) => void;
  onNodeDoubleClick: (nodeId: string) => void;
}

export interface GraphContextValue {
  events: GraphNodeEvents;
  /** Whether nodes snap to new positions and shapes rather than animating. */
  reduceMotion: boolean;
  /** Each node's index in back-to-front paint order. */
  paintOrderAtom: Atom<ReadonlyMap<string, number>>;
}

export function ignoreNodeEvent() {}

export const IGNORE_NODE_EVENTS: GraphNodeEvents = {
  onNodeDragStart: ignoreNodeEvent,
  onNodeDrag: ignoreNodeEvent,
  onNodeDragEnd: ignoreNodeEvent,
  onNodeDoubleClick: ignoreNodeEvent,
};

export function createPaintOrderAtom(raisedNodeIdAtom: Atom<string | null> | undefined) {
  return atom((get) =>
    computePaintOrder(Object.values(get(nodesByIdAtom)), raisedNodeIdAtom ? get(raisedNodeIdAtom) : null),
  );
}

/** Provided by `Graph` from its props. */
export const GraphContext = createContext<GraphContextValue>({
  events: IGNORE_NODE_EVENTS,
  reduceMotion: false,
  paintOrderAtom: createPaintOrderAtom(undefined),
});

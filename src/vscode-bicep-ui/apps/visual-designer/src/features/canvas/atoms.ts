// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";

import { atom } from "jotai";

export type NavigationMode = "select" | "hand";

const navigationModeAtom = atom<NavigationMode>("select");

/** Whether Space is held, which switches to the hand tool until it is released. */
const isSpaceHeldAtom = atom(false);

/** Whether the hand tool is in effect: chosen in the dock, or held with Space. */
export const isHandModeAtom = atom((get) => get(navigationModeAtom) === "hand" || get(isSpaceHeldAtom));

export const setSpaceHeldAtom = atom(null, (_get, set, held: boolean) => {
  set(isSpaceHeldAtom, held);
});

export const setNavigationModeAtom = atom(null, (_get, set, mode: NavigationMode) => {
  set(navigationModeAtom, mode);
});

/** The open node context menu: the node it acts on, and where it opens, in client coordinates. */
export interface NodeMenu {
  nodeId: string;
  clientPoint: Point;
}

export const nodeMenuAtom = atom<NodeMenu | null>(null);

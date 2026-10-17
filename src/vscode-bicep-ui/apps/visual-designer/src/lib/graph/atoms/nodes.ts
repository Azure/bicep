// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Atom, createStore, PrimitiveAtom } from "jotai";
import type { Box, Point } from "@/lib/math";

import { atom } from "jotai";
import { nodeConfigAtom } from "./configs";

export interface AtomicNodeState {
  kind: "atomic";
  id: string;
  boxAtom: PrimitiveAtom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
}

export interface CompoundNodeState {
  kind: "compound";
  id: string;
  childIdsAtom: PrimitiveAtom<string[]>;
  boxAtom: Atom<Box>;
  dataAtom: PrimitiveAtom<unknown>;
  /**
   * How far the box has grown from the box it entered with (0) to the box enclosing its children (1).
   * Always 1 for a compound node created without an entry box.
   */
  enterProgressAtom: PrimitiveAtom<number>;
}

export type NodeState = AtomicNodeState | CompoundNodeState;

export type NodeKind = NodeState["kind"];

type Store = ReturnType<typeof createStore>;

export const nodesByIdAtom = atom<Record<string, NodeState>>({});

/**
 * `nodesByIdAtom` as of the canvas's last commit, when every node in it had mounted. Undefined while no canvas is
 * mounted. Set by `NodeLayer`.
 */
export const mountedNodesByIdAtom = atom<Readonly<Record<string, NodeState>> | undefined>(undefined);

/** One entry for each node changing size on purpose, until it ends (see `beginNodeResize`). */
const nodeResizesAtom = atom<ReadonlySet<object>>(new Set<object>());

/**
 * Mark a node as changing size on purpose, such as a module morphing between shapes, until the returned function is
 * called: what it measures meanwhile is not its size. Call that function once the node has its final size.
 */
export function beginNodeResize(store: Store): () => void {
  const resize = {};
  store.set(nodeResizesAtom, (resizes) => new Set(resizes).add(resize));

  return () =>
    store.set(nodeResizesAtom, (resizes) => {
      if (!resizes.has(resize)) {
        return resizes;
      }
      const next = new Set(resizes);
      next.delete(resize);
      return next;
    });
}

/**
 * For each store, how each mounted atomic node reads its size from the page into its box. A store-scoped registry
 * rather than an atom: the functions are called, never observed.
 */
const nodeMeasurersByStore = new WeakMap<Store, Set<() => void>>();

/** Let `waitForNodeSizes` read a mounted node's size, until the returned function is called. */
export function registerNodeMeasurer(store: Store, measure: () => void): () => void {
  const measurers = nodeMeasurersByStore.get(store) ?? new Set<() => void>();
  nodeMeasurersByStore.set(store, measurers);
  measurers.add(measure);

  return () => measurers.delete(measure);
}

/**
 * Resolves once every node's box holds the size it has on the page: the canvas has mounted every node in
 * `nodesByIdAtom`, no node is changing size on purpose (see `beginNodeResize`), and each mounted atomic node has read
 * its size again. Resolves at once when no canvas is mounted.
 *
 * Each is needed. A node has no size until it mounts, and nodes added outside a React event mount when React next
 * commits, which can be a frame or more later. A node that is already mounted and changes size learns of it only when
 * the browser reports the resize, a frame later. And a node changing size on purpose measures sizes it only passes
 * through.
 */
export function waitForNodeSizes(store: Store): Promise<void> {
  const isSettled = () => {
    const mountedNodes = store.get(mountedNodesByIdAtom);
    const isMounted = mountedNodes === undefined || mountedNodes === store.get(nodesByIdAtom);
    return isMounted && store.get(nodeResizesAtom).size === 0;
  };
  const measure = () => {
    for (const measureNode of nodeMeasurersByStore.get(store) ?? []) {
      measureNode();
    }
  };

  if (isSettled()) {
    measure();
    return Promise.resolve();
  }

  return new Promise((resolve) => {
    const check = () => {
      if (isSettled()) {
        unsubscribeFromMounts();
        unsubscribeFromResizes();
        measure();
        resolve();
      }
    };
    const unsubscribeFromMounts = store.sub(mountedNodesByIdAtom, check);
    const unsubscribeFromResizes = store.sub(nodeResizesAtom, check);
  });
}

/**
 * The ID of the currently focused node, or `null` if nothing is focused.
 * Set after a node click; cleared after a click on the canvas background.
 */
export const focusedNodeIdAtom = atom<string | null>(null);

/**
 * Nodes drawn fully transparent, along with their edges. Adding a node fades it out and removing it
 * fades it in, so nodes can leave or arrive without popping.
 */
export const fadedNodeIdsAtom = atom<ReadonlySet<string>>(new Set<string>());

/**
 * Extracts the parent node ID from a composite `::` delimited ID.
 * Returns `null` for top-level nodes.
 */
export function getParentId(nodeId: string): string | null {
  const lastSep = nodeId.lastIndexOf("::");
  return lastSep >= 0 ? nodeId.slice(0, lastSep) : null;
}

/**
 * Computes how closely related `nodeId` is to `focusedId`.
 *
 * Tier values (lower = closer):
 *  - 0: the focused node itself, or a descendant of the focused node
 *  - 1: a sibling of the focused node, or a descendant of such a sibling
 *  - 2: the parent of the focused node
 *  - 3: a sibling of the parent (or descendant thereof)
 *  - 4: the grandparent
 *  - …and so on recursively
 *  - -1: unrelated (should not happen for valid graph IDs)
 */
export function computeFocusTier(nodeId: string, focusedId: string): number {
  // Self.
  if (nodeId === focusedId) return 0;

  // Descendant of the focused node (e.g. children inside a focused module).
  if (nodeId.startsWith(focusedId + "::")) return 0;

  // Walk up the focused node's ancestor chain.
  let ancestorId: string | null = focusedId;
  let tier = 0;

  while (ancestorId !== null) {
    if (nodeId === ancestorId) return tier;

    const parentOfAncestor = getParentId(ancestorId);

    // Sibling: same parent as this ancestor.
    if (getParentId(nodeId) === parentOfAncestor) {
      return tier + 1;
    }

    // Descendant of a sibling of this ancestor.
    if (parentOfAncestor !== null && nodeId.startsWith(parentOfAncestor + "::")) {
      return tier + 1;
    }

    // Reached the root — every remaining node is a top-level peer.
    if (parentOfAncestor === null) {
      return tier + 1;
    }

    ancestorId = parentOfAncestor;
    tier += 2;
  }

  return -1;
}

/**
 * How far apart consecutive focus tiers are stacked. Wider than any nesting can reach, so depth orders nodes
 * within a tier but never lifts a node into a closer one.
 */
const FOCUS_TIER_Z_SPAN = 1000;

/** How many containers a node is nested in: 0 for a top-level node. */
function getNodeDepth(nodeId: string): number {
  return nodeId.split("::").length - 1;
}

/**
 * Returns the effective z-index for a node given the current focus state.
 *
 * Nodes stack by nesting, so a container is drawn over everything outside it: a compound at depth d sits at
 * 2d, the edges between its children at 2d + 1 (see `EdgeLayer`), and its children from 2d + 2 up. An atomic
 * node sits one above a compound at the same depth.
 *
 * With focus the focused group is boosted highest, siblings next, then parent, and so on up the ancestor
 * chain; nesting still orders nodes within each of those tiers.
 */
export function getNodeZIndex(nodeId: string, nodeKind: NodeKind, focusedId: string | null): number {
  const nestingZ = 2 * getNodeDepth(nodeId) + (nodeKind === "atomic" ? 1 : 0);

  if (focusedId === null) return nestingZ;

  const tier = computeFocusTier(nodeId, focusedId);
  if (tier < 0) return nestingZ;

  // Decrease boost as the relationship becomes more distant.
  const boost = Math.max(10, 100 - tier * 15);
  return boost * FOCUS_TIER_Z_SPAN + nestingZ;
}

/**
 * A leaf node whose box starts as a point at `origin`; it grows to its measured size when it mounts.
 * Add it to `nodesByIdAtom`.
 */
export function createAtomicNode(id: string, origin: Point, data: unknown): AtomicNodeState {
  return {
    kind: "atomic",
    id,
    boxAtom: atom({ min: { ...origin }, max: { ...origin } }),
    dataAtom: atom(data),
  };
}

/**
 * A container node whose box encloses its children's boxes plus the configured padding. Add it to
 * `nodesByIdAtom`.
 *
 * With `enterFrom`, the box starts there instead and grows to enclose the children as
 * `enterProgressAtom` goes from 0 to 1.
 */
export function createCompoundNode(id: string, childIds: string[], data: unknown, enterFrom?: Box): CompoundNodeState {
  const childIdsAtom = atom(childIds);
  const enterProgressAtom = atom(enterFrom ? 0 : 1);
  const enclosingBoxAtom = atom((get) => {
    const nodes = get(nodesByIdAtom);
    const { padding } = get(nodeConfigAtom);
    const childBoxes = get(childIdsAtom)
      .map((childId) => nodes[childId])
      .filter((child) => child !== undefined)
      .map((child) => get(child.boxAtom));

    let minX = Math.min(...childBoxes.map((box) => box.min.x)) - padding.left;
    let minY = Math.min(...childBoxes.map((box) => box.min.y)) - padding.top;
    let maxX = Math.max(...childBoxes.map((box) => box.max.x)) + padding.right;
    let maxY = Math.max(...childBoxes.map((box) => box.max.y)) + padding.bottom;

    minX = isNaN(minX) ? 0 : minX;
    minY = isNaN(minY) ? 0 : minY;
    maxX = isNaN(maxX) ? 0 : maxX;
    maxY = isNaN(maxY) ? 0 : maxY;

    return {
      min: { x: minX, y: minY },
      max: { x: maxX, y: maxY },
    };
  });
  const boxAtom = atom((get) => {
    const box = get(enclosingBoxAtom);
    const progress = get(enterProgressAtom);

    if (!enterFrom || progress >= 1) {
      return box;
    }

    const lerp = (from: number, to: number) => from + (to - from) * progress;

    return {
      min: { x: lerp(enterFrom.min.x, box.min.x), y: lerp(enterFrom.min.y, box.min.y) },
      max: { x: lerp(enterFrom.max.x, box.max.x), y: lerp(enterFrom.max.y, box.max.y) },
    };
  });

  return { kind: "compound", id, childIdsAtom, boxAtom, dataAtom: atom(data), enterProgressAtom };
}

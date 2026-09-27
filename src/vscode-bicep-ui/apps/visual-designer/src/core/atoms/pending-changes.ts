// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { ResourceTypeReference } from "../types";

import { atom } from "jotai";
import { atomFamily } from "jotai-family";

/**
 * Graph changes the designer has started but a graph update has not yet shown: resources being
 * created, nodes being removed by undo, and the edits still in flight.
 */

// ── Changes in flight ──

/**
 * How many source edits and layout resets are in flight. Undo, redo, Reset Layout, and node
 * gestures wait until it drops back to zero, so they never act on a graph that is about to change.
 */
export const graphChangesInProgressCountAtom = atom(0);

export const isGraphChangeInProgressAtom = atom((get) => get(graphChangesInProgressCountAtom) > 0);

// ── Pending resources ──

export interface PendingResource {
  operationId: string;
  resourceType: ResourceTypeReference;
  origin: Point;
  expectedNodeId?: string;
}

/**
 * Placeholders for resources being created, as transitions rather than array surgery at the call
 * site.
 *
 * A resource is optimistically pending from the moment the user drops it, gains an `expectedNodeId`
 * once the host has prepared the edit, and is removed when the canonical node arrives — or when the
 * attempt fails or is undone. Redo makes it pending again until its node returns.
 */
export const pendingResourcesAtom = atom<PendingResource[]>([]);

export const beginResourceCreationAtom = atom(null, (_get, set, resource: PendingResource) => {
  set(pendingResourcesAtom, (pending) => [...pending, resource]);
});

/** Correlate a pending resource with the canonical node the host says it will become. */
export const bindExpectedNodeAtom = atom(
  null,
  (_get, set, { operationId, expectedNodeId }: { operationId: string; expectedNodeId: string }) => {
    set(pendingResourcesAtom, (pending) =>
      pending.map((resource) => (resource.operationId === operationId ? { ...resource, expectedNodeId } : resource)),
    );
  },
);

/** Remove a placeholder whose creation failed or was undone. */
export const discardPendingResourceAtom = atom(null, (_get, set, operationId: string) => {
  set(pendingResourcesAtom, (pending) => pending.filter((resource) => resource.operationId !== operationId));
});

/** Drop the placeholders whose canonical nodes have now arrived. */
export const commitPendingResourcesAtom = atom(null, (_get, set, committedNodeIds: ReadonlySet<string>) => {
  set(pendingResourcesAtom, (pending) =>
    pending.filter((resource) => !resource.expectedNodeId || !committedNodeIds.has(resource.expectedNodeId)),
  );
});

/**
 * Where each expected node should appear, keyed by its node ID. A graph update places an arriving
 * node here instead of running a layout.
 */
export const pendingPlacementsAtom = atom((get) => {
  const placements = new Map<string, Point>();
  for (const { expectedNodeId, origin } of get(pendingResourcesAtom)) {
    if (expectedNodeId !== undefined) {
      placements.set(expectedNodeId, origin);
    }
  }
  return placements;
});

/** Whether a node has just replaced its placeholder, so it can animate in from the placeholder's size. */
export const resourceNodeIsCommittingAtomFamily = atomFamily((_nodeId: string) => atom(false));

// ── Pending removals ──

/**
 * Nodes the designer expects a graph update to remove because the user undid their creation.
 *
 * Removing one must not trigger a layout, so the other nodes and the camera stay put. The graph
 * update can arrive late, so an ID stays here until a graph update sees that node change.
 */
export const pendingRemovalNodeIdsAtom = atom<ReadonlySet<string>>(new Set<string>());

export const expectNodeRemovalAtom = atom(null, (_get, set, nodeId: string) => {
  set(pendingRemovalNodeIdsAtom, (nodeIds) => new Set([...nodeIds, nodeId]));
});

/** The node is coming back (redo or a new creation with the same ID), so it is no longer being removed. */
export const cancelNodeRemovalAtom = atom(null, (_get, set, nodeId: string) => {
  set(pendingRemovalNodeIdsAtom, (nodeIds) => new Set([...nodeIds].filter((id) => id !== nodeId)));
});

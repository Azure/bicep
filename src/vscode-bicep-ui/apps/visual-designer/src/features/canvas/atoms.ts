// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Point } from "@/lib/math";
import type { ResourceTypeReference } from "./types";

import { atom } from "jotai";
import { atomFamily } from "jotai-family";

export interface PendingResource {
  operationId: string;
  resourceType: ResourceTypeReference;
  origin: Point;
  expectedNodeId?: string;
}

export const pendingResourcesAtom = atom<PendingResource[]>([]);
export const resourceCreationErrorAtom = atom<string | null>(null);
export const resourceNodeIsCommittingAtomFamily = atomFamily((_nodeId: string) => atom(false));

/**
 * The pending-resource lifecycle, as four transitions rather than array surgery at the call site.
 *
 * A resource is optimistically pending from the moment the user drops it, gains an `expectedNodeId`
 * once the host has prepared the edit, and is removed when the canonical node arrives — or when the
 * attempt fails.
 */

export const beginResourceCreationAtom = atom(null, (_get, set, resource: PendingResource) => {
  set(pendingResourcesAtom, (pending) => [...pending, resource]);
  set(resourceCreationErrorAtom, null);
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

export const failResourceCreationAtom = atom(
  null,
  (_get, set, { operationId, message }: { operationId: string; message: string }) => {
    set(pendingResourcesAtom, (pending) => pending.filter((resource) => resource.operationId !== operationId));
    set(resourceCreationErrorAtom, message);
  },
);

/** Drop the placeholders whose canonical nodes have now arrived. */
export const commitPendingResourcesAtom = atom(null, (_get, set, committedNodeIds: ReadonlySet<string>) => {
  set(pendingResourcesAtom, (pending) =>
    pending.filter((resource) => !resource.expectedNodeId || !committedNodeIds.has(resource.expectedNodeId)),
  );
});

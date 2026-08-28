// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "@/lib/messaging";
import type { Point } from "@/lib/utils";

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

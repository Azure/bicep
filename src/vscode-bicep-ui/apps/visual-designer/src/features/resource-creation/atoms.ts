// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { VisualResourceTypeReference } from "@/lib/messaging/messages";
import type { Point } from "@/lib/utils/math/geometry";

import { atom } from "jotai";
import { atomFamily } from "jotai-family";

export interface PendingResource {
  operationId: string;
  resourceType: VisualResourceTypeReference;
  origin: Point;
  expectedNodeId?: string;
}

export const pendingResourcesAtom = atom<PendingResource[]>([]);
export const resourceCreationErrorAtom = atom<string | null>(null);
export const resourceNodeIsCommittingAtomFamily = atomFamily((_nodeId: string) => atom(false));

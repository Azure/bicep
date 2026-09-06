// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "@/features/canvas";

import { atom } from "jotai";
import { atomFamily } from "jotai-family";

export interface PaletteDragState {
  item: ResourceTypeReference;
  clientX: number;
  clientY: number;
}

export type NamespaceResourceTypesState =
  | { status: "idle" }
  | { status: "loading" }
  | { status: "loaded"; resourceTypes: ResourceTypeCatalogEntry[] }
  | { status: "error"; message: string };

export interface ResourceTypeCatalogEntry {
  resourceType: string;
  apiVersion: string;
}

export const paletteDragAtom = atom<PaletteDragState | null>(null);
export const namespaceResourceTypesAtomFamily = atomFamily((_key: string) =>
  atom<NamespaceResourceTypesState>({ status: "idle" }),
);
export const resourceTypeCatalogLoadingCountAtom = atom(0);

export function getNamespaceResourceTypesKey(catalogId: string, providerNamespace: string): string {
  return `${catalogId}\0${providerNamespace.toLocaleLowerCase()}`;
}

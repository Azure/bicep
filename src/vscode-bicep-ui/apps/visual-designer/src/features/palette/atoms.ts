// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "@/core";

import { atom } from "jotai";

export interface PaletteDragState {
  item: ResourceTypeReference;
  clientX: number;
  clientY: number;
}

export const paletteDragAtom = atom<PaletteDragState | null>(null);

export type ResourceVersionsState =
  { status: "loading" } | { status: "loaded"; apiVersions: string[] } | { status: "error"; message: string };

export const versionCatalogIdAtom = atom<string | undefined>();
export const resourceVersionsAtom = atom<Record<string, ResourceVersionsState>>({});
export const selectedVersionsAtom = atom<Record<string, string>>({});

export const acceptVersionCatalogAtom = atom(null, (get, set, catalogId: string) => {
  if (get(versionCatalogIdAtom) !== catalogId) {
    set(versionCatalogIdAtom, catalogId);
    set(resourceVersionsAtom, {});
    set(selectedVersionsAtom, {});
  }
});

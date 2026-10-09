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

/**
 * The API versions loaded and chosen for each resource type, for one resource type catalog. Keyed by the
 * lower-cased fully qualified type.
 */
export interface VersionCatalog {
  catalogId?: string;
  versions: Record<string, ResourceVersionsState>;
  selections: Record<string, string>;
}

export const versionCatalogAtom = atom<VersionCatalog>({ versions: {}, selections: {} });

/** A different resource type catalog discards every loaded version list and choice. */
export const acceptVersionCatalogAtom = atom(null, (get, set, catalogId: string) => {
  if (get(versionCatalogAtom).catalogId !== catalogId) {
    set(versionCatalogAtom, { catalogId, versions: {}, selections: {} });
  }
});

/** What browsing the palette shows: featured providers, recently added types, or every provider. */
export type PaletteView = "featured" | "recent" | "all";

/** The browsing view, kept while the palette is closed and reopened. */
export const paletteViewAtom = atom<PaletteView>("featured");

export const MAX_RECENT_RESOURCE_TYPES = 8;

/**
 * Fully qualified types most recently dropped onto the canvas, newest first, for this designer session.
 * Kept across catalog changes; the palette shows only those the current catalog offers.
 */
export const recentResourceTypesAtom = atom<readonly string[]>([]);

export const recordRecentResourceTypeAtom = atom(null, (get, set, fullyQualifiedType: string) => {
  const key = fullyQualifiedType.toLowerCase();
  const others = get(recentResourceTypesAtom).filter((type) => type.toLowerCase() !== key);

  set(recentResourceTypesAtom, [fullyQualifiedType, ...others].slice(0, MAX_RECENT_RESOURCE_TYPES));
});

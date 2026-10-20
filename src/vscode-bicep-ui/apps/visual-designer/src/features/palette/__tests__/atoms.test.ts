// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import {
  acceptVersionCatalogAtom,
  MAX_RECENT_RESOURCE_TYPES,
  recentResourceTypesAtom,
  recordRecentResourceTypeAtom,
  versionCatalogAtom,
} from "../atoms";

const KEY = "microsoft.storage/storageaccounts";

describe("resource version catalog state", () => {
  it("preserves choices and loaded versions when the provider catalog is unchanged", () => {
    const store = createStore();
    store.set(acceptVersionCatalogAtom, "provider-1");
    store.set(versionCatalogAtom, (catalog) => ({
      ...catalog,
      versions: { [KEY]: { status: "loaded", apiVersions: ["2025-01-01", "2024-01-01"] } },
      selections: { [KEY]: "2024-01-01" },
    }));

    store.set(acceptVersionCatalogAtom, "provider-1");

    expect(store.get(versionCatalogAtom).selections).toEqual({ [KEY]: "2024-01-01" });
    expect(store.get(versionCatalogAtom).versions[KEY]?.status).toBe("loaded");
  });

  it("invalidates all choices and requests when the provider catalog changes", () => {
    const store = createStore();
    store.set(acceptVersionCatalogAtom, "provider-1");
    store.set(versionCatalogAtom, (catalog) => ({
      ...catalog,
      versions: { [KEY]: { status: "loading" } },
      selections: { [KEY]: "2024-01-01" },
    }));

    store.set(acceptVersionCatalogAtom, "provider-2");

    expect(store.get(versionCatalogAtom)).toEqual({ catalogId: "provider-2", versions: {}, selections: {} });
  });
});

describe("recent resource types", () => {
  it("keeps the newest first and moves a reused type to the front without duplicating it", () => {
    const store = createStore();
    store.set(recordRecentResourceTypeAtom, "Microsoft.Storage/storageAccounts");
    store.set(recordRecentResourceTypeAtom, "Microsoft.Network/virtualNetworks");
    store.set(recordRecentResourceTypeAtom, "microsoft.storage/storageaccounts");

    expect(store.get(recentResourceTypesAtom)).toEqual([
      "microsoft.storage/storageaccounts",
      "Microsoft.Network/virtualNetworks",
    ]);
  });

  it("forgets the oldest types beyond the limit", () => {
    const store = createStore();
    for (let index = 0; index <= MAX_RECENT_RESOURCE_TYPES; index++) {
      store.set(recordRecentResourceTypeAtom, `Microsoft.Test/type${index}`);
    }

    const recent = store.get(recentResourceTypesAtom);
    expect(recent).toHaveLength(MAX_RECENT_RESOURCE_TYPES);
    expect(recent[0]).toBe(`Microsoft.Test/type${MAX_RECENT_RESOURCE_TYPES}`);
    expect(recent).not.toContain("Microsoft.Test/type0");
  });
});

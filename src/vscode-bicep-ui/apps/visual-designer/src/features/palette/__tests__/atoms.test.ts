// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import { acceptVersionCatalogAtom, versionCatalogAtom } from "../atoms";

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

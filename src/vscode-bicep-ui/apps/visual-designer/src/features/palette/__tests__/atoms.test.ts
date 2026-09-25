// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import { acceptVersionCatalogAtom, resourceVersionsAtom, selectedVersionsAtom, versionCatalogIdAtom } from "../atoms";

describe("resource version catalog state", () => {
  it("preserves choices and loaded versions when the provider catalog is unchanged", () => {
    const store = createStore();
    store.set(acceptVersionCatalogAtom, "provider-1");
    store.set(selectedVersionsAtom, { "microsoft.storage/storageaccounts": "2024-01-01" });
    store.set(resourceVersionsAtom, {
      "microsoft.storage/storageaccounts": { status: "loaded", apiVersions: ["2025-01-01", "2024-01-01"] },
    });

    store.set(acceptVersionCatalogAtom, "provider-1");

    expect(store.get(selectedVersionsAtom)).toEqual({ "microsoft.storage/storageaccounts": "2024-01-01" });
    expect(store.get(resourceVersionsAtom)["microsoft.storage/storageaccounts"]?.status).toBe("loaded");
  });

  it("invalidates all choices and requests when the provider catalog changes", () => {
    const store = createStore();
    store.set(acceptVersionCatalogAtom, "provider-1");
    store.set(selectedVersionsAtom, { "microsoft.storage/storageaccounts": "2024-01-01" });
    store.set(resourceVersionsAtom, { "microsoft.storage/storageaccounts": { status: "loading" } });

    store.set(acceptVersionCatalogAtom, "provider-2");

    expect(store.get(versionCatalogIdAtom)).toBe("provider-2");
    expect(store.get(selectedVersionsAtom)).toEqual({});
    expect(store.get(resourceVersionsAtom)).toEqual({});
  });
});

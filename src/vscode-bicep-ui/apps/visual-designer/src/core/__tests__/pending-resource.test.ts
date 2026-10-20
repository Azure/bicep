// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import {
  beginResourceCreationAtom,
  bindExpectedNodeAtom,
  cancelNodeRemovalAtom,
  commitPendingResourcesAtom,
  discardPendingResourceAtom,
  expectNodeRemovalAtom,
  pendingPlacementsAtom,
  pendingRemovalNodeIdsAtom,
  pendingResourcesAtom,
} from "../atoms";

const storageAccount = { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2024-01-01" };

describe("pending placements", () => {
  it("places only resources whose node ID is known, until their node arrives", () => {
    const store = createStore();
    store.set(beginResourceCreationAtom, {
      operationId: "bound",
      resourceType: storageAccount,
      origin: { x: 1, y: 2 },
    });
    store.set(beginResourceCreationAtom, {
      operationId: "unbound",
      resourceType: storageAccount,
      origin: { x: 3, y: 4 },
    });
    expect(store.get(pendingPlacementsAtom)).toEqual(new Map());

    store.set(bindExpectedNodeAtom, { operationId: "bound", expectedNodeId: "storage" });
    expect(store.get(pendingPlacementsAtom)).toEqual(new Map([["storage", { x: 1, y: 2 }]]));

    store.set(commitPendingResourcesAtom, new Set(["storage"]));
    expect(store.get(pendingPlacementsAtom)).toEqual(new Map());
  });
});

describe("pending removals", () => {
  it("tracks a node until its removal is cancelled", () => {
    const store = createStore();
    store.set(expectNodeRemovalAtom, "a");
    store.set(expectNodeRemovalAtom, "b");
    expect(store.get(pendingRemovalNodeIdsAtom)).toEqual(new Set(["a", "b"]));

    store.set(cancelNodeRemovalAtom, "a");
    expect(store.get(pendingRemovalNodeIdsAtom)).toEqual(new Set(["b"]));
  });
});

describe("pending resource lifecycle", () => {
  it("discards an undone creation preview and allows the same operation to be pending again on redo", () => {
    const store = createStore();
    const resource = {
      operationId: "create-1",
      resourceType: { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2024-01-01" },
      origin: { x: 10, y: 20 },
    };
    store.set(beginResourceCreationAtom, resource);
    store.set(bindExpectedNodeAtom, { operationId: resource.operationId, expectedNodeId: "storage" });
    expect(store.get(pendingResourcesAtom)[0]?.expectedNodeId).toBe("storage");

    store.set(discardPendingResourceAtom, resource.operationId);
    expect(store.get(pendingResourcesAtom)).toEqual([]);

    store.set(beginResourceCreationAtom, resource);
    store.set(bindExpectedNodeAtom, { operationId: resource.operationId, expectedNodeId: "storage" });
    store.set(commitPendingResourcesAtom, new Set(["storage"]));
    expect(store.get(pendingResourcesAtom)).toEqual([]);
  });

  it("does not discard another pending resource when one creation is undone", () => {
    const store = createStore();
    const base = {
      resourceType: { fullyQualifiedType: "Microsoft.Storage/storageAccounts", apiVersion: "2024-01-01" },
      origin: { x: 10, y: 20 },
    };
    store.set(beginResourceCreationAtom, { ...base, operationId: "first" });
    store.set(beginResourceCreationAtom, { ...base, operationId: "second" });

    store.set(discardPendingResourceAtom, "first");

    expect(store.get(pendingResourcesAtom).map(({ operationId }) => operationId)).toEqual(["second"]);
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { afterEach, describe, expect, it, vi } from "vitest";
import { resourceNodeIsCommittingAtomFamily } from "../atoms";

const testNodeIds = ["resourceA", "resourceB"] as const;

afterEach(() => {
  for (const nodeId of testNodeIds) {
    resourceNodeIsCommittingAtomFamily.remove(nodeId);
  }
});

describe("resourceNodeIsCommittingAtomFamily", () => {
  it("notifies only the resource node whose committing state changes", () => {
    const store = createStore();
    const firstAtom = resourceNodeIsCommittingAtomFamily(testNodeIds[0]);
    const secondAtom = resourceNodeIsCommittingAtomFamily(testNodeIds[1]);
    const firstListener = vi.fn();
    const secondListener = vi.fn();
    const unsubscribeFirst = store.sub(firstAtom, firstListener);
    const unsubscribeSecond = store.sub(secondAtom, secondListener);

    store.set(firstAtom, true);

    expect(store.get(firstAtom)).toBe(true);
    expect(store.get(secondAtom)).toBe(false);
    expect(firstListener).toHaveBeenCalledOnce();
    expect(secondListener).not.toHaveBeenCalled();

    unsubscribeFirst();
    unsubscribeSecond();
  });
});

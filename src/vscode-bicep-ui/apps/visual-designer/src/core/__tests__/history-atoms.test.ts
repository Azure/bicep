// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import { activeNodeDragCountAtom, canRedoAtom, canUndoAtom, undoHistoryAtom } from "../atoms";
import { recordLayoutChange } from "../undo-history";

describe("canUndoAtom and canRedoAtom", () => {
  it("wait for node drags to end", () => {
    const store = createStore();
    store.set(undoHistoryAtom, (history) =>
      recordLayoutChange(history, new Map([["a", { x: 0, y: 0 }]]), new Map([["a", { x: 10, y: 0 }]])),
    );
    expect(store.get(canUndoAtom)).toBe(true);

    store.set(activeNodeDragCountAtom, 1);
    expect(store.get(canUndoAtom)).toBe(false);
    expect(store.get(canRedoAtom)).toBe(false);

    store.set(activeNodeDragCountAtom, 0);
    expect(store.get(canUndoAtom)).toBe(true);
  });
});

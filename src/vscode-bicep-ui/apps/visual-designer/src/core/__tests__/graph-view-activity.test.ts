// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import {
  beginGraphChangeAtom,
  canRedoAtom,
  canUndoAtom,
  isGraphChangeInProgressAtom,
  trackGraphChangeAtom,
  undoHistoryAtom,
} from "../atoms";
import { completeUndo, EMPTY_UNDO_HISTORY, recordLayoutChange } from "../undo-history";

describe("graph view activity", () => {
  it("keeps the lock until every overlapping change has finished", () => {
    const store = createStore();
    const first = store.set(beginGraphChangeAtom);
    const second = store.set(beginGraphChangeAtom);
    expect(store.get(isGraphChangeInProgressAtom)).toBe(true);

    first();
    first();
    expect(store.get(isGraphChangeInProgressAtom)).toBe(true);

    second();
    expect(store.get(isGraphChangeInProgressAtom)).toBe(false);
    const next = store.set(beginGraphChangeAtom);
    expect(store.get(isGraphChangeInProgressAtom)).toBe(true);
    next();
    expect(store.get(isGraphChangeInProgressAtom)).toBe(false);
  });

  it("holds the lock for the full operation promise", async () => {
    const store = createStore();
    const work = Promise.withResolvers<void>();
    const tracked = store.set(trackGraphChangeAtom, () => work.promise);
    expect(store.get(isGraphChangeInProgressAtom)).toBe(true);

    work.resolve();
    await tracked;
    expect(store.get(isGraphChangeInProgressAtom)).toBe(false);
  });

  it.each(["throw", "reject"])("releases the lock and propagates an operation's %s", async (failure) => {
    const store = createStore();
    const error = new Error("Operation failed.");
    const tracked = store.set(trackGraphChangeAtom, () => {
      if (failure === "throw") {
        throw error;
      }
      return Promise.reject(error);
    });

    await expect(tracked).rejects.toBe(error);
    expect(store.get(isGraphChangeInProgressAtom)).toBe(false);
  });

  it("does not share activity between stores", () => {
    const store = createStore();
    const finish = store.set(beginGraphChangeAtom);

    expect(createStore().get(isGraphChangeInProgressAtom)).toBe(false);
    finish();
  });

  it("gates undo and redo until graph activity finishes", () => {
    const store = createStore();
    const history = recordLayoutChange(
      EMPTY_UNDO_HISTORY,
      new Map([["node", { x: 0, y: 0 }]]),
      new Map([["node", { x: 10, y: 10 }]]),
    );
    store.set(undoHistoryAtom, history);
    expect(store.get(canUndoAtom)).toBe(true);
    expect(store.get(canRedoAtom)).toBe(false);

    const finish = store.set(beginGraphChangeAtom);
    expect(store.get(canUndoAtom)).toBe(false);
    store.set(undoHistoryAtom, completeUndo(history, history.undoStack[0]!));
    expect(store.get(canRedoAtom)).toBe(false);

    finish();
    expect(store.get(canUndoAtom)).toBe(false);
    expect(store.get(canRedoAtom)).toBe(true);
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import { documentErrorCountAtom, isGraphLoadedAtom } from "@/core";
import { createAtomicNode, nodesByIdAtom } from "@/lib/graph";
import { graphStatusAtom } from "../atoms";

describe("graphStatusAtom", () => {
  it("is loading until the first graph arrives, even with nothing on the canvas", () => {
    const store = createStore();

    expect(store.get(graphStatusAtom)).toEqual({ kind: "loading" });

    store.set(isGraphLoadedAtom, true);

    expect(store.get(graphStatusAtom)).toEqual({ kind: "empty" });
  });

  it("reports errors, then a ready graph, once loaded", () => {
    const store = createStore();
    store.set(isGraphLoadedAtom, true);
    store.set(nodesByIdAtom, { a: createAtomicNode("a", { x: 0, y: 0 }, {}) });

    expect(store.get(graphStatusAtom)).toEqual({ kind: "ready" });

    store.set(documentErrorCountAtom, 2);

    expect(store.get(graphStatusAtom)).toEqual({ kind: "errors", errorCount: 2 });
  });
});

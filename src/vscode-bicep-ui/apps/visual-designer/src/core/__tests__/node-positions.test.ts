// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import { createAtomicNode, createCompoundNode, nodesByIdAtom } from "@/lib/graph";
import { applyNodePositions, captureNodePositions } from "../node-positions";

function addNode(store: ReturnType<typeof createStore>, nodeId: string, x: number, y: number) {
  store.set(nodesByIdAtom, (nodes) => ({ ...nodes, [nodeId]: createAtomicNode(nodeId, { x, y }, null) }));
}

describe("node positions", () => {
  it("captures atomic nodes only, optionally limited to one subtree", () => {
    const store = createStore();
    addNode(store, "module::a", 10, 20);
    addNode(store, "module::nested::b", 30, 40);
    addNode(store, "other", 50, 60);
    store.set(nodesByIdAtom, (nodes) => ({
      ...nodes,
      module: createCompoundNode("module", ["module::a", "module::nested::b"], null),
    }));

    expect(captureNodePositions(store)).toEqual(
      new Map([
        ["module::a", { x: 10, y: 20 }],
        ["module::nested::b", { x: 30, y: 40 }],
        ["other", { x: 50, y: 60 }],
      ]),
    );
    expect([...captureNodePositions(store, "module").keys()]).toEqual(["module::a", "module::nested::b"]);
    expect([...captureNodePositions(store, "other").keys()]).toEqual(["other"]);
  });

  it("moves nodes without changing their size and ignores unknown IDs", () => {
    const store = createStore();
    addNode(store, "resource", 0, 0);
    const node = store.get(nodesByIdAtom).resource;
    if (node?.kind !== "atomic") {
      throw new Error("Expected an atomic node.");
    }
    store.set(node.boxAtom, { min: { x: 10, y: 20 }, max: { x: 110, y: 60 } });

    applyNodePositions(
      store,
      new Map([
        ["resource", { x: 30, y: 40 }],
        ["missing", { x: 0, y: 0 }],
      ]),
    );

    expect(store.get(node.boxAtom)).toEqual({ min: { x: 30, y: 40 }, max: { x: 130, y: 80 } });
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import {
  committedGraphVersionAtom,
  getNodeTargetBox,
  graphBoundsAtom,
  graphCommittedAtom,
  graphVersionAtom,
  nodeConfigAtom,
  nodePositionsAtom,
  nodesByIdAtom,
  setGraphAtom,
} from "../atoms";

const padding = { top: 10, right: 10, bottom: 10, left: 10 };

function createGraphStore() {
  const store = createStore();
  store.set(nodeConfigAtom, (config) => ({ ...config, padding }));
  return store;
}

const graph = {
  nodes: [
    { id: "container", parentId: null, data: "c" },
    { id: "a", parentId: "container", data: "a" },
    { id: "b", parentId: "container", data: "b" },
  ],
  edges: [{ id: "a->b", sourceId: "a", targetId: "b" }],
};

describe("setGraphAtom", () => {
  it("derives containers from parents and edges from the input", () => {
    const store = createGraphStore();
    store.set(setGraphAtom, graph);

    const { container, a } = store.get(nodesByIdAtom);
    expect(store.get(container!.childIdsAtom)).toEqual(["a", "b"]);
    expect(store.get(a!.childIdsAtom)).toEqual([]);
    expect(store.get(a!.dataAtom)).toBe("a");
  });

  it("keeps a node's state while its parent is unchanged, including when it stops being a container", () => {
    const store = createGraphStore();
    store.set(setGraphAtom, graph);
    const before = store.get(nodesByIdAtom);

    store.set(setGraphAtom, { nodes: [{ id: "container", parentId: null, data: "card" }], edges: [] });

    const after = store.get(nodesByIdAtom);
    expect(after.container).toBe(before.container);
    expect(store.get(after.container!.childIdsAtom)).toEqual([]);
    expect(store.get(after.container!.dataAtom)).toBe("card");
    expect(Object.keys(after)).toEqual(["container"]);
  });

  it("replaces a node whose parent changed", () => {
    const store = createGraphStore();
    store.set(setGraphAtom, graph);
    const before = store.get(nodesByIdAtom).a;

    store.set(setGraphAtom, { nodes: [{ id: "a", parentId: null, data: "a" }], edges: [] });

    expect(store.get(nodesByIdAtom).a).not.toBe(before);
  });
});

describe("target boxes", () => {
  it("centers a leaf on its position at its measured size and encloses a container's children", () => {
    const store = createGraphStore();
    store.set(setGraphAtom, graph);
    store.set(nodePositionsAtom, {
      centers: new Map([
        ["a", { x: 0, y: 0 }],
        ["b", { x: 100, y: 0 }],
        ["hidden", { x: 500, y: 500 }],
      ]),
      transition: "instant",
    });

    expect(getNodeTargetBox(store, "a")).toBeNull();

    const { a, b } = store.get(nodesByIdAtom);
    store.set(a!.sizeAtom, { width: 20, height: 10 });
    store.set(b!.sizeAtom, { width: 20, height: 10 });

    expect(getNodeTargetBox(store, "a")).toEqual({ min: { x: -10, y: -5 }, max: { x: 10, y: 5 } });
    expect(getNodeTargetBox(store, "container")).toEqual({ min: { x: -20, y: -15 }, max: { x: 120, y: 15 } });
    expect(store.get(graphBoundsAtom)).toEqual({ min: { x: -20, y: -15 }, max: { x: 120, y: 15 } });
  });
});

describe("graphCommittedAtom", () => {
  it("is false from setting a graph until the node layer commits it", () => {
    const store = createGraphStore();
    expect(store.get(graphCommittedAtom)).toBe(true);

    store.set(setGraphAtom, graph);
    expect(store.get(graphCommittedAtom)).toBe(false);

    store.set(committedGraphVersionAtom, store.get(graphVersionAtom));
    expect(store.get(graphCommittedAtom)).toBe(true);
  });

  it("is false again when only node data changes", () => {
    const store = createGraphStore();
    store.set(setGraphAtom, graph);
    store.set(committedGraphVersionAtom, store.get(graphVersionAtom));

    store.set(setGraphAtom, { ...graph, nodes: graph.nodes.map((node) => ({ ...node, data: "changed" })) });

    expect(store.get(graphCommittedAtom)).toBe(false);
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNode } from "../api";

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import { layoutReadyAtom, nodesByIdAtom } from "@/lib/graph";
import { pendingSubgraphLayoutsAtom, shownGraphAtom } from "../atoms";
import { indexGraph, projectGraph } from "../graph-model";
import { getNodePositions, setNodePositions } from "../node-positions";
import { isMostlyNewGraph, showGraph } from "../show-graph";

function node(id: string, parentId: string | null, overrides: Partial<GraphNode> = {}): GraphNode {
  return {
    id,
    kind: "resource",
    parentId,
    type: "Test.Rp/tests",
    symbolName: id,
    isCollection: false,
    hasChildren: false,
    hasError: false,
    sourceStatus: null,
    sourceError: null,
    ...overrides,
  };
}

const source = indexGraph({
  nodes: [
    node("module", null, { kind: "module", type: "<module>", hasChildren: true }),
    node("module::a", "module"),
    node("module::added", "module"),
    node("other", null),
  ],
  edges: [{ id: "module::a->other", sourceId: "module::a", targetId: "other" }],
});

const getDefaultOrigin = () => ({ x: -1, y: -1 });

describe("showGraph", () => {
  it("places new nodes at their origin or among the others, and reports them", () => {
    const store = createStore();
    setNodePositions(store, new Map([["module::a", { x: 10, y: 10 }]]), "instant");

    const unplaced = showGraph(store, source, {
      origins: new Map([["other", { x: 50, y: 50 }]]),
      getDefaultOrigin,
    });

    expect(unplaced).toEqual(["module::added", "other"]);
    expect(getNodePositions(store)).toEqual(
      new Map([
        ["module::a", { x: 10, y: 10 }],
        ["module::added", { x: 10, y: 10 }],
        ["other", { x: 50, y: 50 }],
      ]),
    );
    expect(Object.keys(store.get(nodesByIdAtom))).toEqual(["module", "module::a", "module::added", "other"]);
    expect(store.get(shownGraphAtom)).toBe(source);
  });

  it("shows a collapsed module as a leaf with the data it renders", () => {
    const store = createStore();
    showGraph(store, projectGraph(source, new Set(["module"])), { getDefaultOrigin });

    const moduleNode = store.get(nodesByIdAtom).module!;
    expect(store.get(moduleNode.childIdsAtom)).toEqual([]);
    expect(store.get(moduleNode.dataAtom)).toMatchObject({ isCollapsed: true, canCollapse: true });
    // The module is placed, and its hidden children get no position until they are shown.
    expect([...getNodePositions(store).keys()]).toEqual(["module", "other"]);
  });

  it("shows a remote module without children with its source status", () => {
    const store = createStore();
    const graph = indexGraph({
      nodes: [
        node("remote", null, { kind: "module", type: "<module>", sourceStatus: "error", sourceError: "Failed." }),
      ],
      edges: [],
    });

    // Collapse is ignored for it: there is nothing to hide.
    showGraph(store, projectGraph(graph, new Set(["remote"])), { getDefaultOrigin });

    expect(store.get(store.get(nodesByIdAtom).remote!.dataAtom)).toMatchObject({
      isCollapsed: false,
      canCollapse: false,
      sourceStatus: "error",
      sourceError: "Failed.",
    });
  });
  it("reports children that were never shown when a module expands", () => {
    const store = createStore();
    setNodePositions(
      store,
      new Map([
        ["module", { x: 0, y: 0 }],
        ["module::a", { x: 10, y: 10 }],
        ["other", { x: 50, y: 50 }],
      ]),
      "instant",
    );

    const unplaced = showGraph(store, source, {
      origins: new Map([["module::added", { x: 0, y: 0 }]]),
      getDefaultOrigin,
    });

    expect(unplaced).toEqual(["module::added"]);
    expect(getNodePositions(store).get("module::added")).toEqual({ x: 0, y: 0 });
  });

  it("leaves the children of a subgraph waiting to be laid out without a position, so they stay hidden", () => {
    const store = createStore();
    setNodePositions(
      store,
      new Map([
        ["module", { x: 0, y: 0 }],
        ["module::a", { x: 10, y: 10 }],
      ]),
      "instant",
    );
    store.set(pendingSubgraphLayoutsAtom, new Map([["module", { x: 0, y: 0 }]]));

    const unplaced = showGraph(store, source, {
      origins: new Map([["module::added", { x: 0, y: 0 }]]),
      getDefaultOrigin,
    });

    // Both had no position; only the one outside the subgraph is given one.
    expect(unplaced).toEqual(["module::added", "other"]);
    expect(getNodePositions(store).has("module::added")).toBe(false);
    expect(getNodePositions(store).get("other")).toEqual({ x: 10, y: 10 });
    expect(Object.keys(store.get(nodesByIdAtom))).toContain("module::added");
  });

  it("hides the canvas for an empty graph, and leaves it visible otherwise", () => {
    const store = createStore();
    store.set(layoutReadyAtom, true);
    showGraph(store, source, { getDefaultOrigin });
    expect(store.get(layoutReadyAtom)).toBe(true);

    showGraph(store, indexGraph({ nodes: [], edges: [] }), { getDefaultOrigin });
    expect(store.get(layoutReadyAtom)).toBe(false);
    expect(store.get(nodesByIdAtom)).toEqual({});
  });
});

describe("isMostlyNewGraph", () => {
  it("is true when fewer than half of the nodes were shown before", () => {
    const collapsed = projectGraph(source, new Set(["module"]));

    expect(isMostlyNewGraph(indexGraph({ nodes: [], edges: [] }), source)).toBe(true);
    expect(isMostlyNewGraph(collapsed, source)).toBe(false);
    expect(isMostlyNewGraph(indexGraph({ nodes: [node("x", null)], edges: [] }), source)).toBe(true);
  });
});

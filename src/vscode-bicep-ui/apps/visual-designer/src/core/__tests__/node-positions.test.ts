// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { GraphNode } from "../api";

import { describe, expect, it } from "vitest";
import { indexGraph } from "../graph-model";
import {
  forgetRemovedNodePositions,
  getCentroid,
  getPositionedNodeIds,
  moveNodes,
  translateNodes,
} from "../node-positions";

function node(id: string, parentId: string | null, kind: GraphNode["kind"] = "resource"): GraphNode {
  return {
    id,
    kind,
    parentId,
    type: kind === "module" ? "<module>" : "Test.Rp/tests",
    symbolName: id,
    isCollection: false,
    hasChildren: false,
    hasError: false,
  };
}

// `outer` contains `inner`, which contains `child`; `sibling` is directly inside `outer`.
const graph = indexGraph({
  nodes: [
    { ...node("outer", null, "module"), hasChildren: true },
    { ...node("inner", "outer", "module"), hasChildren: true },
    node("child", "inner"),
    node("sibling", "outer"),
    node("other", null),
  ],
  edges: [],
});

const positions = new Map([
  ["outer", { x: 0, y: 0 }],
  ["inner", { x: 10, y: 10 }],
  ["child", { x: 20, y: 20 }],
  ["sibling", { x: 30, y: 30 }],
  ["other", { x: 100, y: 100 }],
]);

describe("moveNodes", () => {
  it("moves only the changed nodes when no module is collapsed", () => {
    const moved = moveNodes(positions, new Map([["outer", { x: 5, y: 0 }]]), graph, new Set());

    expect(moved.get("outer")).toEqual({ x: 5, y: 0 });
    expect(moved.get("inner")).toEqual({ x: 10, y: 10 });
    expect(moved.get("child")).toEqual({ x: 20, y: 20 });
  });

  it("moves the hidden contents of nested collapsed modules once, with the outermost card", () => {
    const moved = moveNodes(positions, new Map([["outer", { x: 5, y: 7 }]]), graph, new Set(["outer", "inner"]));

    expect(moved.get("inner")).toEqual({ x: 15, y: 17 });
    expect(moved.get("child")).toEqual({ x: 25, y: 27 });
    expect(moved.get("sibling")).toEqual({ x: 35, y: 37 });
    expect(moved.get("other")).toEqual({ x: 100, y: 100 });
  });

  it("restores hidden contents recorded in a step instead of moving them with the card", () => {
    // Undoing a drag replays every recorded position, including those of hidden nodes.
    const step = new Map([
      ["outer", { x: -1, y: -1 }],
      ["child", { x: 2, y: 3 }],
    ]);

    const moved = moveNodes(positions, step, graph, new Set(["outer"]));

    expect(moved.get("child")).toEqual({ x: 2, y: 3 });
    expect(moved.get("sibling")).toEqual({ x: 29, y: 29 });
  });

  it("does not change the positions it is given", () => {
    moveNodes(positions, new Map([["outer", { x: 5, y: 7 }]]), graph, new Set(["outer"]));

    expect(positions.get("child")).toEqual({ x: 20, y: 20 });
  });
});

describe("translateNodes", () => {
  it("offsets the nodes that have positions and ignores the rest", () => {
    expect(translateNodes(positions, ["child", "missing"], 1, 2)).toEqual(new Map([["child", { x: 21, y: 22 }]]));
  });
});

describe("forgetRemovedNodePositions", () => {
  it("keeps hidden nodes that are still in the graph and drops removed ones", () => {
    const kept = forgetRemovedNodePositions(new Map([...positions, ["removed", { x: 0, y: 0 }]]), graph);

    expect([...kept.keys()]).toEqual(["outer", "inner", "child", "sibling", "other"]);
  });

  it("returns the same map when nothing was removed", () => {
    expect(forgetRemovedNodePositions(positions, graph)).toBe(positions);
  });
});

describe("getPositionedNodeIds", () => {
  it("returns the nodes that are not containers", () => {
    expect(getPositionedNodeIds(graph)).toEqual(["child", "sibling", "other"]);
  });
});

describe("getCentroid", () => {
  it("averages positions, or returns null for none", () => {
    expect(
      getCentroid([
        { x: 0, y: 0 },
        { x: 10, y: 20 },
      ]),
    ).toEqual({ x: 5, y: 10 });
    expect(getCentroid([])).toBeNull();
  });
});

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Box } from "@/lib/math";
import type { GraphEdge, GraphNode, MeasuredGraph, MeasuredGraphNode } from "../api";
import type { ClientGraph } from "../graph-model";

import { describe, expect, it } from "vitest";
import {
  clientGraphsRenderEqually,
  EMPTY_CLIENT_GRAPH,
  indexGraph,
  measuredGraphsEqual,
  measureGraph,
} from "../graph-model";

function node(overrides: Partial<GraphNode> = {}): GraphNode {
  return {
    id: "n",
    kind: "resource",
    parentId: null,
    type: "Microsoft.Storage/storageAccounts",
    symbolName: "n",
    isCollection: false,
    hasChildren: false,
    hasError: false,
    ...overrides,
  };
}

const box = (width: number, height: number): Box => ({ min: { x: 0, y: 0 }, max: { x: width, y: height } });

describe("indexGraph", () => {
  it("indexes nodes and edges by id and keeps the error count", () => {
    const graph = indexGraph({
      nodes: [node({ id: "a" }), node({ id: "b" })],
      edges: [{ id: "a->b", sourceId: "a", targetId: "b" }],
      errorCount: 2,
    });

    expect([...graph.nodes.keys()]).toEqual(["a", "b"]);
    expect(graph.edges.get("a->b")).toEqual({ id: "a->b", sourceId: "a", targetId: "b" });
    expect(graph.errorCount).toBe(2);
  });
});

describe("measureGraph", () => {
  it("reports measured sizes, and zero for a node not yet measured", () => {
    const graph = indexGraph({
      nodes: [node({ id: "measured" }), node({ id: "unmeasured" })],
      edges: [],
      errorCount: 0,
    });

    const measured = measureGraph(graph, new Map([["measured", box(220, 80)]]));

    expect(measured.nodes).toEqual([
      { id: "measured", kind: "resource", parentId: null, width: 220, height: 80 },
      { id: "unmeasured", kind: "resource", parentId: null, width: 0, height: 0 },
    ]);
  });

  it("carries edges through by id", () => {
    const graph = indexGraph({
      nodes: [node({ id: "a" })],
      edges: [{ id: "a->b", sourceId: "a", targetId: "b" }],
      errorCount: 0,
    });

    expect(measureGraph(graph, new Map()).edges).toEqual([{ id: "a->b", sourceId: "a", targetId: "b" }]);
  });
});

function measuredNode(overrides: Partial<MeasuredGraphNode> = {}): MeasuredGraphNode {
  return { id: "a", kind: "resource", parentId: null, width: 220, height: 80, ...overrides };
}

describe("measuredGraphsEqual", () => {
  const base: MeasuredGraph = {
    nodes: [measuredNode({ id: "a" }), measuredNode({ id: "b" })],
    edges: [{ id: "a>b", sourceId: "a", targetId: "b" }],
  };

  it("returns false when the previous input is null", () => {
    expect(measuredGraphsEqual(null, base)).toBe(false);
  });

  it("returns true for the same graph regardless of node and edge order", () => {
    const reordered: MeasuredGraph = {
      nodes: [measuredNode({ id: "b" }), measuredNode({ id: "a" })],
      edges: [{ id: "a>b", sourceId: "a", targetId: "b" }],
    };
    expect(measuredGraphsEqual(base, reordered)).toBe(true);
  });

  it("returns false when a node count differs", () => {
    const extra: MeasuredGraph = { nodes: [...base.nodes, measuredNode({ id: "c" })], edges: base.edges };
    expect(measuredGraphsEqual(base, extra)).toBe(false);
  });

  it("returns false when a measured size differs", () => {
    const widened: MeasuredGraph = {
      nodes: [measuredNode({ id: "a", width: 221 }), measuredNode({ id: "b" })],
      edges: base.edges,
    };
    expect(measuredGraphsEqual(base, widened)).toBe(false);
  });

  it("returns false when containment differs", () => {
    const reparented: MeasuredGraph = {
      nodes: [measuredNode({ id: "a", parentId: "b" }), measuredNode({ id: "b" })],
      edges: base.edges,
    };
    expect(measuredGraphsEqual(base, reparented)).toBe(false);
  });

  it("returns false when the edge set differs", () => {
    const rewired: MeasuredGraph = {
      nodes: base.nodes,
      edges: [{ id: "b>a", sourceId: "b", targetId: "a" }],
    };
    expect(measuredGraphsEqual(base, rewired)).toBe(false);
  });
});
function makeClientGraph(nodes: GraphNode[], edges: GraphEdge[] = [], errorCount = 0): ClientGraph {
  return indexGraph({ nodes, edges, errorCount });
}

describe("clientGraphsRenderEqually", () => {
  it("reports equal for a graph rebuilt from identical parts", () => {
    expect(clientGraphsRenderEqually(makeClientGraph([node()]), makeClientGraph([node()]))).toBe(true);
  });

  it("ignores node and edge ordering", () => {
    const a = node({ id: "a" });
    const b = node({ id: "b" });

    expect(clientGraphsRenderEqually(makeClientGraph([a, b]), makeClientGraph([b, a]))).toBe(true);
  });

  it("ignores fields the canvas never reads", () => {
    const before = makeClientGraph([node({ symbolName: "before" })]);
    const after = makeClientGraph([node({ symbolName: "after" })]);

    expect(clientGraphsRenderEqually(before, after)).toBe(true);
  });

  it("reports a change from or to an empty graph", () => {
    expect(clientGraphsRenderEqually(EMPTY_CLIENT_GRAPH, EMPTY_CLIENT_GRAPH)).toBe(true);
    expect(clientGraphsRenderEqually(EMPTY_CLIENT_GRAPH, makeClientGraph([node()]))).toBe(false);
    expect(clientGraphsRenderEqually(makeClientGraph([node()]), EMPTY_CLIENT_GRAPH)).toBe(false);
  });

  it.each([
    ["id", { id: "other" }],
    ["type", { type: "Microsoft.Web/sites" }],
    ["isCollection", { isCollection: true }],
    ["hasChildren", { hasChildren: true }],
    ["hasError", { hasError: true }],
  ])("reports a change when %s differs", (_field, overrides) => {
    const before = makeClientGraph([node()]);
    const after = makeClientGraph([node(overrides as Partial<GraphNode>)]);

    expect(clientGraphsRenderEqually(before, after)).toBe(false);
  });

  it("reports a change when the error count differs", () => {
    expect(clientGraphsRenderEqually(makeClientGraph([node()], [], 0), makeClientGraph([node()], [], 1))).toBe(false);
  });

  it("reports a change when an edge is added or retargeted", () => {
    const nodes = [node({ id: "a" }), node({ id: "b" })];
    const none = makeClientGraph(nodes);
    const one = makeClientGraph(nodes, [{ id: "e", sourceId: "a", targetId: "b" }]);
    const other = makeClientGraph(nodes, [{ id: "e", sourceId: "b", targetId: "a" }]);

    expect(clientGraphsRenderEqually(none, one)).toBe(false);
    expect(clientGraphsRenderEqually(one, other)).toBe(false);
  });
});
